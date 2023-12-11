# Sip & Sprout (Unity Edition)

# Konzept

## Ziel

Sip & Sprout unterstützt Nutzer\*innen beim Formen und Festigen von gesunden Trinkgewohnheiten. 
Die App erinnert an den regelmäßigen Konsum von Wasser und stellt den täglichen Stand anhand einer Pflanze an, die
nur wächst wenn sie gegossen wird. Benachrichtigungen und Streaks, die beim wiederholten Erreichen von Zielen aufgebaut
werden motivieren Nutzer\*innen die App weiter zu nutzen.

### Umfang

Um einen besseren Überblick über das Projekt zu bekommen und strukturiert an der Umsetzung zu arbeiten habe ich die 
Funktionen in drei Kategorien aufgeteilt: "Must have", "Should have" & "Nice to have".

*Must have:*
- Täglich zurückgesetzter Trink-Counter mit einer Forstschrittsvisualisierung  ✅
- Unterschiedliche Trinkmengen, die Konsumiert werden können ✅

*Should have:*
- Visualisierung des Fortschrittes anhand einer sich verändernden Pflanze ✅
- Regelmäßige reminder per Push-Benachrichtigung ✅

*Nice to have:*
- Anpassbares tägliches Ziel ✅
- Streaks, die zurücksetzten wenn das tägliche Ziel nicht erreicht wird ✅
- Kein senden von Push Benachrichtigungen zu konfigurierbaren Ruhezeiten ✅
- Homescreen Widget mit Abbildung der Pflanze ❌
- Animation von Pflanzenwachstum ❌

## Technologie

Unity 2022.3.10f1 (LTS), Figma, Illustrator
Das Projekt wird in Unity3D umgesetzt, ich nutze hierfür Unity 2022.3.10f1 (LTS). Für die Entwiklung auf einer Android 
Plattform nutze ich das Unity Android Package, es werden Android Version 9 - 13 unterstützt. Portierung auf frühere Versionen
ist nicht möglich, da sich das Speicherverhalten von UserPrefs (welches ich zum lokalen ablegen der Nutzerdaten verwende,
siehe [Datenschutz]) nach Android 8 verändert hat. <br> <br>
UI Konzeption, Mockups und Varianten werden mit Figma erstellt und getestet. Alle benötigten Sprites werden in Adobe 
Illustrator erstellt und exportiert.

## Gestaltung

Ruhiges Design, Warm & Freundlich

Einfache Optik, aufs nötige reduziert

# Umsetzung

## Minimum Viable Product
Die grundlegenden Funktionen ("Must haves") lassen sich mit einer Hand voll UnityUI Elementen darstellen. 
Nutzer\*innen haben mit Buttons die möglichkeit zu "trinken", der tägliche Fortschritt wird mit einem nicht 
interagierbaren Slider dargestellt. Die Logik für diese Funktionen (und alle später folgenden) läuft über ein
Skript welches an das Canvas gebunden ist. <br>
Die UI wird nur ge-updated wenn die App geöffnet wird oder Nutzer\*innen mit ihr interagieren. Das Verzichten auf die `Update()`
Methode soll Performance optimieren und Stromverbrauch reduzieren. Diese Aspekte sind auf einer mobilen Plattform zwar 
wichtig, sollten aber bei einer Anwendung dieser Größe mit ruhigem Gewissen vernachlässigt werden können.

## UI

Alles in einer Szene, settings werden überblendet.

Alle Elemente an constraints gebunden, da Android geräte eine breite masse an screen sizes/resolutions haben

## Fortgeschrittene Funktionalitäten

### Push Benachrichtigungen

Umsetzung mit Unity Notification Center

Constraints:

- Jede Stunde
- Schlafenszeit
- Ganz ausstellbar
- nicht direkt nachdem man getrunken hat

### Streaks

Viel Logik, viele Constraints

### Datenschutz

Lokal gespeicherte Daten müssen verschlüsselt werden

# Post-Morten

## Resultat

## Kritische Einschätzung

## Zukunft