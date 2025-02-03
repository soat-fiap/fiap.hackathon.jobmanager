# output "eks_cluster" {
#   value     = data.aws_eks_cluster.techchallenge_cluster
#   sensitive = true
# }



output "test" {
  value = data.aws_cognito_user_pools.bmb_selected_user_pool.ids
}