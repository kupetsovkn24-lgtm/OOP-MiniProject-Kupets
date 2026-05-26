# CarWorkshop

[![.NET CI](https://github.com/kupetsovkn24-lgtm/OOP-MiniProject-Kupets/actions/workflows/dotnet.yml/badge.svg)](https://github.com/kupetsovkn24-lgtm/OOP-MiniProject-Kupets/actions/workflows/dotnet.yml)

Навчальний ООП-проект на C#/.NET: **система управління ремонтними задачами СТО**.

Проект моделює простий цикл роботи СТО: клієнт записується на ремонт, запис підтверджується, з нього створюється робота, механік призначається на задачу, робота проходить статуси і закривається з розрахунком вартості.

Мета проекту - показати ООП-модель, бізнес-правила, SOLID, патерни, JSON-збереження і unit-тести без зайвого production-рівня. Це не CRM і не комерційний продукт, а навчальне ядро для практики з ООП.

## Що демонструє проект

- доменну модель СТО: `Customer`, `Vehicle`, `Appointment`, `Job`, `Mechanic`;
- інкапсуляцію і валідацію через value objects: `Money`, `Vin`, `PhoneNumber`;
- життєвий цикл `Job`: `Created -> Assigned -> InProgress -> Completed`;
- перевірку зайнятості часового слота і механіка;
- розрахунок вартості через різні pricing strategies;
- асинхронне збереження і відновлення стану через JSON DTO;
- скасування запису до створення ремонтної роботи;
- LINQ-запити, коротку статистику та звіт дорогих завершених ремонтів;
- автоматизовані тести на xUnit і Moq.

## Як запустити

Передумова: встановлений .NET SDK 8.0 або новіший.

З кореня репозиторію:

```powershell
dotnet build CarWorkshop.sln
dotnet run --project src\CarWorkshop.Console\CarWorkshop.Console.csproj
dotnet test CarWorkshop.sln
```

Команди виконують:

- `dotnet build` - збирає всі проекти solution;
- `dotnet run` - запускає консольну демонстрацію сценарію СТО;
- `dotnet test` - запускає unit-тести.

ConsoleApp має меню для повного сценарію ремонту, скасування запису, перегляду робіт і статистики, а також JSON save/load.

Якщо в `bin\Debug\net8.0\data` вже є JSON-файли, застосунок покаже завантажені дані. Щоб побачити сценарій з нуля, можна очистити цю папку перед запуском.

## Архітектура

- `CarWorkshop.Domain` - сутності, value objects, бізнес-правила, enum, exceptions, patterns.
- `CarWorkshop.Application` - use cases і `WorkshopFacade`.
- `CarWorkshop.Infrastructure` - in-memory repositories, DTO, mapper, JSON persistence.
- `CarWorkshop.Console` - демонстраційний сценарій.
- `CarWorkshop.Tests` - unit-тести для ключових правил.

Domain layer не залежить від Infrastructure або Console. Infrastructure не серіалізує domain-класи напряму, а використовує DTO + Mapper.

## Основний scope

- створення `Appointment`;
- підтвердження запису;
- створення `Job` через factory;
- призначення механіка з перевіркою зайнятості;
- переходи статусів `Created -> Assigned -> InProgress -> Completed`;
- розрахунок вартості через strategy;
- асинхронне збереження і відновлення через JSON;
- скасування запису до створення пов'язаної роботи;
- перегляд завершених робіт та агрегованої статистики.
- фільтрація дорогих завершених ремонтів і звіт про їх виручку.

## Поза scope

- CRM, UI, Telegram, авторизація;
- база даних;
- склад запчастин;
- SaaS/multi-tenant;
- складна аналітика.

Причина: це навчальний ООП-проект, а не production-продукт.

## Патерни

- Repository - інтерфейси в Domain, in-memory реалізації в Infrastructure.
- Factory - `JobFactory.CreateFromAppointment(...)`.
- Strategy - `IPricingStrategy`: fixed, hourly, parts + labor.
- Observer - історія змін статусів `Job`.
- Decorator - `LoggingRepositoryDecorator<T>`.
- Facade - `WorkshopFacade` як єдина точка входу для ConsoleApp.

Свідомо не використано State pattern: для 5 статусів таблиця переходів у `Job.TransitionTo(...)` простіша і читабельніша.

## ООП-можливості

- Наслідування: `Customer` і `Mechanic` наслідуються від базового `Person`.
- Поліморфізм: `Person.GetDisplayName()` є `virtual`, а дочірні класи перевизначають його через `override` і використовують `base`.
- Індексатор: `JobStatusHistoryObserver[index]` дає доступ до запису історії за індексом.
- LINQ extension methods: `CompletedRevenueForPeriod(...)` використовує `Aggregate`, `JoinWithMechanics(...)` використовує `Join`, а `BuildCompletedJobsReport(...)` використовує delegate-фільтр.

## Документація

- [USER_GUIDE.md](USER_GUIDE.md) - як запустити та показати сценарій.
- [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) - шари, розширення й правила коду.
- [TESTING.md](TESTING.md) - тести, coverage і CI.
- [DEMO.md](DEMO.md) - сценарій захисту на 3-5 хвилин.
- [FINAL_REPORT.md](FINAL_REPORT.md) - фінальний технічний звіт.
- [docs/release-plan.md](docs/release-plan.md) - scope релізу `v1.0.0`.
- [docs/defense-qa.md](docs/defense-qa.md) - короткі відповіді на питання.
- [docs/syllabus-coverage.md](docs/syllabus-coverage.md) - покриття тем курсу.
- [docs/self-audit.md](docs/self-audit.md) - аудит перед самостійною роботою No29.
- [docs/extension-plan.md](docs/extension-plan.md) - план залежних розширень IW29.
- [docs/extension-report.md](docs/extension-report.md) - результат розширень IW29.
- [docs/defense-checklist.md](docs/defense-checklist.md) - чекліст підготовки до захисту.
- [docs/CarWorkshop.pdf](docs/CarWorkshop.pdf) - коротка презентація до захисту.

## UML

Актуальні UML-артефакти:

- `docs/class-diagram.puml`;
- `docs/sequence-diagram.puml`.

Додаткова state diagram для статусів роботи лежить у `docs/uml/job-state-diagram.puml`.

## Артефакти ітерацій

- `docs/vision.md` - постановка задачі, користувачі, сценарії та обмеження.
- `docs/backlog.md` - план розвитку на Lab 34-37.
- `docs/iteration-1.md` - передача в Lab 35.
- `docs/iteration-2-plan.md` - план виконання Lab 35.
- `docs/iteration-2.md` - результат Lab 35 і передача в Lab 36.
- `docs/test-strategy.md` - тестова стратегія Lab 36.
- `docs/test-matrix.md` - відповідність сценаріїв і тестів.
- `docs/iteration-3.md` - результат Lab 36 і передача в Lab 37.
- `docs/release-plan.md` - фінальний scope і підготовка `v1.0.0`.
- `docs/performance-note.md` - коротка оцінка структур даних.
- `docs/self-audit.md`, `docs/extension-plan.md`, `docs/extension-report.md` - артефакти IW29.
- `docs/defense-checklist.md` - фінальна підготовка до захисту.
- `TESTING.md` - запуск тестів та coverage.
- `.github/workflows/dotnet.yml` - CI для restore, build і test.

## Тести

Тести запускаються командою:

```powershell
dotnet test CarWorkshop.sln
```

Покрито:

- `Money` і операції з валютою;
- валідні та невалідні переходи статусів `Job`;
- `JobFactory`;
- pricing strategies;
- базове наслідування `Person`;
- індексатор історії статусів;
- LINQ extension methods з `Aggregate` і `Join`;
- use cases з Moq для repository;
- async JSON save/load, пошкоджені дані й відсутні файли;
- скасування записів і робіт;
- LINQ-запити через facade.
- delegate-фільтр і звіт дорогих завершених ремонтів.

Після IW29 проходить `62` тести, з них `9` окремих integration tests. Локальний coverage: `90.79%` lines і `61.11%` branches.
