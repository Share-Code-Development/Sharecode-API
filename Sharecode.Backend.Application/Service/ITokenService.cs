using Sharecode.Backend.Application.Models;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Application.Service;

public interface ITokenService
{
    AccessCredentials Generate(User user);
}