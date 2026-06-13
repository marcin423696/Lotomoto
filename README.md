
# Dokumentacja techniczna aplikacji Lotomoto

## 0. Instrukcja 
Dokładne działanie każdego panelu jest dostępne w pliku : Dokumentacja-użytkownika-aplikacji-Lotomoto.docx
Aby uruchomić projekt należy:
1. Pobierz repozytorium na dysk jako ZIP
2. Wypakuj repozytorium
3. Otwórz plik rozwiązania Lotomoto.sln w programie Visual Studio.
4. Wpisz w terminalu polecenie dotnet run oraz skopiuj wygenerowanego przez aplikacje localhost - np. http://localhost:5168 i wpisz w przeglądarke 

## 1. Opis projektu
Lotomoto to nowoczesna aplikacja internetowa służąca do przeglądania i zarządzania ogłoszeniami samochodowymi. Projekt został przygotowany w technologii ASP.NET Core MVC i działa w formie portalu ogłoszeniowego zbliżonego do wiodących serwisów motoryzacyjnych.

Aplikacja pozwala każdemu odwiedzającemu na przeglądanie dostępnych ofert, filtrowanie ich według wybranych kryteriów, sprawdzanie szczegółów konkretnego pojazdu oraz **bezpośrednie dodawanie nowych ogłoszeń** z poziomu sekcji publicznej. Dodatkowo w systemie zaimplementowano zabezpieczony panel administratora przeznaczony do moderacji, edycji oraz usuwania wpisów.

## 2. Zastosowane technologie
W projekcie wykorzystano następujące technologie i narzędzia:
* **Framework:** ASP.NET Core 10.0 MVC
* **Język programowania:** C#
* **Dostęp do danych:** Entity Framework Core (Code First)
* **Baza danych:** Microsoft SQL Server
* **Warstwa prezentacji:** Razor Views, HTML5, CSS3, Bootstrap
* **Kontrola wersji:** Git & GitHub

Aplikacja korzysta z relacyjnej bazy danych obsługiwanej za pomocą Entity Framework Core, dzięki czemu cała komunikacja i mapowanie obiektów odbywa się w sposób silnie typowany poprzez dedykowany kontekst bazy danych.

## 3. Struktura projektu
Projekt został podzielony zgodnie ze wzorcem architektonicznym MVC (Model-View-Controller), co zapewnia separację logiki biznesowej od warstwy prezentacji:
* `Controllers` — zawiera kontrolery odpowiedzialne za obsługę żądań HTTP i przepływ aplikacji.
* `Models` — zawiera klasy opisujące strukturę danych oraz reguły walidacji.
* `Views` — zawiera dynamiczne widoki stron renderowane po stronie serwera.
* `Data` — zawiera konfigurację połączenia z bazą (`ApplicationDbContext`).
* `wwwroot` — przechowuje pliki statyczne (style CSS, skrypty JS oraz przesłane zdjęcia pojazdów).
* `Migrations` — zawiera historię zmian struktury bazy danych.

## 4. Baza danych
Aplikacja wykorzystuje bazę danych o nazwie `LotomotoCoreDb`. Główną tabelą przechowującą oferty jest **`CarListings`**. Zapisywane są w niej następujące informacje:
* Unikalny identyfikator (`Id`)
* Tytuł ogłoszenia
* Cena (format zmiennoprzecinkowy o precyzyjnie określonej skali)
* Przebieg (w kilometrach)
* Rok produkcji
* Kategoria / rodzaj pojazdu
* Moc silnika (w KM)
* Szczegółowy opis
* Ścieżka URL do zdjęcia

Synchronizacja struktury tabel z modelami w kodzie realizowana jest za pomocą mechanizmu migracji Entity Framework Core.

## 5. Model ogłoszenia
Podstawową jednostką informacyjną w systemie jest model `CarListing`. Posiada on wbudowane reguły walidacji danych (Data Annotations). Dzięki temu system uniemożliwia przesłanie formularza, jeśli użytkownik pominie kluczowe parametry, takie jak tytuł, cena, rok produkcji czy przebieg. Właściwość odpowiadająca za cenę posiada jawnie zdefiniowany typ kolumny w bazie danych w celu uniknięcia utraty dokładności groszowej.

## 6. Kontrolery
Za logikę aplikacji odpowiadają trzy główne kontrolery:
* `HomeController` — zarządza stroną startową portalu.
* `ListingsController` — odpowiada za publiczną część systemu. Obsługuje wyświetlanie listy aut, mechanizm zaawansowanego filtrowania, podgląd szczegółów oraz pozwala każdemu użytkownikowi na dodawanie własnych ogłoszeń (`Create`).
* `AdminController` — zarządza panelem administracyjnym. Obsługuje proces bezpiecznego logowania oraz operacje moderacyjne (edycja i usuwanie ogłoszeń z poziomu widoku zarządzania).

## 7. Panel administratora
Panel administratora umożliwia pełną kontrolę nad treściami znajdującymi się w bazie danych. Po poprawnym uwierzytelnieniu administrator uzyskuje dostęp do zestawienia wszystkich ogłoszeń w formie tabeli z bezpośrednim dostępem do funkcji modyfikacji i usuwania ofert. 

Dla zapewnienia stabilnego działania, system wykorzystuje mechanizm automatycznego inicjowania bazy danych (seeding), który tworzy domyślne konto administratora przy pierwszym uruchomieniu projektu.

## 8. Dodawanie i edycja ogłoszeń
Formularz dodawania ogłoszenia został wyciągnięty do strefy publicznej, umożliwiając szybkie wystawienie pojazdu na sprzedaż przez każdego użytkownika. Wybór kategorii pojazdu odbywa się za pomocą ujednoliconej listy rozwijanej, co zapobiega powstawaniu błędów i literówek w bazie.

Formularz edycji (zabezpieczony i dostępny dla administratora) pozwala na sprawną korektę błędów w parametrach technicznych lub aktualizację opisu i ceny.


## 9. Widoki i interfejs użytkownika
Warstwa wizualna została zbudowana przy użyciu silnika Razor Views oraz frameworka Bootstrap. Interfejs aplikacji jest w pełni responsywny (RWD), co zapewnia komfortowe przeglądanie ofert zarówno na komputerach stacjonarnych, jak i na urządzeniach mobilnych. Do najważniejszych widoków należą:
* Strona główna z prezentacją platformy
* Publiczna lista ogłoszeń z panelem filtrów
* Karta szczegółów wybranego pojazdu
* Formularze dodawania oraz edycji
* Ekran logowania administratora

## 10. Filtrowanie ogłoszeń
Wyszukiwarka zaimplementowana na stronie głównej oraz liście ogłoszeń pozwala na dynamiczne zawężanie wyników. Użytkownik może filtrować bazę pojazdów m.in. po:
* Słowach kluczowych (fraza w tytule/opisie)
* Kategorii pojazdu
* Przedziale cenowym
* Roku produkcji

## 11. Bezpieczeństwo
Aplikacja wdraża podstawowe standardy bezpieczeństwa dla systemów webowych:
* **Autentykacja i Autoryzacja:** Dostęp do metod modyfikujących bazę danych (`HttpPost`) oraz do samego panelu zarządzania jest chroniony i wymaga zalogowania.
* **Ochrona przed CSRF:** Wszystkie formularze generują i weryfikują tokeny przeciwko nieautoryzowanemu wysyłaniu żądań (`[ValidateAntiForgeryToken]`).
* **Bezpieczne wylogowanie:** Akcja wylogowania została oparta o bezpieczną metodę `POST`, uniemożliwiającą przypadkowe wylogowanie przez zapytanie typu `GET`.


