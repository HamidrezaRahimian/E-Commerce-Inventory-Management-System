10 | E-Commerce Inventory Management System
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
