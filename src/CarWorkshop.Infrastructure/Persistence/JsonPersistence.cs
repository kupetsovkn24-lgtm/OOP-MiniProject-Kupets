using System.Text.Json;
using CarWorkshop.Domain.Entities;
using CarWorkshop.Infrastructure.Dto;
using CarWorkshop.Infrastructure.Mapping;
using CarWorkshop.Infrastructure.Repositories;

namespace CarWorkshop.Infrastructure.Persistence;

public class JsonPersistence
{
    private readonly string _dataDirectory;

    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public JsonPersistence(string dataDirectory)
    {
        _dataDirectory = dataDirectory;
        Directory.CreateDirectory(dataDirectory);
    }

    public async Task SaveAllAsync(
        InMemoryCustomerRepository customers,
        InMemoryMechanicRepository mechanics,
        InMemoryVehicleRepository vehicles,
        InMemoryAppointmentRepository appointments,
        InMemoryJobRepository jobs,
        CancellationToken cancellationToken = default)
    {
        await WriteJsonAsync("customers.json", customers.GetAll().Select(DomainMapper.ToDto), cancellationToken);
        await WriteJsonAsync("mechanics.json", mechanics.GetAll().Select(DomainMapper.ToDto), cancellationToken);
        await WriteJsonAsync("vehicles.json", vehicles.GetAll().Select(DomainMapper.ToDto), cancellationToken);
        await WriteJsonAsync("appointments.json", appointments.GetAll().Select(DomainMapper.ToDto), cancellationToken);
        await WriteJsonAsync("jobs.json", jobs.GetAll().Select(DomainMapper.ToDto), cancellationToken);
    }

    public async Task LoadAllAsync(
        InMemoryCustomerRepository customers,
        InMemoryMechanicRepository mechanics,
        InMemoryVehicleRepository vehicles,
        InMemoryAppointmentRepository appointments,
        InMemoryJobRepository jobs,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Load order matters because DTO files contain references by id.
            var loadedCustomers = await LoadDomainAsync("customers.json",
                (IEnumerable<CustomerDto> dtos) => dtos.Select(DomainMapper.ToDomain), cancellationToken);

            var loadedMechanics = await LoadDomainAsync("mechanics.json",
                (IEnumerable<MechanicDto> dtos) => dtos.Select(DomainMapper.ToDomain), cancellationToken);

            var customerMap = loadedCustomers.ToDictionary(c => c.Id);
            var mechanicMap = loadedMechanics.ToDictionary(m => m.Id);

            var loadedVehicles = await LoadDomainAsync("vehicles.json",
                (IEnumerable<VehicleDto> dtos) => dtos.Select(dto => DomainMapper.ToDomain(dto, customerMap)),
                cancellationToken);

            var vehicleMap = loadedVehicles.ToDictionary(v => v.Id);

            var loadedAppointments = await LoadDomainAsync("appointments.json",
                (IEnumerable<AppointmentDto> dtos) => dtos.Select(dto => DomainMapper.ToDomain(dto, vehicleMap)),
                cancellationToken);

            var appointmentMap = loadedAppointments.ToDictionary(a => a.Id);

            var loadedJobs = await LoadDomainAsync("jobs.json",
                (IEnumerable<JobDto> dtos) => dtos.Select(dto => DomainMapper.ToDomain(dto, appointmentMap, mechanicMap)),
                cancellationToken);

            customers.ReplaceAll(loadedCustomers);
            mechanics.ReplaceAll(loadedMechanics);
            vehicles.ReplaceAll(loadedVehicles);
            appointments.ReplaceAll(loadedAppointments);
            jobs.ReplaceAll(loadedJobs);
        }
        catch (InvalidDataException)
        {
            throw;
        }
        catch (Exception exception) when (exception is ArgumentException
            or KeyNotFoundException
            or InvalidOperationException)
        {
            throw new InvalidDataException("JSON data contains invalid or conflicting domain records.", exception);
        }
    }

    private async Task WriteJsonAsync<T>(
        string fileName,
        IEnumerable<T> items,
        CancellationToken cancellationToken)
    {
        var path = Path.Combine(_dataDirectory, fileName);
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, items.ToList(), _options, cancellationToken);
    }

    private async Task<List<TDomain>> LoadDomainAsync<TDto, TDomain>(
        string fileName,
        Func<IEnumerable<TDto>, IEnumerable<TDomain>> map,
        CancellationToken cancellationToken)
    {
        var path = Path.Combine(_dataDirectory, fileName);
        if (!File.Exists(path))
            return [];

        try
        {
            await using var stream = File.OpenRead(path);
            var dtos = await JsonSerializer.DeserializeAsync<List<TDto>>(stream, _options, cancellationToken) ?? [];
            return map(dtos).ToList();
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException($"File '{fileName}' contains invalid JSON.", exception);
        }
    }
}
