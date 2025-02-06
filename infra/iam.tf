resource "aws_iam_policy" "lambda-dotnet-web-api-policy" {
  name        = "lambda-dotnet-web-api-policy"
  description = "IAM policies for the .NET Web API Lambda"
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        "Sid" : "AWSLambdaBasicExecutionRole",
        "Effect" : "Allow",
        "Action" : [
          "logs:CreateLogGroup",
          "logs:CreateLogStream",
          "logs:PutLogEvents"
        ],
        "Resource" : "*"
      },
      {
        "Sid" : "AWSLambdaVPCAccessExecutionPermissions",
        "Effect" : "Allow",
        "Action" : [
          "logs:CreateLogGroup",
          "logs:CreateLogStream",
          "logs:PutLogEvents",
          "ec2:CreateNetworkInterface",
          "ec2:DescribeNetworkInterfaces",
          "ec2:DescribeSubnets",
          "ec2:DeleteNetworkInterface",
          "ec2:AssignPrivateIpAddresses",
          "ec2:UnassignPrivateIpAddresses"
        ],
        "Resource" : "*"
      },
      {
        "Sid" : "AWSSecretsManagerReadOnly",
        "Effect" : "Allow",
        "Action" : [
          "secretsmanager:GetSecretValue"
        ],
        "Resource" : "arn:aws:secretsmanager:ap-southeast-2:398018169858:secret:templates/${var.lambda_name}/redis-user-secret*"
    }]
  })
}

resource "aws_iam_role" "lambda-dotnet-web-api-role" {
  name = "lambda-dotnet-web-api-role"
  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = "sts:AssumeRole"
        Principal = {
          Service = "lambda.amazonaws.com"
        }
    }]
  })
}

resource "aws_iam_role_policy_attachment" "lambda-dotnet-web-api-role-policy" {
  role       = aws_iam_role.lambda-dotnet-web-api-role.name
  policy_arn = aws_iam_policy.lambda-dotnet-web-api-policy.arn
}

resource "aws_lambda_permission" "api-gateway-lambda-invoke-perm" {
  statement_id  = "AllowApiGatewayInvoke"
  action        = "lambda:InvokeFunction"
  principal     = "apigateway.amazonaws.com"
  function_name = var.lambda_name
  depends_on    = [aws_lambda_function.aws_lambda_dotnet_web_api]
  # TODO: replace with TF API Gateway resource
  source_arn = "arn:aws:execute-api:ap-southeast-2:398018169858:2wwtpu9vd1/*/*/{proxy+}"
}
