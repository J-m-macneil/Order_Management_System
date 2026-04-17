using Application.DTOs;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/customers/{customerId}/contacts")]
    [Authorize]
    public class CustomerContactsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public CustomerContactsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerContactDto>>> Get(int customerId)
        {
            var contacts = await _dbContext.CustomerContacts
                .Where(x => x.CustomerId == customerId && x.IsActive && x.DeletedAt == null)
                .Select(x => new CustomerContactDto
                {
                    CustomerContactId = x.CustomerContactId,
                    CustomerId = x.CustomerId,
                    Name = x.Name,
                    JobTitle = x.JobTitle,
                    Email = x.Email,
                    Phone = x.Phone,
                    IsPrimary = x.IsPrimary
                })
                .ToListAsync();

            return Ok(contacts);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerContactDto>> Create(int customerId, [FromBody] CreateCustomerContactDto dto)
        {
            var exists = await _dbContext.Customers
                .AnyAsync(x => x.CustomerId == customerId && x.DeletedAt == null);

            if (!exists)
                return NotFound();

            var contact = new CustomerContact
            {
                CustomerId = customerId,
                Name = dto.Name,
                JobTitle = dto.JobTitle,
                Email = dto.Email,
                Phone = dto.Phone,
                IsPrimary = dto.IsPrimary,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.CustomerContacts.Add(contact);
            await _dbContext.SaveChangesAsync();

            return Ok(new CustomerContactDto
            {
                CustomerContactId = contact.CustomerContactId,
                CustomerId = contact.CustomerId,
                Name = contact.Name,
                JobTitle = contact.JobTitle,
                Email = contact.Email,
                Phone = contact.Phone,
                IsPrimary = contact.IsPrimary
            });
        }

        [HttpPut("{contactId}")]
        public async Task<IActionResult> Update(int customerId, int contactId, [FromBody] UpdateCustomerContactDto dto)
        {
            var contact = await _dbContext.CustomerContacts
                .FirstOrDefaultAsync(x =>
                    x.CustomerContactId == contactId &&
                    x.CustomerId == customerId &&
                    x.DeletedAt == null);

            if (contact == null)
                return NotFound();

            contact.Name = dto.Name;
            contact.JobTitle = dto.JobTitle;
            contact.Email = dto.Email;
            contact.Phone = dto.Phone;
            contact.IsPrimary = dto.IsPrimary;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{contactId}")]
        public async Task<IActionResult> Delete(int customerId, int contactId)
        {
            var contact = await _dbContext.CustomerContacts
                .FirstOrDefaultAsync(x =>
                    x.CustomerContactId == contactId &&
                    x.CustomerId == customerId &&
                    x.DeletedAt == null);

            if (contact == null)
                return NotFound();

            // ✅ SOFT DELETE
            contact.IsActive = false;
            contact.DeletedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
