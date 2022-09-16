using System.ComponentModel.DataAnnotations;

namespace SpicyCatsBlogAPI.Models.Content
{
    public class Post
    {
        [Key]
        public string Id { get; set; }

        public string Title { get; set; } = "";
        public string Body { get; set; } = "";
        public string Image { get; set; } = "";

        public string Description { get; set; } = "";
        public string Tags { get; set; } = "";
        public string Category { get; set; } = "";

        public DateTime Created { get; set; } = DateTime.Now;
    }
}