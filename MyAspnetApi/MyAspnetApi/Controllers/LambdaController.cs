using Amazon;
using Amazon.EC2;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAspnetCore.Interfaces.Services;
using Newtonsoft.Json.Linq;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class LambdaController : ControllerBase
    {
        private IAmazonLambda _lambdaclient;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public LambdaController(IConfiguration configuration, IUserService userService)
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
                _lambdaclient = InitializeClient(user.AccessKey, user.SecretKey);
            }
        }
        private IAmazonLambda InitializeClient(string accessKey, string secretKey)
        {
            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                var credentials = new BasicAWSCredentials(accessKey, secretKey);
                var regionEndpoint = RegionEndpoint.USEast1; // Thay đổi region endpoint tại đây
                return new AmazonLambdaClient(credentials, regionEndpoint);
            }

            return null;
        }
        private bool ClientIsNull()
        {
            return _lambdaclient == null;
        }
        [HttpGet("functions")]
        public async Task<IActionResult> ListLambdaFunctions()
        {
            //if (ClientIsNull())
            //{
            //    return BadRequest("Lambda client not initialized.");
            //}

            try
            {
                var listFunctionsResponse = await _lambdaclient.ListFunctionsAsync();
                var functions = listFunctionsResponse.Functions.Select(func => new
                {
                    FunctionName = func.FunctionName,
                    Runtime = func.Runtime,
                    Handler = func.Handler,
                    Role = func.Role,
                    CodeSize = func.CodeSize,
                    Description = func.Description,
                    Timeout = func.Timeout,
                    MemorySize = func.MemorySize,
                    LastModified = func.LastModified
                });

                return Ok(functions);
            }
            catch (AmazonLambdaException ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }
        [HttpGet("function/{functionName}")]
        public async Task<IActionResult> GetLambdaFunction(string functionName)
        {
            if (ClientIsNull())
            {
                return BadRequest("Lambda client not initialized.");
            }

            try
            {
                var getFunctionRequest = new GetFunctionRequest
                {
                    FunctionName = functionName
                };

                var getFunctionResponse = await _lambdaclient.GetFunctionAsync(getFunctionRequest);

                var functionInfo = new
                {
                    getFunctionResponse.Configuration.FunctionName,
                    getFunctionResponse.Configuration.Runtime,
                    getFunctionResponse.Configuration.Role,
                    getFunctionResponse.Configuration.Handler,
                    getFunctionResponse.Configuration.CodeSize,
                    getFunctionResponse.Configuration.Description,
                    getFunctionResponse.Configuration.Timeout,
                    getFunctionResponse.Configuration.MemorySize,
                    getFunctionResponse.Configuration.LastModified,
                    getFunctionResponse.Configuration.VpcConfig,
                    getFunctionResponse.Configuration.Environment,
                    getFunctionResponse.Configuration.TracingConfig
                    // Thêm các thuộc tính khác bạn cần
                };

                return Ok(functionInfo);
            }
            catch (AmazonLambdaException ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }


        [HttpPost("invoke")]
        public async Task<IActionResult> InvokeLambdaFunction(string functionName, [FromBody] JObject payload)
        {
            //if (ClientIsNull())
            //{
            //    return BadRequest("Lambda client not initialized.");
            //}

            try
            {
                var invokeRequest = new InvokeRequest
                {
                    FunctionName = functionName,
                    Payload = payload.ToString()
                };

                var invokeResponse = await _lambdaclient.InvokeAsync(invokeRequest);

                using (var sr = new StreamReader(invokeResponse.Payload))
                {
                    var responseBody = await sr.ReadToEndAsync();
                    return Ok(new
                    {
                        StatusCode = invokeResponse.StatusCode,
                        Response = responseBody
                    });
                }
            }
            catch (AmazonLambdaException ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateLambdaFunction(string functionName, string roleArn, string handler, string zipFilePath, string runtime = "dotnetcore3.1")
        {
            //if (ClientIsNull())
            //{
            //    return BadRequest("Lambda client not initialized.");
            //}

            try
            {
                var createFunctionRequest = new CreateFunctionRequest
                {
                    FunctionName = functionName,
                    Role = roleArn,
                    Handler = handler,
                    Runtime = runtime,
                    Code = new FunctionCode
                    {
                        ZipFile = new MemoryStream(System.IO.File.ReadAllBytes(zipFilePath))
                    }
                };

                var createFunctionResponse = await _lambdaclient.CreateFunctionAsync(createFunctionRequest);
                return Ok(createFunctionResponse);
            }
            catch (AmazonLambdaException ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteLambdaFunction(string functionName)
        {
            //if (ClientIsNull())
            //{
            //    return BadRequest("Lambda client not initialized.");
            //}

            try
            {
                var deleteFunctionRequest = new DeleteFunctionRequest
                {
                    FunctionName = functionName
                };

                var deleteFunctionResponse = await _lambdaclient.DeleteFunctionAsync(deleteFunctionRequest);
                return Ok($"Lambda function '{functionName}' deleted successfully.");
            }
            catch (AmazonLambdaException ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }
        
    }
}
