namespace TransportService.Models
{
    public abstract class BaseEntity
    {
        public DateTime CreatedDate { get; private set; }
        public DateTime? UpdatedDate { get; private set; }

        public void SetCreated()
        {
            CreatedDate = DateTime.UtcNow;
        }

        public void SetUpdated()
        {
            UpdatedDate = DateTime.UtcNow;
        }
    }
}
