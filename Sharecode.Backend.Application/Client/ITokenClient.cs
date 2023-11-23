using Sharecode.Backend.Application.Models;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Application.Client;

public interface ITokenClient
{
    AccessCredentials Generate(User user);
}