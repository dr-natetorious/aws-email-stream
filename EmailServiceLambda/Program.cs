using Amazon;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Amazon.Lambda.KinesisEvents;
using Amazon.Lambda.TestUtilities;
using Amazon.XRay.Recorder.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EmailServiceLambda
{
    /// <summary>
    /// Entry point for local troubleshooting.
    /// </summary>
    /// <remarks>
    /// # Setup credentials...
    /// aws configure
    /// 
    /// # Kick off app
    /// dotnet EmailServiceLambda.dll Payloads/File.txt
    /// </remarks>
    class Program
    {
        static void Main(string[] args)
        {
            // Code running in AWS already has this set.
            Environment.SetEnvironmentVariable("AWS_REGION", "us-east-1");

            // Send a message directly to the stream...
            SendMessageAsync(
                streamName: Environment.GetEnvironmentVariable("STREAM_NAME"),
                fileName: args[0]).Wait();

            // Run the lambda within visual studio...
            LocallyRunFile(args[0]).Wait();
        }

        private static async Task SendMessageAsync(string streamName, string fileName)
        {
            IAmazonKinesis client = new AmazonKinesisClient();
            var response = await client.PutRecordAsync(new PutRecordRequest
            {
                PartitionKey = $"{Guid.NewGuid()}",
                StreamName = streamName,
                Data = new MemoryStream(File.ReadAllBytes(fileName))
            });

            Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }

        private static async Task LocallyRunFile(string fileName)
        {
            AWSXRayRecorder.Instance.BeginSegment("LocalRunFile");
            try
            {
                await LocallyRunFileAnalyzed(fileName);
            }
            finally
            {
                AWSXRayRecorder.Instance.EndSegment();
            }
        }

        private static async Task LocallyRunFileAnalyzed(string fileName)
        {
            // Running locally....
            var function = new FunctionHandler();
            var context = new TestLambdaContext();
            var task = function.HandleKinesisEventAsync(
                context: context,
                kinesisEvent: new KinesisEvent
                {
                    Records = new List<KinesisEvent.KinesisEventRecord>
                    {
                        new KinesisEvent.KinesisEventRecord
                        {
                            Kinesis = new KinesisEvent.Record
                            {
                                Data = new MemoryStream(File.ReadAllBytes(fileName))
                            }
                        }
                    }
                });
            try
            {
                await task;
                Console.WriteLine("Completed.");
            }
            catch (AggregateException error)
            {
                Console.WriteLine(error.GetBaseException());
            }
        }
    }
}
