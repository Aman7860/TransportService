namespace TransportService.Data.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid Id { get; set; } //PK

        public Guid UserId { get; set; } //FK
        public User User { get; set; } = null!;

        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
    }

}
