# aws-email-stream

A simple example that connects RESTful API -> Kinesis -> Lambda -> Simple Email Services.

There is also a DynamoDB table for auditing the flow of messages.

## Setup instructions

```bash
# Download and install npm - https://nodejs.org/en/
npm install -g aws-cdk

# Build .NET projects
build.bat

# Materialize the environment
cdk bootstrap aws://AWS_ACCOUNTID/REGION
cdk synth ./deploy
cdk deploy ./deploy
```

## Resources Deployed in Environment

- VPC (Virtual Private Cloud)
  - 2 x Public Subnets
  - 2 x Private Subnets
- API Gateway (RESTApi)
  - POST /send-email
    - Kinesis Service Integration
- Kinesis Stream
  - Lambda: Email Processor
    - Reads event
    - Audits into EmailAudit DynamoDB table
    - Sends email with Simple Email Services
