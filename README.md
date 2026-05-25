# Warsztat Tuningowy
Wielomodułowa aplikacja webowa do zarządzania warsztatem tuningowym.
Projekt zaliczeniowy Projektowanie Systemów Informatycznych.

## Cel projektu

Aplikacja wspiera obsługę zleceń tuningowych od przyjęcia pojazdu, przez pracę mechanika i zapotrzebowanie na części, aż po dokumenty końcowe, fakturę oraz raporty dla właściciela.


## Stack
- ASP.NET Core MVC (.NET 10)
- Entity Framework Core (Code-First) + MS SQL Server
- Bootstrap 5 + QuestPDF
- ASP.NET Core Identity (role: Właściciel, Mechanik, Magazynier)

## Funkcjonalności

**Zarządzanie warsztatem**
- Ewidencja klientów i pojazdów: VIN, typ ECU, historia zleceń
- Zlecenia tuningowe z maszyną stanów: Received, Diagnosis, InProgress, QualityCheck, Completed
- Przypisanie mechanika i stanowiska roboczego do zlecenia
- Elektroniczne oświadczenie klienta dotyczące zakresu modyfikacji, DPF/OPF i utraty gwarancji

**Praca na hali**
- Widok roboczy mechanika z timerem RBH: start, stop, pauza
- Oznaczanie zużytych części z aktualizacją stanu magazynowego
- Flaga gotowości zlecenia do tuningu
- Alert przekroczenia szacowanego czasu pracy

**Magazyn**
- Ewidencja części z cenami hurtowymi, detalicznymi i marżą
- Rozróżnienie części magazynowych od części zamawianych pod konkretne zlecenie
- Alerty niskiego stanu magazynowego
- Workflow zapotrzebowań: mechanik zgłasza, magazynier realizuje, część trafia do zlecenia
- Blokada zamówienia części bez potwierdzonej zaliczki

**Dokumenty i raporty**
- Faktura VAT w PDF
- Eksport faktur do CSV
- Protokół tuningu w PDF
- Oświadczenie klienta w PDF
- Dashboard właściciela z alertami i statystykami

**Administracja**
- Role użytkowników: właściciel, mechanik, magazynier
- Zarządzanie pracownikami
- Historia zadań i statystyki pracy
- Osobne dashboardy dla każdej roli

## Role użytkowników

| Rola | Dostęp |
|------|--------|
| Właściciel | Pełny dostęp, dashboard, faktury, pracownicy |
| Mechanik | Moje zlecenia, timer, części, zapotrzebowania |
| Magazynier | Magazyn, realizacja zapotrzebowań |

## Konta testowe (hasło: Admin123!)
| Email | Rola |
|-------|------|
| wlasciciel@warsztat.pl | Właściciel |
| mechanik1@warsztat.pl | Mechanik |
| mechanik2@warsztat.pl | Mechanik |
| mechanik3@warsztat.pl | Mechanik |
| magazynier@warsztat.pl | Magazynier |

## Planowane ulepszenia

- Wydzielenie logiki biznesowej z kontrolerów do dedykowanej warstwy serwisów aplikacji
- Historia operacji magazynowych (przyjęcia, wydania, korekty) i archiwizacja nieaktywnych części
- Rozbudowanie walidacji procesów biznesowych
- Testy jednostkowe dla kluczowej logiki domenowej
- Usprawnienie raportów i filtrów w dashboardzie właściciela

## Uruchomienie
1. Ustaw nazwę serwera w `appsettings.json`
2. `dotnet ef database update`
3. Uruchom - seed danych załaduje się automatycznie