using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices.JavaScript;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using WordCounter.Models;
using WordCounter.Contracts;

namespace WordCounter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WordCountController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IWordSearchService _wordSearchService;
        private readonly ILogger<WordCountController> _logger;
        private readonly IConfiguration _configuration;

        public WordCountController(IHttpClientFactory httpClientFactory, IWordSearchService wordSearchService, 
            ILogger<WordCountController> logger, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _wordSearchService = wordSearchService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("{topic}", Name = "GetWordCount")]
        public async Task<ActionResult<WordSearch>> GetWordCount(string topic)
        {
            string apiEndpoint = _configuration["WikipediaEndpoint"]+ topic;
            try
            {
                string articleText = await _wordSearchService.GetArticleText(apiEndpoint);

                int wordCount = _wordSearchService.CountWordOccurrences(articleText, topic);

                WordSearch wordSearch = new WordSearch() {
                    SearchTopic = topic,WordCount = wordCount
                };
                _logger.LogInformation($"Word count for topic {topic}: {wordCount}");

                return Ok(wordSearch);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while processing the request: {ex.Message}");
                return StatusCode(500, $"An error occurred : {ex.Message}");
            }
        }
        
    }
}
