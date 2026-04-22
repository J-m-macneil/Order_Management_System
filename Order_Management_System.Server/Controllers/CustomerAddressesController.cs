using Application.DTOs;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("api/customers/{customerId}/addresses")]
[Authorize]
public class CustomerAddressesController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public CustomerAddressesController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AddressDto>>> GetAll(int customerId)
    {
        var customerExists = await _dbContext.Customers
            .AnyAsync(x => x.CustomerId == customerId && x.IsActive && x.DeletedAt == null);

        if (!customerExists)
            return NotFound("Customer not found");

        var addresses = await _dbContext.Addresses
            .Where(x => x.CustomerId == customerId && x.IsActive && x.DeletedAt == null)
            .OrderBy(x => x.AddressType)
            .ThenByDescending(x => x.IsPrimary)
            .ThenBy(x => x.SiteName)
            .Select(x => new AddressDto
            {
                AddressId = x.AddressId,
                CustomerId = x.CustomerId,
                AddressType = x.AddressType,
                SiteName = x.SiteName,
                Line1 = x.Line1,
                Line2 = x.Line2,
                City = x.City,
                County = x.County,
                Postcode = x.Postcode,
                Country = x.Country,
                ContactName = x.ContactName,
                ContactPhone = x.ContactPhone,
                DeliveryInstructions = x.DeliveryInstructions,
                IsPrimary = x.IsPrimary
            })
            .ToListAsync();

        return Ok(addresses);
    }

    [HttpGet("{addressId}")]
    public async Task<ActionResult<AddressDto>> GetById(int customerId, int addressId)
    {
        var address = await _dbContext.Addresses
            .Where(x => x.AddressId == addressId &&
                        x.CustomerId == customerId &&
                        x.IsActive &&
                        x.DeletedAt == null)
            .Select(x => new AddressDto
            {
                AddressId = x.AddressId,
                CustomerId = x.CustomerId,
                AddressType = x.AddressType,
                SiteName = x.SiteName,
                Line1 = x.Line1,
                Line2 = x.Line2,
                City = x.City,
                County = x.County,
                Postcode = x.Postcode,
                Country = x.Country,
                ContactName = x.ContactName,
                ContactPhone = x.ContactPhone,
                DeliveryInstructions = x.DeliveryInstructions,
                IsPrimary = x.IsPrimary
            })
            .FirstOrDefaultAsync();

        if (address == null)
            return NotFound();

        return Ok(address);
    }

    [HttpPost]
    public async Task<ActionResult<AddressDto>> Create(int customerId, [FromBody] CreateAddressDto dto)
    {
        var customerExists = await _dbContext.Customers
            .AnyAsync(x => x.CustomerId == customerId && x.IsActive && x.DeletedAt == null);

        if (!customerExists)
            return NotFound("Customer not found");

        var address = new Address
        {
            CustomerId = customerId,
            AddressType = dto.AddressType,
            SiteName = dto.SiteName,
            Line1 = dto.Line1,
            Line2 = dto.Line2,
            City = dto.City,
            County = dto.County,
            Postcode = dto.Postcode,
            Country = dto.Country,
            ContactName = dto.ContactName,
            ContactPhone = dto.ContactPhone,
            DeliveryInstructions = dto.DeliveryInstructions,
            IsPrimary = dto.IsPrimary,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            DeletedAt = null
        };

        _dbContext.Addresses.Add(address);
        await _dbContext.SaveChangesAsync();

        var result = new AddressDto
        {
            AddressId = address.AddressId,
            CustomerId = address.CustomerId,
            AddressType = address.AddressType,
            SiteName = address.SiteName,
            Line1 = address.Line1,
            Line2 = address.Line2,
            City = address.City,
            County = address.County,
            Postcode = address.Postcode,
            Country = address.Country,
            ContactName = address.ContactName,
            ContactPhone = address.ContactPhone,
            DeliveryInstructions = address.DeliveryInstructions,
            IsPrimary = address.IsPrimary
        };

        return CreatedAtAction(nameof(GetById), new { customerId, addressId = address.AddressId }, result);
    }

    [HttpPut("{addressId}")]
    public async Task<IActionResult> Update(int customerId, int addressId, [FromBody] UpdateAddressDto dto)
    {
        var address = await _dbContext.Addresses
            .FirstOrDefaultAsync(x => x.AddressId == addressId &&
                                      x.CustomerId == customerId &&
                                      x.IsActive &&
                                      x.DeletedAt == null);

        if (address == null)
            return NotFound();

        address.AddressType = dto.AddressType;
        address.SiteName = dto.SiteName;
        address.Line1 = dto.Line1;
        address.Line2 = dto.Line2;
        address.City = dto.City;
        address.County = dto.County;
        address.Postcode = dto.Postcode;
        address.Country = dto.Country;
        address.ContactName = dto.ContactName;
        address.ContactPhone = dto.ContactPhone;
        address.DeliveryInstructions = dto.DeliveryInstructions;
        address.IsPrimary = dto.IsPrimary;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{addressId}")]
    public async Task<IActionResult> Delete(int customerId, int addressId)
    {
        var address = await _dbContext.Addresses
            .FirstOrDefaultAsync(x => x.AddressId == addressId &&
                                      x.CustomerId == customerId &&
                                      x.IsActive &&
                                      x.DeletedAt == null);

        if (address == null)
            return NotFound();

        address.IsActive = false;
        address.DeletedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}