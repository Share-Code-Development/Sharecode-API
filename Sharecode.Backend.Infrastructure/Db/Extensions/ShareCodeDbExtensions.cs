using Dapper;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CodeCompanion.Extensions.Dapper.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Sharecode.Backend.Infrastructure.Db.Extensions;

public static class ShareCodeDbExtensions
{
    public static Refcursors QueryRefcursors(
        this NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string functionName,
        CommandType commandType = CommandType.StoredProcedure,
        object? param = null)
    {
        NpgsqlConnection connection1 = connection;
        NpgsqlTransaction transaction1 = transaction;
        NpgsqlConnection npgsqlConnection = connection;
        string str = functionName;
        IDbTransaction dbTransaction1 = (IDbTransaction) transaction;
        object obj = param;
        IDbTransaction dbTransaction2 = dbTransaction1;
        CommandType? nullable1 = commandType;
        int? nullable2 = new int?();
        CommandType? nullable3 = nullable1;
        IEnumerator<string> enumerator = SqlMapper.Query<string>((IDbConnection) npgsqlConnection, str, obj, dbTransaction2, true, nullable2, nullable3).GetEnumerator();
        return new Refcursors(connection1, transaction1, enumerator);
    }

    public static async Task<Refcursors> QueryRefcursorsAsync(
        this NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string functionName,
        CommandType commandType = CommandType.StoredProcedure,        
        object? param = null)
    {
        NpgsqlConnection connection1 = connection;
        NpgsqlTransaction transaction1 = transaction;
        NpgsqlConnection npgsqlConnection = connection;
        string str = functionName;
        IDbTransaction dbTransaction1 = (IDbTransaction) transaction;
        object obj = param;
        IDbTransaction dbTransaction2 = dbTransaction1;
        CommandType? nullable1 = commandType;
        int? nullable2 = new int?();
        CommandType? nullable3 = nullable1;
        return new Refcursors(connection1, transaction1, (await SqlMapper.QueryAsync<string>((IDbConnection) npgsqlConnection, str, obj, dbTransaction2, nullable2, nullable3)).GetEnumerator());
    }
    
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
    {
        var converter = new ValueConverter<T, string>(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<T>(v));

        var comparer = new ValueComparer<T>(
            (l, r) => JsonConvert.SerializeObject(l) == JsonConvert.SerializeObject(r),
            v => v == null ? 0 : JsonConvert.SerializeObject(v).GetHashCode(),
            v => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v)));

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueConverter(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);

        return propertyBuilder;
    }

    public static IQueryable<T> SetTracking<T>(this DbSet<T> dbSet, bool track = true) where T : class
    {
        return track ? dbSet.AsTracking() : dbSet.AsNoTracking();
    }
}