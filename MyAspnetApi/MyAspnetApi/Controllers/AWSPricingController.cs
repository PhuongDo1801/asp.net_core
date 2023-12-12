using Amazon.Pricing.Model;
using Amazon.Pricing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Amazon.Runtime;
using Amazon;
using Newtonsoft.Json.Linq;
//using Newtonsoft.Json.Linq;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AWSPricingController : ControllerBase
    {
        private readonly IAmazonPricing _pricingClient;
        private readonly IConfiguration _configuration;

        public AWSPricingController(IConfiguration configuration)
        {
            _configuration = configuration;
            var accessKey = _configuration.GetValue<string>("AWS:AccessKey");
            var secretKey = _configuration.GetValue<string>("AWS:SecretKey");
            var credentials = new BasicAWSCredentials(accessKey, secretKey); // Sử dụng IAM Role
            var regionEndpoint = RegionEndpoint.USEast1; // Thay đổi region endpoint tại đây
            _pricingClient = new AmazonPricingClient(credentials, regionEndpoint);

        }
        /// <summary>
        /// Lấy thông tin về giá của instance Ec2
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="vCpu"></param>
        /// <param name="memory"></param>
        /// <returns></returns>
        [HttpGet("EC2forecast")]
        public async Task<IActionResult> GetEC2PriceForecast([FromQuery] string instanceType, string vCpu, double memory)
        {
            try
            {

                var ec2Request = new GetProductsRequest
                {
                    Filters = new List<Filter>
                    {
                        new Filter
                        {
                            Type = "TERM_MATCH",
                            Field = "ServiceCode",
                            Value = "AmazonEC2"
                        },
                        new Filter
                        {
                            Type = "TERM_MATCH",
                            Field = "instanceType",
                            Value = instanceType // Thay đổi instance type tại đây
                        },
                        new Filter
                        {
                            Type = "TERM_MATCH",
                            Field = "memory",
                            Value = $"{memory} GiB" // Thay đổi dung lượng bộ nhớ tại đây
                        },
                        new Filter
                        {
                            Type = "TERM_MATCH",
                            Field = "vcpu",
                            Value = vCpu // Thay đổi số lượng vCPU tại đây
                        },
                        //new Filter
                        //{
                        //    Type = "TERM_MATCH",
                        //    Field = "location",
                        //    Value = "Asia Pacific (Ho Chi Minh)"
                        //},
                        //new Filter
                        //    {
                        //        Field = "priceListId",
                        //        Value = priceListArn
                        //    }
                    },
                    FormatVersion = "aws_v1",
                    NextToken = null,
                    MaxResults = 1,
                    ServiceCode = "AmazonEC2"
                };

                var ec2Response = await _pricingClient.GetProductsAsync(ec2Request);

                // Xử lý thông tin giá cả ở đây
                if (ec2Response.PriceList.Count > 0)
                {
                    var priceList = ec2Response.PriceList[0];
                    var priceListJson = JToken.Parse(priceList);

                    var terms = priceListJson["terms"]?["OnDemand"];
                    var termValues = terms?.Values().FirstOrDefault();

                    var priceDimensions = termValues?["priceDimensions"];
                    var pricePerUnit = priceDimensions?.Values().FirstOrDefault()?["pricePerUnit"]?["USD"]?.Value<string>();

                    if (pricePerUnit != null)
                    {
                        // Tính giá theo đơn vị GB-month
                        var pricePerGBMonth = double.Parse(pricePerUnit);

                        // Xử lý dữ liệu khác nếu cần thiết
                        var description = priceDimensions?.Values().FirstOrDefault()?["description"]?.Value<string>();


                        return Ok(new { Description = description, PricePerGBMonth = pricePerGBMonth });
                    }
                    else
                    {
                        return NotFound(new { ErrorMessage = "Không tìm thấy thông tin giá cả cho instance type đã cho." });
                    }

                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về mã lỗi
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
            return NotFound(new { ErrorMessage = "Không tìm thấy thông tin giá cả cho instance type đã cho." });
        }
        /// <summary>
        /// Lấy list giá của dịch vụ AWS theo serviceCode
        /// </summary>
        /// <returns></returns>
        [HttpGet("getListprice")]
        public async Task<IActionResult> GetListPriceLists()
        {
            try
            {
                var request = new ListPriceListsRequest
                {
                    ServiceCode = "AmazonS3",
                    CurrencyCode = "USD", // Thay thế bằng mã tiền tệ mong muốn
                    EffectiveDate = DateTime.UtcNow,
                    RegionCode = "us-east-1",
                };

                var response = await _pricingClient.ListPriceListsAsync(request);

                // Lấy toàn bộ thông tin giá của EC2 instances
                var ec2PriceList = response.PriceLists;

                return Ok(ec2PriceList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
