// Naming
variable "name" {
  type        = "string"
  description = "Location of the azure resource group."
  default     = "mtc-whack-a-mole"
}

variable "environment" {
  type        = "string"
  description = "Name of the deployment environment"
  default     = "dev"
}

// Resource information

variable "location" {
  type        = "string"
  description = "Location of the azure resource group."
  default     = "WestUS2"
}

variable tags {
  type        = "map"
  description = "Tags to apply on all groups and resources."

  default = {
    mtc-architect = "Joey Lorich"
    deployed-with = "Terraform"
  }
}

// Node type information

variable "node_count" {
  type        = "string"
  description = "The number of K8S nodes to provision."
  default     = 3
}

variable "node_type" {
  type        = "string"
  description = "The size of each node."
  default     = "Standard_D1_v2"
}

variable "node_os" {
  type        = "string"
  description = "Windows or Linux"
  default     = "Linux"
}

variable "dns_prefix" {
  type        = "string"
  description = "DNS Prefix"
  default     = "mtc-whack-a-mole"
}
