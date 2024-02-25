using Amazon.CostExplorer.Model;
using Amazon.CostExplorer;
using Amazon.Runtime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Amazon;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using MyAspnetCore.Entities;
using Microsoft.AspNetCore.Authorization;
using MyAspnetCore.Interfaces.Services;
using Amazon.Budgets;
using MyAspnetCore.Helper;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class AWSCostExplorerController : ControllerBase
    {
        private IAmazonCostExplorer _costExplorerClient;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AWSCostExplorerController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
            Initialize().Wait();
        }
        /// <summary>
        /// Khởi tạo thông tin của user
        /// </summary>
        /// <returns></returns>
        private async Task Initialize()
        {
            var user = await _userService.GetUserInfo(); ;
            if (user != null)
            {
                _costExplorerClient = InitializeClient(user.AccessKey, user.SecretKey);
            }
        }
        /// <summary>
        /// Khởi tạo credenttials để sử dụng API
        /// </summary>
        /// <param name="accessKey"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        private IAmazonCostExplorer InitializeClient(string accessKey, string secretKey)
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
            return _costExplorerClient == null;
        }
        /// <summary>
        /// Xem lịch sử phí sử dụng dịch vụ
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpPost("cost-explorer-data")]
        public async Task<IActionResult> GetCostExplorerData([FromQuery(Name = "startDate")] DateTime startDate,
                                                                [FromQuery(Name = "endDate")] DateTime endDate)
        {
            try
            {
                if (ClientIsNull())
                {
                    return BadRequest(new { ErrorMessage = "Không thể tạo budget vì thiếu thông tin key." });
                }
                var request = new GetCostAndUsageRequest
                {
                    TimePeriod = new DateInterval
                    {
                        Start = startDate.ToString("yyyy-MM-dd"),
                        End = endDate.ToString("yyyy-MM-dd"),
                    },
                    Granularity = Granularity.MONTHLY,
                    //Filter = new Expression
                    //{
                    //    Dimensions = new DimensionValues
                    //    {
                    //        Key = "SERVICE",
                    //        Values = new List<string> { "AWS Cost Explorer" }
                    //    }
                    //},
                    Metrics = new List<string> { "BlendedCost" },
                    GroupBy = new List<GroupDefinition>
                        {
                            new GroupDefinition
                            {
                                Key = "SERVICE",
                                Type = GroupDefinitionType.DIMENSION,
                            },
                        },
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
        /// <summary>
        /// Lấy danh sách các dịch vụ người dùng đang sử dụng
        /// </summary>
        /// <returns></returns>
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
                    Metrics = new List<string> { "BlendedCost", "UnblendedCost" },
                    GroupBy = new List<GroupDefinition>
                        {
                            new GroupDefinition
                            {
                                Key = "SERVICE",
                                Type = GroupDefinitionType.DIMENSION,
                            },
                        },
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
                        var serviceName = keys;

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
        /// <summary>
        /// Lấy chi phí hiện tại tháng này
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-monthly-cost")]
        public async Task<IActionResult> GetMonthlyCost()
        {
            try
            {
                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var request = new Amazon.CostExplorer.Model.GetCostAndUsageRequest
                {
                    TimePeriod = new Amazon.CostExplorer.Model.DateInterval
                    {
                        Start = startDate.ToString("yyyy-MM-dd"),
                        End = endDate.ToString("yyyy-MM-dd")
                    },
                    Granularity = Granularity.MONTHLY,
                    Metrics = new List<string> { "BlendedCost"},
                    GroupBy = new List<GroupDefinition>
                        {
                            new GroupDefinition
                            {
                                Key = "SERVICE",
                                Type = GroupDefinitionType.DIMENSION,
                            },
                        },
                    // Thêm các Metrics khác tùy thuộc vào nhu cầu của bạn
                };

                var response = await _costExplorerClient.GetCostAndUsageAsync(request);

                return Ok(response.ResultsByTime);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpGet("get-monthly-cost-and-service")]
        public async Task<IActionResult> GetMonthlyCostAndService()
        {
            try
            {
                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var request = new Amazon.CostExplorer.Model.GetCostAndUsageRequest
                {
                    TimePeriod = new Amazon.CostExplorer.Model.DateInterval
                    {
                        Start = startDate.ToString("yyyy-MM-dd"),
                        End = endDate.ToString("yyyy-MM-dd")
                    },
                    Granularity = Granularity.MONTHLY,
                    Metrics = new List<string> { "BlendedCost" },
                    GroupBy = new List<GroupDefinition>
                        {
                            new GroupDefinition
                            {
                                Key = "SERVICE",
                                Type = GroupDefinitionType.DIMENSION,
                            },
                        },
                };

                var response = await _costExplorerClient.GetCostAndUsageAsync(request);

                // Xử lý dữ liệu trả về
                var result = response.ResultsByTime.FirstOrDefault();
                if (result != null)
                {
                    var groups = result.Groups;

                    // Tạo danh sách chi phí
                    var costList = new List<CostInfo>();

                    foreach (var group in groups)
                    {
                        var serviceName = group.Keys.FirstOrDefault();
                        var blendedCost = group.Metrics["BlendedCost"].Amount;

                        var costInfo = new CostInfo
                        {
                            ServiceName = serviceName,
                            BlendedCost = blendedCost
                        };

                        costList.Add(costInfo);
                    }

                    return Ok(costList);
                }
                else
                {
                    return BadRequest("Không có dữ liệu");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        /// <summary>
        /// Phí sử dụng 6 tháng gần nhất
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-last-six-months-cost")]
        public async Task<IActionResult> GetLastSixMonthsCost()
        {
            try
            {
                var today = DateTime.Today;
                var startDate = today.AddMonths(-5);
                startDate = new DateTime(startDate.Year, startDate.Month, 1);
                var endDate = today;

                var request = new Amazon.CostExplorer.Model.GetCostAndUsageRequest
                {
                    TimePeriod = new Amazon.CostExplorer.Model.DateInterval
                    {
                        Start = startDate.ToString("yyyy-MM-dd"),
                        End = endDate.ToString("yyyy-MM-dd")
                    },
                    Granularity = Granularity.MONTHLY,
                    Metrics = new List<string> { "BlendedCost" },
                    GroupBy = new List<GroupDefinition>
                        {
                            new GroupDefinition
                            {
                                Key = "SERVICE",
                                Type = GroupDefinitionType.DIMENSION,
                            },
                        },
                    // Thêm các Metrics khác tùy thuộc vào nhu cầu của bạn
                };

                var response = await _costExplorerClient.GetCostAndUsageAsync(request);

                var relevantData = response.ResultsByTime
                .Select(result => new
                {
                    TimePeriod = result.TimePeriod,
                    TotalCost = result.Groups.Sum(group =>
                        Convert.ToDouble(group.Metrics["BlendedCost"].Amount))
                })
                .ToList();

                return Ok(relevantData);

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("history-cost-data")]
        public async Task<IActionResult> GetHistoryCostData([FromQuery(Name = "startDate")] DateTime startDate,
                                                                [FromQuery(Name = "endDate")] DateTime endDate)
        {
            try
            {
                var request = new GetCostAndUsageRequest
                {
                    TimePeriod = new DateInterval
                    {
                        Start = startDate.ToString("yyyy-MM-dd"),
                        End = endDate.ToString("yyyy-MM-dd"),
                    },
                    Granularity = Granularity.MONTHLY,
                    //Filter = new Expression
                    //{
                    //    Dimensions = new DimensionValues
                    //    {
                    //        Key = "SERVICE",
                    //        Values = new List<string> { "AWS Cost Explorer" }
                    //    }
                    //},
                    Metrics = new List<string> { "BlendedCost" },
                    GroupBy = new List<GroupDefinition>
                        {
                            new GroupDefinition
                            {
                                Key = "SERVICE",
                                Type = GroupDefinitionType.DIMENSION,
                            },
                        },
                };

                var response = await _costExplorerClient.GetCostAndUsageAsync(request);

                // Xử lý dữ liệu response ở đây...
                // Ví dụ: Trả về dữ liệu dưới dạng JSON
                var relevantData = response.ResultsByTime
                .Select(result => new
                {
                    TimePeriod = result.TimePeriod,
                    TotalCost = result.Groups.Sum(group =>
                        Convert.ToDouble(group.Metrics["BlendedCost"].Amount))
                })
                .ToList();

                return Ok(relevantData);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về mã lỗi
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpGet("get-last-six-months-cost-and-service")]
        public async Task<IActionResult> GetLastSixMonthsCostAndService()
        {
            try
            {
                var today = DateTime.Today;
                var startDate = today.AddMonths(-5);
                startDate = new DateTime(startDate.Year, startDate.Month, 1);
                var endDate = today;

                var request = new Amazon.CostExplorer.Model.GetCostAndUsageRequest
                {
                    TimePeriod = new Amazon.CostExplorer.Model.DateInterval
                    {
                        Start = startDate.ToString("yyyy-MM-dd"),
                        End = endDate.ToString("yyyy-MM-dd")
                    },
                    Granularity = Granularity.MONTHLY,
                    Metrics = new List<string> { "BlendedCost" },
                    GroupBy = new List<GroupDefinition>
                        {
                            new GroupDefinition
                            {
                                Key = "SERVICE",
                                Type = GroupDefinitionType.DIMENSION,
                            },
                        },
                    // Thêm các Metrics khác tùy thuộc vào nhu cầu của bạn
                };

                var response = await _costExplorerClient.GetCostAndUsageAsync(request);


                var resultData = response.ResultsByTime.Select(result => new
                {
                    TimePeriod = result.TimePeriod,
                    TotalCost = result.Groups.Sum(group =>
                        Convert.ToDouble(group.Metrics["BlendedCost"].Amount)),
                    Services = result.Groups.Select(group => new
                    {
                        ServiceName = group.Keys.FirstOrDefault(),
                        Cost = group.Metrics["BlendedCost"].Amount
                    })
                }).ToList();

                return Ok(resultData);

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy kết quả dự đoán chi phí tháng này
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("GetForecastedMonthEndCosts")]
        public async Task<IActionResult> GetForecastedMonthEndCosts()
        {
            try
            {
                var today = DateTime.Today;

                // Ngày bắt đầu là ngày hiện tại
                var startDate = today;

                // Ngày kết thúc là ngày cuối cùng của tháng
                var lastDayOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                var endDate = lastDayOfMonth;
                var request = new GetCostForecastRequest
                {
                    TimePeriod = new DateInterval
                    {
                        Start = startDate.ToString("yyyy-MM-dd"),
                        End = endDate.ToString("yyyy-MM-dd"),
                    },
                    Granularity = Granularity.MONTHLY,
                    Metric = "BLENDED_COST", // Hoặc "UNBLENDED_COST" hoặc "AMORTIZED_COST" tùy thuộc vào yêu cầu của bạn
                    PredictionIntervalLevel = 80 // Độ tin cậy của dự đoán
                };

                var response = await _costExplorerClient.GetCostForecastAsync(request);

                // Lấy thông tin về dự đoán chi phí
                var forecastedCosts = response.ForecastResultsByTime;

                return Ok(forecastedCosts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("GetForecastedCosts")]
        public async Task<IActionResult> GetForecastedCosts(DateTime startDate, DateTime endDate)
        {
            try
            {

                var request = new GetCostForecastRequest
                {
                    TimePeriod = new DateInterval
                    {
                        Start = startDate.ToString("yyyy-MM-dd"),
                        End = endDate.ToString("yyyy-MM-dd"),
                    },
                    Granularity = Granularity.MONTHLY,
                    Metric = "BLENDED_COST", // Hoặc "UNBLENDED_COST" hoặc "AMORTIZED_COST" tùy thuộc vào yêu cầu của bạn
                    PredictionIntervalLevel = 80, // Độ tin cậy của dự đoán
                    //Filter = new Expression
                    //{
                    //    Dimensions = new DimensionValues
                    //    {
                    //        Key = "SERVICE",
                    //        Values = new List<string> { "AWS Cost Explorer" }
                    //    }
                    //},
                };

                var response = await _costExplorerClient.GetCostForecastAsync(request);

                // Lấy thông tin về dự đoán chi phí
                var forecastedCosts = response.ForecastResultsByTime;

                return Ok(forecastedCosts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
