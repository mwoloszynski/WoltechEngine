# WoltechEngine

WoltechEngine to zalążek własnego silnika gier na .NET 10. Obecny prototyp demonstruje podstawy silnika 2D. Przechowuje obiekty gry jako encje z komponentami, aktualizuje ich pozycje w pętli okna i przekazuje dane do renderera OpenGL. Punktem wejścia do tego prototypu jest `WE.Sandbox`.

## Stan projektów

| Projekt | Co jest w kodzie |
| --- | --- |
| [WE.Core](WE.Core/WE.Core.csproj) | Rejestr ECS (*Entity Component System*) nadaje encjom identyfikatory, przechowuje komponenty w słownikach i usuwa je przy niszczeniu encji. `TransformComponent` zawiera pozycję i prędkość 2D, `SpriteComponent` kolor RGB i rozmiar. `TimeManager` mierzy czas całkowity i odstępy między aktualizacjami za pomocą `Stopwatch`. |
| [WE.Graphics](WE.Graphics/WE.Graphics.csproj) | Renderer C# korzysta z Silk.NET 2.23.0 i OpenGL. Współdzieli geometrię czworokąta zapisaną w buforach GPU i rysuje ją dla każdej encji mającej oba komponenty. Wbudowane shadery GLSL 330 pobierają pozycję, rozmiar i kolor. |
| [WE.Sandbox](WE.Sandbox/WE.Sandbox.csproj) | Aplikacja demonstracyjna tworzy okno Silk.NET i spina rejestr, zegar i renderer. Scena jest zapisana w kodzie. Czerwony czworokąt porusza się poziomo i zmienia kierunek po przekroczeniu granicy, a niebieski pozostaje nieruchomy. |
| [WE.Editor](WE.Editor/WE.Editor.fsproj) | Szkielet aplikacji desktopowej w F# i Avalonia 12.1.4. Otwiera okno powitalne i jest niezależny od projektów silnika. |
| [WE.AssetPipeline](WE.AssetPipeline/WE.AssetPipeline.csproj) | Szkielet biblioteki C# z pustą klasą. Import i przetwarzanie zasobów są jeszcze do zaimplementowania. |
| [WE.Launcher](WE.Launcher/WE.Launcher.csproj) | Szkielet aplikacji konsolowej wypisujący `Hello, World!`. Integracja z uruchamianiem gier jest jeszcze do zaimplementowania. |

Wszystkie sześć projektów należy do [WoltechEngine.slnx](WoltechEngine.slnx). `WE.Sandbox` odwołuje się do `WE.Core` i `WE.Graphics`, a `WE.Graphics` do `WE.Core`. Pozostałe projekty nie mają referencji międzyprojektowych.

## Jak działa prototyp

1. `OnLoad` w [WE.Sandbox/Program.cs](WE.Sandbox/Program.cs) pobiera kontekst OpenGL, uruchamia zegar, inicjalizuje renderer i tworzy dwie encje.
2. `OnUpdate` pobiera czas od poprzedniej aktualizacji i dodaje do pozycji iloczyn prędkości oraz tego czasu. Przekroczenie przez środek czerwonego czworokąta wartości X = ±1 odwraca jego prędkość poziomą.
3. `OnRender` czyści ekran i wywołuje [RenderSystem](WE.Graphics/RenderSystem.cs). Shader zapisuje pozycję bezpośrednio w przestrzeni przycięcia, a każda widoczna encja ma osobne wywołanie rysowania.
4. `OnClosing` zwalnia zasoby renderera przed zwolnieniem obiektu API OpenGL.

Aktualna scena ma stały zestaw obiektów i prostą regułę ruchu. Kamera, tekstury, obsługa wejścia, kolizje między obiektami, zapis scen i integracja edytora to obszary do dalszego rozwoju. W rozwiązaniu nie ma projektu testowego.

## Kompilacja i uruchamianie

Polecenia uruchamiaj z głównego katalogu projektu. Potrzebny jest .NET 10 SDK, a Sandbox wymaga sesji graficznej z obsługą GLSL 330.

```powershell
dotnet build WoltechEngine.slnx
dotnet run --project WE.Sandbox/WE.Sandbox.csproj
dotnet run --project WE.Editor/WE.Editor.fsproj
```

W trakcie przeglądu kompilacja całego rozwiązania na .NET SDK 10.0.402 z użyciem wcześniej odtworzonych pakietów zakończyła się bez błędów. Zgłosiła 9 ostrzeżeń `CS8618` dotyczących inicjalizacji pól w `WE.Graphics` i `WE.Sandbox`. Zachowanie okien opisano na podstawie kodu.

## Praca z AI

[AGENTS.md](AGENTS.md) opisuje granice modułów, sposób weryfikacji zmian i wskazuje skille dla ECS i pętli gry, grafiki OpenGL oraz edytora Avalonia.
