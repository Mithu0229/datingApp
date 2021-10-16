using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Net;
using API.Errors;
using System.Text.Json;
namespace API.Middleware
{
  public class ExceptionMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    
    private readonly IHostEnvironment _emv;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
     IHostEnvironment emv)
    {
      _next = next;
      _logger = logger;
      _emv = emv;

    }

    public async Task InvokeAsync(HttpContext context){
        try
        {
             await _next(context);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, ex.Message);
          context.Response.ContentType= "application/json";
          context.Response.StatusCode =(int) HttpStatusCode.InternalServerError;
          var response= _emv.IsDevelopment() ? new ApiException(context.Response.StatusCode,ex.Message, ex.StackTrace?.ToString()) 
          : new ApiException(context.Response.StatusCode,"Internal server error");

          var options=new JsonSerializerOptions{
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
          };

          var json=JsonSerializer.Serialize(response,options);
          await context.Response.WriteAsync(json);
        }
    }
  }
}