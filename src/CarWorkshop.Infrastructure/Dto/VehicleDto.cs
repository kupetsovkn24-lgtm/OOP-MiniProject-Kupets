namespace CarWorkshop.Infrastructure.Dto;

public record VehicleDto(Guid Id, string Vin, string Brand, string Model, Guid OwnerId);
