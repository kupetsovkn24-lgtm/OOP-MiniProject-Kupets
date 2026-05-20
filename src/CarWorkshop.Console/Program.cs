using CarWorkshop.Application.Facade;
using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.Observers;
using CarWorkshop.Domain.Pricing;
using CarWorkshop.Domain.ValueObjects;
using CarWorkshop.Infrastructure.Persistence;
using CarWorkshop.Infrastructure.Repositories;

// ── Bootstrap — repositories (Decorator wraps jobs repo with logging) ──────────

var customers    = new InMemoryCustomerRepository();
var mechanics    = new InMemoryMechanicRepository();
var vehicles     = new InMemoryVehicleRepository();
var appointments = new InMemoryAppointmentRepository();
var jobsInner    = new InMemoryJobRepository();

// Decorator: логування всіх звернень до job repository
var jobsLogged = new LoggingRepositoryDecorator<Job>(
    jobsInner,
    msg => Console.WriteLine($"  LOG {msg}"));

var dataDir     = Path.Combine(AppContext.BaseDirectory, "data");
var persistence = new JsonPersistence(dataDir);
persistence.LoadAll(customers, mechanics, vehicles, appointments, jobsInner);

// ── Facade (єдина точка входу) ────────────────────────────────────────────────

var workshop = new WorkshopFacade(customers, vehicles, appointments, jobsInner, mechanics);

// Observer: підписуємось на зміни статусів ще до першого Job
var historyObserver = new JobStatusHistoryObserver();
workshop.AddJobObserver(historyObserver);

// ── Console menu ──────────────────────────────────────────────────────────────

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== Система управління ремонтними задачами СТО ===\n");

while (true)
{
    Console.WriteLine("1. Запустити повний сценарій ремонту");
    Console.WriteLine("2. Показати завантажені роботи");
    Console.WriteLine("0. Вийти");
    Console.Write("Оберіть дію: ");

    var choice = Console.ReadLine();
    Console.WriteLine();

    if (choice is null)
        return;

    switch (choice.Trim())
    {
        case "1":
            RunDemoScenario();
            break;
        case "2":
            PrintLoadedJobs();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Невідома команда. Спробуйте ще раз.");
            break;
    }

    Console.WriteLine();
}

void RunDemoScenario()
{
    if (customers.GetAll().Any())
    {
        Console.WriteLine("Дані вже є в JSON, тому повторний seed не створюється.");
        PrintLoadedJobs();
        return;
    }

    var customer = new Customer(Guid.NewGuid(), "Іван Петренко", new PhoneNumber("+380671234567"));
    customers.Add(customer);

    var vehicle = new Vehicle(
        Guid.NewGuid(), new Vin("1HGCM82633A123456"),
        "Toyota", "Camry", customer);
    vehicles.Add(vehicle);

    var sasha = new Mechanic(Guid.NewGuid(), "Саша Коваль", MechanicSpecialization.Engine);
    mechanics.Add(sasha);

    Console.WriteLine($"Клієнт:  {customer.FullName} ({customer.Phone})");
    Console.WriteLine($"Авто:    {vehicle.Brand} {vehicle.Model} [{vehicle.Vin}]");
    Console.WriteLine($"Механік: {sasha.FullName} ({sasha.Specialization})\n");

    var scheduledAt = DateTime.UtcNow.AddDays(1);
    var appointment = workshop.CreateAppointment(vehicle.Id, scheduledAt, "Стукіт у двигуні");
    Console.WriteLine($"[1] Appointment створено     | {appointment.Status}");

    workshop.ConfirmAppointment(appointment.Id);
    Console.WriteLine($"[2] Appointment підтверджено | {appointment.Status}");

    var job = workshop.CreateJob(appointment.Id);
    Console.WriteLine($"[3] Job створено             | {job.Status}");

    workshop.AssignMechanic(job.Id, sasha.Id);
    Console.WriteLine($"[4] Механіка призначено      | {job.Mechanic!.FullName} | {job.Status}");

    workshop.StartJob(job.Id);
    Console.WriteLine($"[5] Роботу розпочато         | {job.Status}");

    IPricingStrategy strategy = new PartsAndLaborPricingStrategy(
        laborCost: new Money(1200, "UAH"),
        partsCost: new Money(3500, "UAH"));

    workshop.CompleteJob(job.Id, strategy);
    Console.WriteLine($"[6] Роботу завершено         | {job.Status}");
    Console.WriteLine($"    Стратегія: {strategy.GetType().Name}");
    Console.WriteLine($"    Робота:    {job.LaborCost}");
    Console.WriteLine($"    Запчастини:{job.PartsCost}");
    Console.WriteLine($"    Разом:     {job.TotalCost()}");

    Console.WriteLine("\n── Історія статусів Job ─────────────────────────────");
    foreach (var entry in historyObserver.History)
        Console.WriteLine($"  {entry.ChangedAt:HH:mm:ss.fff}  {entry.OldStatus,-10} → {entry.NewStatus}");

    Console.WriteLine("\n── LINQ: записи на сьогодні ─────────────────────────");
    foreach (var a in workshop.GetAppointmentsForDay(scheduledAt))
        Console.WriteLine($"  {a.ScheduledAt:dd.MM HH:mm}  {a.ProblemDescription}  [{a.Status}]");

    Console.WriteLine("\n── LINQ: виручка за останні 7 днів ─────────────────");
    var revenue = workshop.GetRevenueForPeriod(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
    Console.WriteLine($"  {revenue} UAH");

    Console.WriteLine("\n── LINQ: роботи по механіках ────────────────────────");
    foreach (var (mechanic, count) in workshop.GetJobCountByMechanic())
        Console.WriteLine($"  {mechanic}: {count} роб.");

    Console.WriteLine("\n── Decorator: звернення через LoggingRepository ─────");
    _ = jobsLogged.GetAll().ToList();
    _ = jobsLogged.GetById(job.Id);

    persistence.SaveAll(customers, mechanics, vehicles, appointments, jobsInner);
    Console.WriteLine($"\nДані збережено у: {dataDir}");
}

void PrintLoadedJobs()
{
    var jobs = jobsInner.GetAll().ToList();
    if (!jobs.Any())
    {
        Console.WriteLine("Збережених робіт ще немає.");
        return;
    }

    Console.WriteLine("Завантажені дані з JSON:\n");
    foreach (var job in jobs)
    {
        Console.WriteLine($"Job {job.Id} | {job.Status} | Механік: {job.Mechanic?.FullName ?? "не призначений"}");
        if (job.Status == JobStatus.Completed)
            Console.WriteLine($"  Вартість: {job.TotalCost()}");
    }
}
