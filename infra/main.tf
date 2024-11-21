terraform {
  cloud {
    organization = "personal-burrt"
    workspaces {
      name = "template-examples"
    }
  }

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.16"
    }
  }

  required_version = ">= 1.2.0"
}

provider "aws" {
  region = "ap-southeast-2"
}

resource "aws_lambda_function" "aws_lambda_dotnet_web_api" {
  function_name = var.lambda_name
  role          = var.lambda_role
  handler       = var.lambda_function_handler
  package_type  = "Zip"
  runtime       = "dotnet8"
  architectures = [ "arm64" ]
  memory_size   = 512
  timeout       = 30

  # If the file is not in the current working directory you will need to include a
  # path.module in the filename.
  filename      = "../build/lambda_package/.zip"
  source_code_hash = filebase64sha256("../build/lambda_package.zip")

  ephemeral_storage {
    size = 512
  }
  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT = "Production"
    }
  }
}
