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

    // ── Save ──────────────────────────────────────────────────────────────────

    public void SaveAll(
        InMemoryCustomerRepository customers,
        InMemoryMechanicRepository mechanics,
        InMemoryVehicleRepository vehicles,
        InMemoryAppointmentRepository appointments,
        InMemoryJobRepository jobs)
    {
        WriteJson("customers.json", customers.GetAll().Select(DomainMapper.ToDto));
        WriteJson("mechanics.json", mechanics.GetAll().Select(DomainMapper.ToDto));
        WriteJson("vehicles.json", vehicles.GetAll().Select(DomainMapper.ToDto));
        WriteJson("appointments.json", appointments.GetAll().Select(DomainMapper.ToDto));
        WriteJson("jobs.json", jobs.GetAll().Select(DomainMapper.ToDto));
    }

    // ── Load ──────────────────────────────────────────────────────────────────

    public void LoadAll(
        InMemoryCustomerRepository customers,
        InMemoryMechanicRepository mechanics,
        InMemoryVehicleRepository vehicles,
        InMemoryAppointmentRepository appointments,
        InMemoryJobRepository jobs)
    {
        // Load order matters: primitives first, then entities that reference them
        var loadedCustomers = LoadDomain("customers.json",
            (IEnumerable<CustomerDto> dtos) => dtos.Select(DomainMapper.ToDomain));

        var loadedMechanics = LoadDomain("mechanics.json",
            (IEnumerable<MechanicDto> dtos) => dtos.Select(DomainMapper.ToDomain));

        var customerMap = loadedCustomers.ToDictionary(c => c.Id);
        var mechanicMap = loadedMechanics.ToDictionary(m => m.Id);

        var loadedVehicles = LoadDomain("vehicles.json",
            (IEnumerable<VehicleDto> dtos) => dtos.Select(dto => DomainMapper.ToDomain(dto, customerMap)));

        var vehicleMap = loadedVehicles.ToDictionary(v => v.Id);

        var loadedAppointments = LoadDomain("appointments.json",
            (IEnumerable<AppointmentDto> dtos) => dtos.Select(dto => DomainMapper.ToDomain(dto, vehicleMap)));

        var appointmentMap = loadedAppointments.ToDictionary(a => a.Id);

        var loadedJobs = LoadDomain("jobs.json",
            (IEnumerable<JobDto> dtos) => dtos.Select(dto => DomainMapper.ToDomain(dto, appointmentMap, mechanicMap)));

        foreach (var c in loadedCustomers) customers.Add(c);
        foreach (var m in loadedMechanics) mechanics.Add(m);
        foreach (var v in loadedVehicles) vehicles.Add(v);
        foreach (var a in loadedAppointments) appointments.Add(a);
        foreach (var j in loadedJobs) jobs.Add(j);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void WriteJson<T>(string fileName, IEnumerable<T> items)
    {
        var path = Path.Combine(_dataDirectory, fileName);
        File.WriteAllText(path, JsonSerializer.Serialize(items.ToList(), _options));
    }

    private List<TDomain> LoadDomain<TDto, TDomain>(
        string fileName, Func<IEnumerable<TDto>, IEnumerable<TDomain>> map)
    {
        var path = Path.Combine(_dataDirectory, fileName);
        if (!File.Exists(path)) return [];

        var json = File.ReadAllText(path);
        var dtos = JsonSerializer.Deserialize<List<TDto>>(json, _options) ?? [];
        return map(dtos).ToList();
    }
}
