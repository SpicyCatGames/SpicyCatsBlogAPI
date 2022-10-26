using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using SpicyCatsBlogAPI.Models.Error;

// https://learn.microsoft.com/en-us/answers/questions/620570/net-core-web-api-model-validation-error-response-t.html

namespace SpicyCatsBlogAPI.Utils.ActionFilters.Validation
{
    public class ValidationErrorResponse : IErrorResponse<ValidationErrorModel>
    {
        public List<ValidationErrorModel> Errors { get; } = new List<ValidationErrorModel>();

        public ValidationErrorResponse(ModelStateDictionary modelState)
        {
            Errors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => new ValidationErrorModel(x.ErrorMessage, key)))
                    .ToList();
        }
        public ValidationErrorResponse(string error)
        {
            Errors.Add(new ValidationErrorModel(error));
        }
    }

    public class ValidationErrorModel : IErrorResponseModel
    {
        public string Field { get; set; }
        public string Message { get; set; }
        public ValidationErrorModel(string error, string field = "")
        {
            Message = error;
            Field = field;
        }
    }
}
