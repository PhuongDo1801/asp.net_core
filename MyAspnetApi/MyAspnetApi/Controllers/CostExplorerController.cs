using Amazon.CostExplorer.Model;
using Amazon.CostExplorer;
using Amazon.Runtime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Amazon;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CostExplorerController : ControllerBase
    {
        private readonly IAmazonCostExplorer _costExplorerClient;
        private readonly IConfiguration _configuration;

        public CostExplorerController(IConfiguration configuration)
        {
            _configuration = configuration;
            var accessKey = _configuration.GetValue<string>("AWS:AccessKey");
            var secretKey = _configuration.GetValue<string>("AWS:SecretKey");
            var credentials = new BasicAWSCredentials(accessKey, secretKey); // Sử dụng IAM Role
            var regionEndpoint = RegionEndpoint.USEast1; // Thay đổi region endpoint tại đây
            _costExplorerClient = new AmazonCostExplorerClient(credentials, regionEndpoint);
            
        }

        [HttpGet("cost-explorer-data")]
        public async Task<IActionResult> GetCostExplorerData()
        {
            try
            {
                var request = new GetCostAndUsageRequest
                {
                    TimePeriod = new DateInterval
                    {
                        Start = DateTime.UtcNow.AddMonths(-1).ToString("yyyy-MM-dd"),
                        End = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    },
                    Granularity = Granularity.MONTHLY,
                    Filter = new Expression
                    {
                        Dimensions = new DimensionValues
                        {
                            Key = "SERVICE",
                            Values = new List<string> { "Amazon Elastic Compute Cloud - Compute" }
                        }
                    },
                    Metrics = new List<string> { "BlendedCost", "UnblendedCost", "UsageQuantity" },
                };

                var response = await _costExplorerClient.GetCostAndUsageAsync(request);

                // Xử lý dữ liệu response ở đây...
                // Ví dụ: Trả về dữ liệu dưới dạng JSON
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về mã lỗi
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpGet("get-service-names")]
        public async Task<IActionResult> GetServiceNames()
        {
            try
            {
                var request = new GetCostAndUsageRequest
                {
                    TimePeriod = new DateInterval
                    {
                        Start = DateTime.UtcNow.AddMonths(-1).ToString("yyyy-MM-dd"),
                        End = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    },
                    Granularity = Granularity.MONTHLY,
                    Metrics = new List<string> { "BlendedCost", "UnblendedCost", "UsageQuantity" },
                };

                var response = await _costExplorerClient.GetCostAndUsageAsync(request);

                // Xử lý dữ liệu response ở đây...
                var serviceNames = GetServiceNames(response);

                // Ví dụ: Trả về dữ liệu dưới dạng JSON
                return Ok(serviceNames);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về mã lỗi
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
        private IEnumerable<string> GetServiceNames(GetCostAndUsageResponse response)
        {
            var serviceNames = new List<string>();

            foreach (var resultByTime in response.ResultsByTime)
            {
                foreach (var group in resultByTime.Groups)
                {
                    foreach (var keys in group.Keys)
                    {
                        // Tách các phần tử trong keys và lấy tên dịch vụ
                        var serviceName = keys.FirstOrDefault().ToString();

                        // Kiểm tra xem tên dịch vụ có tồn tại và không trùng lặp
                        if (!string.IsNullOrEmpty(serviceName) && !serviceNames.Contains(serviceName))
                        {
                            // Thêm tên dịch vụ vào danh sách
                            serviceNames.Add(serviceName);
                        }
                    }
                }
            }

            return serviceNames;
        }
    }
}
