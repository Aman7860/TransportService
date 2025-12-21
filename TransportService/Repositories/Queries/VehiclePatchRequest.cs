namespace TransportService.Repositories.Queries
{
    public class VehiclePatchRequest
    {
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public int? Year { get; set; }
        public decimal? Price { get; set; }
    }
}
