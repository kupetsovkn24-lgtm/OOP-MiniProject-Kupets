# Developer Guide

## Структура solution

| Проєкт | Відповідальність |
| --- | --- |
| `CarWorkshop.Domain` | Сутності, value objects, інтерфейси, доменні правила і патерни |
| `CarWorkshop.Application` | Use cases, facade і LINQ-запити |
| `CarWorkshop.Infrastructure` | In-memory repositories, DTO, mapping і JSON persistence |
| `CarWorkshop.Console` | Меню та показ результатів користувачу |
| `CarWorkshop.Tests` | Unit та integration tests |

Залежності йдуть від UI та Infrastructure до контрактів і Domain, а не навпаки.

## Ключові рішення

- `Job` містить життєвий цикл і перевіряє допустимі статуси через `Dictionary<JobStatus, HashSet<JobStatus>>`.
- `IPricingStrategy` дозволяє додати інший розрахунок ціни окремим класом.
- `WorkshopFacade` дає Console простий API без дублювання бізнес-логіки.
- `JsonPersistence` серіалізує DTO, потім відновлює domain objects через mapper.

## Як розширювати

- Нова ціна: створити реалізацію `IPricingStrategy`.
- Новий запит: додати метод application-рівня або extension method, а не логіку в Console.
- Нове сховище: реалізувати Infrastructure-рішення поверх існуючих repository contracts.
- Нова реакція на статус роботи: реалізувати `IJobStatusObserver`.

## Помилки

- Порушення правил предметної області повідомляються доменними винятками.
- Некоректні JSON-дані призводять до `InvalidDataException`.
- При невдалому завантаженні репозиторії не замінюються частково відновленим станом.

## Перевірка

```powershell
dotnet restore CarWorkshop.sln
dotnet build CarWorkshop.sln --no-restore
dotnet test CarWorkshop.sln --no-build
```

Деталі coverage і матриця сценаріїв: [TESTING.md](TESTING.md) та [docs/test-matrix.md](docs/test-matrix.md).
