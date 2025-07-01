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

        #region Compare Functionality Tests (TDD)

        [TestMethod]
        public void CompareTo_ReturnsNegative_WhenCurrentItemPriceIsLower()
        {
            // Arrange
            var item1 = new BasketItem
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "Cheaper Item",
                UnitPrice = 10.0m,
                OldUnitPrice = 12.0m,
                Quantity = 1
            };

            var item2 = new BasketItem
            {
                Id = "item2",
                ProductId = 2,
                ProductName = "Expensive Item",
                UnitPrice = 20.0m,
                OldUnitPrice = 22.0m,
                Quantity = 1
            };

            // Act
            var result = item1.CompareTo(item2);

            // Assert
            Assert.IsTrue(result < 0, "CompareTo should return negative when current item price is lower");
        }

        [TestMethod]
        public void CompareTo_ReturnsPositive_WhenCurrentItemPriceIsHigher()
        {
            // Arrange
            var item1 = new BasketItem
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "Expensive Item",
                UnitPrice = 25.0m,
                OldUnitPrice = 27.0m,
                Quantity = 1
            };

            var item2 = new BasketItem
            {
                Id = "item2",
                ProductId = 2,
                ProductName = "Cheaper Item",
                UnitPrice = 15.0m,
                OldUnitPrice = 17.0m,
                Quantity = 1
            };

            // Act
            var result = item1.CompareTo(item2);

            // Assert
            Assert.IsTrue(result > 0, "CompareTo should return positive when current item price is higher");
        }

        [TestMethod]
        public void CompareTo_ReturnsZero_WhenItemPricesAreEqual()
        {
            // Arrange
            var item1 = new BasketItem
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "Item A",
                UnitPrice = 15.50m,
                OldUnitPrice = 17.0m,
                Quantity = 1
            };

            var item2 = new BasketItem
            {
                Id = "item2",
                ProductId = 2,
                ProductName = "Item B",
                UnitPrice = 15.50m,
                OldUnitPrice = 18.0m,
                Quantity = 2
            };

            // Act
            var result = item1.CompareTo(item2);

            // Assert
            Assert.AreEqual(0, result, "CompareTo should return zero when item prices are equal");
        }

        [TestMethod]
        public void CompareTo_ThrowsArgumentNullException_WhenOtherItemIsNull()
        {
            // Arrange
            var item = new BasketItem
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "Test Item",
                UnitPrice = 10.0m,
                OldUnitPrice = 12.0m,
                Quantity = 1
            };

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => item.CompareTo(null));
        }

        [TestMethod]
        public void IsCheaperThan_ReturnsTrue_WhenCurrentItemPriceIsLower()
        {
            // Arrange
            var cheaperItem = new BasketItem
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "Cheaper Item",
                UnitPrice = 8.99m,
                OldUnitPrice = 10.0m,
                Quantity = 1
            };

            var expensiveItem = new BasketItem
            {
                Id = "item2",
                ProductId = 2,
                ProductName = "Expensive Item",
                UnitPrice = 15.99m,
                OldUnitPrice = 18.0m,
                Quantity = 1
            };

            // Act
            var result = cheaperItem.IsCheaperThan(expensiveItem);

            // Assert
            Assert.IsTrue(result, "IsCheaperThan should return true when current item price is lower");
        }

        [TestMethod]
        public void IsCheaperThan_ReturnsFalse_WhenCurrentItemPriceIsHigherOrEqual()
        {
            // Arrange
            var expensiveItem = new BasketItem
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "Expensive Item",
                UnitPrice = 20.0m,
                OldUnitPrice = 22.0m,
                Quantity = 1
            };

            var cheaperItem = new BasketItem
            {
                Id = "item2",
                ProductId = 2,
                ProductName = "Cheaper Item",
                UnitPrice = 15.0m,
                OldUnitPrice = 17.0m,
                Quantity = 1
            };

            var equalPriceItem = new BasketItem
            {
                Id = "item3",
                ProductId = 3,
                ProductName = "Equal Price Item",
                UnitPrice = 20.0m,
                OldUnitPrice = 25.0m,
                Quantity = 1
            };

            // Act
            var resultHigher = expensiveItem.IsCheaperThan(cheaperItem);
            var resultEqual = expensiveItem.IsCheaperThan(equalPriceItem);

            // Assert
            Assert.IsFalse(resultHigher, "IsCheaperThan should return false when current item price is higher");
            Assert.IsFalse(resultEqual, "IsCheaperThan should return false when current item price is equal");
        }

        [TestMethod]
        public void IsMoreExpensiveThan_ReturnsTrue_WhenCurrentItemPriceIsHigher()
        {
            // Arrange
            var expensiveItem = new BasketItem
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "Expensive Item",
                UnitPrice = 25.99m,
                OldUnitPrice = 28.0m,
                Quantity = 1
            };

            var cheaperItem = new BasketItem
            {
                Id = "item2",
                ProductId = 2,
                ProductName = "Cheaper Item",
                UnitPrice = 12.50m,
                OldUnitPrice = 15.0m,
                Quantity = 1
            };

            // Act
            var result = expensiveItem.IsMoreExpensiveThan(cheaperItem);

            // Assert
            Assert.IsTrue(result, "IsMoreExpensiveThan should return true when current item price is higher");
        }

        [TestMethod]
        public void IsMoreExpensiveThan_ReturnsFalse_WhenCurrentItemPriceIsLowerOrEqual()
        {
            // Arrange
            var cheaperItem = new BasketItem
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "Cheaper Item",
                UnitPrice = 10.0m,
                OldUnitPrice = 12.0m,
                Quantity = 1
            };

            var expensiveItem = new BasketItem
            {
                Id = "item2",
                ProductId = 2,
                ProductName = "Expensive Item",
                UnitPrice = 20.0m,
                OldUnitPrice = 22.0m,
                Quantity = 1
            };

            var equalPriceItem = new BasketItem
            {
                Id = "item3",
                ProductId = 3,
                ProductName = "Equal Price Item",
                UnitPrice = 10.0m,
                OldUnitPrice = 15.0m,
                Quantity = 1
            };

            // Act
            var resultLower = cheaperItem.IsMoreExpensiveThan(expensiveItem);
            var resultEqual = cheaperItem.IsMoreExpensiveThan(equalPriceItem);

            // Assert
            Assert.IsFalse(resultLower, "IsMoreExpensiveThan should return false when current item price is lower");
            Assert.IsFalse(resultEqual, "IsMoreExpensiveThan should return false when current item price is equal");
        }

        [TestMethod]
        public void HasSamePrice_ReturnsTrue_WhenItemPricesAreEqual()
        {
            // Arrange
            var item1 = new BasketItem
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "Item A",
                UnitPrice = 19.99m,
                OldUnitPrice = 22.0m,
                Quantity = 1
            };

            var item2 = new BasketItem
            {
                Id = "item2",
                ProductId = 2,
                ProductName = "Item B",
                UnitPrice = 19.99m,
                OldUnitPrice = 25.0m,
                Quantity = 3
            };

            // Act
            var result = item1.HasSamePrice(item2);

            // Assert
            Assert.IsTrue(result, "HasSamePrice should return true when item prices are exactly equal");
        }

        [TestMethod]
        public void HasSamePrice_ReturnsFalse_WhenItemPricesAreDifferent()
        {
            // Arrange
            var item1 = new BasketItem
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "Item A",
                UnitPrice = 19.99m,
                OldUnitPrice = 22.0m,
                Quantity = 1
            };

            var item2 = new BasketItem
            {
                Id = "item2",
                ProductId = 2,
                ProductName = "Item B",
                UnitPrice = 20.00m,
                OldUnitPrice = 25.0m,
                Quantity = 1
            };

            // Act
            var result = item1.HasSamePrice(item2);

            // Assert
            Assert.IsFalse(result, "HasSamePrice should return false when item prices are different");
        }

        [TestMethod]
        public void CompareByTotalValue_ReturnsNegative_WhenCurrentItemTotalIsLower()
        {
            // Arrange
            var item1 = new BasketItem // Total: 10.0 * 2 = 20.0
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "Item A",
                UnitPrice = 10.0m,
                OldUnitPrice = 12.0m,
                Quantity = 2
            };

            var item2 = new BasketItem // Total: 15.0 * 3 = 45.0
            {
                Id = "item2",
                ProductId = 2,
                ProductName = "Item B",
                UnitPrice = 15.0m,
                OldUnitPrice = 18.0m,
                Quantity = 3
            };

            // Act
            var result = item1.CompareByTotalValue(item2);

            // Assert
            Assert.IsTrue(result < 0, "CompareByTotalValue should return negative when current item total value is lower");
        }

        [TestMethod]
        public void CompareByTotalValue_ReturnsPositive_WhenCurrentItemTotalIsHigher()
        {
            // Arrange
            var item1 = new BasketItem // Total: 20.0 * 4 = 80.0
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "Item A",
                UnitPrice = 20.0m,
                OldUnitPrice = 22.0m,
                Quantity = 4
            };

            var item2 = new BasketItem // Total: 15.0 * 2 = 30.0
            {
                Id = "item2",
                ProductId = 2,
                ProductName = "Item B",
                UnitPrice = 15.0m,
                OldUnitPrice = 18.0m,
                Quantity = 2
            };

            // Act
            var result = item1.CompareByTotalValue(item2);

            // Assert
            Assert.IsTrue(result > 0, "CompareByTotalValue should return positive when current item total value is higher");
        }

        [TestMethod]
        public void CompareByTotalValue_ReturnsZero_WhenItemTotalValuesAreEqual()
        {
            // Arrange
            var item1 = new BasketItem // Total: 10.0 * 3 = 30.0
            {
                Id = "item1",
                ProductId = 1,
                ProductName = "Item A",
                UnitPrice = 10.0m,
                OldUnitPrice = 12.0m,
                Quantity = 3
            };

            var item2 = new BasketItem // Total: 15.0 * 2 = 30.0
            {
                Id = "item2",
                ProductId = 2,
                ProductName = "Item B",
                UnitPrice = 15.0m,
                OldUnitPrice = 18.0m,
                Quantity = 2
            };

            // Act
            var result = item1.CompareByTotalValue(item2);

            // Assert
            Assert.AreEqual(0, result, "CompareByTotalValue should return zero when item total values are equal");
        }

        #endregion
    }
}
