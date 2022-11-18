using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using PhotoAlbumConsoleApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoAlbumConsoleAppTests
{
    [TestClass]
    public class PhotoAlbumAccessorTests
    {
        private PhotoAlbumAccessor photoAlbumAccessor;
        private Mock<HttpMessageHandler> mockHttpMessageHandler;

        [TestInitialize]
        public void Initialize()
        {
            mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var client = new HttpClient(mockHttpMessageHandler.Object);
            photoAlbumAccessor = new PhotoAlbumAccessor(client);
        }

        #region GetAlbumIdsAndTitlesAsync Tests
        [TestMethod]
        public async Task GetAlbumIdsAndTitlesAsync_HappyPathAsync()
        {
            var sampleList = new List<PhotoAlbum>
            {
                new PhotoAlbum
                {
                    Title = "test",
                    Id = 41
                },
                new PhotoAlbum
                {
                    Title = "test2",
                    Id = 42
                }
            };
            string consoleOutput = "";
            string expectedResponse = "[41] test" + Environment.NewLine + "[42] test2" + Environment.NewLine;
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(sampleList))});

            using (StringWriter stringWriter = new StringWriter())
            {
                Console.SetOut(stringWriter);
                await photoAlbumAccessor.GetAlbumIdsAndTitlesAsync(2);
                consoleOutput = stringWriter.ToString();
            }

            Assert.AreEqual(expectedResponse, consoleOutput);
        }

        [TestMethod]
        public async Task GetAlbumIdsAndTitlesAsync_EmptyList()
        {
            string consoleOutput = "";
            string expectedResponse = "No information found with given album number";
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("[]") });

            using (StringWriter stringWriter = new StringWriter())
            {
                Console.SetOut(stringWriter);
                await photoAlbumAccessor.GetAlbumIdsAndTitlesAsync(2);
                consoleOutput = stringWriter.ToString();
            }

            Assert.IsTrue(consoleOutput.Contains(expectedResponse));
        }

        [TestMethod]
        public async Task GetAlbumIdsAndTitlesAsync_BadResponse()
        {
            string consoleOutput = "";
            string expectedResponse = "There is an issue getting a response";
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest});

            using (StringWriter stringWriter = new StringWriter())
            {
                Console.SetOut(stringWriter);
                await photoAlbumAccessor.GetAlbumIdsAndTitlesAsync(2);
                consoleOutput = stringWriter.ToString();
            }

            Assert.IsTrue(consoleOutput.Contains(expectedResponse));
        }

        [TestMethod]
        public async Task GetAlbumIdsAndTitlesAsync_SerializationError()
        {
            string consoleOutput = "";
            string expectedResponse = "Exception occurred when deserializing response";
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = 
                new StringContent("[{\"albumId\",},]") });

            using (StringWriter stringWriter = new StringWriter())
            {
                Console.SetOut(stringWriter);
                await photoAlbumAccessor.GetAlbumIdsAndTitlesAsync(2);
                consoleOutput = stringWriter.ToString();
            }

            Assert.IsTrue(consoleOutput.Contains(expectedResponse));
        }
        #endregion

        #region ValidateInput Tests
        [TestMethod]
        public void ValidateInput_HappyPath()
        {
            var input = "photo-album 2";

            var result = PhotoAlbumAccessor.ValidateInput(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("photo-album 3 id")]
        [DataRow("photoalbum 2")]
        [DataRow("photo-album n9")]
        public void ValidateInput_InvalidInput(string input)
        {
            var expectedResponse = "Input is invalid. Please format input as: photo-album {integer number}, e.g. photo-album 100";
            string consoleOutput = "";

            using (StringWriter stringWriter = new StringWriter())
            {
                Console.SetOut(stringWriter);
                var result = PhotoAlbumAccessor.ValidateInput(input);
                Assert.IsNull(result);
                consoleOutput = stringWriter.ToString();
            }

            Assert.IsTrue(consoleOutput.Contains(expectedResponse));
        }
        #endregion
    }
}