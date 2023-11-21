namespace WordCounter.Contracts
{
    public interface IWordSearchService
    {
        Task<string> GetArticleText(string urlWithTopic);

        int CountWordOccurrences(string text, string topic);
    }
}
