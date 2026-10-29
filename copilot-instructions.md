# SmartRestaurant — Instrucciones para GitHub Copilot

## Arquitectura
- El proyecto utiliza ASP.NET Core MVC con Razor Views.
- No crear ni proponer una API separada.
- Respetar los proyectos existentes: Domain, Application, Infrastructure, Web y UnitTests.
- Domain no debe depender de Entity Framework Core, MVC ni SQL Server.
- Mantener las responsabilidades separadas entre las capas.

## Forma de trabajar
- Priorizar aprendizaje y revisión sobre generación masiva de código.
- Antes de modificar archivos, explicar el problema y proponer un plan breve.
- Realizar cambios pequeños y relacionados con una sola tarea.
- Explicar qué archivos se modificarán y por qué.
- No reemplazar archivos completos si bastan cambios localizados.
- No añadir paquetes NuGet sin justificar su necesidad.
- Crear o actualizar pruebas cuando corresponda.
- Explicar los cambios y señalar sus posibles riesgos.

## Restricciones del proyecto
- No modificar Program.cs, appsettings.json, DbContext ni migraciones sin coordinación con el responsable de infraestructura.
- No asumir que SQL Server, Entity Framework Core o las migraciones están configurados.
- Para reservas, validar nuevamente la disponibilidad de la mesa antes de confirmar y guardar.
- No bloquear temporalmente una mesa mientras el usuario completa el formulario.
- No aceptar cambios automáticamente sin revisión.