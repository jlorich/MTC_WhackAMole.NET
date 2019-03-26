resource "azurerm_resource_group" "default" {
  name     = "${var.name}-${var.environment}-rg"
  location = "${var.location}"

  tags = "${var.tags}"
}

data "azurerm_subscription" "current" {}
