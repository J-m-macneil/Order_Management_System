using Domain.Entities.Organisation;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

public class CarrierRepository : ICarrierRepository
{
	private readonly AppDbContext _db;

	public CarrierRepository(AppDbContext db)
	{
		_db = db;
	}

	public async Task<List<Carrier>> GetActiveAsync(CancellationToken ct)
	{
		return await _db.Carriers
			.Where(x => x.IsActive)
			.AsNoTracking()
			.ToListAsync(ct);
	}

	public async Task<Carrier?> GetByIdAsync(int id, CancellationToken ct)
	{
		return await _db.Carriers
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.CarrierId == id && x.IsActive, ct);
	}
}