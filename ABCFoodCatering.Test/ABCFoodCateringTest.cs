using ABCFoodCatering.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ABCFoodCatering.Test
{
    public class ABCFoodCateringTest
    {
        [Fact]
        public void ChangeFoodDesc()
        {
            // Arrange
            var orderDesc = new Order { FoodDescription = "Food Test 55" };
            // Act
            orderDesc.FoodDescription = "New Food Item 60";
            // Assert
            Assert.Equal("New Food Item 60", orderDesc.FoodDescription);
        }

        [Fact]
        public void ChangeDeliveryAddr()
        {
            // Arrange
            var orderAddr = new Order { DeliveryAddress = "Test Address" };
            // Act
            orderAddr.DeliveryAddress = "New Address";
            //Assert
            Assert.Equal("New Address", orderAddr.DeliveryAddress);
        }

        [Fact]
        public void ChangeOrderQuantity()
        {
            // Arrange
            var orderQty = new Order { Quantity = 2 };
            // Act
            orderQty.Quantity = 20;
            // Assert
            Assert.Equal(20, orderQty.Quantity);
        }

        [Fact]
        public void ChangeClientName()
        {
            // Arrange
            var clientName = new Order { ClientName = "JohnXina" };
            // Act
            clientName.ClientName = "Zhong Xi Na";
            // Asset
            Assert.Equal("Zhong Xi Na", clientName.ClientName);
        }
    }
}
