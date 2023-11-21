using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using WordCounter.Contracts;

namespace WordCounter.Services
{
    public class WordSearchService: IWordSearchService
    {

        private readonly HttpClient _httpClient;

        private readonly ILogger<WordSearchService> _logger;
        public WordSearchService(IHttpClientFactory httpClientFactory, ILogger<WordSearchService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }
        public async Task<string> GetArticleText(string apiUrl)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();

                    JObject jsonResponse = JObject.Parse(jsonResult);

                    //handling the case when the searched topic is not present
                    if(jsonResponse != null && jsonResponse["parse"] == null) {
                        _logger.LogError($"Error fetching data from {apiUrl}. The requested Topic does not exist");
                        throw new HttpRequestException($"Error: The requested Topic does not exist");
                    }

                    string articleText = jsonResponse["parse"]["text"]["*"].ToString();

                    return articleText;
                }
                else
                {
                    _logger.LogError($"Error fetching data from {apiUrl}. Status code: {response.StatusCode}");
                    throw new HttpRequestException($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching article text: {ex.Message}");
                throw;
            }
        }

        public int CountWordOccurrences(string text, string topic)
        {
            try
            {
                _logger.LogInformation($"Counting word occurrences for topic {topic}");
                return Regex.Matches(text, $"\\b{topic}\\b", RegexOptions.IgnoreCase).Count;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while counting word occurrences: {ex.Message}");
                throw;
            }
        }
    }
}
