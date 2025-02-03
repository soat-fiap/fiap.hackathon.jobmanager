locals {
  jwt_issuer            = "https://cognito-idp.${var.region}.amazonaws.com/${data.aws_cognito_user_pools.bmb_selected_user_pool.ids[0]}"
  jwt_aud               = var.jwt_aud
  docker_image          = var.api_docker_image
  aws_access_key        = var.api_access_key_id
  aws_secret_access_key = var.api_secret_access_key
  aws_region            = var.region
  jwt_signing_key       = var.jwt_signing_key
}

##############################
# DATABASE
##############################

resource "aws_dynamodb_table" "jobs_table" {
  name           = "Job"
  billing_mode   = "PAY_PER_REQUEST"
  hash_key       = "UserId"
  range_key      = "Id"
  stream_enabled = false

  attribute {
    name = "UserId"
    type = "S"
  }
  attribute {
    name = "Id"
    type = "S"
  }
}

# S3 
resource "aws_s3_bucket" "video_bucket" {
  bucket = "hackathon-video-processor"

}

# resource "aws_s3_directory_bucket" "video_bucket" {
#   bucket = "hackathon-video-processor"

#   location {
#     name = "use1-az4"
#   }
# }



data "aws_iam_policy_document" "queue_policy" {
  statement {
    effect = "Allow"

    principals {
      type        = "*"
      identifiers = ["*"]
    }

    actions   = ["sqs:SendMessage"]
    resources = ["arn:aws:sqs:*:*:received_videos"]

    condition {
      test     = "ArnEquals"
      variable = "aws:SourceArn"
      values   = [aws_s3_bucket.video_bucket.arn]
    }
  }
}

# SQS processar_arquivo
resource "aws_sqs_queue" "received_videos" {
  name                       = "received_videos"
  policy                     = data.aws_iam_policy_document.queue_policy.json
  visibility_timeout_seconds = 30
  delay_seconds              = 60
  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.received_videos_dlq.arn
    maxReceiveCount     = 5
  })
}

resource "aws_sqs_queue" "received_videos_dlq" {
  name = "received_videos_dlq"
}

resource "aws_sqs_queue_redrive_allow_policy" "received_videos_dlq_policy" {
  queue_url = aws_sqs_queue.received_videos_dlq.id

  redrive_allow_policy = jsonencode({
    redrivePermission = "byQueue",
    sourceQueueArns   = [aws_sqs_queue.received_videos.arn]
  })
}

resource "aws_s3_bucket_notification" "bucket_notification" {
  bucket = aws_s3_bucket.video_bucket.id

  queue {
    queue_arn     = aws_sqs_queue.received_videos.arn
    events        = ["s3:ObjectCreated:*"]
    filter_suffix = ".mkv"
  }
}

# SQS Notificação
resource "aws_sqs_queue" "sqs_notificacao" {
  name                       = "sqs_notificacao"
  visibility_timeout_seconds = 120
}

##############################
# CONFIGS/SECRETS
##############################


resource "kubernetes_config_map_v1" "config_map_api" {
  metadata {
    name = "configmap-jobmanager-api"
    labels = {
      "app"       = "jobmanager-api"
      "terraform" = true
    }
  }
  data = {
    "ASPNETCORE_ENVIRONMENT"               = "Development"
    "Serilog__WriteTo__2__Args__serverUrl" = "http://api-internal.fiap-log.svc.cluster.local"
    "RedisSettings__Host"                  = "redis"
    "RedisSettings__Port"                  = 6379
    "JwtOptions__Issuer"                   = local.jwt_issuer
    "JwtOptions__Audience"                 = local.jwt_aud
    "JwtOptions__ExpirationSeconds"        = 3600
    "JwtOptions__UseAccessToken"           = true
    "VideoReceivedConfig__QueueUrl"        = aws_sqs_queue.received_videos.url
  }
}

resource "kubernetes_secret" "secret_api" {
  metadata {
    name = "secret-jobmanager-api"
    labels = {
      app         = "api-pod"
      "terraform" = true
    }
  }
  data = {
    "JwtOptions__SigningKey" = local.jwt_signing_key
    "AWS_SECRET_ACCESS_KEY"  = local.aws_secret_access_key
    "AWS_ACCESS_KEY_ID"      = local.aws_access_key
    "AWS_REGION"             = local.aws_region
  }
  type = "Opaque"
}

# ####################################
# # API
# ####################################

resource "kubernetes_service" "jobmanager-api-svc" {
  metadata {
    name      = "api-internal"
    annotations = {
      "service.beta.kubernetes.io/aws-load-balancer-type"   = "nlb"
      "service.beta.kubernetes.io/aws-load-balancer-scheme" = "internal"
    }
  }
  spec {
    port {
      port        = 80
      target_port = 8080
      node_port   = 30007
      protocol    = "TCP"
    }
    type = "LoadBalancer"
    selector = {
      app : "jobmanager-api"
    }
  }
}

# resource "kubernetes_deployment" "deployment_production_api" {
#   depends_on = [
#     kubernetes_secret.secret_api,
#     kubernetes_config_map_v1.config_map_api
#   ]

#   metadata {
#     name      = "deployment-jobmanager-api"
#     labels = {
#       app         = "jobmanager-api"
#       "terraform" = true
#     }
#   }
#   spec {
#     replicas = 1
#     selector {
#       match_labels = {
#         app = "jobmanager-api"
#       }
#     }
#     template {
#       metadata {
#         name = "pod-jobmanager-api"
#         labels = {
#           app         = "jobmanager-api"
#           "terraform" = true
#         }
#       }
#       spec {
#         automount_service_account_token = false
#         container {
#           name  = "jobmanager-api-container"
#           image = local.docker_image
#           port {
#             name           = "liveness-port"
#             container_port = 8080
#           }
#           port {
#             container_port = 80
#           }

#           image_pull_policy = "IfNotPresent"
#           liveness_probe {
#             http_get {
#               path = "/healthz"
#               port = "liveness-port"
#             }
#             period_seconds        = 10
#             failure_threshold     = 3
#             initial_delay_seconds = 20
#           }
#           readiness_probe {
#             http_get {
#               path = "/healthz"
#               port = "liveness-port"
#             }
#             period_seconds        = 10
#             failure_threshold     = 3
#             initial_delay_seconds = 10
#           }

#           resources {
#             requests = {
#               cpu    = "100m"
#               memory = "120Mi"
#             }
#             limits = {
#               cpu    = "150m"
#               memory = "200Mi"
#             }
#           }
#           env_from {
#             config_map_ref {
#               name = "configmap-jobmanager-api"
#             }
#           }
#           env_from {
#             secret_ref {
#               name = "secret-jobmanager-api"
#             }
#           }
#         }
#       }
#     }
#   }
# }

# resource "kubernetes_horizontal_pod_autoscaler_v2" "hpa_api" {
#   metadata {
#     name      = "hpa-jobmanager-api"
#   }
#   spec {
#     max_replicas = 3
#     min_replicas = 1
#     scale_target_ref {
#       api_version = "apps/v1"
#       kind        = "Deployment"
#       name        = "deployment-jobmanager-api"
#     }

#     metric {
#       type = "ContainerResource"
#       container_resource {
#         container = "jobmanager-api-container"
#         name      = "cpu"
#         target {
#           average_utilization = 65
#           type                = "Utilization"
#         }
#       }
#     }
#   }
# }


#################################
# SEQ
#################################

resource "kubernetes_namespace" "fiap_log" {
  metadata {
    name = "fiap-log"
  }
}

resource "kubernetes_service" "svc_seq" {
  metadata {
    name      = "api-internal"
    namespace = kubernetes_namespace.fiap_log.metadata.0.name
    labels = {
      "terraform" = true
    }
    annotations = {
      "service.beta.kubernetes.io/aws-load-balancer-type"   = "nlb"
      "service.beta.kubernetes.io/aws-load-balancer-scheme" = "internal"
    }
  }
  spec {
    type = "LoadBalancer"
    port {
      port      = 80
      node_port = 30008
    }
    selector = {
      app = "seq"
    }
  }
}

resource "kubernetes_deployment" "deployment_seq" {
  metadata {
    name      = "deployment-seq"
    namespace = kubernetes_namespace.fiap_log.metadata.0.name
    labels = {
      app         = "seq"
      "terraform" = true
    }
  }
  spec {
    replicas = 1
    selector {
      match_labels = {
        app = "seq"
      }
    }
    template {
      metadata {
        name = "pod-seq"
        labels = {
          app         = "seq"
          "terraform" = true
        }
      }
      spec {
        automount_service_account_token = false
        container {
          name  = "seq-container"
          image = "datalust/seq:latest"
          port {
            container_port = 80
          }
          image_pull_policy = "IfNotPresent"
          env {
            name  = "ACCEPT_EULA"
            value = "Y"
          }
          resources {
            requests = {
              cpu    = "50m"
              memory = "120Mi"
            }
            limits = {
              cpu    = "100m"
              memory = "220Mi"
            }
          }
        }
        volume {
          name = "dashboards-volume"
          host_path {
            path = "/home/docker/seq"
          }
        }
      }
    }
  }
}
