using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Sharecode.Backend.Application.Data;

public interface IShareCodeDbContext
{
    
    public DatabaseFacade Database { get; }

}