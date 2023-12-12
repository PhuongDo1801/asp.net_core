using Amazon;
using Amazon.CostExplorer;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EC2instancesController : ControllerBase
    {
        private readonly IAmazonEC2 _ec2client;
        private readonly IConfiguration _configuration;

        public EC2instancesController(IConfiguration configuration)
        {
            _configuration = configuration;
            var accessKey = _configuration.GetValue<string>("AWS:AccessKey");
            var secretKey = _configuration.GetValue<string>("AWS:SecretKey");
            var credentials = new BasicAWSCredentials(accessKey, secretKey); // Sử dụng IAM Role
            var regionEndpoint = RegionEndpoint.USEast1; // Thay đổi region endpoint tại đây
            _ec2client = new AmazonEC2Client(credentials, regionEndpoint);

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
                        InstanceId = instance.InstanceId,
                        InstanceType = instance.InstanceType.Value,
                        State = instance.State.Name.Value,
                        Cpu = instance.CpuOptions?.CoreCount,
                        Platform = instance.Platform.Value,
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

    }
}
