resource "azurerm_api_management" "default" {
  name                = "${var.name}-apim"
  location            = "${azurerm_resource_group.default.location}"
  resource_group_name = "${azurerm_resource_group.default.name}"
  publisher_name      = "MTC Denver"
  publisher_email     = "jolorich@microsoft.com"

  sku {
    name     = "Developer"
    capacity = 1
  }
}
