using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application;

public interface IAppDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }
}