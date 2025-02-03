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

variable "jwt_signing_key" {
  type      = string
  sensitive = true
  default = "PkOhRwy6UtniEMo7lLWp3bADctYgnDHCTvH+2YkDeGg="
}

variable "jwt_issuer" {
  type      = string
  sensitive = false
  default   = "https://localhost:7004"
}

variable "jwt_aud" {
  type      = string
  sensitive = false
  default   = "https://localhost:7004"
}

variable "api_docker_image" {
  type    = string
  default = "ghcr.io/soat-fiap/fiap.hackathon.jobmanager/api:latest"
}

variable "internal_elb_name" {
  type    = string
  default = "kitchen-api-internal-elb"
}

variable "api_access_key_id" {
  type      = string
  nullable  = false
  sensitive = true
  default = ""
}

variable "api_secret_access_key" {
  type      = string
  nullable  = false
  sensitive = true
  default = ""
}

variable "user_pool_name" {
  type        = string
  description = "Cognito user pool name"
  default     = "User pool - -ylrfa"
}