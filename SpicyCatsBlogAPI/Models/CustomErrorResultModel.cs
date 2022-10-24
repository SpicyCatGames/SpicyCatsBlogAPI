using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

// https://learn.microsoft.com/en-us/answers/questions/620570/net-core-web-api-model-validation-error-response-t.html

namespace SpicyCatsBlogAPI.Models
{
    public class CustomErrorResult
    {
        public List<string> Errors { get; }
        public CustomErrorResult(ModelStateDictionary modelState)
        {
            Errors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => $"{key}: {x.ErrorMessage}"))
                    .ToList();
        }
        public CustomErrorResult(string error)
        {
            Errors.Add(error);
        }
    }
}
