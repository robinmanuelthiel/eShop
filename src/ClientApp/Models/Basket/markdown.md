# CustomerBasket

Die Klasse `CustomerBasket` repräsentiert den Warenkorb eines Kunden in der eShop-Anwendung.

## Eigenschaften

- **BuyerId**: `string`  
  Die ID des Käufers, dem der Warenkorb gehört.

- **Items**: `IReadOnlyList<BasketItem>`  
  Eine schreibgeschützte Liste aller Artikel im Warenkorb.

- **ItemCount**: `int`  
  Die Gesamtanzahl aller Artikel (über alle Positionen hinweg) im Warenkorb.

## Methoden

- **AddItemToBasket(BasketItem basketItem)**  
  Fügt einen Artikel zum Warenkorb hinzu.  
  - Falls der Artikel bereits im Warenkorb ist, wird die Menge (`Quantity`) erhöht.
  - Andernfalls wird der Artikel neu hinzugefügt.

- **RemoveItemFromBasket(BasketItem basketItem)**  
  Entfernt einen Artikel aus dem Warenkorb anhand der Produkt-ID.

- **ClearBasket()**  
  Entfernt alle Artikel aus dem Warenkorb.

## Beispiel

```csharp
var basket = new CustomerBasket { BuyerId = "user123" };
basket.AddItemToBasket(new BasketItem { ProductId = 1, Quantity = 1 });
Console.WriteLine(basket.ItemCount); // Ausgabe: 1