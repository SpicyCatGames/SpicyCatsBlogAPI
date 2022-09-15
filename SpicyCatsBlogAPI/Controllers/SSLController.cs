using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpicyCatsBlogAPI.Models;
using SpicyCatsBlogAPI.Models.Auth;

namespace SpicyCatsBlogAPI.Controllers
{
    [Route(".well-known/pki-validation")]
    [ApiController]
    public class SSLController : ControllerBase
    {
        [HttpGet("A5971507AADCD9FB7FB9480A48DF07A8.txt")]
        public async Task<ActionResult> DownloadFile()
        {
            var filePath = $"./wwwroot/A5971507AADCD9FB7FB9480A48DF07A8.txt"; // Here, you should validate the request and the existance of the file.

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, "text/plain", Path.GetFileName(filePath));
        }
    }
}

//DEBF1C3ED4B589E3CE4671B97A915A79.txt