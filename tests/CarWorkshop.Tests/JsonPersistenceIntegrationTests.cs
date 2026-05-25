using CarWorkshop.Application.Facade;
using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.Pricing;
using CarWorkshop.Domain.ValueObjects;
using CarWorkshop.Infrastructure.Persistence;
using CarWorkshop.Infrastructure.Repositories;

namespace CarWorkshop.Tests;

public class JsonPersistenceIntegrationTests
{
    [Fact]
    public async Task FullRepairCycle_SaveAndReload_PreservesCompletedJob()
    {
        using var workspace = new PersistenceWorkspace();
        var source = workspace.CreateState();
        var (_, vehicle, mechanic) = source.AddReferenceData();
        var job = CompleteRepair(source, vehicle, mechanic);

        await workspace.Persistence.SaveAllAsync(
            source.Customers, source.Mechanics, source.Vehicles, source.Appointments, source.Jobs);

        var restored = workspace.CreateState();
        await workspace.Persistence.LoadAllAsync(
            restored.Customers, restored.Mechanics, restored.Vehicles, restored.Appointments, restored.Jobs);

        var loadedJob = restored.Jobs.GetById(job.Id);
        Assert.NotNull(loadedJob);
        Assert.Equal(JobStatus.Completed, loadedJob!.Status);
        Assert.Equal(new Money(1500, "UAH"), loadedJob.TotalCost());
    }

    [Fact]
    public async Task RestoredState_AllowsCreatingAndCancellingNewAppointment()
    {
        using var workspace = new PersistenceWorkspace();
        var source = workspace.CreateState();
        var (_, vehicle, _) = source.AddReferenceData();
        await workspace.Persistence.SaveAllAsync(
            source.Customers, source.Mechanics, source.Vehicles, source.Appointments, source.Jobs);

        var restored = workspace.CreateState();
        await workspace.Persistence.LoadAllAsync(
            restored.Customers, restored.Mechanics, restored.Vehicles, restored.Appointments, restored.Jobs);
        var loadedVehicle = restored.Vehicles.GetById(vehicle.Id)!;

        var appointment = restored.Facade.CreateAppointment(
            loadedVehicle.Id, DateTime.UtcNow.AddDays(3), "Brake inspection");
        restored.Facade.CancelAppointment(appointment.Id);

        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
    }

    [Fact]
    public async Task RestoredState_ProvidesRevenueQueryForCompletedJobs()
    {
        using var workspace = new PersistenceWorkspace();
        var source = workspace.CreateState();
        var (_, vehicle, mechanic) = source.AddReferenceData();
        CompleteRepair(source, vehicle, mechanic);
        await workspace.Persistence.SaveAllAsync(
            source.Customers, source.Mechanics, source.Vehicles, source.Appointments, source.Jobs);

        var restored = workspace.CreateState();
        await workspace.Persistence.LoadAllAsync(
            restored.Customers, restored.Mechanics, restored.Vehicles, restored.Appointments, restored.Jobs);

        var revenue = restored.Facade.GetRevenueForPeriod(
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(2));

        Assert.Equal(1500m, revenue);
    }

    [Fact]
    public async Task MissingFiles_AllowSavingAndReloadingNewState()
    {
        using var workspace = new PersistenceWorkspace();
        var state = workspace.CreateState();

        await workspace.Persistence.LoadAllAsync(
            state.Customers, state.Mechanics, state.Vehicles, state.Appointments, state.Jobs);
        var (customer, _, _) = state.AddReferenceData();
        await workspace.Persistence.SaveAllAsync(
            state.Customers, state.Mechanics, state.Vehicles, state.Appointments, state.Jobs);

        var restored = workspace.CreateState();
        await workspace.Persistence.LoadAllAsync(
            restored.Customers, restored.Mechanics, restored.Vehicles, restored.Appointments, restored.Jobs);

        Assert.Equal(customer.Id, restored.Customers.GetAll().Single().Id);
    }

    [Fact]
    public async Task CorruptedJson_ThrowsAndKeepsCurrentInMemoryState()
    {
        using var workspace = new PersistenceWorkspace();
        await File.WriteAllTextAsync(Path.Combine(workspace.DirectoryPath, "customers.json"), "{broken");
        var state = workspace.CreateState();
        var (customer, _, _) = state.AddReferenceData();

        await Assert.ThrowsAsync<InvalidDataException>(() => workspace.Persistence.LoadAllAsync(
            state.Customers, state.Mechanics, state.Vehicles, state.Appointments, state.Jobs));

        Assert.Equal(customer.Id, state.Customers.GetAll().Single().Id);
    }

    [Fact]
    public async Task ConflictingJsonReference_ThrowsAndWritesFailureLog()
    {
        using var workspace = new PersistenceWorkspace();
        var vehicleJson = $$"""
            [{ "Id": "{{Guid.NewGuid()}}", "Vin": "1HGCM82633A123456",
               "Brand": "Toyota", "Model": "Camry", "OwnerId": "{{Guid.NewGuid()}}" }]
            """;
        await File.WriteAllTextAsync(Path.Combine(workspace.DirectoryPath, "vehicles.json"), vehicleJson);
        var loggedMessages = new List<string>();
        var persistence = new JsonPersistence(workspace.DirectoryPath, loggedMessages.Add);
        var state = workspace.CreateState();

        await Assert.ThrowsAsync<InvalidDataException>(() => persistence.LoadAllAsync(
            state.Customers, state.Mechanics, state.Vehicles, state.Appointments, state.Jobs));

        Assert.Single(loggedMessages);
        Assert.Contains("invalid or conflicting", loggedMessages[0]);
    }

    [Fact]
    public async Task SaveFailure_ThrowsIOExceptionAndWritesFailureLog()
    {
        using var workspace = new PersistenceWorkspace();
        var loggedMessages = new List<string>();
        var persistence = new JsonPersistence(workspace.DirectoryPath, loggedMessages.Add);
        var state = workspace.CreateState();
        state.AddReferenceData();
        Directory.Delete(workspace.DirectoryPath, recursive: true);
        await File.WriteAllTextAsync(workspace.DirectoryPath, "path occupied by file");

        await Assert.ThrowsAnyAsync<IOException>(() => persistence.SaveAllAsync(
            state.Customers, state.Mechanics, state.Vehicles, state.Appointments, state.Jobs));

        Assert.Single(loggedMessages);
        Assert.Contains("Could not save JSON data", loggedMessages[0]);
    }

    [Fact]
    public async Task ConsecutiveSaves_ReloadOnlyLatestAppointmentState()
    {
        using var workspace = new PersistenceWorkspace();
        var source = workspace.CreateState();
        var (_, vehicle, _) = source.AddReferenceData();
        var appointment = source.Facade.CreateAppointment(
            vehicle.Id, DateTime.UtcNow.AddDays(2), "Diagnostics");
        await workspace.Persistence.SaveAllAsync(
            source.Customers, source.Mechanics, source.Vehicles, source.Appointments, source.Jobs);
        source.Facade.CancelAppointment(appointment.Id);
        await workspace.Persistence.SaveAllAsync(
            source.Customers, source.Mechanics, source.Vehicles, source.Appointments, source.Jobs);

        var restored = workspace.CreateState();
        await workspace.Persistence.LoadAllAsync(
            restored.Customers, restored.Mechanics, restored.Vehicles, restored.Appointments, restored.Jobs);

        var loadedAppointment = restored.Appointments.GetAll().Single();
        Assert.Equal(AppointmentStatus.Cancelled, loadedAppointment.Status);
    }

    [Fact]
    public async Task RestoredCancelledAppointment_ReleasesSlotForNewAppointment()
    {
        using var workspace = new PersistenceWorkspace();
        var source = workspace.CreateState();
        var (_, vehicle, _) = source.AddReferenceData();
        var scheduledAt = DateTime.UtcNow.AddDays(4);
        var original = source.Facade.CreateAppointment(vehicle.Id, scheduledAt, "First visit");
        source.Facade.CancelAppointment(original.Id);
        await workspace.Persistence.SaveAllAsync(
            source.Customers, source.Mechanics, source.Vehicles, source.Appointments, source.Jobs);

        var restored = workspace.CreateState();
        await workspace.Persistence.LoadAllAsync(
            restored.Customers, restored.Mechanics, restored.Vehicles, restored.Appointments, restored.Jobs);

        var repeated = restored.Facade.CreateAppointment(vehicle.Id, scheduledAt, "Repeated visit");
        Assert.Equal(AppointmentStatus.Created, repeated.Status);
    }

    private static Job CompleteRepair(WorkshopState state, Vehicle vehicle, Mechanic mechanic)
    {
        var appointment = state.Facade.CreateAppointment(
            vehicle.Id, DateTime.UtcNow.AddDays(1), "Engine repair");
        state.Facade.ConfirmAppointment(appointment.Id);
        var job = state.Facade.CreateJob(appointment.Id);
        state.Facade.AssignMechanic(job.Id, mechanic.Id);
        state.Facade.StartJob(job.Id);
        state.Facade.CompleteJob(
            job.Id,
            new PartsAndLaborPricingStrategy(new Money(1000, "UAH"), new Money(500, "UAH")));
        return job;
    }

    private sealed class PersistenceWorkspace : IDisposable
    {
        public string DirectoryPath { get; } =
            Path.Combine(Path.GetTempPath(), "CarWorkshopIntegrationTests", Guid.NewGuid().ToString());

        public JsonPersistence Persistence { get; }

        public PersistenceWorkspace()
        {
            Persistence = new JsonPersistence(DirectoryPath);
        }

        public WorkshopState CreateState() => new();

        public void Dispose()
        {
            if (Directory.Exists(DirectoryPath))
                Directory.Delete(DirectoryPath, recursive: true);
            else if (File.Exists(DirectoryPath))
                File.Delete(DirectoryPath);
        }
    }

    private sealed class WorkshopState
    {
        public InMemoryCustomerRepository Customers { get; } = new();
        public InMemoryMechanicRepository Mechanics { get; } = new();
        public InMemoryVehicleRepository Vehicles { get; } = new();
        public InMemoryAppointmentRepository Appointments { get; } = new();
        public InMemoryJobRepository Jobs { get; } = new();
        public WorkshopFacade Facade { get; }

        public WorkshopState()
        {
            Facade = new WorkshopFacade(Customers, Vehicles, Appointments, Jobs, Mechanics);
        }

        public (Customer Customer, Vehicle Vehicle, Mechanic Mechanic) AddReferenceData()
        {
            var customer = TestData.Customer();
            var vehicle = TestData.Vehicle(customer);
            var mechanic = TestData.Mechanic();
            Customers.Add(customer);
            Vehicles.Add(vehicle);
            Mechanics.Add(mechanic);
            return (customer, vehicle, mechanic);
        }
    }
}
