using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eShop.Basket.API.Model
{
    /// <summary>
    /// Represents an item stored in a shopping basket.
    /// Contains product identification, pricing and quantity information
    /// and implements validation logic for business rules.
    /// </summary>
    public class BasketItem : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the unique identifier for the basket item instance.
        /// This identifier is specific to the basket entry and not necessarily the product id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the product identifier that this basket item references.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the product.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the current unit price for the product.
        /// </summary>
        public decimal UnitPrice { get; set; }


        /// <summary>
        /// Gets or sets the previous unit price for the product.
        /// Used to show discounts or price changes.
        /// </summary>
        public decimal OldUnitPrice { get; set; }

        /// <summary>
        /// Gets or sets the number of units of the product in the basket.
        /// Must be greater than or equal to 1.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the URL of the product picture to be displayed in the basket UI.
        /// </summary>
        public string PictureUrl { get; set; }

        /// <summary>
        /// Validates the basket item for business rules.
        /// Returns a collection of <see cref="ValidationResult"/> indicating validation failures.
        /// </summary>
        /// <param name="validationContext">Context information about the validation operation.</param>
        /// <returns>
        /// An <see cref="IEnumerable{ValidationResult}"/> containing validation errors.
        /// An empty collection indicates the item is valid.
        /// </returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (Quantity < 1)
            {
                results.Add(new ValidationResult("Invalid number of units", new[] { "Quantity" }));
            }

            return results;
        }
    }
}
