# Define Kubernetes provider to use the AKS cluster
provider "kubernetes" {
  host = "${azurerm_kubernetes_cluster.default.kube_config.0.host}"

  client_certificate     = "${base64decode(azurerm_kubernetes_cluster.default.kube_config.0.client_certificate)}"
  client_key             = "${base64decode(azurerm_kubernetes_cluster.default.kube_config.0.client_key)}"
  cluster_ca_certificate = "${base64decode(azurerm_kubernetes_cluster.default.kube_config.0.cluster_ca_certificate)}"
}

# Create a service account for the Helm Tiller
resource "kubernetes_service_account" "tiller" {
  metadata {
    name      = "tiller"
    namespace = "kube-system"
  }
}

# Grant cluster-admin rights to the Tiller Service Account
resource "kubernetes_cluster_role_binding" "tiller" {
  metadata {
    name = "${kubernetes_service_account.tiller.metadata.0.name}"
  }

  role_ref {
    api_group = "rbac.authorization.k8s.io"
    kind      = "ClusterRole"
    name      = "cluster-admin"
  }

  subject {
    kind      = "ServiceAccount"
    name      = "${kubernetes_service_account.tiller.metadata.0.name}"
    namespace = "kube-system"
  }
}

# Grant cluster-admin rights to the kubernetes-dashboard account
resource "kubernetes_cluster_role_binding" "dashboard" {
  metadata {
    name = "kubernetes-dashboard"
  }

  role_ref {
    api_group = "rbac.authorization.k8s.io"
    kind      = "ClusterRole"
    name      = "cluster-admin"
  }

  subject {
    kind      = "ServiceAccount"
    name      = "kubernetes-dashboard"
    namespace = "kube-system"
  }
}

# Define the helm provider to use the AKS cluster
provider "helm" {
  kubernetes {
    host = "${azurerm_kubernetes_cluster.default.kube_config.0.host}"

    client_certificate     = "${base64decode(azurerm_kubernetes_cluster.default.kube_config.0.client_certificate)}"
    client_key             = "${base64decode(azurerm_kubernetes_cluster.default.kube_config.0.client_key)}"
    cluster_ca_certificate = "${base64decode(azurerm_kubernetes_cluster.default.kube_config.0.cluster_ca_certificate)}"
  }

  service_account = "tiller"
}

# Install a load-balanced nginx-ingress controller onto the cluster
resource "helm_release" "ingress" {
  name      = "nginx-ingress"
  chart     = "stable/nginx-ingress"
  namespace = "kube-system"

  set {
    name  = "controller.replicaCount"
    value = 2
  }

  depends_on = ["kubernetes_cluster_role_binding.tiller"]
}
