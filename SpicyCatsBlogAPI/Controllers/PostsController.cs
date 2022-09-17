﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SpicyCatsBlogAPI.Data.FileManager;
using SpicyCatsBlogAPI.Data.Repository;
using SpicyCatsBlogAPI.Models.Content;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

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
                    Created = post.Created,
                    Id = post.Id,
                    ImageUrl = post.Image,
                    Tags = post.Tags,
                    Author = post.User.Username
                };
            });
        }

        [HttpGet("Image/{image}")]
        public IActionResult Image(string image)
        {
            var mime = image.Substring(image.LastIndexOf('.') + 1);
            try
            {
                return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
            }
            catch
            {
                // TODO send an imagefound.jpeg
                return null;
            }
        }

        [HttpPost("createpost"), Authorize]
        public async Task<ActionResult> CreatePost([FromForm] PostDto postDto)
        {
            var user = await _repository.GetUserWPostsAsync(User.FindFirstValue(ClaimTypes.Name));
            Enum.TryParse<PostCategory>(postDto.Category, out var categoryEnum);

            if (user.Posts == null)
            {
                user.Posts = new List<Post>();
            }

            var imageName = await _fileManager.SaveImage(postDto.Image);
            if (String.IsNullOrEmpty(imageName))
                return BadRequest("Could not submit post: image saving failed");

            user.Posts.Add(new Post
            {
                Title = postDto.Title,
                Body = postDto.Body,
                Category = categoryEnum,
                Created = DateTime.Now,
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
                return BadRequest("Post could not be saved successfully");
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