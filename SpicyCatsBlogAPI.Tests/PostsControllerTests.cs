using Microsoft.AspNetCore.Mvc;
using SpicyCatsBlogAPI.Controllers;
using SpicyCatsBlogAPI.Data.FileManager;
using SpicyCatsBlogAPI.Data.Repository;
using SpicyCatsBlogAPI.Models.Content;
using SpicyCatsBlogAPI.Utils.GuidEncoder;
using System;

namespace SpicyCatsBlogAPI.Tests
{
    public class PostsControllerTests
    {
        private readonly PostsController _sut;
        private readonly Mock<IRepository> _repoMoq = new Mock<IRepository>();
        // only needed for Image() and CreatePost()
        private readonly Mock<IFileManager> _fileManagerMoq = new Mock<IFileManager>();
        private readonly Mock<IGuidEncoder> _guidEncoder = new Mock<IGuidEncoder>();

        public PostsControllerTests()
        {
            _sut = new PostsController(_repoMoq.Object, _fileManagerMoq.Object, _guidEncoder.Object);
        }

        [Fact]
        public async Task GetPostsAsync_ShouldReturnPostsWhenPostsExist()
        {
            // Arrange
            int pageIndex = 1;
            int pageSize = 10;

            _repoMoq.Setup(x => x.GetPostsAsync(pageIndex, pageSize, ""))
                .ReturnsAsync(GetFakePosts());

            SetUpGuidEncoderMock(_guidEncoder);

            // Act
            var result = (await _sut.GetAllPosts(1, 10));
            var actual = result.Value;

            // Assert
            Assert.Equal("1", actual.First().Id);

        }

        private static List<Post> GetFakePosts()
        {
            return new List<Post>
            {
                new Post
                {
                    Id = "1",
                    Title = "Test Post",
                    Body = "Test Post Body",
                    Category = PostCategory.Others,
                    Created = DateTime.Now,
                    Description = "",
                    Image = "",
                    Tags = "",
                    User = new Models.Auth.User()
                    {
                        Username = "s"
                    }

                }
            };
        }

        private void SetUpGuidEncoderMock(Mock<IGuidEncoder> _guidEncoder)
        {
            _guidEncoder.Setup(x => x.Encode(It.IsAny<String>())).Returns((string myval) => { return myval; });
            _guidEncoder.Setup(x => x.Decode2Str(It.IsAny<String>())).Returns((string myval) => { return myval; });
        }
    }
}
