using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Sharecode.Backend.Application;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Exceptions;

namespace Sharecode.Backend.Infrastructure.Service;

public class RefreshTokenService(IRefreshTokenRepository tokenRepository, IUnitOfWork unitOfWork) : IRefreshTokenService
{
    public async Task<(Guid?, string?, string?, string?, string?)?> ValidateTokenIfPresent(Guid tokenIdentifier, Guid issuedFor)
    {
        var newExpiry = DateTime.UtcNow.AddDays(2);
        using var dapperContext = tokenRepository.CreateDapperContext();
        if(dapperContext == null)
            throw new InfrastructureDownException("Failed to validate the token", $"Failed to create the dapper context for token validation");

        var parameters = new DynamicParameters();
        parameters.Add("token", tokenIdentifier);
        parameters.Add("issuedfor", issuedFor);
        parameters.Add("newexpiry", newExpiry);
        var token = dapperContext.QueryFirstOrDefault<dynamic>("SELECT * FROM validate_token(@token, @issuedfor, @newexpiry)", parameters,
            commandTimeout: 1000);
        if (token == null)
            return null;
        
        return (token?.TokenIdentifier, token?.EmailAddress, token?.FirstName, token?.MiddleName, token?.LastName);
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

    public async Task InvalidateAllOfUserAsync(Guid issuedFor, CancellationToken token = default)
    {
        var deleteSpecification = new EntitySpecification<UserRefreshToken>(x => x.IssuedFor == issuedFor);
        await tokenRepository.DeleteBatchAsync(deleteSpecification, token);
    }
}