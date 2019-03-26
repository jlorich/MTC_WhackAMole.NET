# Virtual Network to deploy resources into
resource "azurerm_virtual_network" "default" {
  name                = "${var.name}-vnet"
  location            = "${azurerm_resource_group.default.location}"
  resource_group_name = "${azurerm_resource_group.default.name}"
  address_space       = ["${var.vnet_address_space}"]
}

# Subnets
resource "azurerm_subnet" "aks" {
  name                      = "${var.name}-aks-subnet"
  resource_group_name       = "${azurerm_resource_group.default.name}"
  address_prefix            = "${var.vnet_aks_subnet_space}"
  virtual_network_name      = "${azurerm_virtual_network.default.name}"
}

resource "azurerm_subnet" "ingress" {
  name                      = "${var.name}-ingress-subnet"
  resource_group_name       = "${azurerm_resource_group.default.name}"
  virtual_network_name      = "${azurerm_virtual_network.default.name}"
  address_prefix            = "${var.vnet_ingress_subnet_space}"
}

resource "azurerm_subnet" "gateway" {
  name                      = "${var.name}-gateway-subnet"
  resource_group_name       = "${azurerm_resource_group.default.name}"
  virtual_network_name      = "${azurerm_virtual_network.default.name}"
  address_prefix            = "${var.vnet_gateway_subnet_space}"
}


# Network security groups
resource azurerm_network_security_group "aks" {
  name                = "${var.name}-aks-nsg"
  location            = "${azurerm_resource_group.default.location}"
  resource_group_name = "${azurerm_resource_group.default.name}"
}

resource azurerm_network_security_group "ingress" {
  name                = "${var.name}-ingress-nsg"
  location            = "${azurerm_resource_group.default.location}"
  resource_group_name = "${azurerm_resource_group.default.name}"
}

resource azurerm_network_security_group "gateway" {
  name                = "${var.name}-gateway-nsg"
  location            = "${azurerm_resource_group.default.location}"
  resource_group_name = "${azurerm_resource_group.default.name}"
}

# Network security group associations

resource "azurerm_subnet_network_security_group_association" "aks" {
  subnet_id                 = "${azurerm_subnet.aks.id}"
  network_security_group_id = "${azurerm_network_security_group.aks.id}"
}

resource "azurerm_subnet_network_security_group_association" "ingress" {
  subnet_id                 = "${azurerm_subnet.ingress.id}"
  network_security_group_id = "${azurerm_network_security_group.ingress.id}"
}

resource "azurerm_subnet_network_security_group_association" "gateway" {
  subnet_id                 = "${azurerm_subnet.gateway.id}"
  network_security_group_id = "${azurerm_network_security_group.gateway.id}"
}