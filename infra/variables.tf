variable "lambda_name" {
  description = "Value of the Name tag for the Lambda"
  type        = string
  default     = "AwsLambdaDotnetWebApi"
}

variable "lambda_role" {
    description = "Lambda IAM role to assume"
    type = string
    default = "arn:aws:iam::398018169858:role/AwsLambdaDotnetWebApiRole"
}

variable "lambda_function_handler" {
    description = "Lambda Function handler"
    type = string
    default = "AwsLambdaDotnetWebApi::AwsLambdaDotnetWebApi.LambdaEntryPoint::FunctionHandlerAsync"
}

variable "git_commit_sha" {
  description = "The Git commit SHA for the configuration version."
  type        = string
  default     = ""
}
