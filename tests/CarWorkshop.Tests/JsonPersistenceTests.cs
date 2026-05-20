using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.ValueObjects;
using CarWorkshop.Infrastructure.Persistence;
using CarWorkshop.Infrastructure.Repositories;

namespace CarWorkshop.Tests;

public class JsonPersistenceTests
{
    [Fact]
    public void SaveAllAndLoadAll_RestoresDomainData()
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
            persistence.SaveAll(sourceCustomers, sourceMechanics, sourceVehicles, sourceAppointments, sourceJobs);

            var loadedCustomers = new InMemoryCustomerRepository();
            var loadedMechanics = new InMemoryMechanicRepository();
            var loadedVehicles = new InMemoryVehicleRepository();
            var loadedAppointments = new InMemoryAppointmentRepository();
            var loadedJobs = new InMemoryJobRepository();

            persistence.LoadAll(loadedCustomers, loadedMechanics, loadedVehicles, loadedAppointments, loadedJobs);

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
}
