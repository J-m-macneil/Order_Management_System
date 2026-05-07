namespace Application.Features.Customers.DTOs
{
    public class CustomerContactDto
    {
        public int CustomerContactId { get; set; }
        public int CustomerId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }

        public bool IsPrimary { get; set; }
    }
}