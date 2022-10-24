using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

// https://learn.microsoft.com/en-us/answers/questions/620570/net-core-web-api-model-validation-error-response-t.html

namespace SpicyCatsBlogAPI.Models
{
    public class APIErrorResult
    {
        public List<string> Errors { get; } = new List<string>();
        public APIErrorResult(ModelStateDictionary modelState)
        {
            Errors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => $"{key}: {x.ErrorMessage}"))
                    .ToList();
        }
        public APIErrorResult(string error)
        {
            Errors.Add(error);
        }
    }
}
