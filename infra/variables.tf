variable "lambda_name" {
  description = "Value of the Name tag for the Lambda"
  type        = string
  default     = "AwsLambdaDotnetWebApi"
}

variable "lambda_role" {
    description = "Lambda IAM role to assume"
    type = string
    default = "AwsLambdaDotnetWebApiRole"
}

variable "lambda_function_handler" {
    description = "Lambda Function handler"
    type = string
    default = "AwsLambdaDotnetWebApi::AwsLambdaDotnetWebApi.LambdaEntryPoint::FunctionHandlerAsync"
}
