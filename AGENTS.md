# Instrukcje dla agentów WoltechEngine

[README.md](README.md) opisuje cel projektu, stan sześciu modułów i sposób uruchomienia. Sprawdź odpowiednie pliki źródłowe przed przyjęciem założeń o bieżącej implementacji.

## Granice modułów

- `WE.Core` utrzymuje dane ECS i czas symulacji. Zachowuj jego niezależność od OpenGL, okna i Avalonia.
- `WE.Graphics` odpowiada za renderowanie i zasoby GPU. Korzysta z danych `WE.Core`.
- `WE.Sandbox` jest punktem integracji silnika i miejscem na scenę demonstracyjną. Zmiany w rdzeniu i grafice sprawdzaj również na tym wejściu, gdy dotyczą działania demo.
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
