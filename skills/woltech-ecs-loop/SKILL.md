---
name: woltech-ecs-loop
description: Rozwijaj encje, komponenty, pomiar czasu i aktualizację symulacji w WE.Core oraz pętli WE.Sandbox.
---

# ECS i pętla gry

- Systemy sterowania postacią i kamerą udostępniaj przez publiczne API silnika. `WE.Runtime` łączy odczyt wejścia z tymi systemami, a `WE.Sandbox` konfiguruje przykładową scenę i wywołuje gotowe API.
- Zacznij od [Registry.cs](../../WE.Core/ECS/Registry.cs) i [CoreComponents.cs](../../WE.Core/Components/CoreComponents.cs). Rejestr przechowuje aktywne identyfikatory w liście i komponenty w słownikach. Przy dodaniu magazynu komponentów uwzględnij jego czyszczenie w `DestroyEntity`. Odczytuj komponenty z uwzględnieniem encji, które ich nie mają.
- Komponenty są strukturami. Po zmianie wartości pobranej ze słownika zapisz ją z powrotem. Jeśli aktualizacja tworzy lub usuwa encje, wykonaj modyfikacje listy poza jej bieżącą enumeracją.
- Sprawdź [TimeManager.cs](../../WE.Core/Time/TimeManager.cs) i `OnUpdate` w [Program.cs Sandboxa](../../WE.Sandbox/Program.cs). `GetDeltaTime()` zwraca sekundy i przesuwa znacznik poprzedniego odczytu. Pobierz czas raz na aktualizację i przekaż tę wartość systemom uczestniczącym w tym samym kroku.
- Przy zmianie zachowania ruchu zweryfikuj ruch czerwonego czworokąta, zmianę kierunku na granicach i nieruchomy niebieski czworokąt, zgodnie z zakresem zadania.
