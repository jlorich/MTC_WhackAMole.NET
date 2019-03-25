resource "azurerm_resource_group" "default" {
  name     = "${var.name}-${var.environment}-rg"
  location = "${var.location}"

  tags = "${var.tags}"
}

resource "azurerm_application_insights" "default" {
  name                = "${var.name}-ai"
  location            = "${azurerm_resource_group.default.location}"
  resource_group_name = "${azurerm_resource_group.default.name}"
  application_type    = "Web"

  tags = "${var.tags}"
}

resource "azurerm_log_analytics_workspace" "default" {
  name                = "${var.name}-law"
  location            = "${azurerm_resource_group.default.location}"
  resource_group_name = "${azurerm_resource_group.default.name}"
  sku                 = "PerGB2018"
  retention_in_days   = 30

  tags = "${var.tags}"
}

resource "azurerm_log_analytics_solution" "test" {
  solution_name         = "ContainerInsights"
  location              = "${azurerm_log_analytics_workspace.default.location}"
  resource_group_name   = "${azurerm_resource_group.default.name}"
  workspace_resource_id = "${azurerm_log_analytics_workspace.default.id}"
  workspace_name        = "${azurerm_log_analytics_workspace.default.name}"

  plan {
    publisher = "Microsoft"
    product   = "OMSGallery/ContainerInsights"
  }
}

resource "azurerm_kubernetes_cluster" "default" {
  name                = "${var.name}-aks"
  location            = "${azurerm_resource_group.default.location}"
  resource_group_name = "${azurerm_resource_group.default.name}"
  dns_prefix          = "${var.name}-aks-${var.environment}"
  depends_on          = ["azurerm_role_assignment.default"]

  agent_pool_profile {
    name            = "default"
    count           = "${var.node_count}"
    vm_size         = "${var.node_type}"
    os_type         = "${var.node_os}"
    os_disk_size_gb = 30
  }

  service_principal {
    client_id     = "${azuread_application.default.application_id}"
    client_secret = "${azuread_service_principal_password.default.value}"
  }

  addon_profile {
    oms_agent {
      enabled                    = true
      log_analytics_workspace_id = "${azurerm_log_analytics_workspace.default.id}"
    }

    http_application_routing {
      enabled = true
    }
  }

  role_based_access_control {
    enabled = true
  }

  tags = "${var.tags}"
}
