from aws_cdk import (
    aws_ec2 as ec2,
    aws_lambda as lambda_,
    aws_iam as iam,
    aws_kinesis as ks,
    aws_lambda_event_sources as sources,
    core
)

class LambdaLayer(core.Construct):
  """
  Configure and deploy the network
  """
  def __init__(self, scope: core.Construct, id: str, stream:ks.Stream, **kwargs) -> None:
    super().__init__(scope, id, **kwargs)

    self.kinesis_processor = lambda_.Function(self, 'KinesisProcessor',
      handler='EmailServiceLambda::FunctionHandler.HandleKinesisEventAsync',
      code= lambda_.Code.from_asset(path='../bin/EmailServiceLambda.zip'),
      runtime = lambda_.Runtime.DOTNET_CORE_2_1)

    for policy in ['AmazonSESFullAccess', 'AmazonDynamoDBFullAccess']:
      self.kinesis_processor.role.add_managed_policy(
        iam.ManagedPolicy.from_aws_managed_policy_name(policy))

    self.kinesis_processor.add_event_source(sources.KinesisEventSource(
      stream= stream,
      starting_position= lambda_.StartingPosition.LATEST,
      batch_size=10))