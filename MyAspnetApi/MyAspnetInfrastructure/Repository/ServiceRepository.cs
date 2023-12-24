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
    public class ServiceRepository : BaseRepository<Service>, IServiceRepository
    {
        public ServiceRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<(int, IEnumerable<Service>)> GetListAsync(string? queryName, int? recordsPerPage, int? page)
        {
            using (connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@querySearch", queryName);
                parameters.Add("@recordsPerPage", recordsPerPage);
                parameters.Add("@pageOffset", recordsPerPage * (page - 1));
                parameters.Add("@totalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var services = await connection.QueryAsync<Service>("Proc_Service_GetList", parameters, commandType: CommandType.StoredProcedure);
                int totalRecord = parameters.Get<int>("totalRecord");
                return (totalRecord, services);
            }
        }
    }
}
