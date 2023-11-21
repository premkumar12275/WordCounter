using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WordCounter.Controllers;
using Moq;
using WordCounter.Contracts;
using NuGet.Frameworks;
using Microsoft.AspNetCore.Mvc;
using WordCounter.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace WordCounterTest.Controllers
{
    [TestFixture]
    public class WordCounterControllerTests
    {
        private readonly int _countResult1=3;
        private readonly string _dummyText1 = "Dummy article text";        

        [Test]
        public async Task GetWordCount_ValidTopic_ReturnsOkResultWithWordSearch()
        {
            var wordSearchServiceMock = new Mock<IWordSearchService>();
            var httclientFactoryMock = new Mock<IHttpClientFactory>();
            var loggerMock = new Mock<ILogger<WordCountController>>();
            var configurationMock = new Mock<IConfiguration>();
            wordSearchServiceMock.Setup(s => s.GetArticleText(It.IsAny<string>())).ReturnsAsync(_dummyText1);
            wordSearchServiceMock.Setup(s => s.CountWordOccurrences(It.IsAny<string>(), It.IsAny<string>())).Returns(_countResult1);

            var controller = new WordCountController(httclientFactoryMock.Object,  wordSearchServiceMock.Object, loggerMock.Object, configurationMock.Object);
            // Act
            var result = await controller.GetWordCount("apple");

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOf<WordSearch>(okResult.Value);
            var wordSearch = (WordSearch)okResult.Value;
            Assert.AreEqual("apple", wordSearch.SearchTopic);
            Assert.AreEqual(_countResult1, wordSearch.WordCount);

            Assert.Pass();
        }

        [Test]
        public async Task GetWordCount_Exception_ReturnsInternalServerError()
        {
            // Arrange
            var wordSearchServiceMock = new Mock<IWordSearchService>();
            var httclientFactoryMock = new Mock<IHttpClientFactory>();
            var loggerMock = new Mock<ILogger<WordCountController>>();
            var configurationMock = new Mock<IConfiguration>();
            wordSearchServiceMock.Setup(s => s.GetArticleText(It.IsAny<string>())).Throws(new Exception("Some error"));

            var controller = new WordCountController(httclientFactoryMock.Object, wordSearchServiceMock.Object, loggerMock.Object, configurationMock.Object);

            // Act
            var result = await controller.GetWordCount("apple");

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            var statusCodeResult = (StatusCodeResult)result.Result;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }
    }
}
