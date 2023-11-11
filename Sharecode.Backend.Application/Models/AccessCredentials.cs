using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Application.Models;

public record AccessCredentials(string AccessToken, string RefreshToken, UserRefreshToken UserRefreshToken);