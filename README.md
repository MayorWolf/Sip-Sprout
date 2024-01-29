# Sip & Sprout (Unity Edition)

# Inspiration

Ich bin nicht gut darin, regelmäßig Wasser zu trinken. Ich vergesse es einfach. Es gibt viel zu tun, ich bin mit dem Kopf woanders, ich bin unterwegs. 
Hundert Gründe, dasselbe Resultat: Am Ende des Tages habe ich zu wenig getrunken. <br>
Natürlich habe ich schon versucht, mir das Trinken zur Gewohnheit zu machen: Ich habe mir Apps heruntergeladen, die mich daran erinnern sollen.
Ich habe mir einen Timer gestellt, der mich alle 30 Minuten an das Trinken erinnert. Ich habe mir einen Wasserkrug auf den Schreibtisch gestellt, bringt alles nichts.
Die Apps sind voll mit Funktionen, die ich nicht brauche, spammen mich mit Werbung zu oder wollen absurd viel Geld und/der Daten von mir. Der Timer ist nervig, das Wasser auf meinem Schreibtisch ignoriere ich einfach. <br>

Aber mir ist aufgefallen, was ich früher nie ignoriert habe: Meinen Tamagotchi. Ich habe ihn gefüttert, ihn gepflegt, ihn geliebt. Warum schaffe ich das nicht mit mir selbst? <br>

So, oder so ähnlich, kam mir die Idee für eine App, in der mein eigenes Trinkverhalten verantwortlich für das Wohlergehen von etwas anderem ist als mir selbst. Pflanzen müssen, genau wie ich, regelmäßig gegossen werden.
Eine digitale Pflanze muss her, die mich erinnert, dass ich verdorre, wenn ich nichts trinke. Vielleicht klappt es ja diesmal mit einer neuen, gesunden Gewohnheit...

Die Inspiration für weitere Funktionen kam hauptsächlich von der Sprachenlern-App Duolingo, dessen Nutzung durch 
Streak-System und Push-Benachrichtigungen bei mir bereits zu einer Gewohnheit geworden ist.

# Konzept

## Ziel

Sip & Sprout unterstützt Nutzer\*innen beim Formen und Festigen von gesunden Trinkgewohnheiten. 
Die App erinnert an den regelmäßigen Konsum von Wasser und stellt den täglichen Stand anhand einer Pflanze dar, die
nur wächst, wenn sie gegossen wird. Benachrichtigungen und Streaks, die beim wiederholten Erreichen von Zielen aufgebaut
werden, motivieren Nutzer\*innen, die App weiterzunutzen.

### Umfang

Um einen besseren Überblick über das Projekt zu bekommen und strukturiert an der Umsetzung zu arbeiten habe ich die 
Funktionen in drei Kategorien aufgeteilt: "Must have", "Should have" & "Nice to have".

*Must have:*
- Täglich zurückgesetzter Trink-Counter mit einer Fortschrittsvisualisierung  ✅
- Unterschiedliche Trinkmengen, die konsumiert werden können ✅

*Should have:*
- Visualisierung des Fortschrittes anhand einer sich verändernden Pflanze ✅
- Regelmäßige Erinnerungen per Push-Benachrichtigung ✅

*Nice to have:*
- Anpassbares tägliches Ziel ✅
- Streaks, die zurücksetzten, wenn das tägliche Ziel nicht erreicht wird ✅
- Kein Senden von Push-Benachrichtigungen zu konfigurierbaren Ruhezeiten ✅
- Homescreen Widget mit Abbildung der Pflanze
- Animation von Pflanzenwachstum

Bis auf die letzten beiden Punkte konnte ich alle Funktionen umsetzten. 

## Technologie

Das Projekt wird in Unity3D umgesetzt, ich nutze hierfür Unity 2022.3.10f1 (LTS). Für die Entwicklung auf einer Android-Plattform 
nutze ich das Unity Android Package, es werden Android Version 9 - 13 unterstützt. Portierung auf frühere Versionen
ist nicht möglich, da sich das Speicherverhalten von UserPrefs (welches ich zum lokalen Ablegen der Nutzerdaten verwende,
siehe [Datenschutz]) nach Android 8 verändert hat. <br> <br>
UI Konzeption, Mockups und Varianten wurden mit Figma erstellt und getestet. Alle benötigten Sprites habe ich in Adobe 
Illustrator erstellt und exportiert.

## Gestaltung

Die App soll mit einem ruhigen, freundlichen Design und einer entspannten Atmosphäre überzeugen. Die UI wird auf das Wesentliche reduziert,
um Nutzer\*innen nicht mit unnötigen Informationen zu überfordern. Die Farbpalette ist warm und freundlich, die Schrift klar und gut lesbar.

# Umsetzung

## Minimum Viable Product
Die grundlegenden Funktionen ("Must haves") lassen sich mit einer Handvoll UnityUI Elementen darstellen. 
Nutzer\*innen haben mit Buttons die Möglichkeit zu "trinken", der tägliche Fortschritt wird mit einem nicht 
interagierbaren Slider dargestellt. Die Logik für diese Funktionen (und alle später folgenden) läuft über ein
Skript, welches an das Canvas gebunden ist. <br>
Die UI wird nur geupdatet, wenn die App geöffnet wird oder Nutzer\*innen mit ihr interagieren. Das Verzichten auf die `Update()`
Methode soll Performance optimieren und Stromverbrauch reduzieren. Weiter Methoden der Optimierung sind zwar besonders auf einer mobilen Plattform 
wichtig, sollten aber bei einer Anwendung dieser Größe mit ruhigem Gewissen vernachlässigt werden können.

## UI

Alle Elemente laufen in einer Unity Szene, die Einstellungen werden als Pop-up über den eigentlichen Inhalt geblendet.
Die UI ist dynamisch, sie passt sich der Auflösung und Bildschirmgröße/Format des jeweiligen Endgerätes an. Dies geschieht natürlich
nur in einem gewissen Rahmen, bei besonders esoterischen Formaten (z.B. 1:1) wird die UI nicht mehr optimal dargestellt.

Die Pflanze wird in drei verschiedenen Sprites gerendert, welche je nach täglichem Fortschritt ausgetauscht werden. Alle Buttons
sind einheitlich gestaltet und haben eine Hierachie, die beim Nutzen von Screenreadern helfen sollte, sinnhaft testen konnte ich dies allerdings noch nicht.

## Fortgeschrittene Funktionalitäten

### Push-Benachrichtigungen

Push-Benachrichtigungen werden über das Unity Notifiactions an die Android Notification Center gepostet. Beim ersten Starten der Anwendung
wird Nutzer\*innen die Erlaubnis abgefragt, Benachrichtigungen zu senden. Diese Erlaubnis kann jederzeit in den Systemeinstellungen widerrufen werden, alternativ ist es dem nutzer aber auch möglich,
Benachrichtigungen in der App selbst zu (de)aktivieren. <br>

Die Frage, wann und wie viele Benachrichtigungen gesendet werden benötigte etwas Planung. Folgende Constraints legte ich für das Senden von Benachrichtigungen fest:

- Jede Stunde wird eine Erinnerung gesendet
- Reset dieses Timers nach jedem Trinken
- Nach einem Tag gänzlicher Inaktivität des Nutzenden (kein trinken) werden zwei Erinnerungen gesendet, keine stündlichen Reminder
- Konfigurierbare Schlafenszeiten werden geachtet, in denen keine Benachrichtigungen gesendet werden
- Benachrichtigungen werden nur gesendet, wenn die App nicht geöffnet ist

Die Logik hinter diesen Constraints findet sich in 'ResetHourlyNotifications()'. Die Funktion wird nach jedem Trinken aufgerufen, um die Benachrichtigungen 
im Notification Center zu aktualisieren, um die Schlafenszeiten zu beachten wird außerdem 'GetNextDay()' benötigt, damit auch am nächsten Morgen Push Notifications gesendet werden.

### Streaks

Streaks mögen auf der Oberfläche simpel wirken (zählen wie oft Nutzer\*innen ihr tägliches Ziel erreichen, gehe auf 0, wenn es einen Tag mal nicht passiert), 
die Umsetzung war aber eine der komplizierteren Aufgaben dieses Projektes.
Das Problem der Streaks ist konsistentes Zählen. Die App muss sich merken, wie oft Nutzer\*innen ihr Ziel erreichen, auch wenn sie die App nicht geöffnet haben, ohne dass Fehler unterlaufen.
Eine nicht gesendete Benachrichtigung ist nichts Dramatisches, aber ein fehlerhaftes Zurücksetzen der Streak kann Nutzer\*innen frustrieren und die App für sie unbrauchbar machen. <br>

Die Streak wird, wie alle anderen Daten auch, in den Unity UserPrefs gespeichert. Beim Öffnen der App wird geprüft, wann das letzte Mal getrunken wurde und ob die Streak zurückgesetzt werden muss.
Genutzt wird hierfür die DateTime Klasse, die auch auf Android als Systemklasse zur Verfügung steht. Aufgrund der Klasse müssen einige Edgecases bei der Prüfung von Streaks beachtet werden:

- Monatswechsel
- Jahreswechsel
- Schaltjahre
- Zeitzonen

Es gibt sicherlich noch weitere Edgecases, die ich nicht bedacht habe, aber diese vier sind die, die mir beim Testen aufgefallen sind. <br>
Die if-Statements in '_UpdateStreak()' sind daher etwas verworren, aber sollten alle oben genannten Fälle abdecken. <br>

### Datenschutz

Um die Grundsätze des Datenschutzes zu wahren, habe ich mich entschieden, die App vollständig offline laufen zu lassen und alle lokal gespeicherten Daten zu verschlüsseln.
Die Restriktion auf Android 8+ ist hierbei ein Kompromiss, den ich eingehen musste, da die Speicherung von UserPrefs auf Android 8+ anders funktioniert als auf früheren Versionen.
Offline-only bedeutet auch, dass ich keine Cloud-Services wie Firebase oder AWS nutzen kann, um Daten zu erheben, Benachrichtigungen zu senden oder Uhrzeit und Datum zu synchronisieren.

# Reflexion

Die größten Learnings aus dem Projekt sind für mich die folgenden: Unterschätze niemals die Komplexität von scheinbar simplen Schnittstellen zwischen Anwendung und OS & 
Vertraue niemals veralteter Dokumentation. <br>
Die Must- und Should-Haves leisen sich alle in einem moderaten Zeitrahmen umsetzten. Eine gute Vorbereitung der UI in Figma ließ mich auch die Visualisierung schnell in Unity implementieren.
Das erweiterte Push-Benachrichtigungs System und die Streaks haben schon etwas mehr Planung und Zeit in Anspruch genommen, waren aber auch noch im Rahmen des machbaren. <br>
<br>
Dann ist da noch das Homescreen Widget. Gut ein Fünftel meiner Arbeitszeit ist in die Recherche, Machbarkeitsanalyse und Prototypisierung eines Widgets geflossen,
welches den Zustand der Pflanze auf dem Homescreen darstellt. Leider musste ich feststellen, dass die Dokumentationen etwaiger benötigter Schnittstellen im besten Falle veraltet, im schlechtesten gar nicht vorhanden sind, 
die meisten Workarounds auf aktuelleren Android Versionen nicht mehr funktionieren und generell recht wenig Austausch über Android Widgets mit Unity online zu finden ist.
<br>
Bis zu guter Letzt ist es mir daher nicht möglich, ein App-Widget zu implementieren. Ich konnte mir die Zeit nehmen, da der Rest des Projektes schon früh einen akzeptablen Stand erreicht hatte.
Unter anderen, zeitkritischeren Umständen hätte ich das Widget wohl früher aufgegeben.

# Resultat

Die App ist in einem Zustand, in dem sie für Nutzer/*innen funktioniert. Ich habe sie seit einiger Zeit auf meinem Handy installiert, ursprünglich zum Testen, und nutze
sie jetzt tatsächlich, um meine Trinkgewohnheiten zu verbessern. Zumindest für den Eigennutzen ist sie also ein Erfolg. <br>
Ich konnte fast alle Funktionen umsetzten, die ich mir vorgenommen habe. Die App ist stabil und läuft auf allen getesteten Geräten. <br>

Zukünftig plane ich, die UI noch einmal gänzlich zu überarbeiten und das bis dato recht generische Design mit mehr Persönlichkeit zu füllen. Parallel bereite ich einen 
potenziellen Release im Google Play Store vor, um die App auch für andere Nutzer\*innen verfügbar zu machen. Die Recherche über Veröffentlichungsbedingungen und -prozesse
ist allerdings noch nicht abgeschlossen, ich kann also noch nicht sagen, ob und wann die App im Play Store verfügbar sein wird.

Der Quellcode des gesamten Projektes ist in einem offenen Repository auf GitHub verfügbar: https://github.com/MayorWolf/Sip-Sprout

# Quellen

Modifizierte version der ZPlayerPrefs by Maksym Yemelianov (https://github.com/mlnv/EnergySuite/tree/master/EnergySuite/Assets/EnergySuite/ZFrame)