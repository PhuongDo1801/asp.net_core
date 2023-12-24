using Amazon;
using Amazon.AWSSupport;
using Amazon.Pricing;
using Amazon.Runtime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")] 
    [ApiController]
    public class AWSSupportController : ControllerBase
    {
        private readonly IAmazonAWSSupport _supportClient;
        private readonly IConfiguration _configuration;
        public AWSSupportController(IConfiguration configuration)
        {
            _configuration = configuration;
            var accessKey = _configuration.GetValue<string>("AWS:AccessKey");
            var secretKey = _configuration.GetValue<string>("AWS:SecretKey");
            var credentials = new BasicAWSCredentials(accessKey, secretKey); // Sử dụng IAM Role
            var regionEndpoint = RegionEndpoint.USEast1; // Thay đổi region endpoint tại đây
            _supportClient = new AmazonAWSSupportClient(credentials, regionEndpoint);

        }
        [HttpGet("services")]
        public async Task<IActionResult> GetServices()
        {
            try
            {
                var describeServicesRequest = new Amazon.AWSSupport.Model.DescribeServicesRequest();
                var describeServicesResponse = await _supportClient.DescribeServicesAsync(describeServicesRequest);

                return Ok(describeServicesResponse.Services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
