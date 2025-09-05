# BasketItem Klasse

Die `BasketItem`-Klasse repräsentiert einen einzelnen Artikel im Warenkorb eines Benutzers im eShop-System.

## Eigenschaften

| Eigenschaft     | Typ      | Beschreibung                                      |
|----------------|----------|---------------------------------------------------|
| Id             | string   | Eindeutige Kennung des Warenkorb-Artikels         |
| ProductId      | int      | Produkt-ID des Artikels                           |
| ProductName    | string   | Name des Produkts                                 |
| UnitPrice      | decimal  | Aktueller Einzelpreis des Produkts                |
| OldUnitPrice   | decimal  | Vorheriger Einzelpreis des Produkts               |
| Quantity       | int      | Anzahl des Produkts im Warenkorb                  |
| PictureUrl     | string   | Bild-URL des Produkts                             |

## Methoden

### Validate

Validiert die Eigenschaften des Warenkorb-Artikels. Folgende Prüfungen werden durchgeführt:
- **Quantity** muss mindestens 1 sein
- **UnitPrice** darf nicht negativ sein
- **ProductName** darf nicht leer oder null sein
- **ProductId** darf nicht negativ sein

Gibt eine Liste von Validierungsfehlern zurück, falls ungültige Werte gefunden werden.

## Beispiel
```csharp
var item = new BasketItem {
    Id = "123",
    ProductId = 42,
    ProductName = "T-Shirt",
    UnitPrice = 19.99m,
    OldUnitPrice = 24.99m,
    Quantity = 2,
    PictureUrl = "https://example.com/tshirt.png"
};
```

## Verwendung
Die Klasse wird verwendet, um einzelne Produkte im Warenkorb eines Benutzers zu speichern und zu validieren.
