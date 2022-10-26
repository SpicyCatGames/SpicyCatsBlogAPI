using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpicyCatsBlogAPI.Models.Error
{
    public interface IErrorResponse<T> where T : IErrorResponseModel
    {
        public List<T> Errors { get; }
    }
    public interface IErrorResponseModel
    {
        public string Message { get; set; }
    }
}
