#   VoltFlow – Field Service Management System (MVP)

VoltFlow to system klasy ERP/CRM wspierający zarządzanie zleceniami serwisowymi w terenie.
Projekt został zbudowany jako MVP, z naciskiem na poprawną architekturę backendową, spójne workflow biznesowe oraz ergonomię pracy technika mobilnego.
System obejmuje pełny cykl obsługi zlecenia — od autoryzacji użytkownika, przez wizję lokalną i kalkulację kosztów, aż po automatyczną komunikację z klientem.

Cel Projektu
-   Usprawnienie pracy techników terenowych (mobile-first)
-   Minimalizacja błędów kosztorysowych
-   Automatyzacja ofertowania i komunikacji z klientem
-   Skalowalna architektura gotowa pod kolejne branże

Architektura i Zakres MVP
-   System realizuje pełną ścieżkę operacyjną:
-   Autoryzacja → Workflow Zlecenia → Magazyn → Kalkulacja Kosztów → Notyfikacje
-   Projekt został zaprojektowany z myślą o pracy na tabletach, umożliwiając edycję danych u klienta w czasie rzeczywistym.

Funkcjonalności (MVP)
-   Zarządzanie Tożsamością
-   Autoryzacja użytkowników z wykorzystaniem JWT
-   Rygorystyczne zabezpieczenie endpointów API


Workflow Zleceń
-   Obsługa pełnego cyklu życia zlecenia serwisowego
-   Silnik stanów i przejść biznesowych
-   Moduł Magazynowy
-   Rezerwacja materiałów przypisanych do zlecenia
-   Powiązanie zasobów z konkretnymi pracami


Silnik Wycen
-   Dynamiczna kalkulacja kosztów: 
                                    robocizna, 
                                    materiały
-   Przeliczanie w czasie rzeczywistym podczas wizji lokalnej
-   Automatyzacja Notyfikacji
-   Generowanie ofert handlowych (PDF)
-   Wysyłka e-mail po zatwierdzeniu zakresu prac
-   Interfejs Mobile-First
-   Panel technika zoptymalizowany pod tablety
-   Dynamiczne formularze zależne od typu usterki i etapu prac

Stack Technologiczny
Backend – .NET 10
        Moduł               |                 Opis
-   Identity & Security     |   JWT, autoryzacja i ochrona endpointów
-   Generic Job Engine      |   Silnik workflow i stanów zleceń
-   Advanced Inventory      |   Rezerwacja i powiązania materiałowe
-   Cost Estimation         |   Algorytmy kalkulacyjne w czasie rzeczywistym
-   Notification Gateway    |   Integracja SMTP (e-mail / PDF)


Frontend – Angular
-   State Management: Angular Signals
-   UI Framework: 
        PrimeNG
-   UX:
        dynamiczne formularze
        natychmiastowa aktualizacja kosztorysu
        ergonomia pracy w terenie



Baza Danych – PostgreSQL
-   Modelowanie: Table-Per-Hierarchy (TPH)
-   Relacje CRM: Klient → Obiekt → Historia serwisowa
-   Audyt: Wersjonowanie zmian po wizji lokalnej

Proces Obsługi Zlecenia (User Flow)

Analiza
-   Technik przyjmuje zlecenie i analizuje historię klienta

Planowanie
-   Wstępna rezerwacja komponentów w magazynie

Realizacja
-   Edycja zakresu prac na tablecie podczas wizji lokalnej

Kalkulacja
-  Automatyczne wygenerowanie kosztorysu

Finalizacja
-   Zatwierdzenie prac
-   Wysłanie oferty e-mail
-   Zmiana statusu na „W realizacji”



Harmonogram
-   Projekt realizowany w cyklu 28-dniowym, z priorytetem na:
-   Jakość architektury backendowej
-   Poprawność logiki biznesowej
-   Czytelność i skalowalność rozwiązania
-   Postępy oraz dokumentacja techniczna dostępne są w zakładce Projects repozytorium.
