using Dapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MyAspnetCore.DTO.ActivityLog;
using MyAspnetCore.Entities;
using MyAspnetCore.Interfaces.Infrastructure;
using MyAspnetCore.Interfaces.Services;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetInfrastructure.Repository
{
    public class ActivityLogRepository : BaseRepository<ActivityLog>, IActivityLogRepository
    {
        public ActivityLogRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IEnumerable<ActivityLog>> GetLogById(string id)
        {
            using (connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@p_UserId", id);

                var res = await connection.QueryAsync<ActivityLog>("Proc_ActivityLog_GetLog", parameters, commandType: CommandType.StoredProcedure);
                return res;
            }
        }
    }
}
