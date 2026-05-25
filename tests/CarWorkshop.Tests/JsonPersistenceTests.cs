using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.ValueObjects;
using CarWorkshop.Infrastructure.Persistence;
using CarWorkshop.Infrastructure.Repositories;

namespace CarWorkshop.Tests;

public class JsonPersistenceTests
{
    [Fact]
    public async Task SaveAllAsyncAndLoadAllAsync_RestoresDomainData()
    {
        var dataDirectory = Path.Combine(Path.GetTempPath(), "CarWorkshopTests", Guid.NewGuid().ToString());

        try
        {
            var sourceCustomers = new InMemoryCustomerRepository();
            var sourceMechanics = new InMemoryMechanicRepository();
            var sourceVehicles = new InMemoryVehicleRepository();
            var sourceAppointments = new InMemoryAppointmentRepository();
            var sourceJobs = new InMemoryJobRepository();

            var customer = TestData.Customer();
            var vehicle = TestData.Vehicle(customer);
            var mechanic = TestData.Mechanic();
            var appointment = TestData.ConfirmedAppointment(vehicle);
            var job = TestData.Job(appointment);
            job.AssignMechanic(mechanic);
            job.Start();
            job.Complete(new Money(1200, "UAH"), new Money(3500, "UAH"));

            sourceCustomers.Add(customer);
            sourceVehicles.Add(vehicle);
            sourceMechanics.Add(mechanic);
            sourceAppointments.Add(appointment);
            sourceJobs.Add(job);

            var persistence = new JsonPersistence(dataDirectory);
            await persistence.SaveAllAsync(sourceCustomers, sourceMechanics, sourceVehicles, sourceAppointments, sourceJobs);

            var loadedCustomers = new InMemoryCustomerRepository();
            var loadedMechanics = new InMemoryMechanicRepository();
            var loadedVehicles = new InMemoryVehicleRepository();
            var loadedAppointments = new InMemoryAppointmentRepository();
            var loadedJobs = new InMemoryJobRepository();

            await persistence.LoadAllAsync(loadedCustomers, loadedMechanics, loadedVehicles, loadedAppointments, loadedJobs);

            var loadedJob = loadedJobs.GetAll().Single();
            Assert.Single(loadedCustomers.GetAll());
            Assert.Single(loadedVehicles.GetAll());
            Assert.Single(loadedMechanics.GetAll());
            Assert.Single(loadedAppointments.GetAll());
            Assert.Equal(JobStatus.Completed, loadedJob.Status);
            Assert.Equal(new Money(4700, "UAH"), loadedJob.TotalCost());
        }
        finally
        {
            if (Directory.Exists(dataDirectory))
                Directory.Delete(dataDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task LoadAllAsync_WhenFilesDoNotExist_ReturnsEmptyState()
    {
        var dataDirectory = Path.Combine(Path.GetTempPath(), "CarWorkshopTests", Guid.NewGuid().ToString());

        try
        {
            var persistence = new JsonPersistence(dataDirectory);
            var customers = new InMemoryCustomerRepository();
            var jobs = new InMemoryJobRepository();

            await persistence.LoadAllAsync(
                customers,
                new InMemoryMechanicRepository(),
                new InMemoryVehicleRepository(),
                new InMemoryAppointmentRepository(),
                jobs);

            Assert.Empty(customers.GetAll());
            Assert.Empty(jobs.GetAll());
        }
        finally
        {
            if (Directory.Exists(dataDirectory))
                Directory.Delete(dataDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task LoadAllAsync_WhenJsonIsCorrupted_ThrowsInvalidDataException()
    {
        var dataDirectory = Path.Combine(Path.GetTempPath(), "CarWorkshopTests", Guid.NewGuid().ToString());

        try
        {
            Directory.CreateDirectory(dataDirectory);
            await File.WriteAllTextAsync(Path.Combine(dataDirectory, "customers.json"), "{ not-json");

            var persistence = new JsonPersistence(dataDirectory);

            await Assert.ThrowsAsync<InvalidDataException>(() => persistence.LoadAllAsync(
                new InMemoryCustomerRepository(),
                new InMemoryMechanicRepository(),
                new InMemoryVehicleRepository(),
                new InMemoryAppointmentRepository(),
                new InMemoryJobRepository()));
        }
        finally
        {
            if (Directory.Exists(dataDirectory))
                Directory.Delete(dataDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task LoadAllAsync_WhenReferenceIsMissing_ThrowsInvalidDataException()
    {
        var dataDirectory = Path.Combine(Path.GetTempPath(), "CarWorkshopTests", Guid.NewGuid().ToString());

        try
        {
            Directory.CreateDirectory(dataDirectory);
            var vehicleJson = $$"""
                [
                  {
                    "Id": "{{Guid.NewGuid()}}",
                    "Vin": "1HGCM82633A123456",
                    "Brand": "Toyota",
                    "Model": "Camry",
                    "OwnerId": "{{Guid.NewGuid()}}"
                  }
                ]
                """;
            await File.WriteAllTextAsync(Path.Combine(dataDirectory, "vehicles.json"), vehicleJson);

            var persistence = new JsonPersistence(dataDirectory);

            await Assert.ThrowsAsync<InvalidDataException>(() => persistence.LoadAllAsync(
                new InMemoryCustomerRepository(),
                new InMemoryMechanicRepository(),
                new InMemoryVehicleRepository(),
                new InMemoryAppointmentRepository(),
                new InMemoryJobRepository()));
        }
        finally
        {
            if (Directory.Exists(dataDirectory))
                Directory.Delete(dataDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task LoadAllAsync_ReplacesPreviouslyLoadedState()
    {
        var dataDirectory = Path.Combine(Path.GetTempPath(), "CarWorkshopTests", Guid.NewGuid().ToString());

        try
        {
            var sourceCustomers = new InMemoryCustomerRepository();
            sourceCustomers.Add(TestData.Customer());
            var persistence = new JsonPersistence(dataDirectory);
            await persistence.SaveAllAsync(
                sourceCustomers,
                new InMemoryMechanicRepository(),
                new InMemoryVehicleRepository(),
                new InMemoryAppointmentRepository(),
                new InMemoryJobRepository());

            var loadedCustomers = new InMemoryCustomerRepository();
            loadedCustomers.Add(TestData.Customer());

            await persistence.LoadAllAsync(
                loadedCustomers,
                new InMemoryMechanicRepository(),
                new InMemoryVehicleRepository(),
                new InMemoryAppointmentRepository(),
                new InMemoryJobRepository());

            Assert.Single(loadedCustomers.GetAll());
            Assert.Equal(sourceCustomers.GetAll().Single().Id, loadedCustomers.GetAll().Single().Id);
        }
        finally
        {
            if (Directory.Exists(dataDirectory))
                Directory.Delete(dataDirectory, recursive: true);
        }
    }
}
