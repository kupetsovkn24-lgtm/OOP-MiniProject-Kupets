# Архітектурні рішення (ADR-lite)

Коротко: що вирішено, чому, які альтернативи відкинуто.

---

## ADR-001: Чистий C# ООП без фреймворку

**Рішення:** Domain і Application layers — чистий .NET без ASP.NET, EF, DI-контейнерів. Тільки `System.*`.

**Чому:**
- Викладач хоче бачити ООП, не вміння конфігурувати фреймворк
- Domain не повинен залежати від інфраструктури (Clean Architecture)
- Простіше тестувати

**Альтернатива:** ASP.NET + EF Core. Відкинуто — додає шуму без академічної цінності.

---

## ADR-002: ConsoleApp як точка входу

**Рішення:** Демонстрація сценаріїв через консольний застосунок.

**Чому:**
- Викладач не вимагає UI
- Дозволяє показати use cases через скриптовий сценарій
- Швидко

**Альтернатива:** WinForms / WPF / Web. Відкинуто — UI не входить у scope.

---

## ADR-003: JSON через DTO + Mapper

**Рішення:** Доменні класи не серіалізуються напряму. DTO + Mapper в Infrastructure.

**Чому:**
- Domain не залежить від `System.Text.Json`
- DTO може еволюціонувати окремо від домену
- Захищає від випадкового експоузу внутрішнього стану через JSON

**Альтернатива:** атрибути `[JsonInclude]` на доменних класах. Відкинуто — зливає шари.

---

## ADR-004: Замість State pattern — метод TransitionTo з таблицею

**Рішення:** `Job.TransitionTo(JobStatus newStatus)` з внутрішньою таблицею дозволених переходів. State pattern не використовується.

**Чому:**
- 5 станів — мало для виправдання State
- Таблиця переходів читається за 10 секунд, State — за 5 хвилин по класах
- На захисті обґрунтовуємо: "State був би виправданий при більшій кількості станів і складніших правилах переходу"

**Альтернатива:** State pattern з абстрактним `JobState` і конкретними станами. Відкинуто як overengineering для цього scope.

**Ризик:** викладач може спитати "чому не State?". Відповідь готова.

---

## ADR-005: In-memory repositories як основна реалізація

**Рішення:** Репозиторії — `List<T>` всередині, з JSON load/save при старті/завершенні.

**Чому:**
- БД поза scope
- Достатньо для демонстрації pattern-у Repository
- Тести не потребують infrastructure setup

**Альтернатива:** SQLite, EF Core, файлова БД. Відкинуто — додає складності.

---

## ADR-006: Customer на Appointment — через Vehicle.Owner

**Рішення:** Appointment не має прямого посилання на Customer. Дістається через `Appointment.Vehicle.Owner`.

**Чому:**
- Уникає неконсистентності "Appointment.Customer ≠ Vehicle.Owner"
- Менше дублювання даних

**Виключення:** якщо реально потрібен сценарій "не власник привіз авто" — переглянути. Зараз поза scope.

---

## ADR-007: Mechanic не зберігає список активних Jobs

**Рішення:** Зайнятість механіка перевіряється через `IJobRepository.IsMechanicAvailable(mechanicId, scheduledAt)`. У самій сутності Mechanic немає колекції Jobs.

**Чому:**
- Уникає двостороннього зв'язку
- Single source of truth — Job знає свого Mechanic, Mechanic нічого не знає про Jobs
- Простіше серіалізується

**Альтернатива:** колекція `Mechanic.ActiveJobs`. Відкинуто — створює циклічні залежності і ускладнює серіалізацію.

---

## ADR-008: Money як value object з перевантаженими операторами

**Рішення:** `Money` — immutable VO з `==`, `!=`, `+`, `-`. Операції тільки між Money однієї валюти.

**Чому:**
- Закриває вимогу "перевантаження операторів" з маршруту
- Природно лягає на домен
- Захищає від помилок типу `decimal price + decimal tax` без розуміння валюти

---

## ADR-009: Custom exception ієрархія

**Рішення:**

```
DomainException (base)
├── InvalidJobTransitionException
├── MechanicUnavailableException
├── TimeSlotConflictException
├── InvalidMoneyAmountException
└── JobAlreadyCompletedException
```

**Чому:**
- Закриває вимогу "Custom Exceptions" з маршруту
- Дозволяє Application layer ловити конкретні винятки
- Кожен виняток несе доменний сенс, не технічну деталь

---

## ADR-010: Strategy для розрахунку вартості

**Рішення:** `IPricingStrategy` з реалізаціями `FixedPricing`, `HourlyPricing`, `PartsAndLaborPricing`.

**Чому:**
- Закриває обов'язкову вимогу Strategy pattern
- Природно лягає на домен — реальні СТО використовують різні моделі ціноутворення
- OCP: новий тип ціноутворення додається без зміни Job

---

## ADR-011: Observer для історії статусів

**Рішення:** `IJobStatusObserver` + `JobStatusHistoryObserver` через `event EventHandler<JobStatusChangedEventArgs>`.

**Чому:**
- Закриває обов'язкову вимогу Observer pattern
- Природно лягає на домен — історія змін потрібна реально
- Розв'язує Job від компонента, який пише історію

---

## ADR-012: Decorator на репозиторіях

**Рішення:** `LoggingRepositoryDecorator<T> : IRepository<T>` — обгортає базовий repository і логує операції.

**Чому:**
- Закриває вимогу Decorator pattern
- Закриває вимогу generics одночасно
- OCP: логування додається без зміни самих репозиторіїв
