using Amazon;
using Amazon.CostExplorer.Model;
using Amazon.CostExplorer;
using Amazon.EC2;
using Amazon.RDS;
using Amazon.RDS.Model;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAspnetCore.Interfaces.Services;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class RDSController : ControllerBase
    {
        private IAmazonRDS _rdsclient;
        private IAmazonCostExplorer _costExplorerClient;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public RDSController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
            Initialize().Wait();
        }
        private async Task Initialize()
        {
            var user = await _userService.GetUserInfo();
            if (user != null)
            {
                _rdsclient = InitializeClient(user.AccessKey, user.SecretKey);
                _costExplorerClient = InitializeClientCost(user.AccessKey, user.SecretKey);
            }
        }
        private IAmazonRDS InitializeClient(string accessKey, string secretKey)
        {
            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                var credentials = new BasicAWSCredentials(accessKey, secretKey);
                var regionEndpoint = RegionEndpoint.USEast1; // Thay đổi region endpoint tại đây
                return new AmazonRDSClient(credentials, regionEndpoint);
            }

            return null;
        }
        private IAmazonCostExplorer InitializeClientCost(string accessKey, string secretKey)
        {
            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                // var decryptedAccessKey = CryptoHelper.Decrypt(accessKey);
                //var decryptedSecretKey = CryptoHelper.Decrypt(secretKey);
                var credentials = new BasicAWSCredentials(accessKey, secretKey);
                var regionEndpoint = RegionEndpoint.USEast1;
                return new AmazonCostExplorerClient(credentials, regionEndpoint);
            }

            return null;
        }
        private bool ClientIsNull()
        {
            return _rdsclient == null;
        }
        [HttpGet("instances")]
        public async Task<IActionResult> ListRDSInstances()
        {
            //if (ClientIsNull())
            //{
            //    return BadRequest("RDS client not initialized.");
            //}

            try
            {
                var describeDBInstancesResponse = await _rdsclient.DescribeDBInstancesAsync();
                var dbInstances = describeDBInstancesResponse.DBInstances.Select(instance => new
                {
                    DBInstanceIdentifier = instance.DBInstanceIdentifier,
                    DBInstanceClass = instance.DBInstanceClass,
                    Engine = instance.Engine,
                    DBInstanceStatus = instance.DBInstanceStatus,
                    Endpoint = instance.Endpoint?.Address,
                    Port = instance.Endpoint?.Port,
                    AvailabilityZone = instance.AvailabilityZone
                });

                return Ok(dbInstances);
            }
            catch (AmazonRDSException ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }
        [HttpGet("instance/{dbInstanceIdentifier}")]
        public async Task<IActionResult> GetRDSInstance(string dbInstanceIdentifier)
        {
            if (ClientIsNull())
            {
                return BadRequest("RDS client not initialized.");
            }

            try
            {
                var describeDBInstancesRequest = new DescribeDBInstancesRequest
                {
                    DBInstanceIdentifier = dbInstanceIdentifier
                };

                var describeDBInstancesResponse = await _rdsclient.DescribeDBInstancesAsync(describeDBInstancesRequest);

                var instance = describeDBInstancesResponse.DBInstances.FirstOrDefault();
                if (instance == null)
                {
                    return NotFound($"RDS instance '{dbInstanceIdentifier}' not found.");
                }

                var instanceInfo = new
                {
                    instance.DBInstanceIdentifier,
                    instance.DBInstanceClass,
                    instance.Engine,
                    instance.DBInstanceStatus,
                    instance.MasterUsername,
                    instance.DBName,
                    instance.Endpoint.Address,
                    instance.Endpoint.Port,
                    instance.AllocatedStorage,
                    instance.InstanceCreateTime,
                    instance.AvailabilityZone,
                    instance.MultiAZ,
                    instance.PubliclyAccessible,
                    instance.StorageType
                    // Thêm các thuộc tính khác bạn cần
                };

                return Ok(instanceInfo);
            }
            catch (AmazonRDSException ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

        [HttpGet("GetRDSInstanceMonthlyCost")]
        public async Task<IActionResult> GetRDSInstanceMonthlyCost(string dbInstanceIdentifier)
        {
            try
            {
                //var costExplorerClient = new AmazonCostExplorerClient(); // Khởi tạo client Cost Explorer
                var today = DateTime.Today;
                var startDate = new DateTime(today.Year, today.Month, 1).AddMonths(-1); // Thời điểm bắt đầu là đầu tháng trước
                var endDate = new DateTime(today.Year, today.Month, 1).AddDays(-1); // Thời điểm kết thúc là cuối tháng trước


                var request = new GetCostAndUsageRequest
                {
                    TimePeriod = new DateInterval { Start = startDate.ToString("yyyy-MM-dd"), End = endDate.ToString("yyyy-MM-dd") },
                    Granularity = Granularity.MONTHLY,
                    Metrics = new List<string> { "BlendedCost" },
                    Filter = new Expression
                    {
                        Dimensions = new DimensionValues
                        {
                            Key = "USAGE_TYPE", // Hoặc sử dụng chiều dữ liệu khác phù hợp
                            Values = new List<string> { $"RDS:{dbInstanceIdentifier}" } // Sử dụng định dạng phù hợp với chiều dữ liệu
                        }
                    }
                };

                var response = await _costExplorerClient.GetCostAndUsageAsync(request);
                var totalCost = response.ResultsByTime.Sum(result => decimal.Parse(result.Total["BlendedCost"].Amount));

                return Ok(new { DBInstanceIdentifier = dbInstanceIdentifier, MonthlyBlendedCost = totalCost });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving cost: {ex.Message}");
            }
        }

        [HttpGet("GetEC2InstanceTypeCost")]
        public async Task<IActionResult> GetEC2InstanceTypeCost(string instanceType)
        {
            try
            {
                var today = DateTime.UtcNow;
                var startDate = new DateTime(today.Year, today.Month, 1).AddMonths(-1); // Thời điểm bắt đầu là đầu tháng trước (theo múi giờ UTC)
                var endDate = new DateTime(today.Year, today.Month, 1).AddDays(-1); // T
                var request = new GetCostAndUsageRequest
                {
                    TimePeriod = new DateInterval { Start = startDate.ToString("yyyy-MM-dd"), End = endDate.ToString("yyyy-MM-dd") },
                    Granularity = Granularity.MONTHLY,
                    Metrics = new List<string> { "BlendedCost" },
                    Filter = new Expression
                    {
                        Dimensions = new DimensionValues
                        {
                            Key = "INSTANCE_TYPE",
                            Values = new List<string> { instanceType }
                        }
                    }
                };

                var response = await _costExplorerClient.GetCostAndUsageAsync(request);
                var totalCost = response.ResultsByTime.Sum(result => decimal.Parse(result.Total["BlendedCost"].Amount));

                return Ok(new { InstanceType = instanceType, MonthlyBlendedCost = totalCost });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving cost: {ex.Message}");
            }
        }


        [HttpPost("start")]
        public async Task<IActionResult> StartRDSInstance([FromBody] string dbInstanceIdentifier)
        {
            //if (ClientIsNull())
            //{
            //    return BadRequest("RDS client not initialized.");
            //}

            try
            {
                var startDBInstanceRequest = new StartDBInstanceRequest
                {
                    DBInstanceIdentifier = dbInstanceIdentifier
                };

                var startDBInstanceResponse = await _rdsclient.StartDBInstanceAsync(startDBInstanceRequest);
                return Ok($"RDS instance '{dbInstanceIdentifier}' started successfully.");
            }
            catch (AmazonRDSException ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopRDSInstance([FromBody] string dbInstanceIdentifier)
        {
            //if (ClientIsNull())
            //{
            //    return BadRequest("RDS client not initialized.");
            //}

            try
            {
                var stopDBInstanceRequest = new StopDBInstanceRequest
                {
                    DBInstanceIdentifier = dbInstanceIdentifier
                };

                var stopDBInstanceResponse = await _rdsclient.StopDBInstanceAsync(stopDBInstanceRequest);
                return Ok($"RDS instance '{dbInstanceIdentifier}' stopped successfully.");
            }
            catch (AmazonRDSException ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

        [HttpPost("reboot")]
        public async Task<IActionResult> RebootRDSInstance([FromBody] string dbInstanceIdentifier)
        {
            //if (ClientIsNull())
            //{
            //    return BadRequest("RDS client not initialized.");
            //}

            try
            {
                var rebootDBInstanceRequest = new RebootDBInstanceRequest
                {
                    DBInstanceIdentifier = dbInstanceIdentifier
                };

                var rebootDBInstanceResponse = await _rdsclient.RebootDBInstanceAsync(rebootDBInstanceRequest);
                return Ok($"RDS instance '{dbInstanceIdentifier}' rebooted successfully.");
            }
            catch (AmazonRDSException ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }
    }
}