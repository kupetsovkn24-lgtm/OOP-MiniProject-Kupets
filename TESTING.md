# Testing

## Запуск

З кореня репозиторію:

```powershell
dotnet restore CarWorkshop.sln
dotnet build CarWorkshop.sln --no-restore
dotnet test CarWorkshop.sln --no-build
```

Запуск із coverage:

```powershell
dotnet test CarWorkshop.sln --no-build --collect:"XPlat Code Coverage" --results-directory TestResults -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
```

XML-звіт буде створений у `TestResults/<run-id>/coverage.cobertura.xml`.

## Що покрито

- Unit tests: доменні інваріанти, статуси, pricing strategy, factory, observer, use cases та запити.
- IW29 unit tests: delegate-фільтр завершених робіт, аналітичний звіт, порожній результат і некоректний поріг.
- Integration tests: реальне JSON save/load, робота після відновлення стану, послідовні збереження і негативні файлові сценарії.
- Fault handling: відсутній файл, пошкоджений JSON, конфліктні DTO-посилання та журналювання відмови.

## Поточний результат

- Усього тестів: `62`.
- Окремі integration tests для Lab 36: `9`.
- Фінальний локальний coverage після IW29: `90.79%` lines (`592/652`), `61.11%` branches (`110/180`).

## CI quality gate

GitHub Actions виконує restore, build і тести з coverage. Якщо build або тести падають, pipeline не проходить. XML coverage додається до workflow run як artifact `coverage-report`.

Матриця перевірок наведена у [docs/test-matrix.md](docs/test-matrix.md), а тестова стратегія - у [docs/test-strategy.md](docs/test-strategy.md).
