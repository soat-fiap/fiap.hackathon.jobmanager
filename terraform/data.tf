data "aws_cognito_user_pools" "bmb_selected_user_pool" {
  name = var.user_pool_name
}

data "aws_cognito_user_pool" "user_pool" {
  user_pool_id = data.aws_cognito_user_pools.bmb_selected_user_pool.ids[0]
}
