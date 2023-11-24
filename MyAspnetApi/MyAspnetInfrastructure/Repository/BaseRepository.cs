using MyAspnetCore.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Net.WebSockets;
using System.ComponentModel;

namespace MyAspnetInfrastructure.Repository
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    {
        #region property
        protected readonly string? connectionString;
        protected MySqlConnection connection;
        #endregion

        public BaseRepository(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionString"];
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            using (connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var entityName = typeof(TEntity).Name;
                var result = await connection.QueryAsync<TEntity>($"Proc_{entityName}_GetAll", null, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        public async Task<TEntity> GetByIdAsync(Guid Id)
        {
            using(connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var entityName = typeof(TEntity).Name;
                var parameters = new DynamicParameters();
                parameters.Add($"p_{entityName}Id", Id);
                var result = await connection.QuerySingleOrDefaultAsync<TEntity>($"Proc_{entityName}_GetById", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<int> InsertAsync(TEntity entity)
        {
            using(connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var entityName = typeof(TEntity).Name;
                var parameters = new DynamicParameters();
                foreach (var prop in entity.GetType().GetProperties())
                {
                    if (prop.Name.Contains($"{entityName}Id"))
                    {
                        parameters.Add($"@p_{entityName}Id", Guid.NewGuid());
                    }
                    else
                    {
                        parameters.Add($"@p_{prop.Name}", prop.GetValue(entity));
                    }
                }
                var result = await connection.ExecuteAsync($"Proc_{entityName}_Insert", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<int> UpdateAsync(TEntity entity, Guid Id)
        {
            using (connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var entityName = typeof(TEntity).Name;
                var parameters = new DynamicParameters();
                foreach (var prop in entity.GetType().GetProperties())
                {
                    parameters.Add($"@p_{prop.Name}", prop.GetValue(entity));
                }
                parameters.Add($"@p_{entityName}Id", Id);
                var result = await connection.ExecuteAsync($"Proc_{entityName}_Update", parameters, commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<int> DeleteAsync(Guid Id)
        {
            using (connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var entityName = typeof(TEntity).Name;
                var parameters = new DynamicParameters();
                parameters.Add($"@p_{entityName}Id", Id);
                var result = await connection.ExecuteAsync($"Proc_{entityName}_Delete", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }
    }
}
