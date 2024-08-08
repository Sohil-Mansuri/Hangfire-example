using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ScheduleJob
{
    public class APIKeyHeaderAttribute : ActionFilterAttribute
    {
        //get from config 
        private readonly string _apiKey = "TestAPIKey";
        private readonly string _apiKeySecret = "sohil";  
       
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(_apiKey, out var extractedApiKey) || extractedApiKey != _apiKeySecret)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            context.HttpContext.Response.Headers[_apiKey] = extractedApiKey;
            await next();
        }
    }
}
