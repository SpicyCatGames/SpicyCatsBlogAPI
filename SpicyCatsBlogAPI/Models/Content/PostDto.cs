namespace SpicyCatsBlogAPI.Models.Content
{
    public class PostDto
    {
        public string Id { get; set; }

        public string Title { get; set; } = "";
        public string Body { get; set; } = "";
        public IFormFile Image { get; set; } = null;
        public string ImageUrl { get; set; } = "";

        public string Description { get; set; } = "";
        public string Tags { get; set; } = "";
        public string Category { get; set; } = "";

        public string Author { get; set; } = "";
        public DateTime Created { get; set; } = DateTime.Now;
    }
}