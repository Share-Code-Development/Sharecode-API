using Sharecode.Backend.Application.Models;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Application.Service;

public interface ITokenClient
{
    AccessCredentials Generate(User user);
}