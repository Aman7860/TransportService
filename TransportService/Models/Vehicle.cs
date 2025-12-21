namespace TransportService.Models
{
    public class Vehicle : BaseEntity
    {
        public int Id { get; private set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Price { get; set; }
    }
}
