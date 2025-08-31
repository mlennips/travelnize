# arc42 Architektur-Dokumentation für Travelnize

## 1. Einleitung und Ziele
Travelnize ist eine modulare Plattform zur Reiseorganisation und -planung. Ziel ist eine moderne, sichere und offline-fähige Lösung mit klarer Trennung von Domäne, Infrastruktur, API und UseCases. Die Architektur soll Erweiterbarkeit, Wartbarkeit und eine gute User Experience gewährleisten.

## 2. Randbedingungen
- .NET 9, Blazor WebAssembly für das Frontend
- Modularer Aufbau mit mehreren Projekten
- JWT-basierte Authentifizierung
- Offline-Fähigkeit (Client kann auch eingeschränkt ohne Server arbeiten)
- Speicherung von Daten im LocalStorage und Backend
- Einsatz von Domain-Driven Design (DDD) für die Kernlogik

## 3. Kontextabgrenzung
- **Systemkontext:** Travelnize interagiert mit Nutzern und dem Browser.
- **Abgrenzung:** Keine Integration mit Fremdsystemen außer Auth und Storage. Die API ist das einzige Bindeglied zwischen Client und Server.

## 4. Lösungsstrategie
- **Frontend:** Blazor WebAssembly für eine moderne, responsive und offline-fähige UI.
- **Backend:** .NET 9 API für Authentifizierung, Datenhaltung und Geschäftslogik.
- **Modularisierung:** Trennung in Domain, Infrastructure, Shared und UseCases für klare Verantwortlichkeiten und bessere Wartbarkeit.
- **DTOs und Commands:** Gemeinsame Schnittstellen und Datenstrukturen in Shared.
- **Offline-Fähigkeit:** Speicherung und Synchronisation von Daten im LocalStorage.

**Entscheidungsgründe:**
- **Blazor WebAssembly:** Ermöglicht eine Single-Page-Application mit .NET-Knowhow und Offline-Fähigkeit.
- **DDD:** Erleichtert die Modellierung komplexer Geschäftslogik und fördert die Trennung von fachlicher und technischer Logik.
- **JWT:** Standard für sichere, stateless Authentifizierung.
- **Modularisierung:** Erhöht die Testbarkeit und Wiederverwendbarkeit der Komponenten.

## 5. Bausteinsicht
- **LIT.Travelnize (Frontend):** UI-Komponenten, LocalStorage-Helper, AuthProvider. Entscheidung für Blazor, um C# durchgängig zu nutzen und Offline-Fähigkeit zu ermöglichen.
- **LIT.Travelnize.Domain:** Entitäten, ValueObjects, Geschäftslogik. Entscheidung für DDD, um die Kernlogik unabhängig von Infrastruktur zu halten.
- **LIT.Travelnize.API:** Endpunkte für Auth, User, Reisen. Entscheidung für REST, da es einfach, verbreitet und mit Blazor kompatibel ist.
- **LIT.Travelnize.Infrastructure:** Datenbankzugriff, externe Schnittstellen. Entscheidung für eigene Infrastruktur-Schicht, um Datenzugriff und externe Dienste zu kapseln.
- **LIT.Travelnize.Shared:** DTOs, Commands, Queries. Entscheidung für Shared, um Redundanz zu vermeiden und Konsistenz zwischen Client und Server zu gewährleisten.
- **LIT.Travelnize.UseCases:** Anwendungsfälle, orchestriert zwischen Domain und Infrastruktur. Entscheidung für UseCases, um die Geschäftsprozesse explizit und testbar zu machen.

## 6. Laufzeitsicht
- Nutzer interagiert mit Blazor-UI (LIT.Travelnize)
- Authentifizierung und Datenzugriff über API (LIT.Travelnize.API)
- Geschäftslogik in UseCases und Domain
- Datenhaltung in Infrastructure und LocalStorage
- Synchronisation bei Rückkehr der Online-Verbindung

**Entscheidungsgründe:**  
Die Trennung der Laufzeitkomponenten ermöglicht eine klare Verantwortlichkeit und vereinfacht die Fehleranalyse.

## 7. Verteilungssicht
- **Client:** Browser (Blazor WebAssembly, LIT.Travelnize)
- **Server:** .NET 9 API (LIT.Travelnize.API)
- **Datenbank:** über Infrastructure angebunden

**Entscheidungsgründe:**  
Die Verteilung ermöglicht Offline-Fähigkeit und Skalierbarkeit. Die API kann unabhängig vom Client weiterentwickelt werden.

## 8. Querschnittliche Konzepte
- **Sicherheit:** JWT, Obfuscation, LocalStorage. Entscheidung für JWT, da es stateless und weit verbreitet ist.
- **Offline-Fähigkeit:** Datenhaltung im Browser. Entscheidung für LocalStorage, da es einfach und browserübergreifend funktioniert.
- **Modularität:** Trennung von UI, Domain, API, Infrastruktur, Shared, UseCases. Entscheidung für Modularität, um Wartbarkeit und Erweiterbarkeit zu erhöhen.
- **Fehlerbehandlung:** Exception-Handling und Logging in allen Schichten.

## 9. Architekturentscheidungen
- **Blazor WebAssembly:** Für Offline-Fähigkeit und moderne UI.
- **JWT:** Für sichere, stateless Authentifizierung.
- **Domain-Driven Design:** Für klare Trennung von Geschäftslogik und Infrastruktur.
- **UseCases:** Für explizite, testbare Geschäftsprozesse.
- **Shared:** Für konsistente Datenstrukturen zwischen Client und Server.

## 10. Qualitätsanforderungen
- **Usability:** Moderne, responsive UI.
- **Sicherheit:** Grundlegende Absicherung der Daten und Authentifizierung.
- **Offline-Fähigkeit:** Client kann auch ohne Server arbeiten.
- **Erweiterbarkeit:** Neue Features können einfach ergänzt werden.
- **Wartbarkeit:** Klare Trennung der Verantwortlichkeiten.

## 11. Risiken und technische Schulden
- **Clientseitige Sicherheit:** Schlüssel und Logik sind im Client sichtbar und werden optional verschleiert.
- **Offline-Synchronisation:** Konflikte bei gleichzeitigen Änderungen möglich.
- **Komplexität durch Modularität:** Erhöht den initialen Entwicklungsaufwand.

## 12. Glossar
- **Blazor:** Webframework für .NET
- **JWT:** JSON Web Token
- **LocalStorage:** Browser-Speicher
- **Obfuscation:** Verschleierung von Daten
- **Domain:** Fachliche Logik und Modelle
- **Infrastructure:** Technische Anbindung und Persistenz
- **UseCases:** Geschäftsprozesse und Anwendungslogik
- **Shared:** Gemeinsame Schnittstellen und DTOs
