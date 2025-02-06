resource "aws_secretsmanager_secret" "sm-redis-user-secret" {
  name                    = "templates/${var.lambda_name}/redis-user-secret"
  description             = "Redis user secret for the .NET Web API Lambda"
  recovery_window_in_days = 0
}

resource "aws_secretsmanager_secret_version" "sm-redis-user-secret" {
  secret_id     = aws_secretsmanager_secret.sm-redis-user-secret.id
  secret_string = var.lambda_dotnet_redis_user_password
}
