# BasketItem Klasse Dokumentation

## Übersicht

Die `BasketItem` Klasse repräsentiert einen einzelnen Artikel in einem Einkaufskorb und enthält alle notwendigen Produktinformationen, Preisdetails und Mengenangaben. Sie implementiert die `IValidatableObject` Schnittstelle, um benutzerdefinierte Validierungslogik für die Eigenschaften des Korb-Artikels bereitzustellen.

## Namespace

```csharp
eShop.Basket.API.Model
```

## Klassendeklaration

```csharp
public class BasketItem : IValidatableObject
```

## Zweck

Diese Klasse wird verwendet, um einzelne Produktartikel innerhalb des Einkaufskorbs eines Kunden zu speichern, einschließlich aktueller und vorheriger Preisinformationen für Vergleichszwecke.

## Eigenschaften

### Id
- **Typ**: `string`
- **Zugriff**: `get; set;`
- **Beschreibung**: Eindeutige Identifikation des Korb-Artikels

### ProductId
- **Typ**: `int`
- **Zugriff**: `get; set;`
- **Beschreibung**: Eindeutige Identifikation des Produkts im Katalog

### ProductName
- **Typ**: `string`
- **Zugriff**: `get; set;`
- **Beschreibung**: Name des Produkts für die Anzeige

### UnitPrice
- **Typ**: `decimal`
- **Zugriff**: `get; set;`
- **Beschreibung**: Aktueller Preis pro Einheit des Produkts

### OldUnitPrice
- **Typ**: `decimal`
- **Zugriff**: `get; set;`
- **Beschreibung**: Vorheriger Preis pro Einheit für Preisvergleiche

### Quantity
- **Typ**: `int`
- **Zugriff**: `get; set;`
- **Beschreibung**: Anzahl der Einheiten dieses Produkts im Korb
- **Validierung**: Muss mindestens 1 sein

### PictureUrl
- **Typ**: `string`
- **Zugriff**: `get; set;`
- **Beschreibung**: URL zum Produktbild für die Anzeige

## Methoden

### Validate(ValidationContext validationContext)

```csharp
public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
```

**Beschreibung**: Implementiert die `IValidatableObject.Validate` Methode zur benutzerdefinierten Validierung der Korb-Artikel-Eigenschaften.

**Parameter**:
- `validationContext`: Der Kontext für die Validierung

**Rückgabe**: 
- `IEnumerable<ValidationResult>`: Eine Sammlung von Validierungsfehlern, falls vorhanden

**Validierungsregeln**:
- Die `Quantity` muss mindestens 1 sein
- Bei ungültiger Menge wird ein `ValidationResult` mit der Nachricht "Invalid number of units" zurückgegeben

## Verwendungsbeispiele

### Erstellen eines neuen BasketItem

```csharp
var basketItem = new BasketItem
{
    Id = Guid.NewGuid().ToString(),
    ProductId = 123,
    ProductName = "Smartphone XYZ",
    UnitPrice = 599.99m,
    OldUnitPrice = 699.99m,
    Quantity = 2,
    PictureUrl = "https://example.com/images/smartphone-xyz.jpg"
};
```

### Validierung eines BasketItem

```csharp
var validationContext = new ValidationContext(basketItem);
var validationResults = basketItem.Validate(validationContext);

if (validationResults.Any())
{
    foreach (var result in validationResults)
    {
        Console.WriteLine($"Validierungsfehler: {result.ErrorMessage}");
    }
}
```

### Berechnung des Gesamtpreises

```csharp
decimal totalPrice = basketItem.UnitPrice * basketItem.Quantity;
```

### Preisvergleich

```csharp
if (basketItem.OldUnitPrice > basketItem.UnitPrice)
{
    decimal savings = (basketItem.OldUnitPrice - basketItem.UnitPrice) * basketItem.Quantity;
    Console.WriteLine($"Sie sparen: {savings:C}");
}
```

## Validierungsverhalten

Die Klasse implementiert folgende Validierungslogik:

1. **Mengenvalidierung**: Die `Quantity` muss mindestens 1 sein
2. **Fehlerbehandlung**: Ungültige Werte führen zu aussagekräftigen Fehlermeldungen
3. **Flexible Validierung**: Durch `IValidatableObject` können weitere Validierungsregeln einfach hinzugefügt werden

## Abhängigkeiten

- `System.ComponentModel.DataAnnotations` für die Validierung
- `IValidatableObject` Interface für benutzerdefinierte Validierung

## Architekturhinweise

- **Domain Model**: Repräsentiert eine Geschäftsentität im Basket-Kontext
- **Datenübertragung**: Kann als DTO zwischen verschiedenen Schichten verwendet werden
- **Serialisierung**: Alle Eigenschaften sind öffentlich und serialisierungsfreundlich
- **Unveränderlichkeit**: Die Klasse ist mutable, was Flexibilität bei Updates ermöglicht

## Best Practices

1. **Validierung**: Nutzen Sie die `Validate` Methode vor dem Speichern
2. **Null-Checks**: Prüfen Sie auf null-Werte bei String-Eigenschaften
3. **Preisberechnung**: Verwenden Sie `decimal` für monetäre Werte
4. **Fehlerbehandlung**: Behandeln Sie Validierungsfehler angemessen

## Siehe auch

- `CustomerBasket` - Container für mehrere BasketItem-Objekte
- `IValidatableObject` - Interface für benutzerdefinierte Validierung
- `ValidationResult` - Repräsentiert Validierungsergebnisse