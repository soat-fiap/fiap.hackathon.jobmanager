variable "profile" {
  description = "AWS profile name"
  type        = string
  default     = "default"
}

variable "region" {
  description = "AWS region"
  type        = string
  default     = "us-east-1"
}

variable "eks_cluster_name" {
  type    = string
  default = "eks_dev_quixada"
}

variable "api_docker_image" {
  type    = string
  default = "ghcr.io/soat-fiap/fiap.hackathon.jobmanager/api:latest"
}

variable "internal_elb_name" {
  type    = string
  default = "jobmanager-api-internal-elb"
}

variable "api_access_key_id" {
  type      = string
  nullable  = false
  sensitive = true
  default   = ""
}

variable "api_secret_access_key" {
  type      = string
  nullable  = false
  sensitive = true
  default   = ""
}

variable "user_pool_name" {
  type        = string
  description = "Cognito user pool name"
  default     = "bmb-users-pool-test"
}

variable "notification_email" {
  type        = string
  description = "Email from where notifications are sent"
  nullable    = false
}
