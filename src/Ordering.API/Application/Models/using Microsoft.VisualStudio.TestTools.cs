using Microsoft.VisualStudio.TestTools.UnitTesting;
using eShop.Ordering.API.Application.Models;

namespace eShop.Ordering.UnitTests.Application.Models
{
    [TestClass]
    public class BasketItemTest
    {
        [TestMethod]
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
            Assert.AreEqual(id, item.Id);
            Assert.AreEqual(productId, item.ProductId);
            Assert.AreEqual(productName, item.ProductName);
            Assert.AreEqual(unitPrice, item.UnitPrice);
            Assert.AreEqual(oldUnitPrice, item.OldUnitPrice);
            Assert.AreEqual(quantity, item.Quantity);
            Assert.AreEqual(pictureUrl, item.PictureUrl);
        }

        [TestMethod]
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

            Assert.AreEqual("item2", item.Id);
            Assert.AreEqual(99, item.ProductId);
            Assert.AreEqual("Immutable Product", item.ProductName);
            Assert.AreEqual(20.0m, item.UnitPrice);
            Assert.AreEqual(25.0m, item.OldUnitPrice);
            Assert.AreEqual(1, item.Quantity);
            Assert.AreEqual("http://example.com/immutable.png", item.PictureUrl);
        }
    }
}