---
name: woltech-editor
description: Rozwijaj powłokę WE.Editor, widoki AXAML i kod F# oraz diagnozuj inicjalizację aplikacji Avalonia.
---

# Edytor Avalonia

- Prześledź uruchomienie od [Program.fs](../../WE.Editor/Program.fs) do [App.axaml.fs](../../WE.Editor/App.axaml.fs), który przypisuje `MainWindow` do desktopowego cyklu życia aplikacji. Widok startowy ma parę `MainWindow.axaml` i `MainWindow.axaml.fs`.
- Przy dodawaniu typów F# wpisz pliki do [WE.Editor.fsproj](../../WE.Editor/WE.Editor.fsproj) w kolejności zależności. Przy zmianach widoków sprawdź zgodność `x:Class` z przestrzenią nazw i typem w F#.
- Logikę interfejsu trzymaj w edytorze. Jeśli zadanie wymaga połączenia z silnikiem, sprawdź aktualne referencje projektów i określ, kto zarządza stanem podglądanej sceny. Sandbox jest dziś osobną aplikacją demonstracyjną.
- Po zmianie widoku lub inicjalizacji uruchom `dotnet run --project WE.Editor/WE.Editor.fsproj` i sprawdź, czy otwiera się zmienione okno.
