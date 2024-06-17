using Amazon;
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
    //[Authorize]
    public class RDSController : ControllerBase
    {
        private IAmazonRDS _rdsclient;
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