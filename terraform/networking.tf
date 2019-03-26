# Virtual Network to deploy resources into
resource "azurerm_virtual_network" "default" {
  name                = "${var.name}-vnet"
  location            = "${azurerm_resource_group.default.location}"
  resource_group_name = "${azurerm_resource_group.default.name}"
  address_space       = ["${var.vnet_address_space}"]
}

# Security group and subnet for AKS internal services
resource azurerm_network_security_group "aks" {
  name                = "${var.name}-aks-nsg"
  location            = "${azurerm_resource_group.default.location}"
  resource_group_name = "${azurerm_resource_group.default.name}"
}

resource "azurerm_subnet" "aks" {
  name                      = "{var.name}-aks-subnet"
  resource_group_name       = "${azurerm_resource_group.default.name}"
  network_security_group_id = "${azurerm_network_security_group.aks.id}"
  address_prefix            = "${var.vnet_aks_subnet_space}"
  virtual_network_name      = "${azurerm_virtual_network.default.name}"
}

# Security group and subnet for AKS ingress
resource azurerm_network_security_group "ingress" {
  name                = "${var.name}-ingress-nsg"
  location            = "${azurerm_resource_group.default.location}"
  resource_group_name = "${azurerm_resource_group.default.name}"
}

resource "azurerm_subnet" "ingress" {
  name                      = "${var.name}-ingress-subnet"
  resource_group_name       = "${azurerm_resource_group.default.name}"
  network_security_group_id = "${azurerm_network_security_group.ingress.id}"
  virtual_network_name      = "${azurerm_virtual_network.default.name}"
  address_prefix            = "${var.vnet_ingress_subnet_space}"
}

# Security group and subnet for Gateways
resource azurerm_network_security_group "gateway" {
  name                = "${var.name}-gateway-nsg"
  location            = "${azurerm_resource_group.default.location}"
  resource_group_name = "${azurerm_resource_group.default.name}"
}

resource "azurerm_subnet" "gateway" {
  name                      = "${var.name}-gateway-subnet"
  resource_group_name       = "${azurerm_resource_group.default.name}"
  network_security_group_id = "${azurerm_network_security_group.gateway.id}"
  virtual_network_name      = "${azurerm_virtual_network.default.name}"
  address_prefix            = "${var.vnet_gateway_subnet_space}"
}
