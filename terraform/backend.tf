terraform {
  backend "azurerm" {
    container_name = "tfstate"
    key            = "demo-whack-a-mole.tfstate"
  }
}
