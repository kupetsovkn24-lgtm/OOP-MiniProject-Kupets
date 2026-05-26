using System.Text.Json;
using CarWorkshop.Domain.Entities;
using CarWorkshop.Infrastructure.Dto;
using CarWorkshop.Infrastructure.Mapping;
using CarWorkshop.Infrastructure.Repositories;

namespace CarWorkshop.Infrastructure.Persistence;

/// <summary>
/// Saves and restores workshop state as JSON DTO files.
/// Domain invariants are recreated through <see cref="DomainMapper"/>.
/// </summary>
public class JsonPersistence
{
    private readonly string _dataDirectory;
    private readonly Action<string>? _errorLog;

    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public JsonPersistence(string dataDirectory, Action<string>? errorLog = null)
    {
        _dataDirectory = dataDirectory;
        _errorLog = errorLog;
        Directory.CreateDirectory(dataDirectory);
    }

    /// <summary>
    /// Persists the current state to JSON files asynchronously.
    /// </summary>
    public async Task SaveAllAsync(
        InMemoryCustomerRepository customers,
        InMemoryMechanicRepository mechanics,
        InMemoryVehicleRepository vehicles,
        InMemoryAppointmentRepository appointments,
        InMemoryJobRepository jobs,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await WriteJsonAsync("customers.json", customers.GetAll().Select(DomainMapper.ToDto), cancellationToken);
            await WriteJsonAsync("mechanics.json", mechanics.GetAll().Select(DomainMapper.ToDto), cancellationToken);
            await WriteJsonAsync("vehicles.json", vehicles.GetAll().Select(DomainMapper.ToDto), cancellationToken);
            await WriteJsonAsync("appointments.json", appointments.GetAll().Select(DomainMapper.ToDto), cancellationToken);
            await WriteJsonAsync("jobs.json", jobs.GetAll().Select(DomainMapper.ToDto), cancellationToken);
        }
        catch (IOException exception)
        {
            _errorLog?.Invoke($"Could not save JSON data: {exception.Message}");
            throw;
        }
    }

    /// <summary>
    /// Restores state from JSON files; missing files represent an empty initial state.
    /// Invalid data causes <see cref="InvalidDataException"/> without replacing repository contents.
    /// </summary>
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
        catch (InvalidDataException exception)
        {
            _errorLog?.Invoke(exception.Message);
            throw;
        }
        catch (Exception exception) when (exception is ArgumentException
            or KeyNotFoundException
            or InvalidOperationException)
        {
            var dataException = new InvalidDataException(
                "JSON data contains invalid or conflicting domain records.", exception);
            _errorLog?.Invoke(dataException.Message);
            throw dataException;
        }
        catch (IOException exception)
        {
            _errorLog?.Invoke($"Could not load JSON data: {exception.Message}");
            throw;
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
