variable "lambda_name" {
  description = "Value of the Name tag for the Lambda"
  type        = string
  default     = "AwsLambdaDotnetWebApi"
}

variable "lambda_function_handler" {
  description = "Lambda Function handler"
  type        = string
  default     = "AwsLambdaDotnetWebApi::AwsLambdaDotnetWebApi.LambdaEntryPoint::FunctionHandlerAsync"
}

variable "lambda_dotnet_redis_user_password" {}
