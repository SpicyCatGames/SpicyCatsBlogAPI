using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

// https://learn.microsoft.com/en-us/answers/questions/620570/net-core-web-api-model-validation-error-response-t.html

namespace SpicyCatsBlogAPI.Utils.ActionFilters.Validation
{
    public class ValidationError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Code { get; set; }
        public string Message { get; }
        public ValidationError(string field, int code, string message)
        {
            Field = field != string.Empty ? field : null;
            Code = code != 0 ? code : 55;  //set the default code to 55. you can remove it or change it to 400.
            Message = message;
        }
    }
    public class ValidationResultModel
    {
        public string Message { get; }
        public List<ValidationError> Errors { get; }
        public ValidationResultModel(ModelStateDictionary modelState)
        {
            Message = "Validation Failed";
            Errors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, 0, x.ErrorMessage)))
                    .ToList();
        }
    }
}
