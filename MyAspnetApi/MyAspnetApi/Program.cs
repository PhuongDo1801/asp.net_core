using Amazon.BCMDataExports;
using Amazon.Budgets;
using Amazon.CostAndUsageReport;
using Amazon.CostExplorer;
using Amazon.Pricing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyAspnetApi.Middleware;
using MyAspnetCore.Exceptions;
using MyAspnetCore.Interfaces.Infrastructure;
using MyAspnetCore.Interfaces.Services;
using MyAspnetCore.Services;
using MyAspnetInfrastructure.Repository;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = actionContext =>
        {
            var modelState = actionContext.ModelState;
            var keys = actionContext.ModelState.Keys;

            var dictionary = modelState.Keys.ToDictionary(
                key => key,
                key => modelState[key].Errors.Select(error => error.ErrorMessage).ToList()
            );


            return new BadRequestObjectResult(new BaseException
            {
                ErrCode = (int)HttpStatusCode.BadRequest,
                DevMsg = ReasonPhrases.GetReasonPhrase((int)HttpStatusCode.BadRequest),
                UserMsg = ReasonPhrases.GetReasonPhrase((int)HttpStatusCode.BadRequest),
                TraceId = "",
                MoreInfo = "",
                ErrorMsgs = dictionary
            });
        };
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<AmazonCostExplorerClient>();
builder.Services.AddAWSService<AmazonBCMDataExportsClient>();
builder.Services.AddAWSService<AmazonBudgetsClient>();
builder.Services.AddAWSService<AmazonCostAndUsageReportClient>();
builder.Services.AddAWSService<AmazonPricingClient>();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
builder.Services.AddScoped<IProviderService, ProviderService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowedOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:8080") // note the port is included 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
var app = builder.Build();
app.UseCors("MyAllowedOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddlewares>();

app.MapControllers();

app.Run();
