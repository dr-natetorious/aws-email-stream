# aws-Windows Containers

A simple example for using Windows Containers + IIS + AD

## Setup instructions

1. Download and install [npm](https://nodejs.org/en/)

2. Run `npm install -g aws-cdk`

3. Run `cdk bootstrap aws://AWS_ACCOUNTID/REGION`

4. Run `cdk synth ./deploy *`

5. Run `cdk deploy ./deploy *`

## Resources Deployed in Environment

- VPC (Virtual Private Cloud)
  - 2 x Public Subnets
  - 2 x Private Subnets
- Microsoft AD Services
  - fqdn: ad.virtual.world
  - netbios: virtualworld
  - 2 x Private Subnets
- Elastic Container Services
  - 1 x EKS Cluster
    - Open Endpoint
    - Associated with all subnets
  - ECR Container Registry
    - mywin-webapp
