# AutoService – Sistem za zakazivanje termina u auto-servisu

AutoService je višeslojna ASP.NET Core MVC aplikacija za upravljanje korisnicima, vozilima, serviserima, servisnim uslugama i terminima u auto-servisu.

Aplikacija omogućava zakazivanje termina, pregled podataka, izmenu, brisanje, pretragu i upravljanje statusima termina.

## Tehnologije

* C#
* .NET 10
* ASP.NET Core MVC
* Entity Framework Core
* SQL Server LocalDB
* Razor Views
* Bootstrap
* Repository Pattern
* Unit of Work Pattern
* System Operation Pattern
* Dependency Injection

## Struktura projekta

Solution je podeljen na četiri projekta:

```text
AutoService
├── AutoService.Domain
├── AutoService.Application
├── AutoService.Infrastructure
└── AutoService.Web
```

### AutoService.Domain

Sadrži domenske entitete i enum vrednosti:

* Korisnik
* Vozilo
* Serviser
* ServisnaUsluga
* Termin
* StatusTermina

### AutoService.Application

Sadrži poslovnu logiku aplikacije:

* DTO modele
* repository interfejse
* Unit of Work interfejs
* sistemske operacije

Implementirane sistemske operacije:

* ZakaziTerminOperation
* OtkaziTerminOperation
* ZavrsiTerminOperation

### AutoService.Infrastructure

Sadrži implementaciju rada sa bazom:

* AutoServiceDbContext
* Entity Framework konfiguracije
* generički Repository
* TerminRepository
* UnitOfWork
* migracije
* početne podatke

### AutoService.Web

ASP.NET Core MVC projekat koji sadrži:

* MVC kontrolere
* Razor View stranice
* Bootstrap frontend
* konfiguraciju Dependency Injection-a
* connection string

## Funkcionalnosti

### Korisnici

* prikaz svih korisnika
* dodavanje korisnika
* izmena korisnika
* detalji korisnika
* pretraga
* brisanje korisnika bez vozila

### Vozila

* prikaz svih vozila
* dodavanje vozila
* izmena vozila
* detalji vozila
* povezivanje vozila sa vlasnikom
* pretraga po marki, modelu, registraciji i vlasniku
* brisanje vozila bez termina

### Serviseri

* prikaz svih servisera
* dodavanje servisera
* izmena servisera
* detalji servisera
* filtriranje po statusu
* aktivacija i deaktivacija servisera
* brisanje servisera bez termina

### Servisne usluge

* prikaz svih servisnih usluga
* dodavanje usluge
* izmena usluge
* detalji usluge
* definisanje cene i trajanja
* aktivacija i deaktivacija
* brisanje usluge bez termina

### Termini

* zakazivanje termina
* prikaz svih termina
* povezivanje termina sa vozilom, serviserom i uslugom
* završavanje termina
* otkazivanje termina
* prikaz statusa termina

## Poslovna pravila

Prilikom zakazivanja termina proverava se:

* termin ne može biti zakazan u prošlosti
* termin mora biti unutar radnog vremena
* termin mora počinjati na pun sat
* serviser ne može imati dva termina u isto vreme
* vozilo ne može imati dva termina u isto vreme
* otkazan termin ne može biti završen
* završen termin ne može biti otkazan
* neaktivni serviseri se ne prikazuju prilikom zakazivanja
* neaktivne servisne usluge se ne prikazuju prilikom zakazivanja

## Relacije u bazi

```text
Korisnik 1 ─── N Vozilo

Vozilo 1 ─── N Termin

Serviser 1 ─── N Termin

ServisnaUsluga 1 ─── N Termin
```

## Pokretanje projekta

### Preduslovi

Potrebno je instalirati:

* Visual Studio sa ASP.NET and web development workload-om
* .NET 10 SDK
* SQL Server LocalDB

### Koraci

1. Klonirati repozitorijum:

```bash
git clone URL_REPOZITORIJUMA
```

2. Otvoriti solution fajl u Visual Studio-u.

3. Postaviti `AutoService.Web` kao startup projekat:

```text
Desni klik na AutoService.Web
→ Set as Startup Project
```

4. Proveriti connection string u fajlu:

```text
AutoService.Web/appsettings.json
```

Podrazumevani connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AutoServiceDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

5. Otvoriti Package Manager Console.

6. Postaviti `AutoService.Infrastructure` kao Default project.

7. Pokrenuti migracije:

```powershell
Update-Database -StartupProject AutoService.Web
```

8. Pokrenuti aplikaciju:

```text
Ctrl + F5
```

Pri prvom pokretanju aplikacija automatski ubacuje početne test podatke u bazu.

## Početni podaci

Aplikacija kreira test podatke za:

* korisnike
* vozila
* servisere
* servisne usluge

Primeri servisnih usluga:

* Mali servis
* Veliki servis
* Dijagnostika
* Zamena kočionih pločica

## Arhitektonski obrasci

### Repository Pattern

Repository sloj odvaja poslovnu logiku od direktnog rada sa Entity Framework Core-om.

Generički repository koristi se za osnovne CRUD operacije.

### Unit of Work

Unit of Work omogućava centralizovano čuvanje promena u bazi preko metode:

```csharp
SaveChangesAsync()
```

### System Operation Pattern

Složenije poslovne operacije izdvojene su u posebne klase.

Primer toka sistemske operacije:

```text
Validacija
→ izvršavanje poslovne logike
→ čuvanje promena
```

### Dependency Injection

Repository klase, Unit of Work i sistemske operacije registruju se u `Program.cs` i automatski prosleđuju kontrolerima.

## Autor

Veljko Cukanić
Broj indeksa: 2022/0005

## Napomena

Projekat je izrađen u okviru fakultetskog zadatka i služi kao demonstracija rada sa ASP.NET Core MVC aplikacijom, Entity Framework Core-om, višeslojnom arhitekturom i sistemskim operacijama.
