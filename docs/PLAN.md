# План навчальної практики з ООП

## Контекст

Тема: **Система управління ремонтними задачами СТО**
Стек: C#/.NET (вимога викладача)
Тип: навчальне ООП-ядро, не бойовий продукт

## Мета

Здати практику + отримати валідний доменний прототип, який залишиться корисним як mental model, навіть якщо потенційний клієнт (СТО Діми) не дасть зелене світло.

## Принцип

Ядро на C# **не переноситься** в майбутній продукт як код. Переноситься: модель, правила, розуміння домену, словник. Це нормально і це достатньо.

---

## Тиждень 1 — моделювання, без коду

**Що робиться:**
- Опис домену по факту візиту на СТО (DOMAIN.md)
- UML class diagram
- UML state diagram для JobStatus
- UML sequence diagram для основного сценарію
- Список винятків і edge cases (в DOMAIN.md, не на діаграмах)

**Що НЕ робиться:**
- жодного рядка C#
- жодного `dotnet new`
- структура солюшну ще не створюється

**Критерій готовності:**
Можу за 5 хвилин на словах розповісти весь цикл "дзвінок → авто виїхало" з усіма правилами і винятками.

---

## Тиждень 2 — Domain layer

**Що робиться:**
- `dotnet new sln`, структура проєктів
- Value Objects: `Money`, `Vin`, `PhoneNumber`
- Entities: `Customer`, `Vehicle`, `Mechanic`
- Aggregates: `Appointment`, `Job`
- Enums: `AppointmentStatus`, `JobStatus`
- Custom exceptions: `DomainException` як база, далі специфічні
- Перевантажені оператори на `Money`
- Метод `Job.TransitionTo(JobStatus)` з таблицею дозволених переходів

**Свідоме рішення:**
НЕ використовуємо State pattern. Простий метод з таблицею переходів читабельніший для 5 станів. На захисті: "State був би виправданий при більшій кількості станів і складніших правилах".

**Критерій готовності:**
Domain projects компілюється, не залежить ні від чого крім `System.*`.

---

## Тиждень 3 — Application + Infrastructure

**Application use cases (по одному класу на use case, SRP):**
- `CreateAppointmentUseCase`
- `ConfirmAppointmentUseCase`
- `CreateJobFromAppointmentUseCase`
- `AssignMechanicUseCase`
- `StartJobUseCase`
- `CompleteJobUseCase`

**Repositories:**
- Інтерфейси `ICustomerRepository`, `IVehicleRepository`, `IAppointmentRepository`, `IJobRepository`, `IMechanicRepository` — у Domain layer
- In-memory реалізації — в Infrastructure
- Базовий `Repository<T>` через generics (закриває вимогу generics)

**Infrastructure:**
- DTO для кожної доменної сутності
- Mapper між Domain і DTO
- JSON save/load через `System.Text.Json`
- НЕ серіалізуємо domain класи напряму

**Критерій готовності:**
Console-сценарій працює end-to-end з in-memory repository і JSON persistence.

---

## Тиждень 4 — патерни (точково, виправдано)

**Обов'язкові з вимог викладача:**

1. **Factory** — `JobFactory.CreateFromAppointment(appointment)`
   Виправдано: створення Job має правила і залежить від Appointment.

2. **Strategy** — `IPricingStrategy` з реалізаціями:
   - `FixedPricingStrategy`
   - `HourlyPricingStrategy`
   - `PartsAndLaborPricingStrategy`
   Виправдано: різні типи робіт реально рахуються по-різному.

3. **Observer** — `IJobStatusObserver` + `JobStatusHistoryObserver`
   Виправдано: історія зміни статусів — реальна потреба.

4. **Decorator** — `LoggingRepositoryDecorator<T>`
   Виправдано: логування доступу до даних без зміни repositories. Бонус: закриває generics + decorator одним пострілом.

5. **Facade** — `WorkshopFacade` як єдина точка входу для ConsoleApp
   Виправдано: ConsoleApp не повинен знати про всі use cases окремо.

**НЕ використовуємо:**
- Singleton (антипатерн у більшості випадків, викладачі це знають)
- State (заміняємо простим методом, обґрунтовуємо)
- Composite (натягнуто на цей домен)

**LINQ-запити в use cases:**
- "знайти зайняті слоти на день"
- "знайти роботи механіка за період"
- "порахувати виручку за тиждень"
- "групування робіт по механіках"

---

## Тиждень 5 — тести, рефакторинг, документація

**Тести (NUnit або xUnit):**
- AAA pattern
- параметризовані тести (TestCase / Theory) — окрема вимога з маршруту
- Moq для repositories — окрема вимога

**Що покривається тестами:**
- усі бізнес-правила (див. DOMAIN.md)
- переходи статусів (валідні і невалідні)
- розрахунок вартості різними стратегіями
- JSON DTO save/load
- зайнятість механіка
- зайнятість часового слоту

**Рефакторинг:**
Self-review по чеклісту: магічні числа, довгі методи, погані імена, дублювання.

**Документація:**
- README.md з UML, патернами і обґрунтуванням
- секція "що НЕ входить у scope і чому" — обов'язково
- CHANGELOG.md
- інструкція запуску

---

## Захист

Готові відповіді на типові питання:
- "Чому Strategy для pricing?" — реальні різні моделі ціноутворення на СТО
- "Чому не State pattern?" — свідома декомпозиція, обґрунтування в README
- "Де SOLID дав виграш?" — DIP через repository interfaces, OCP через Strategy
- "Що поза scope?" — окрема секція в README
- "Як перевіряється зайнятість?" — через repository query, не через стан в Mechanic
