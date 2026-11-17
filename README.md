# WoltechEngine

WoltechEngine to prototyp silnika gier na .NET 10 z demonstracjami renderowania 2D i 3D. Przechowuje obiekty gry jako encje z komponentami, aktualizuje ich pozycje i renderuje je przez OpenGL. `WE.Runtime` udostępnia punkt wejścia silnika, a `WE.Sandbox` pokazuje, jak uruchomić przez niego przykładową scenę.

## Stan projektów

| Projekt | Co jest w kodzie |
| --- | --- |
| [WE.Core](WE.Core/WE.Core.csproj) | Rejestr ECS (*Entity Component System*) zarządza encjami i ich komponentami. `TransformComponent` zawiera pozycję i prędkość 2D, a `SpriteComponent` kolor RGB i rozmiar. `TimeManager` mierzy czas aktualizacji, a `MovementSystem` aktualizuje pozycje na podstawie prędkości i upływu czasu. |
| [WE.Graphics](WE.Graphics/WE.Graphics.csproj) | Renderer C# korzysta z Silk.NET 2.23.0 i OpenGL. Encje 3D mają własny kolor i transformację, a encje używające tej samej siatki współdzielą jej bufory GPU. Kamera 3D obsługuje rzut perspektywiczny i ortograficzny. |
| [WE.Runtime](WE.Runtime/WE.Runtime.csproj) | `GameHost.Run` obsługuje okno, wejście i pętlę aplikacji. Twórca gry wybiera tytuł okna oraz tryby ruchu 3D i kamery w `GameSettings`, a przy ładowaniu sceny wskazuje encję sterowaną przez gracza. |
| [WE.Sandbox](WE.Sandbox/WE.Sandbox.csproj) | Przykład użycia `GameHost.Run`. W scenie 2D czerwony czworokąt odbija się od granic, a niebieskim można sterować strzałkami. W scenie 3D kamera śledzi sterowaną postać. |
| [WE.Editor](WE.Editor/WE.Editor.fsproj) | Szkielet aplikacji desktopowej w F# i Avalonia 12.1.4. Otwiera okno powitalne i jest niezależny od projektów silnika. |
| [WE.AssetPipeline](WE.AssetPipeline/WE.AssetPipeline.csproj) | Szkielet biblioteki C# z pustą klasą. Import i przetwarzanie zasobów są jeszcze do zaimplementowania. |
| [WE.Launcher](WE.Launcher/WE.Launcher.csproj) | Szkielet aplikacji konsolowej wypisujący `Hello, World!`. Integracja z uruchamianiem gier jest jeszcze do zaimplementowania. |

Wszystkie projekty należą do [WoltechEngine.slnx](WoltechEngine.slnx). Sandbox korzysta z `WE.Runtime` do uruchomienia sceny i z `WE.Core` do jej zdefiniowania. `WE.Runtime` łączy `WE.Core` z `WE.Graphics`, a renderer odczytuje dane sceny z `WE.Core`.

## Jak działa prototyp

1. [Sandbox](WE.Sandbox/Program.cs) wywołuje [`GameHost.Run`](WE.Runtime/GameHost.cs), przekazując tytuł okna oraz metody `LoadScene` i `UpdateScene`. Host inicjalizuje okno i renderer, tworzy rejestr ECS i przekazuje go do `LoadScene`.
2. Przy każdej aktualizacji host pobiera czas od poprzedniego kroku i wywołuje [`MovementSystem`](WE.Core/Systems/MovementSystem.cs). Potem wywołuje `UpdateScene`, w której Sandbox odwraca poziomą prędkość czerwonej figury, gdy jej środek przekroczy X = ±1.
3. Podczas renderowania host czyści bufory koloru i głębi i przekazuje rejestr do [`RenderSystem`](WE.Graphics/RenderSystem.cs). Dla siatek 3D renderer przekazuje do shadera macierz modelu, widoku i projekcji złożoną w jedną macierz. Każda rysowana encja ma osobne wywołanie rysowania.
4. Po wyjściu z pętli host zwalnia renderer, obiekt API OpenGL i okno, w tej kolejności.

Tekstury, kolizje 3D, zapis scen i integracja edytora to obszary do dalszego rozwoju. W rozwiązaniu nie ma projektu testowego.

## Kompilacja i uruchamianie

Polecenia uruchamiaj z głównego katalogu projektu. Potrzebny jest .NET 10 SDK, a Sandbox wymaga sesji graficznej z obsługą GLSL 330.

```powershell
dotnet build WoltechEngine.slnx
dotnet run --project WE.Sandbox/WE.Sandbox.csproj --no-launch-profile
dotnet run --project WE.Sandbox/WE.Sandbox.csproj --no-launch-profile -- --3d
dotnet run --project WE.Editor/WE.Editor.fsproj
```

Tryby sceny `--3d` ustawisz w `GameSettings` w [Program.cs](WE.Sandbox/Program.cs). W `FreePan` strzałki przesuwają kamerę względem aktualnego widoku. Gdy ruch postaci ustawiono na `ClickToMove`, prawy klik wyznacza jej cel, a `WASD` także przesuwa kamerę. Przy `PlayerMovementMode.Wasd` te klawisze poruszają postacią. `FollowPlayer` ustawia kamerę śledzącą postać. `CameraPanSpeed` określa prędkość przesuwania w `FreePan` w jednostkach świata na sekundę. W obu trybach kamery przeciąganie środkowym przyciskiem obraca widok, a rolka przybliża i oddala.

Sandbox ustawia `UseOrthographicProjection = true` i `CameraInclination = 38` w `GameSettings`, co daje widok po przekątnej, około 34° w dół. Ustaw `UseOrthographicProjection = false`, żeby wybrać perspektywę. `CameraInclination` przyjmuje liczby całkowite od 0 do 100 jako procent kąta 90°. Wartość 100 daje kąt 89,9°, aby zachować kierunek obrotu nad sceną. Gdy pominiesz ustawienie, nachylenie wynika z początkowej pozycji i celu kamery. W rzucie ortograficznym rolka zmienia wysokość widocznego obszaru (`Camera3D.OrthographicHeight`).

## Praca z AI

[AGENTS.md](AGENTS.md) opisuje granice modułów, sposób weryfikacji zmian i zasady pracy krok po kroku z asystentem. Wskazuje też skille projektowe dla ECS, grafiki OpenGL i edytora Avalonia.
