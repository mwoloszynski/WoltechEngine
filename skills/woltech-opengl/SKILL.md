---
name: woltech-opengl
description: Rozwijaj i diagnozuj renderer Silk.NET w WE.Graphics, w tym shadery GLSL, geometrię, bufory i cykl życia zasobów OpenGL.
---

# Renderer OpenGL

- Zacznij od [RenderSystem.cs](../../WE.Graphics/RenderSystem.cs). Prześledź zgodność atrybutów shadera z formatem wierzchołka, konfiguracją VAO i typem indeksu. Obecna ścieżka używa jednej indeksowanej geometrii czworokąta oraz pozycji, rozmiaru i koloru z ECS.
- Przy zmianie filtra encji sprawdź kontrakt komponentów w `WE.Core`. Renderer obecnie rysuje encje zawierające zarówno `TransformComponent`, jak i `SpriteComponent`. Pozycja trafia bezpośrednio do przestrzeni przycięcia. Przy zmianach układu współrzędnych sprawdź też regułę odbicia w Sandboxie.
- Prześledź `OnLoad` i `OnClosing` w [Program.cs Sandboxa](../../WE.Sandbox/Program.cs). Twórz i zwalniaj uchwyty przy aktywnym kontekście OpenGL. Przy zmianach inicjalizacji uwzględnij zwalnianie uchwytów przy częściowym niepowodzeniu, np. błędzie linkowania shadera.
- Po zmianach w shaderach lub stanie GL zweryfikuj start, obie figury i zamknięcie okna Sandboxa. Kompilacja .NET nie uruchamia kompilatora GLSL.
