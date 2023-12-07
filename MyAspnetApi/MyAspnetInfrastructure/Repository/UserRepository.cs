using Dapper;
using Microsoft.Extensions.Configuration;
using MyAspnetCore.Entities;
using MyAspnetCore.Interfaces.Infrastructure;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;  
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace MyAspnetInfrastructure.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<User> GetByEmail(string email)
        {
            using(connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@p_Email", email);
                var user = await connection.QueryFirstOrDefaultAsync<User>("Proc_User_GetByEmail", parameters, commandType: CommandType.StoredProcedure);
                return user;
            }
        }

        public async Task<(int, IEnumerable<User>)> GetListAsync(string? queryName, int? recordsPerPage, int? page)
        {
            using(connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@querySearch", queryName);
                parameters.Add("@recordsPerPage", recordsPerPage);
                parameters.Add("@pageOffset", recordsPerPage * (page - 1));
                parameters.Add("@totalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var users = await connection.QueryAsync<User>("Proc_User_GetList", parameters, commandType: CommandType.StoredProcedure);
                int totalRecord = parameters.Get<int>("totalRecord");
                return (totalRecord, users);
            }
        }

        public async Task<bool> IsExistEmail(string email)
        {
            using(connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@p_Email", email);
                var user = await connection.QueryFirstOrDefaultAsync<User>("Proc_User_GetByEmail", parameters, commandType: CommandType.StoredProcedure);
                if(user != null)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<User> Login(string email, string password)
        {
            using (connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@p_Email", email);
                parameters.Add("@p_Password", password);
                var res = await connection.QueryFirstOrDefaultAsync<User>("Proc_User_Login", parameters, commandType: CommandType.StoredProcedure);
                return res;
            }
        }

        public async Task<int> Register(User user)
        {
            using(connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var entityName = typeof(User).Name;
                var parameters = new DynamicParameters();
                foreach (var prop in user.GetType().GetProperties())
                {
                    if (prop.Name.Contains($"{entityName}Id"))
                    {
                        parameters.Add($"@p_{entityName}Id", Guid.NewGuid());
                    }
                    else
                    {
                        parameters.Add($"@p_{prop.Name}", prop.GetValue(user));
                    }
                }
                var result = await connection.ExecuteAsync($"Proc_{entityName}_Register", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }
    }
}
