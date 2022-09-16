using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpicyCatsBlogAPI.Data;
using SpicyCatsBlogAPI.Models.Content;
using System.Security.Claims;

namespace SpicyCatsBlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IRepository _repository;

        public PostsController(IRepository repository)
        {
            _repository = repository;
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
                    Created = post.Created,
                    Id = post.Id,
                    Image = post.Image,
                    Tags = post.Tags
                };
            });
        }

        [HttpPost("createpost"), Authorize]
        public async Task<ActionResult> CreatePost(PostDto postDto)
        {
            var user = await _repository.GetUserWPostsAsync(User.FindFirstValue(ClaimTypes.Name));
            Enum.TryParse<PostCategory>(postDto.Category, out var categoryEnum);

            if (user.Posts == null)
            {
                user.Posts = new List<Post>();
            }

            user.Posts.Add(new Post
            {
                Title = postDto.Title,
                Body = postDto.Body,
                Category = categoryEnum,
                Created = DateTime.Now,
                Description = postDto.Description,
                Image = postDto.Image,
                Tags = postDto.Tags
            });
            try
            {
                _repository.UpdateUser(user);
                await _repository.SaveChangesAsync();
            }
            catch
            {
                return BadRequest("Post could not be saved successfully");
            }
            return Ok();
        }
    }
}