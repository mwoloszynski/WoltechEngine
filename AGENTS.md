# Instrukcje dla agentów WoltechEngine

[README.md](README.md) opisuje aktualny stan modułów i sposób uruchomienia. Sprawdź odpowiednie pliki źródłowe przed przyjęciem założeń o bieżącej implementacji.

## Tryb pracy

Rozwijasz WoltechEngine wspólnie z użytkownikiem. Użytkownik sam pisze kod silnika, a Ty doradzasz i pokazujesz w czacie, jak wykonać i sprawdzić kolejny krok. Stosuj [woltech-guided-coding](skills/woltech-guided-coding/SKILL.md) przy zadaniach dotyczących kodu. Edytuj źródła wyłącznie na wyraźne polecenie wdrożenia konkretnego kroku.

Przed proponowaniem kodu dla nowej funkcji ustal z użytkownikiem, co ma się dziać i do czego ma to służyć. Dopiero na podstawie uzgodnionego zachowania dobierz kolejny krok techniczny.

## Kierunek rozwoju

- WoltechEngine ma służyć do tworzenia gier 3D z gatunków city builder, survival i simulator. Inspiracje użytkownika to zbieranie zasobów i przetrwanie z *The Forest*, budowanie i rozwój osady z *Twierdzy* oraz planowanie przestrzeni i symulacja miasta z *Cities: Skylines*.
- W docelowej grze gracz steruje główną postacią, porusza się nią po świecie 3D i za jej pośrednictwem wchodzi w interakcje z otoczeniem, buduje i zarządza osadą. Perspektywa kamery i sposób wydawania poleceń pozostają do ustalenia z użytkownikiem.
- Przy przejściu z prototypu 2D do 3D wykorzystuj istniejący podział na rejestr ECS, pętlę aplikacji i renderer. Silnik dostarcza mechanizmy wielokrotnego użytku, takie jak scena, renderowanie 3D, kamera i wejście. Koszty budynków, zasady budowy i gospodarka zasobami należą do konkretnej gry.
- Propozycja pierwszego scenariusza gry do omówienia to zebranie drewna przez postać i postawienie chaty w wybranym miejscu.

## Plan budowy silnika

Punktem wyjścia są istniejący rejestr ECS, `GameHost` z oknem, wejściem i pętlą aktualizacji oraz renderer OpenGL używany przez demo 2D. Pierwszy etap kończy się, gdy można poruszyć postacią po małej scenie 3D, wskazać miejsce i postawić obiekt.

1. **Renderowanie 3D.** Dodaj pozycje XYZ, siatkę sześcianu, transformację obiektu, stałą kamerę z macierzami widoku i projekcji oraz test głębi. Zacznij od jednego sześcianu, potem dodaj drugi i sprawdź, czy poprawnie się zasłaniają bez względu na kolejność rysowania.
2. **Postać, wejście i kamera.** Udostępnij odczyt myszy i akcje sterowania w silniku. Po ustaleniu perspektywy przygotuj demonstrację poruszania główną postacią po płaskiej powierzchni i oglądania sceny z wybranego widoku. Sprawdź zachowanie projekcji po zmianie rozmiaru okna zgodnie z zasadą uzgodnioną dla kamery 3D.
3. **Wskazywanie i ustawianie obiektów.** Przelicz wskazanie kursora lub celownika na promień w świecie, znajdź jego przecięcie z podłożem i wyświetl podgląd obiektu. Sprawdź, czy gracz może za pomocą postaci wybrać miejsce i postawić tam obiekt.
4. **Sprawdzanie zajętości.** Po ustaleniu sposobu budowania użyj zapytań o obszary kolizji 3D lub zajętość pól. Sprawdź, czy scena rozpoznaje zajęte miejsce i pozwala postawić obiekt obok.
5. **Modele i materiały.** Przygotuj prostą ścieżkę importu w `WE.AssetPipeline` oraz obsługę materiałów, tekstur i zasobów GPU w `WE.Graphics`. Sprawdź, czy można podmienić sześcian na model i użyć tego samego modelu w kilku miejscach sceny.
6. **Zapis i odczyt sceny.** Ustal format sceny, identyfikatory zasobów i zakres zapisywanego stanu. Sprawdź, czy po zamknięciu i ponownym uruchomieniu aplikacji można odtworzyć układ postawionych obiektów.

Kolejne obszary rozwijaj według potrzeb prototypu gry:

- **Świat i poruszanie się.** Teren o zmiennej wysokości, zapytania o powierzchnię i kolizje postaci z otoczeniem.
- **Symulacja i nawigacja.** Krok symulacji niezależny od renderowania, pauza i tempo czasu oraz wyszukiwanie ścieżek. Przykład do sprawdzenia to pracownik dostarczający drewno do magazynu.
- **Edytor.** Po uruchomieniu zapisu sceny rozbuduj `WE.Editor` o wczytywanie scen, wybór encji, edycję ich położenia i właściwości oraz zapis.
- **Większe mapy i wydajność.** Zmierz czas renderowania i symulacji na większej osadzie. Wybierz usprawnienia na podstawie pomiarów, np. rysowanie wielu kopii modeli lub doczytywanie fragmentów mapy.
- **Uruchamianie gotowej gry.** Przygotuj uruchamialną aplikację z zasobami. Rozwijaj `WE.Launcher`, jeśli potrzebny będzie osobny wybór gry lub konfiguracji startowej.

## Granice modułów

- `WE.Core` utrzymuje dane ECS i czas symulacji. Zachowuj jego niezależność od OpenGL, okna i Avalonia.
- `WE.Graphics` odpowiada za renderowanie i zasoby GPU. Korzysta z danych `WE.Core`.
- `WE.Runtime` łączy rdzeń i renderer. Udostępnia hosta gry oraz zarządza oknem, wejściem i pętlą aplikacji.
- `WE.Sandbox` służy wyłącznie do prezentowania publicznego API silnika na przykładowych scenach. Umieszczaj w nim dane sceny, konfigurację dema i wywołania API. Systemy wejścia, sterowania postacią i kamerą oraz inne mechanizmy wielokrotnego użytku implementuj w odpowiednich modułach silnika i udostępniaj przez publiczne API. Zmiany w silniku, które wpływają na demo, sprawdzaj również w Sandboxie.
- `WE.Editor` to aplikacja F#/Avalonia. `WE.AssetPipeline` i `WE.Launcher` to osobne szkielety. Dodawaj powiązania między tymi modułami a silnikiem w zadaniach, które ich potrzebują.
- `.vs/`, `bin/` i `obj/` zawierają dane IDE lub wyniki kompilacji. Zmiany wprowadzaj w źródłach i plikach projektów.

## Weryfikacja

- Po zmianach w kodzie uruchom `dotnet build WoltechEngine.slnx` z katalogu głównego. Jeśli pakiety już odtworzono, możesz użyć `--no-restore`. Przy anulowaniu kompilacji równoległej ponów z `--disable-build-servers -m:1`.
- Dla zmian widocznych w oknie uruchom odpowiednią aplikację z [README.md](README.md), jeśli masz dostęp do sesji graficznej. Podaj rzeczywisty wynik testu uruchomieniowego i kompilacji.
- Zaktualizuj README, gdy zmieniają się punkty wejścia, zależności modułów lub opisane możliwości.

## Skille projektowe

Przy zadaniu z danego obszaru otwórz wskazany `SKILL.md`. Jeśli zadanie obejmuje kilka obszarów, użyj odpowiednich instrukcji razem.

| Obszar | Skill |
| --- | --- |
| Encje, komponenty, czas i ruch w Sandboxie | [woltech-ecs-loop](skills/woltech-ecs-loop/SKILL.md) |
| RenderSystem, shadery GLSL, bufory i zasoby OpenGL | [woltech-opengl](skills/woltech-opengl/SKILL.md) |
| Edytor F#, widoki AXAML i start Avalonia | [woltech-editor](skills/woltech-editor/SKILL.md) |
