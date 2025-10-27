#10 | E-Commerce Inventory Management System DATA STRUCTURE
Erstellen Sie ein Lagerverwaltungssystem für einen E-Commerce-Shop mit Datentypen wie String
für SKU und Produktnamen, Int für Lagerbestand, Decimal für Preise und DateTime für Daten.
String-Operationen sollen SKU-Codes validieren, Produktbeschreibungen zusammensetzen, Barcode-Daten generieren und Lagerstandortcodes formatieren. Datum-/Zeitoperationen müssen Lagerumschlagsraten und Lagerdauer berechnen, Ablaufdaten überwachen sowie Nachbestellzeitpunkte ermitteln. Definieren Sie Enums für ProductCategory (Electronics, Clothing, Food, HomeGarden, Sports, Books), StockStatus (InStock, LowStock, OutOfStock, Discontinued, Backordered), WarehouseLocation (MainWarehouse, RegionalCenter, ReturnCenter) und ProductCondition
(New, Refurbished, OpenBox, Damaged). Erstellen Sie eine IInventoryManager-Schnittstelle mit
AddStock, RemoveStock, TransferStock und CheckStockLevel. Überladen Sie die CalculateReorderPoint-Methode für verschiedene Berechnungsmodelle: Standard-Formel, saisonale Anpassung
oder historische Daten. Überschreiben Sie ToString für die Product-Klasse zur formatierten Produktübersicht. Nutzen Sie Generalisierung durch eine generische Klasse Warehouse<T> where T :
IStorable. Verwenden Sie ein Array für Lagerebenen, eine List<Product> für den Produktkatalog,
ein HashSet<string> für eindeutige SKU-Codes, ein Dictionary<string, InventoryLevel> zur Bestandsverwaltung nach SKU, eine Queue<RestockOrder> für Nachbestellungen und einen Stack
<StockMovement> für die Bewegungshistorie. Implementieren Sie Sortierung nach Bestandswert,
Umschlagsrate oder Verfügbarkeit sowie Suchfunktionen nach Kategorie, Lieferant oder SKU.







#10 | E-Commerce Inventory Management System
a) Suchen Sie alle Produkte einer bestimmten Kategorie, deren Lagerbestand unter dem Mindestbestand liegt und die in den letzten 30 Tagen mindestens einmal verkauft wurden. Verwenden Sie Where-Klauseln zur Filterung nach ProductCategory-Enum, Bestandsvergleich
(CurrentStock < MinimumStock) und Verkaufsdatum. Projizieren Sie auf ein Objekt mit Produktname, SKU, aktueller Bestand, Mindestbestand, Differenz und letztes Verkaufsdatum,
sortiert nach Bestandsdifferenz aufsteigend (kritischste zuerst).
b) Berechnen Sie die Lagerumschlagsrate pro Produktkategorie für das letzte Halbjahr. Gruppieren Sie alle Verkaufstransaktionen nach ProductCategory, summieren Sie die verkauften
Mengen mit Sum, berechnen Sie den durchschnittlichen Lagerbestand über den Zeitraum
mit Average und ermitteln Sie die Umschlagsrate (verkaufte Menge / durchschnittlicher Bestand). Verwenden Sie GroupBy mit komplexen Aggregatfunktionen, Where zur zeitlichen
Eingrenzung und OrderByDescending zur Sortierung nach Umschlagsrate. Projizieren Sie
Kategorie, Gesamtverkäufe, durchschnittlicher Bestand und Umschlagsrate.
c) Identifizieren Sie die 15 profitabelsten Produkte basierend auf der Gesamtmarge (Verkaufspreis minus Einkaufspreis multipliziert mit verkaufter Menge) im aktuellen Quartal. Verwenden Sie Where zur zeitlichen Filterung, GroupBy zur Gruppierung nach Product-ID oder
SKU, Select zur Berechnung der Marge pro Verkauf, Sum zur Gesamtmarge, OrderByDescending zur Sortierung und Take für die Top 15. Verknüpfen Sie mit Produktdaten mittels
Join für vollständige Informationen wie Name, Kategorie und aktuellen Bestand.
d) Finden Sie alle Produkte mit Ablaufdatum in den nächsten 30 Tagen, gruppiert nach WarehouseLocation. Verwenden Sie Where zur Filterung nach Ablaufdatum und StockStatus
(nicht Discontinued), GroupBy zur Gruppierung nach Lagerstandort, Count zur Zählung pro
Standort und Sum zur Berechnung des potenziellen Verlustwerts. Projizieren Sie auf ein
Objekt mit Standort, Anzahl ablaufender Produkte, Gesamtwert und Liste der kritischsten
Artikel, sortiert nach Verlustwert absteigend.
e) Erstellen Sie einen Nachbestellbericht, der für jede Produktkategorie die Anzahl der Artikel
unter Mindestbestand, den durchschnittlichen Fehlbestand (Mindestbestand minus aktueller Bestand), die geschätzten Nachbestellkosten und die Dringlichkeit basierend auf Verkaufsgeschwindigkeit zeigt. Verwenden Sie Where zur Filterung nach LowStock oder Out
OfStock Status, GroupBy zur Gruppierung nach ProductCategory, komplexe Berechnungen
mit Select für Fehlbestand und Kosten, und OrderBy zur Sortierung nach Dringlichkeit. Verknüpfen Sie mit Verkaufsdaten mittels Join zur Ermittlung der täglichen Verkaufsrate.
