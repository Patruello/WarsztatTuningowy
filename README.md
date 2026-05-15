# Warsztat Tuningowy

## Stack
- ASP.NET Core MVC (.NET 10)
- Entity Framework Core (Code-First) + MS SQL Server
- Bootstrap 5 + QuestPDF
- ASP.NET Core Identity (role: Właściciel, Mechanik, Magazynier)

## Konta testowe (hasło: Admin123!)
| Email | Rola |
|-------|------|
| wlasciciel@warsztat.pl | Właściciel |
| mechanik1@warsztat.pl | Mechanik |
| magazynier@warsztat.pl | Magazynier |

## Uruchomienie
1. Ustaw nazwę serwera w `appsettings.json`
2. `dotnet ef database update`
3. Uruchom - seed danych załaduje się automatycznie