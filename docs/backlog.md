# Backlog

## Ітерація 1 - Lab 34: baseline

Мета: створити основу проєкту, яка буде розвиватися в наступних лабораторних.

Пріоритети:

- Зафіксувати vision, scope, backlog і UML-артефакти.
- Створити solution з шарами `Domain`, `Application`, `Infrastructure`, `Console`, `Tests`.
- Реалізувати предметну модель СТО: `Customer`, `Vehicle`, `Appointment`, `Job`, `Mechanic`.
- Додати value objects: `Money`, `Vin`, `PhoneNumber`.
- Реалізувати repository interfaces та in-memory repositories.
- Реалізувати перший вертикальний зріз через консоль.
- Додати базові unit-тести.
- Налаштувати README, `.gitignore` і GitHub Actions CI.

## Ітерація 2 - Lab 35: бізнес-логіка, persistence, запити

Статус: завершено і змерджено у `main` через Pull Request #1.

Реалізовано:

- Асинхронне JSON-збереження та завантаження із контрольованою обробкою пошкоджених або суперечливих даних.
- Скасування запису до створення роботи та скасування незавершеної роботи.
- Запити по записах, роботах механіка, завершених роботах, виручці та групуванню по механіках.
- Розширене консольне меню для сценаріїв, save/load і статистики.
- `docs/iteration-2-plan.md` та `docs/iteration-2.md` як передача до Lab 36.

## Ітерація 3 - Lab 36: quality gate, тести, fault handling

Статус: завершено і змерджено у `main` через Pull Request #2.

Реалізовано:

- Додано `docs/test-strategy.md`, `docs/test-matrix.md`, `docs/iteration-3.md` і `TESTING.md`.
- Додано 9 окремих інтеграційних тестів для JSON persistence і повного циклу сценаріїв.
- Перевірено fault handling і додано журналювання помилок persistence через callback.
- Підключено coverage report до GitHub Actions.
- Зафіксовано результати coverage і ризики для Lab 37.

## Ітерація 4 - Lab 37: release, документація, демо

Статус: підготовлено локально у гілці `lab37-release-hardening`; commit/push/tag тільки після підтвердження.

Реалізовано:

- Додано release plan, user/developer guide, final report, demo script і питання до захисту.
- Зафіксовано syllabus coverage і microanalysis структур даних.
- Проведено невеликий release-refactoring та уточнено документацію API.
- Оновлено README, UML, changelog і testing documentation.
- Підготовлено основу для tag `v1.0.0`.

Підготовлено для захисту:

- Додано коротку презентацію `docs/CarWorkshop.pdf`.

## Самостійна робота No29: аналітичне розширення

Статус: виконується локально у гілці `iw29-filtered-repair-report`; commit/push тільки після підтвердження.

Реалізовано:

- Гнучкий delegate-фільтр завершених робіт.
- Звіт дорогих ремонтів із кількістю та сумою виручки.
- Показ звіту через наявний console-сценарій статистики.
- Аудит, план, extension report і defense checklist.
- Unit tests нового фільтра та звіту.

## Можливі розширення

- Нові `IPricingStrategy` для інших типів ремонту.
- Повідомлення клієнта про зміну статусу через новий observer.
- Інша реалізація persistence без зміни `Domain`.
- Додаткові LINQ-запити для менеджера.
- Мінімальне інтерактивне меню в Console.
