# Proyecto: Hogar de Ancianos Nuestra Señora del Rosario

## Tabla de Contenidos
1. [Introducción](#introducción)
2. [Flujo de Trabajo con Git](#flujo-de-trabajo-con-git)
   - [Creación y Gestión de Ramas](#creación-y-gestión-de-ramas)
   - [Estándares para Mensajes de Commits](#estándares-para-mensajes-de-commits)
   - [Comandos Git Frecuentes](#comandos-git-frecuentes)
3. [Estándares de Código](#estándares-de-código)
4. [Recomendaciones Generales](#recomendaciones-generales)

## Introducción

Este documento proporciona una guía clara para el equipo de desarrollo sobre cómo gestionar el flujo de trabajo, adherirse a los estándares de código, y seguir las mejores prácticas durante el desarrollo de [Nombre del Proyecto].

## Flujo de Trabajo con Git

### Creación y Gestión de Ramas

- **Trabaja en ramas específicas para cada funcionalidad**:
  - Crea una nueva rama para cada funcionalidad o tarea en la que trabajes. Usa nombres descriptivos y sigue una convención clara.
  - **Ejemplo**: `feature/sobre-nosotros`, `fix/bug-visualizacion-perfil`.
- **No acumules múltiples funcionalidades en una misma rama**:
  - Una vez que termines una funcionalidad, fusiona la rama con `main` o `develop` a través de un pull request, y crea una nueva rama para la siguiente tarea.
- **Nombra las ramas consistentemente**:
  - Usa prefijos como `feature/`, `fix/`, `improvement/` para indicar el tipo de trabajo que se está realizando.
- **Mantén las ramas sincronizadas**:
  - Regularmente sincroniza tu rama con `main` o `develop` para minimizar conflictos al fusionar.
- **Revisa y elimina ramas inactivas**:
  - Elimina las ramas que ya se han fusionado para mantener el repositorio limpio.

### Estándares para Mensajes de Commits

- **Usa prefijos claros en los mensajes de commit**:
  - `feat`: Para nuevas funcionalidades.
  - `fix`: Para corrección de bugs.
  - `docs`: Para cambios en la documentación.
  - `style`: Para cambios de formato.
  - `refactor`: Para refactorización sin agregar ni corregir funcionalidades.
  - `test`: Para añadir o modificar tests.
  - `chore`: Para tareas de mantenimiento.
- **Estructura del mensaje de commit**:
  - **Título breve**: 50 caracteres o menos, en presente y sin punto final.
  - **Descripción opcional**: Explica qué y por qué se hicieron los cambios. Incluye referencias a issues o PRs.

### Comandos Git Frecuentes

- **`git init`**: Inicializar un nuevo repositorio.
- **`git clone <url>`**: Clonar un repositorio remoto.
- **`git status`**: Ver el estado actual del repositorio.
- **`git add <archivo>`**: Añadir archivos al área de preparación.
- **`git commit -m "mensaje"`**: Crear un commit con un mensaje corto.
- **`git push`**: Enviar commits al repositorio remoto.
- **`git pull`**: Traer los cambios del repositorio remoto al repositorio local.
- **`git branch`**: Listar, crear o cambiar ramas.

## Estándares de Código

- **Lenguaje del código**: Todo debe estar en inglés, incluyendo nombres de variables, funciones, clases y comentarios.
- **Nombres de variables**: Deben ser claros, concisos y descriptivos. Usa nombres que reflejen el propósito de la variable o función.
- **Evita dependencias múltiples entre métodos**: Mantén los métodos independientes para facilitar su prueba y mantenimiento.
- **Sigue los principios SOLID**:
  - Responsabilidad Única, Abierto/Cerrado, Sustitución de Liskov, Segregación de Interfaces, Inversión de Dependencias.
- **Comentarios claros y funcionales**:
  - Los comentarios deben explicar la funcionalidad del código, no simplemente lo que hace.
- **Código limpio y entendible**:
  - Mantén las funciones y métodos pequeños y enfocados en una sola tarea.
  - Elimina código redundante y organiza el código de manera lógica.

## Recomendaciones Generales

- **Documentación clara de requisitos**: Asegúrate de que todos los miembros del equipo comprendan los requisitos antes de comenzar el desarrollo.
- **Metodologías ágiles**: Considera usar Scrum o Kanban para gestionar el trabajo y facilitar iteraciones rápidas.
- **Integración y despliegue continuo**: Configura pipelines de CI/CD para asegurar que las versiones sean estables y se puedan desplegar de manera segura.
- **Testing exhaustivo**: Desarrolla pruebas automatizadas que cubran tanto pruebas unitarias como de integración.
- **Gestión de la configuración**: Separa la configuración del código y mantén la información sensible cifrada.
- **Monitorización y mantenimiento**: Implementa sistemas de monitorización para asegurar el rendimiento y la estabilidad en producción.
