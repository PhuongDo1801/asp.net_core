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

namespace MyAspnetInfrastructure.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
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
    }
}
