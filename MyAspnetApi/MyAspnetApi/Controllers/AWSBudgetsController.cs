using Amazon;
using Amazon.Budgets;
using Amazon.Budgets.Model;
using Amazon.CostExplorer;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAspnetCore.DTO.Budget;
using MyAspnetCore.Interfaces.Services;
using MyAspnetCore.Services;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class AWSBudgetsController : ControllerBase
    {
        private IAmazonBudgets _budgetsClient;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private string _awsId;

        public AWSBudgetsController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
            Initialize().Wait();
        }
        private async Task Initialize()
        {
            var user = await _userService.GetUserInfo();;
            if (user != null)
            {
                _awsId = user.AwsId;
                _budgetsClient = InitializeAmazonBudgetsClient(user.AccessKey, user.SecretKey);
            }
        }
        private IAmazonBudgets InitializeAmazonBudgetsClient(string accessKey, string secretKey)
        {
            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                var credentials = new BasicAWSCredentials(accessKey, secretKey);
                var regionEndpoint = RegionEndpoint.USEast1; // Thay đổi region endpoint tại đây
                return new AmazonBudgetsClient(credentials, regionEndpoint);
            }

            return null;
        }
        private bool BudgetsClientIsNull()
        {
            return _budgetsClient == null;
        }

        [HttpPost("create-budget")]
        public async Task<IActionResult> CreateBudget([FromBody] BudgetCreateDto createBudgetRequest)
        {
            try
            {
                if (BudgetsClientIsNull())
                {
                    return BadRequest(new { ErrorMessage = "Không thể tạo budget vì thiếu thông tin key." });
                }
                var createBudgetResponse = await _budgetsClient.CreateBudgetAsync(new Amazon.Budgets.Model.CreateBudgetRequest
                {
                    AccountId = _awsId,
                    Budget = new Budget
                    {
                        BudgetName = createBudgetRequest.BudgetName,
                        BudgetLimit = new Spend { Amount = createBudgetRequest.BudgetLimit, Unit = "USD" },
                        TimeUnit = "MONTHLY",
                        TimePeriod = new TimePeriod
                        {
                            Start = createBudgetRequest.StartDate,
                            End = createBudgetRequest.EndDate
                        },
                        BudgetType = BudgetType.COST,
                        CostTypes = new CostTypes
                        {
                            UseBlended = true,
                            IncludeTax = true,
                        }
                    },
                    NotificationsWithSubscribers = new List<NotificationWithSubscribers>
                            {
                                new NotificationWithSubscribers
                                {
                                    Notification = new Notification
                                    {
                                        ComparisonOperator = "GREATER_THAN",
                                        NotificationType = "ACTUAL",
                                        Threshold = createBudgetRequest.limitPercent,
                                        ThresholdType = "PERCENTAGE"
                                    },
                                    Subscribers = new List<Subscriber>
                                    {
                                        new Subscriber
                                        {
                                            Address = createBudgetRequest.Email,
                                            SubscriptionType = "EMAIL"
                                        }
                                    }
                                }
                            }
                });

                return Ok(new { Message = "Tạo thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
        [HttpGet("get-budget/{budgetName}")]
        public async Task<IActionResult> GetBudget([FromRoute] string budgetName)
        {
            try
            {
                var describeBudgetResponse = await _budgetsClient.DescribeBudgetAsync(new Amazon.Budgets.Model.DescribeBudgetRequest
                {
                    AccountId = _awsId,
                    BudgetName = budgetName
                });
                var notificationsResponse = await _budgetsClient.DescribeNotificationsForBudgetAsync(new DescribeNotificationsForBudgetRequest
                {
                    AccountId = _awsId,
                    BudgetName = budgetName
                });

                var thresholdAmount = notificationsResponse.Notifications.FirstOrDefault()?.Threshold ?? 0;
                var budget = new BudgetDto
                {
                    BudgetName = describeBudgetResponse.Budget.BudgetName,
                    BudgetLimit = describeBudgetResponse.Budget.BudgetLimit.Amount,
                    ActualSpend = describeBudgetResponse.Budget.CalculatedSpend?.ActualSpend.Amount,
                    ForecastedSpend = describeBudgetResponse.Budget.CalculatedSpend?.ForecastedSpend.Amount,
                    StartTime = describeBudgetResponse.Budget.TimePeriod.Start,
                    EndTime = describeBudgetResponse.Budget.TimePeriod.End,
                    TimeUnit = describeBudgetResponse.Budget.TimeUnit.Value,
                    LastUpdatedTime = describeBudgetResponse.Budget.LastUpdatedTime,
                    Threshold = thresholdAmount,
                    // Thêm các thuộc tính khác nếu cần thiết
                };

                return Ok(budget);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpPut("update-budget/{budgetName}")]
        public async Task<IActionResult> UpdateBudget([FromBody] BudgetUpdateDto updateBudgetRequest, string budgetName)
        {
            try
            {
                var updateBudgetResponse = await _budgetsClient.UpdateBudgetAsync(new Amazon.Budgets.Model.UpdateBudgetRequest
                {
                    AccountId = _awsId,
                    NewBudget = new Budget
                    {
                        BudgetName = budgetName,
                        BudgetLimit = new Spend { Amount = updateBudgetRequest.BudgetLimit, Unit = "USD" },
                        TimeUnit = "MONTHLY",
                        TimePeriod = new TimePeriod
                        {
                            Start = updateBudgetRequest.StartDate,
                            End = updateBudgetRequest.EndDate
                        },
                        BudgetType = BudgetType.COST,
                        CostTypes = new CostTypes
                        {
                            UseBlended = true,
                            IncludeTax = true,
                        }
                    },
                });

                return Ok(new { Message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpDelete("delete-budget/{budgetName}")]
        public async Task<IActionResult> DeleteBudget([FromRoute] string budgetName)
        {
            try
            {
                await _budgetsClient.DeleteBudgetAsync(new Amazon.Budgets.Model.DeleteBudgetRequest
                {
                    AccountId = _awsId,
                    BudgetName = budgetName
                });

                return Ok(new { Message = "Xoá thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
        [HttpGet("get-budget-notifications")]
        public async Task<IActionResult> GetBudgetNotifications([FromQuery] string budgetName)
        {
            try
            {
                var budgetNotificationsResponse = await _budgetsClient.DescribeBudgetPerformanceHistoryAsync(new DescribeBudgetPerformanceHistoryRequest
                {
                    AccountId = _awsId,
                    BudgetName = budgetName
                });

                // Xử lý dữ liệu từ budgetNotificationsResponse và trả về kết quả cho client
                // Ví dụ: Trả về dữ liệu dưới dạng JSON
                return Ok(budgetNotificationsResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
        [HttpGet("get-budgets")]
        public async Task<IActionResult> GetBudgets()
        {
            try
            {
                var budgetsResponse = await _budgetsClient.DescribeBudgetsAsync(new DescribeBudgetsRequest
                {
                    AccountId = _awsId
                });
                var budgets = new List<object>();

                foreach (var budget in budgetsResponse.Budgets)
                {
                    var notificationsResponse = await _budgetsClient.DescribeNotificationsForBudgetAsync(new DescribeNotificationsForBudgetRequest
                    {
                        AccountId = _awsId,
                        BudgetName = budget.BudgetName
                    });

                    var thresholdAmount = notificationsResponse.Notifications.FirstOrDefault()?.Threshold ?? 0;

                    var budgetInfo = new BudgetDto
                    {
                        BudgetName = budget.BudgetName,
                        BudgetLimit = budget.BudgetLimit.Amount,
                        ActualSpend = budget.CalculatedSpend?.ActualSpend.Amount,
                        ForecastedSpend = budget.CalculatedSpend?.ForecastedSpend.Amount,
                        StartTime = budget.TimePeriod.Start,
                        EndTime = budget.TimePeriod.End,
                        TimeUnit = budget.TimeUnit.Value,
                        LastUpdatedTime = budget.LastUpdatedTime,
                        Threshold = thresholdAmount,
                    };

                    budgets.Add(budgetInfo);
                }
                return Ok(budgets);;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }


    }
}
