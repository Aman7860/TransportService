namespace TransportService.Data.Entities
{
    public class User : BaseEntity
    {
        public Guid Id { get; set; } //PK

        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsActive { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
