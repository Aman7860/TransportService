namespace TransportService.Models
{
    public class Vehicle
    {
        public int Id { get; private set; }   // Encapsulation
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Price { get; set; }

        public DateTime? CreatedDate { get; private set; }
        public DateTime? UpdatedDate { get; private set; }

        public void MarkUpdated()
        {
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }

    }
}
