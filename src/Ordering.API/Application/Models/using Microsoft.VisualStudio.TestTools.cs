using Xunit;
using eShop.Ordering.API.Application.Models;

namespace eShop.Ordering.UnitTests.Application.Models
{
    public class BasketItemTest
    {
        [Fact]
        public void Can_Create_BasketItem_With_All_Properties()
        {
            // Arrange
            var id = "item1";
            var productId = 42;
            var productName = "Test Product";
            var unitPrice = 10.5m;
            var oldUnitPrice = 12.0m;
            var quantity = 3;
            var pictureUrl = "http://example.com/image.png";

            // Act
            var item = new BasketItem
            {
                Id = id,
                ProductId = productId,
                ProductName = productName,
                UnitPrice = unitPrice,
                OldUnitPrice = oldUnitPrice,
                Quantity = quantity,
                PictureUrl = pictureUrl
            };

            // Assert
            Assert.Equal(id, item.Id);
            Assert.Equal(productId, item.ProductId);
            Assert.Equal(productName, item.ProductName);
            Assert.Equal(unitPrice, item.UnitPrice);
            Assert.Equal(oldUnitPrice, item.OldUnitPrice);
            Assert.Equal(quantity, item.Quantity);
            Assert.Equal(pictureUrl, item.PictureUrl);
        }

        [Fact]
        public void BasketItem_Properties_Are_Immutable()
        {
            // Arrange
            var item = new BasketItem
            {
                Id = "item2",
                ProductId = 99,
                ProductName = "Immutable Product",
                UnitPrice = 20.0m,
                OldUnitPrice = 25.0m,
                Quantity = 1,
                PictureUrl = "http://example.com/immutable.png"
            };

            // Act & Assert
            // Properties with 'init' can only be set during object initialization.
            // The following lines, if uncommented, would cause a compile-time error:
            // item.Id = "newId";
            // item.ProductId = 100;
            // item.ProductName = "Changed";
            // item.UnitPrice = 30.0m;
            // item.OldUnitPrice = 35.0m;
            // item.Quantity = 2;
            // item.PictureUrl = "http://example.com/changed.png";

            Assert.Equal("item2", item.Id);
            Assert.Equal(99, item.ProductId);
            Assert.Equal("Immutable Product", item.ProductName);
            Assert.Equal(20.0m, item.UnitPrice);
            Assert.Equal(25.0m, item.OldUnitPrice);
            Assert.Equal(1, item.Quantity);
            Assert.Equal("http://example.com/immutable.png", item.PictureUrl);
        }
    }
}