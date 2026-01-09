using Xunit;
using eShop.Ordering.API.Application.Models;

namespace Ordering.API.Tests.Application.Models
{
    public class BasketItemTests
    {
        [Fact]
        public void BasketItem_CanBeCreated_WithValidProperties()
        {
            // Arrange
            var id = "item-1";
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
    }
}
