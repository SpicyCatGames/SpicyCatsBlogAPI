using System.ComponentModel.DataAnnotations;

namespace SpicyCatsBlogAPI.Models.Content
{
    public class PostDto
    {
        public string Id { get; set; }

        [Required(ErrorMessage ="Post title cannot be empty")]
        public string Title { get; set; } = "";
        [Required(ErrorMessage ="Post body cannot be empty")]
        public string Body { get; set; } = "";

        // in Post, use either Image (to save image file) or ImageUrl(to save only external image link to db)
        // in get, imageUrl has the filename of image and Image is null
        public IFormFile Image { get; set; } = null;
        public string ImageUrl { get; set; } = "";

        public string Description { get; set; } = "";
        public string Tags { get; set; } = "";
        public string Category { get; set; } = "";

        public string Author { get; set; } = "";
        public string Created { get; set; } = "";
    }
}