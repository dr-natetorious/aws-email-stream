using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailServiceLambda
{
    public sealed class FunctionHandler
    {
        private readonly IAmazonSimpleEmailService emailService;
        private readonly IDynamoDBContext dynamo;

        public FunctionHandler() :this(
            emailService: new AmazonSimpleEmailServiceClient(),
            dynamo: new DynamoDBContext(new AmazonDynamoDBClient(), new DynamoDBContextConfig
            { 
                TableNamePrefix = Environment.GetEnvironmentVariable("TABLE_NAME_PREFIX")
            }))
        {
        }

        public FunctionHandler(
            IAmazonSimpleEmailService emailService,
            IDynamoDBContext dynamo)
        {
            // Enable XRay...
            AWSSDKHandler.RegisterXRayForAllServices();

            this.emailService = emailService;
            this.dynamo = dynamo;
        }

        public async Task HandleKinesisEventAsync(KinesisEvent kinesisEvent, ILambdaContext context)
        {
            context.Logger.LogLine($"Processing {kinesisEvent.Records.Count} records...");
            var batch = this.dynamo.CreateBatchWrite<EmailAudit>();

            foreach (var record in kinesisEvent.Records)
            {
                var payload = record.Kinesis.Data.ToArray();
                var contents = Encoding.UTF8.GetString(payload);

                // Send through SES....
                await this.emailService.SendEmailAsync(new SendEmailRequest
                {
                    Destination = new Destination(new List<string> { Environment.GetEnvironmentVariable("EMAIL_TO") }),
                    Message = new Message()
                    {
                        Subject = new Content("My Mail"),
                        Body = new Body(new Content(contents))
                    }
                });

                // Make a record in Dynamo for auditing purposes...
                batch.AddPutItem(new EmailAudit
                {
                    PartitionKey = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    SortKey = record.EventId,
                    Contents = contents
                });
            }

            // Record in the audit table...
            await batch.ExecuteAsync();
        }
    }
}
