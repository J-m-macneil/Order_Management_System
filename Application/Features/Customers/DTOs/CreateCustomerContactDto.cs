using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Customers.DTOs
{
    public class CreateCustomerContactDto
    {
        public string Name { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public bool IsPrimary { get; set; }
    }
}
