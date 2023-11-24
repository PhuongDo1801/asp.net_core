using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MyAspnetApi.Middleware;
using MyAspnetCore.Exceptions;
using MyAspnetCore.Interfaces.Infrastructure;
using MyAspnetCore.Interfaces.Services;
using MyAspnetCore.Services;
using MyAspnetInfrastructure.Repository;
using System.Net;

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
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

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

app.UseAuthorization();
app.UseMiddleware<ExceptionMiddlewares>();

app.MapControllers();

app.Run();
