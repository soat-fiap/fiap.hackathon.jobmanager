output "test" {
  value = data.aws_cognito_user_pools.bmb_selected_user_pool.ids
}