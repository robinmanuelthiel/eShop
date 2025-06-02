# Dokumentation der Klasse `CatalogApi`

## Übersicht
Die statische Klasse `CatalogApi` stellt die REST-API-Endpunkte für den Katalogdienst im eShop-Projekt bereit. Sie kapselt die gesamte Logik zur Abfrage, Erstellung, Aktualisierung und Löschung von Katalog-Items sowie zur Verwaltung von Typen und Marken.

## Hauptfunktionen
- **Registrierung der Endpunkte:**
  - `MapCatalogApi`: Registriert alle API-Endpunkte für verschiedene Versionen (v1, v2) und gruppiert sie thematisch.
- **Abfrage von Katalogdaten:**
  - `GetAllItemsV1`, `GetAllItems`: Gibt paginierte Listen von Katalog-Items zurück, optional gefiltert nach Name, Typ oder Marke.
  - `GetItemsByIds`, `GetItemById`, `GetItemsByName`: Verschiedene Methoden zur gezielten Abfrage von Items.
  - `GetItemPictureById`: Gibt das Bild eines Katalog-Items zurück.
  - `GetItemsBySemanticRelevance`, `GetItemsBySemanticRelevanceV1`: Sucht Items mit semantischer Relevanz (AI-gestützt).
  - `GetItemsByBrandAndTypeId`, `GetItemsByBrandId`: Filtert Items nach Typ und/oder Marke.
- **Modifikation von Katalogdaten:**
  - `UpdateItemV1`, `UpdateItem`: Aktualisiert bestehende Katalog-Items.
  - `CreateItem`: Erstellt ein neues Katalog-Item.
  - `DeleteItemById`: Löscht ein Katalog-Item.
- **Hilfsmethoden:**
  - `GetImageMimeTypeFromImageFileExtension`: Liefert den MIME-Typ zu einer Bilddateiendung.
  - `GetFullPath`: Erzeugt den vollständigen Pfad zu einem Bild.

## Besonderheiten
- **Versionierung:** Die API unterstützt mehrere Versionen (v1, v2) mit unterschiedlichen Endpunkt-Definitionen.
- **AI-Unterstützung:** Über die Methoden `GetItemsBySemanticRelevance` und `GetItemsBySemanticRelevanceV1` können semantisch relevante Items mittels Vektor-Ähnlichkeit gesucht werden.
- **Paginierung:** Die meisten Abfrage-Methoden unterstützen Paginierung über das `PaginationRequest`-Objekt.
- **Fehlerbehandlung:** Einheitliche Fehlerantworten mit `ProblemDetails`.

## Beispiel-Endpunkte
- `GET /api/catalog/items` – Liste aller Items (paginierbar, filterbar)
- `GET /api/catalog/items/{id}` – Einzelnes Item nach ID
- `POST /api/catalog/items` – Neues Item anlegen
- `PUT /api/catalog/items/{id}` – Item aktualisieren
- `DELETE /api/catalog/items/{id}` – Item löschen

---

*Diese Datei wurde automatisch generiert und beschreibt die wichtigsten Aspekte der Klasse `CatalogApi` im eShop-Katalogdienst.*
