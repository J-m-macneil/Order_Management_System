using Application.DTOs;
using Domain.Entities;
using Domain.Entities.Customers;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public CustomersController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var customers = await _dbContext.Customers
            .Where(x => x.IsActive && x.DeletedAt == null)
            .Select(x => new CustomerDto
            {
                CustomerId = x.CustomerId,
                AccountNumber = x.AccountNumber,
                CompanyName = x.CompanyName,
                IndustryType = x.IndustryType,
                MainContactName = x.MainContactName,
                MainContactEmail = x.MainContactEmail,
                MainContactPhone = x.MainContactPhone,
                BillingAddressId = x.BillingAddressId,
                DefaultDeliveryAddressId = x.DefaultDeliveryAddressId,
                PricingTierId = x.PricingTierId,
                PaymentTermsDays = x.PaymentTermsDays,
                CreditLimit = x.CreditLimit,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var customer = await _dbContext.Customers
            .Where(x => x.CustomerId == id && x.IsActive && x.DeletedAt == null)
            .Select(x => new CustomerDto
            {
                CustomerId = x.CustomerId,
                AccountNumber = x.AccountNumber,
                CompanyName = x.CompanyName,
                IndustryType = x.IndustryType,
                MainContactName = x.MainContactName,
                MainContactEmail = x.MainContactEmail,
                MainContactPhone = x.MainContactPhone,
                BillingAddressId = x.BillingAddressId,
                DefaultDeliveryAddressId = x.DefaultDeliveryAddressId,
                PricingTierId = x.PricingTierId,
                PaymentTermsDays = x.PaymentTermsDays,
                CreditLimit = x.CreditLimit,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (customer == null)
            return NotFound();

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            AccountNumber = dto.AccountNumber,
            CompanyName = dto.CompanyName,
            IndustryType = dto.IndustryType,
            MainContactName = dto.MainContactName,
            MainContactEmail = dto.MainContactEmail,
            MainContactPhone = dto.MainContactPhone,
            BillingAddressId = dto.BillingAddressId,
            DefaultDeliveryAddressId = dto.DefaultDeliveryAddressId,
            PricingTierId = dto.PricingTierId,
            PaymentTermsDays = dto.PaymentTermsDays,
            CreditLimit = dto.CreditLimit,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            DeletedAt = null
        };

        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();

        var result = new CustomerDto
        {
            CustomerId = customer.CustomerId,
            AccountNumber = customer.AccountNumber,
            CompanyName = customer.CompanyName,
            IndustryType = customer.IndustryType,
            MainContactName = customer.MainContactName,
            MainContactEmail = customer.MainContactEmail,
            MainContactPhone = customer.MainContactPhone,
            BillingAddressId = customer.BillingAddressId,
            DefaultDeliveryAddressId = customer.DefaultDeliveryAddressId,
            PricingTierId = customer.PricingTierId,
            PaymentTermsDays = customer.PaymentTermsDays,
            CreditLimit = customer.CreditLimit,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = customer.CustomerId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dto)
    {
        var customer = await _dbContext.Customers.FindAsync(id);

        if (customer == null || !customer.IsActive || customer.DeletedAt != null)
            return NotFound();

        customer.AccountNumber = dto.AccountNumber;
        customer.CompanyName = dto.CompanyName;
        customer.IndustryType = dto.IndustryType;
        customer.MainContactName = dto.MainContactName;
        customer.MainContactEmail = dto.MainContactEmail;
        customer.MainContactPhone = dto.MainContactPhone;
        customer.BillingAddressId = dto.BillingAddressId;
        customer.DefaultDeliveryAddressId = dto.DefaultDeliveryAddressId;
        customer.PricingTierId = dto.PricingTierId;
        customer.PaymentTermsDays = dto.PaymentTermsDays;
        customer.CreditLimit = dto.CreditLimit;
        customer.IsActive = dto.IsActive;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _dbContext.Customers.FindAsync(id);

        if (customer == null || customer.DeletedAt != null)
            return NotFound();

        customer.IsActive = false;
        customer.DeletedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}