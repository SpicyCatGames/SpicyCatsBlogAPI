using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpicyCatsBlogAPI.Data.FileManager;
using SpicyCatsBlogAPI.Data.Repository;
using SpicyCatsBlogAPI.Models.Content;
using SpicyCatsBlogAPI.Utils.ActionFilters.Validation;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SpicyCatsBlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IRepository _repository;

        private readonly IFileManager _fileManager;

        public PostsController(IRepository repository, IFileManager fileManager)
        {
            _repository = repository;
            _fileManager = fileManager;
        }

        [HttpGet("getposts")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetAllPosts(int pageNum, int postsPerPage, string category = "")
        {
            var posts = await _repository.GetPostsAsync(pageNum, postsPerPage, category);
            return posts.ConvertAll((post) =>
            {
                return new PostDto
                {
                    Title = post.Title,
                    Description = post.Description,
                    Body = post.Body,
                    Category = post.Category.ToString(),
                    Created = post.Created.ToString(),
                    Id = post.Id,
                    ImageUrl = post.Image,
                    Tags = post.Tags,
                    Author = post.User.Username
                };
            });
        }

        [HttpGet("Post/{id}")]
        public async Task<ActionResult<PostDto>> GetPost(string id)
        {
            var post = await _repository.GetPost(id);

            PostDto postDto = new PostDto
            {
                Title = post.Title,
                Description = post.Description,
                Body = post.Body,
                Category = post.Category.ToString(),
                Created = post.Created.ToString(),
                Id = post.Id,
                ImageUrl = post.Image,
                Tags = post.Tags,
                Author = post.User.Username
            };
            return postDto;
        }

        [HttpGet("Image")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public IActionResult Image(string image)
        {
            try
            {
                var mime = image.Substring(image.LastIndexOf('.') + 1);
                return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
            }
            catch
            {
                try
                {
                    return new FileStreamResult(_fileManager.NotFoundImageStream(), "image/jpeg");
                }
                catch
                {
                    return null;
                }
            }
        }

        [HttpPost("createpost"), Authorize]
        [ValidateModel]
        [ProducesResponseType(typeof(ValidationErrorResponse), 400)]
        [ProducesResponseType(typeof(ValidationErrorResponse), 422)]
        public async Task<ActionResult> CreatePost([FromForm] PostDto postDto)
        {
            string bodyWithoutTags = Regex.Replace(postDto.Body, @"<([\w\-/]+)( +[\w\-]+(=(('[^']*')|(""[^""]*"")))?)* *>", string.Empty);
            if (bodyWithoutTags.Length == 0)
            {
                return BadRequest(new ValidationErrorResponse("Post body cannot be empty", nameof(postDto.Body).ToLower()));
            }

            var user = await _repository.GetUserWPostsAsync(User.FindFirstValue(ClaimTypes.Name));
            Enum.TryParse<PostCategory>(postDto.Category, out var categoryEnum);

            if (user.Posts == null)
            {
                user.Posts = new List<Post>();
            }

            var imageName = "";
            if (postDto.Image != null)
            {
                imageName = await _fileManager.SaveImage(postDto.Image);
                if (String.IsNullOrEmpty(imageName))
                    return BadRequest(new ValidationErrorResponse("Could not submit post: image saving failed"));
            }

            user.Posts.Add(new Post
            {
                Title = postDto.Title,
                Body = postDto.Body,
                Category = categoryEnum,
                Created = DateTime.UtcNow,
                Description = postDto.Description,
                Image = imageName,
                Tags = postDto.Tags
            });
            try
            {
                _repository.UpdateUser(user);
                await _repository.SaveChangesAsync();
            }
            catch
            {
                return BadRequest(new ValidationErrorResponse("Post could not be saved successfully"));
            }
            return Ok();
        }

        //private static IFormFile FileStreamToFormFile(FileStream fs)
        //{
        //    if (fs == null) return null;

        //    using (var stream = fs)
        //    {
        //        FormFile file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
        //        {
        //            Headers = new HeaderDictionary(),
        //            ContentType = "image/jpeg"
        //        };
        //        return file;
        //    }
        //}
    }
}