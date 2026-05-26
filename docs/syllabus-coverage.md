# Syllabus Coverage

| Група тем | Реалізація у CarWorkshop | Статус |
| --- | --- | --- |
| Основи ООП | `Customer`, `Vehicle`, `Appointment`, `Job`, `Mechanic`, інкапсульовані стани | Використано |
| Абстракції, поліморфізм, інтерфейси | `Person`, `Customer`/`Mechanic`, repository interfaces, `IPricingStrategy` | Використано |
| Generics і колекції | `IRepository<T>`, `List<T>`, `Dictionary<JobStatus, HashSet<JobStatus>>` | Використано |
| LINQ і делегати | фільтрація/агрегація робіт, `Action<string>` для журналювання, `Func<Job, bool>` для аналітичного звіту IW29 | Використано, посилено IW29 |
| Помилки й persistence | доменні винятки, `InvalidDataException`, async JSON DTO save/load | Використано |
| SOLID | розділені шари, dependency inversion через контракти, вузькі use cases | Використано |
| Патерни | Repository, Strategy, Factory, Observer, Facade, Decorator | Використано |
| UML | class і sequence diagrams, додаткові diagram sources | Використано |
| Тестування | xUnit, Moq, unit та integration tests, coverage і CI | Використано |
| Рефакторинг | видалення зайвої залежності facade, документація API, release scope review | Використано |
| Продуктивність | microanalysis `Dictionary` + `HashSet`, оцінка LINQ `O(n)` | Частково, достатньо для scope |

## Висновок

Проєкт покриває обов'язкові теми курсу на одному невеликому, але завершеному сценарії. Додаткові технології не додаються, якщо вони не роблять цей сценарій зрозумілішим або надійнішим.

У IW29 точково посилено делегати, LINQ і тестування: гнучкий фільтр використовується у звіті дорогих завершених ремонтів, а звіт демонструється через Console і перевіряється тестами.
