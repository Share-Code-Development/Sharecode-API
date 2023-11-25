using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Infrastructure.Service;

public class RefreshTokenService(IRefreshTokenRepository tokenRepository, IShareCodeDbContext dbContext, IUnitOfWork unitOfWork) : IRefreshTokenService
{
    
    public async Task<Guid?> ValidateTokenIfPresent(Guid tokenIdentifier, Guid issuedFor)
    {
        var newExpiry = DateTime.UtcNow.AddDays(2);
        await using var connection = dbContext.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT sharecode.validate_token(@token, @issuedFor, @newExpiry)";
        command.CommandType = CommandType.Text;
        command.Parameters.Add(new NpgsqlParameter("token", tokenIdentifier));
        command.Parameters.Add(new NpgsqlParameter("issuedFor", issuedFor));
        command.Parameters.Add(new NpgsqlParameter("newExpiry", newExpiry));
        var result = await command.ExecuteScalarAsync();
        if (connection.State == System.Data.ConnectionState.Open)
        {
            await connection.CloseAsync();
        }
        return result == DBNull.Value ? null : (Guid?)result;
    }

    public async Task<UserRefreshToken> GenerateRefreshTokenAsync(Guid issuedFor, bool saveWithUnitOfWork = false, Guid? tokenIdentifier = null, DateTime? expiry = null, CancellationToken token = default)
    {
        expiry ??= DateTime.UtcNow.AddDays(2);
        expiry = DateTime.SpecifyKind(expiry.Value, DateTimeKind.Utc);

        UserRefreshToken refreshToken = new UserRefreshToken()
        {
            IssuedFor = issuedFor,
            Expiry = expiry.Value,
            TokenIdentifier = tokenIdentifier ?? Guid.NewGuid(),
        };

        await tokenRepository.AddAsync(refreshToken, token);
        if (saveWithUnitOfWork)
        {
            await unitOfWork.CommitAsync(cancellationToken: token);
        }
        return refreshToken;
    }
}