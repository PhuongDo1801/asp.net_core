using Amazon;
using Amazon.CostExplorer;
using Amazon.CostExplorer.Model;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAspnetCore.Interfaces.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class EC2instancesController : ControllerBase
    {
        private IAmazonEC2 _ec2client;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public EC2instancesController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
            Initialize().Wait();
        }
        private async Task Initialize()
        {
            var user = await _userService.GetUserInfo(); ;
            if (user != null)
            {
                _ec2client = InitializeClient(user.AccessKey, user.SecretKey);
            }
            //else
            //{
            //    // Nếu không có thông tin từ HttpContext, thử lấy từ localStorage
            //    var accessKey = _configuration["LocalStorage:AccessKey"];
            //    var secretKey = _configuration["LocalStorage:SecretKey"];
            //    if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            //    {
            //        _ec2client = InitializeClient(accessKey, secretKey);
            //    }
            //}
        }
        private IAmazonEC2 InitializeClient(string accessKey, string secretKey)
        {
            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                var credentials = new BasicAWSCredentials(accessKey, secretKey);
                var regionEndpoint = RegionEndpoint.USEast1; // Thay đổi region endpoint tại đây
                return new AmazonEC2Client(credentials, regionEndpoint);
            }

            return null;
        }
        private bool ClientIsNull()
        {
            return _ec2client == null;
        }
        /// <summary>
        /// Lấy thông tin Ec2 bản thân người dùng đang sử dụng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetInstances()
        {
            try
            {
                var describeInstancesRequest = new DescribeInstancesRequest();
                var describeInstancesResponse = await _ec2client.DescribeInstancesAsync(describeInstancesRequest);

                var instances = describeInstancesResponse.Reservations
                    .SelectMany(reservation => reservation.Instances)
                    .Select(async instance => new
                    {
                        Name = instance.KeyName,
                        InstanceId = instance.InstanceId,
                        InstanceType = instance.InstanceType.Value,
                        Zone = instance.Placement.AvailabilityZone,
                        LaunchTime = instance.LaunchTime,
                        State = instance.State.Name.Value,
                        vCpu = instance.CpuOptions?.CoreCount,
                        Platform = instance.Platform.Value,
                        PrivateIpAddress = instance.PrivateIpAddress, // Địa chỉ IPv4 nội bộ
                        PublicIpAddress = instance.PublicIpAddress,   // Địa chỉ IPv4 công khai (nếu có)
                        //z = instance.
                        //Ram = instance.GetMetadata()["instance-memory"].ToInt32(),
                        //Thêm các thông tin khác mà bạn quan tâm
                    });

                var results = await Task.WhenAll(instances);

                return Ok(results);
            }
            catch (AmazonEC2Exception ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

        [HttpGet("GetEC2InstanceTypeCost")]
        public async Task<IActionResult> GetEC2InstanceTypeCost(string instanceType)
        {
            try
            {
                var costExplorerClient = new AmazonCostExplorerClient(); // Khởi tạo client Cost Explorer
                var today = DateTime.Today;
                var startDate = new DateTime(today.Year, today.Month, 1).AddMonths(-1); // Thời điểm bắt đầu là đầu tháng trước
                var endDate = new DateTime(today.Year, today.Month, 1).AddDays(-1); // Thời điểm kết thúc là cuối tháng trước

                var request = new GetCostAndUsageRequest
                {
                    TimePeriod = new DateInterval { Start = startDate.ToString("yyyy-MM-dd"), End = endDate.ToString("yyyy-MM-dd") },
                    Granularity = Granularity.MONTHLY,
                    Metrics = new List<string> { "UnblendedCost" },
                    Filter = new Expression
                    {
                        Dimensions = new DimensionValues
                        {
                            Key = "INSTANCE_TYPE",
                            Values = new List<string> { instanceType }
                        }
                    }
                };

                var response = await costExplorerClient.GetCostAndUsageAsync(request);
                var totalCost = response.ResultsByTime.Sum(result => decimal.Parse(result.Total["UnblendedCost"].Amount));

                return Ok(new { InstanceType = instanceType, MonthlyBlendedCost = totalCost });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving cost: {ex.Message}");
            }
        }



        [HttpPost("start")]
        public async Task<IActionResult> StartInstance(string instanceId)
        {
            try
            {
                var startInstancesRequest = new StartInstancesRequest
                {
                    InstanceIds = new List<string> { instanceId }
                };

                var startInstancesResponse = await _ec2client.StartInstancesAsync(startInstancesRequest);

                // Kiểm tra trạng thái hoạt động của instance sau khi khởi động

                return Ok("Instance started successfully");
            }
            catch (AmazonEC2Exception ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopInstance(string instanceId)
        {
            try
            {
                var stopInstancesRequest = new StopInstancesRequest
                {
                    InstanceIds = new List<string> { instanceId }
                };

                var stopInstancesResponse = await _ec2client.StopInstancesAsync(stopInstancesRequest);

                // Kiểm tra trạng thái tắt của instance sau khi dừng

                return Ok("Instance stopped successfully");
            }
            catch (AmazonEC2Exception ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

        [HttpPost("reboot")]
        public async Task<IActionResult> RebootInstance(string instanceId)
        {
            try
            {
                var rebootInstancesRequest = new RebootInstancesRequest
                {
                    InstanceIds = new List<string> { instanceId }
                };

                var rebootInstancesResponse = await _ec2client.RebootInstancesAsync(rebootInstancesRequest);

                // Kiểm tra trạng thái sau khi khởi động lại

                return Ok("Instance rebooted successfully");
            }
            catch (AmazonEC2Exception ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

        // GET: api/instancetypes/ec2/t2.micro
        [HttpGet("ec2/{instanceType}")]
        public async Task<ActionResult<string>> GetEC2InstanceType(string instanceType)
        {
            try
            {
                var response = await _ec2client.DescribeInstanceTypesAsync(new DescribeInstanceTypesRequest
                {
                    InstanceTypes = new List<string> { instanceType }
                });

                if (response.InstanceTypes.Count > 0)
                {
                    var details = response.InstanceTypes[0]; // Assume only one match
                    return Ok(details);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving EC2 instance type: {ex.Message}");
            }
        }

    }
}
