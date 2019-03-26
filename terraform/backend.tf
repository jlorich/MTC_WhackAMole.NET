terraform {
  backend "azurerm" {
    container_name = "tfstate"
    key            = "${var.name}-${var.environment}.tfstate"
  }
}
