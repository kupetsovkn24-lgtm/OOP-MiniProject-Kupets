# Test Matrix - Lab 36

| Сценарій або ризик | Рівень | Основні тести |
| --- | --- | --- |
| Некоректні сума або валюта | Unit | `MoneyTests`, `JobStatusTests` |
| Створення запису у минулому або зайнятий слот | Unit | `AppointmentTests`, `UseCaseTests` |
| Робота тільки з підтвердженого запису | Unit | `JobFactoryTests` |
| Зайнятий механік | Unit | `UseCaseTests` |
| Заборонені переходи та завершена робота | Unit | `JobStatusTests`, `UseCaseTests` |
| Strategy/Factory/Observer | Unit | `PricingStrategyTests`, `JobFactoryTests`, `JobStatusHistoryObserverTests` |
| Запити та статистика | Unit | `JobQueryExtensionsTests`, `WorkshopFacadeTests` |
| Повний ремонт із save/load | Integration | `FullRepairCycle_SaveAndReload_PreservesCompletedJob` |
| Операції після відновлення | Integration | `RestoredState_AllowsCreatingAndCancellingNewAppointment`, `RestoredState_ProvidesRevenueQueryForCompletedJobs` |
| Перший запуск без файлів | Integration | `MissingFiles_AllowSavingAndReloadingNewState` |
| Пошкоджений JSON | Integration | `CorruptedJson_ThrowsAndKeepsCurrentInMemoryState` |
| Некоректні зв'язки JSON та логування | Integration | `ConflictingJsonReference_ThrowsAndWritesFailureLog` |
| Помилка файлового запису та логування | Integration | `SaveFailure_ThrowsIOExceptionAndWritesFailureLog` |
| Декілька збережень | Integration | `ConsecutiveSaves_ReloadOnlyLatestAppointmentState` |
| Повторне використання слота після скасування | Integration | `RestoredCancelledAppointment_ReleasesSlotForNewAppointment` |
