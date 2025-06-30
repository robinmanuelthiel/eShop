# BasketItem Class Documentation

## Overview

The `BasketItem` class represents an individual item within a shopping basket in the eShop application. It implements the `IValidatableObject` interface to provide custom validation logic for basket items.

## Namespace

```csharp
eShop.Basket.API.Model
```

## Class Declaration

```csharp
public class BasketItem : IValidatableObject
```

## Properties

### Id
- **Type**: `string`
- **Description**: Unique identifier for the basket item
- **Access**: Get/Set

### ProductId
- **Type**: `int`
- **Description**: The unique identifier of the product being added to the basket
- **Access**: Get/Set

### ProductName
- **Type**: `string`
- **Description**: The display name of the product
- **Access**: Get/Set

### UnitPrice
- **Type**: `decimal`
- **Description**: The current price per unit of the product
- **Access**: Get/Set

### OldUnitPrice
- **Type**: `decimal`
- **Description**: The previous price per unit of the product (used for price comparison/discount display)
- **Access**: Get/Set

### Quantity
- **Type**: `int`
- **Description**: The number of units of this product in the basket
- **Access**: Get/Set
- **Validation**: Must be greater than 0

### PictureUrl
- **Type**: `string`
- **Description**: URL to the product's image/picture
- **Access**: Get/Set

## Methods

### Validate(ValidationContext validationContext)

Implements the `IValidatableObject.Validate` method to provide custom validation logic for the basket item.

#### Parameters
- `validationContext` (`ValidationContext`): The validation context providing additional information about the validation operation

#### Returns
- `IEnumerable<ValidationResult>`: A collection of validation results, empty if validation passes

#### Validation Rules
- **Quantity Validation**: Ensures the quantity is at least 1 unit
  - **Error Message**: "Invalid number of units"
  - **Property**: "Quantity"

#### Example Usage

```csharp
var basketItem = new BasketItem
{
    Id = "item-123",
    ProductId = 1,
    ProductName = "Sample Product",
    UnitPrice = 29.99m,
    OldUnitPrice = 34.99m,
    Quantity = 2,
    PictureUrl = "https://example.com/product.jpg"
};

var validationContext = new ValidationContext(basketItem);
var validationResults = basketItem.Validate(validationContext);

if (validationResults.Any())
{
    // Handle validation errors
    foreach (var result in validationResults)
    {
        Console.WriteLine($"Error: {result.ErrorMessage}");
    }
}
```

## Usage Context

The `BasketItem` class is primarily used within the Basket API service of the eShop application to:

1. **Represent basket contents**: Store individual items that users have added to their shopping basket
2. **Price tracking**: Maintain both current and previous prices for discount calculations
3. **Validation**: Ensure basket items meet business rules before processing
4. **API serialization**: Transfer basket item data between client and server

## Dependencies

- `System.ComponentModel.DataAnnotations`: For `IValidatableObject`, `ValidationContext`, and `ValidationResult`
- `System.Collections.Generic`: For `List<T>` and `IEnumerable<T>`

## Design Patterns

- **Data Transfer Object (DTO)**: Simple data container with properties and minimal logic
- **Validation Pattern**: Implements `IValidatableObject` for custom validation logic

## Related Classes

- `CustomerBasket`: Contains collections of `BasketItem` objects
- `BasketController`: Manages operations on baskets containing `BasketItem` instances

## File Location

```
/workspaces/eShop/src/Basket.API/Model/BasketItem.cs
```

## Last Updated

June 30, 2025
