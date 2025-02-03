data "aws_cognito_user_pools" "bmb_selected_user_pool" {
  name = var.user_pool_name
}