# Changelog

## 2026-05-01

- Додано базовий клас `Person` для `Customer` і `Mechanic`.
- Додано `virtual/override/base` через `GetDisplayName()`.
- Додано індексатор у `JobStatusHistoryObserver`.
- Додано LINQ extension methods з `Aggregate` і `Join`.
- Додано тести для наслідування, індексатора та LINQ extension methods.
- Оновлено README і UML class diagram.

## 2026-04-30

- Додано тестовий проект `CarWorkshop.Tests`.
- Додано unit-тести для `Money`, `Job` status transitions, `JobFactory`, pricing strategies, use cases і JSON persistence.
- Додано Moq для перевірки use cases через repository interfaces.
- Виправлено створення дубльованого `Job` для одного `Appointment`.
- Додано доменну перевірку, що новий `Appointment` не створюється в минулому.
- Додано перевірку сумісності валют перед завершенням `Job`.
- Додано `README.md` з інструкцією запуску, scope, патернами і тестами.
