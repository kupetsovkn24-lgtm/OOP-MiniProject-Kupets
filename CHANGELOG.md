# Changelog

## [Unreleased]

### Added

- Додано фінальну документацію, сценарій демонстрації, syllabus coverage і release plan для `v1.0.0`.

### Changed

- Прибрано невикористану залежність із `WorkshopFacade`.
- Уточнено XML-документацію ключових API та фінальні UML/README.

### Fixed

- Синхронізовано статуси backlog і дублікати UML-діаграм із фактичним станом проєкту.

## [0.3.0] - 2026-05-25

### Added

- Додано `9` integration tests, test strategy, test matrix і `TESTING.md`.
- Підключено coverage report у GitHub Actions.

### Changed

- Додано контрольоване журналювання відмов JSON persistence.

## [0.2.0] - 2026-05-25

### Added

- Додано асинхронний JSON persistence, скасування записів/робіт і розширене меню.
- Додано iteration 2 documentation, LINQ-запити та тести нових сценаріїв.

## [0.1.0] - 2026-05-20

### Added

- Підготовлено репозиторій Lab 34 із шарами solution, UML, CI, README і першим вертикальним зрізом.

## Earlier coursework history - 2026-05-01

- Додано базовий клас `Person` для `Customer` і `Mechanic`.
- Додано `virtual/override/base` через `GetDisplayName()`.
- Додано індексатор у `JobStatusHistoryObserver`.
- Додано LINQ extension methods з `Aggregate` і `Join`.
- Додано тести для наслідування, індексатора та LINQ extension methods.
- Оновлено README і UML class diagram.

## Earlier coursework history - 2026-04-30

- Додано тестовий проект `CarWorkshop.Tests`.
- Додано unit-тести для `Money`, `Job` status transitions, `JobFactory`, pricing strategies, use cases і JSON persistence.
- Додано Moq для перевірки use cases через repository interfaces.
- Виправлено створення дубльованого `Job` для одного `Appointment`.
- Додано доменну перевірку, що новий `Appointment` не створюється в минулому.
- Додано перевірку сумісності валют перед завершенням `Job`.
- Додано `README.md` з інструкцією запуску, scope, патернами і тестами.
