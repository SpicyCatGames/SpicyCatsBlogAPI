using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

// https://learn.microsoft.com/en-us/answers/questions/620570/net-core-web-api-model-validation-error-response-t.html

namespace SpicyCatsBlogAPI.Utils.ActionFilters.Validation
{
    public class CustomErrorResultModel
    {
        public List<String> Errors { get; }
        public CustomErrorResultModel(ModelStateDictionary modelState)
        {
            Errors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => $"{key}: {x.ErrorMessage}"))
                    .ToList();
        }
    }
}
