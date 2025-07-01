using Microsoft.VisualStudio.TestTools.UnitTesting;
using eShop.WebApp.Services;

namespace eShop.WebApp.Services.Tests
{
    [TestClass]
    public class BasketItemTest
    {
        [TestMethod]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            // Arrange
            var item = new BasketItem
            {
                Id = "item1",
                ProductId = 42,
                ProductName = "Test Product",
                UnitPrice = 10.5m,
                OldUnitPrice = 12.0m,
                Quantity = 3
            };

            // Assert
            Assert.AreEqual("item1", item.Id);
            Assert.AreEqual(42, item.ProductId);
            Assert.AreEqual("Test Product", item.ProductName);
            Assert.AreEqual(10.5m, item.UnitPrice);
            Assert.AreEqual(12.0m, item.OldUnitPrice);
            Assert.AreEqual(3, item.Quantity);
        }

        [TestMethod]
        public void CamelCaseProductName_ReturnsCamelCase_WhenProductNameHasSpaces()
        {
            // Arrange
            var item = new BasketItem
            {
                Id = "item2",
                ProductId = 1,
                ProductName = "sample product name",
                UnitPrice = 1m,
                OldUnitPrice = 2m,
                Quantity = 1
            };

            // Act
            var camelCase = item.CamelCaseProductName;

            // Assert
            Assert.AreEqual("SampleProductName", camelCase);
        }

        [TestMethod]
        public void CamelCaseProductName_ReturnsEmpty_WhenProductNameIsNullOrWhitespace()
        {
            // Arrange
            var item1 = new BasketItem
            {
                Id = "item3",
                ProductId = 1,
                ProductName = null!,
                UnitPrice = 1m,
                OldUnitPrice = 2m,
                Quantity = 1
            };

            var item2 = new BasketItem
            {
                Id = "item4",
                ProductId = 1,
                ProductName = "",
                UnitPrice = 1m,
                OldUnitPrice = 2m,
                Quantity = 1
            };

            var item3 = new BasketItem
            {
                Id = "item5",
                ProductId = 1,
                ProductName = "   ",
                UnitPrice = 1m,
                OldUnitPrice = 2m,
                Quantity = 1
            };

            // Assert
            Assert.AreEqual(string.Empty, item1.CamelCaseProductName);
            Assert.AreEqual(string.Empty, item2.CamelCaseProductName);
            Assert.AreEqual(string.Empty, item3.CamelCaseProductName);
        }

        [TestMethod]
        public void CamelCaseProductName_ReturnsSingleWordCapitalized_WhenProductNameIsSingleWord()
        {
            // Arrange
            var item = new BasketItem
            {
                Id = "item6",
                ProductId = 1,
                ProductName = "singleword",
                UnitPrice = 1m,
                OldUnitPrice = 2m,
                Quantity = 1
            };

            // Act
            var camelCase = item.CamelCaseProductName;

            // Assert
            Assert.AreEqual("Singleword", camelCase);
        }

        #region Price Comparison Tests (TDD)

        [TestMethod]
        public void ComparePriceTo_ReturnsPositive_WhenCurrentItemIsMoreExpensive()
        {
            // Arrange
            var expensiveItem = new BasketItem
            {
                Id = "expensive",
                ProductId = 1,
                ProductName = "Expensive Item",
                UnitPrice = 100m,
                OldUnitPrice = 110m,
                Quantity = 1
            };

            var cheapItem = new BasketItem
            {
                Id = "cheap",
                ProductId = 2,
                ProductName = "Cheap Item",
                UnitPrice = 50m,
                OldUnitPrice = 60m,
                Quantity = 1
            };

            // Act
            var result = expensiveItem.ComparePriceTo(cheapItem);

            // Assert
            Assert.IsTrue(result > 0, "ComparePriceTo should return positive value when current item is more expensive");
        }

        [TestMethod]
        public void ComparePriceTo_ReturnsNegative_WhenCurrentItemIsCheaper()
        {
            // Arrange
            var cheapItem = new BasketItem
            {
                Id = "cheap",
                ProductId = 1,
                ProductName = "Cheap Item",
                UnitPrice = 25m,
                OldUnitPrice = 30m,
                Quantity = 1
            };

            var expensiveItem = new BasketItem
            {
                Id = "expensive",
                ProductId = 2,
                ProductName = "Expensive Item",
                UnitPrice = 75m,
                OldUnitPrice = 80m,
                Quantity = 1
            };

            // Act
            var result = cheapItem.ComparePriceTo(expensiveItem);

            // Assert
            Assert.IsTrue(result < 0, "ComparePriceTo should return negative value when current item is cheaper");
        }

        [TestMethod]
        public void ComparePriceTo_ReturnsZero_WhenItemsHaveSamePrice()
        {
            // Arrange
            var item1 = new BasketItem
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "Item One",
                UnitPrice = 50m,
                OldUnitPrice = 60m,
                Quantity = 1
            };

            var item2 = new BasketItem
            {
                Id = "item2",
                ProductId = 2,
                ProductName = "Item Two",
                UnitPrice = 50m,
                OldUnitPrice = 55m,
                Quantity = 2
            };

            // Act
            var result = item1.ComparePriceTo(item2);

            // Assert
            Assert.AreEqual(0, result, "ComparePriceTo should return zero when items have the same price");
        }

        [TestMethod]
        public void ComparePriceTo_ThrowsArgumentNullException_WhenOtherItemIsNull()
        {
            // Arrange
            var item = new BasketItem
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "Test Item",
                UnitPrice = 50m,
                OldUnitPrice = 60m,
                Quantity = 1
            };

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => item.ComparePriceTo(null!));
        }

        [TestMethod]
        public void IsMoreExpensiveThan_ReturnsTrue_WhenCurrentItemIsMoreExpensive()
        {
            // Arrange
            var expensiveItem = new BasketItem
            {
                Id = "expensive",
                ProductId = 1,
                ProductName = "Expensive Item",
                UnitPrice = 150m,
                OldUnitPrice = 160m,
                Quantity = 1
            };

            var cheapItem = new BasketItem
            {
                Id = "cheap",
                ProductId = 2,
                ProductName = "Cheap Item",
                UnitPrice = 75m,
                OldUnitPrice = 80m,
                Quantity = 1
            };

            // Act
            var result = expensiveItem.IsMoreExpensiveThan(cheapItem);

            // Assert
            Assert.IsTrue(result, "IsMoreExpensiveThan should return true when current item is more expensive");
        }

        [TestMethod]
        public void IsMoreExpensiveThan_ReturnsFalse_WhenCurrentItemIsCheaperOrEqual()
        {
            // Arrange
            var cheapItem = new BasketItem
            {
                Id = "cheap",
                ProductId = 1,
                ProductName = "Cheap Item",
                UnitPrice = 40m,
                OldUnitPrice = 45m,
                Quantity = 1
            };

            var expensiveItem = new BasketItem
            {
                Id = "expensive",
                ProductId = 2,
                ProductName = "Expensive Item",
                UnitPrice = 90m,
                OldUnitPrice = 95m,
                Quantity = 1
            };

            var equalPriceItem = new BasketItem
            {
                Id = "equal",
                ProductId = 3,
                ProductName = "Equal Price Item",
                UnitPrice = 40m,
                OldUnitPrice = 50m,
                Quantity = 1
            };

            // Act & Assert
            Assert.IsFalse(cheapItem.IsMoreExpensiveThan(expensiveItem), "Should return false when current item is cheaper");
            Assert.IsFalse(cheapItem.IsMoreExpensiveThan(equalPriceItem), "Should return false when current item has equal price");
        }

        [TestMethod]
        public void IsCheaperThan_ReturnsTrue_WhenCurrentItemIsCheaper()
        {
            // Arrange
            var cheapItem = new BasketItem
            {
                Id = "cheap",
                ProductId = 1,
                ProductName = "Cheap Item",
                UnitPrice = 30m,
                OldUnitPrice = 35m,
                Quantity = 1
            };

            var expensiveItem = new BasketItem
            {
                Id = "expensive",
                ProductId = 2,
                ProductName = "Expensive Item",
                UnitPrice = 120m,
                OldUnitPrice = 125m,
                Quantity = 1
            };

            // Act
            var result = cheapItem.IsCheaperThan(expensiveItem);

            // Assert
            Assert.IsTrue(result, "IsCheaperThan should return true when current item is cheaper");
        }

        [TestMethod]
        public void IsCheaperThan_ReturnsFalse_WhenCurrentItemIsMoreExpensiveOrEqual()
        {
            // Arrange
            var expensiveItem = new BasketItem
            {
                Id = "expensive",
                ProductId = 1,
                ProductName = "Expensive Item",
                UnitPrice = 200m,
                OldUnitPrice = 210m,
                Quantity = 1
            };

            var cheapItem = new BasketItem
            {
                Id = "cheap",
                ProductId = 2,
                ProductName = "Cheap Item",
                UnitPrice = 80m,
                OldUnitPrice = 85m,
                Quantity = 1
            };

            var equalPriceItem = new BasketItem
            {
                Id = "equal",
                ProductId = 3,
                ProductName = "Equal Price Item",
                UnitPrice = 200m,
                OldUnitPrice = 220m,
                Quantity = 1
            };

            // Act & Assert
            Assert.IsFalse(expensiveItem.IsCheaperThan(cheapItem), "Should return false when current item is more expensive");
            Assert.IsFalse(expensiveItem.IsCheaperThan(equalPriceItem), "Should return false when current item has equal price");
        }

        [TestMethod]
        public void HasSamePriceAs_ReturnsTrue_WhenItemsHaveSamePrice()
        {
            // Arrange
            var item1 = new BasketItem
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "First Item",
                UnitPrice = 99.99m,
                OldUnitPrice = 109.99m,
                Quantity = 1
            };

            var item2 = new BasketItem
            {
                Id = "item2",
                ProductId = 2,
                ProductName = "Second Item",
                UnitPrice = 99.99m,
                OldUnitPrice = 104.99m,
                Quantity = 3
            };

            // Act
            var result = item1.HasSamePriceAs(item2);

            // Assert
            Assert.IsTrue(result, "HasSamePriceAs should return true when items have the same price");
        }

        [TestMethod]
        public void HasSamePriceAs_ReturnsFalse_WhenItemsHaveDifferentPrices()
        {
            // Arrange
            var item1 = new BasketItem
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "First Item",
                UnitPrice = 45.50m,
                OldUnitPrice = 50m,
                Quantity = 1
            };

            var item2 = new BasketItem
            {
                Id = "item2",
                ProductId = 2,
                ProductName = "Second Item",
                UnitPrice = 45.51m,
                OldUnitPrice = 48m,
                Quantity = 1
            };

            // Act
            var result = item1.HasSamePriceAs(item2);

            // Assert
            Assert.IsFalse(result, "HasSamePriceAs should return false when items have different prices");
        }

        #endregion
    }
}
