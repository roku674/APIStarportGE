
//Created by Alexander Fields 
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Optimization.Objects;
using Optimization.Encryption;

namespace APIStarportGE.Filters
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method)]
    public class APIKeyAttribute : System.Attribute, IAsyncActionFilter
    {
        private const string apiKeyHeaderName = "APIStarportGEKey";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Microsoft.Extensions.Primitives.StringValues potentialKey = "";

            if (!context.HttpContext.Request.Headers.TryGetValue(apiKeyHeaderName, out potentialKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            string decryptedKey = GetKey();
            potentialKey = AES.DecryptString(Settings.Configuration["key"], potentialKey);

            if (!decryptedKey.Equals(potentialKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }

        public string GetKey()
        {
            string key = Settings.Configuration["APIKey"];
            string decryptedKey = AES.DecryptString(Settings.Configuration["key"], Settings.Configuration["APIKey"]);
            return decryptedKey;
        }
    }
}
