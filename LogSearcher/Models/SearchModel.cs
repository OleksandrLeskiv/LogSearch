namespace LogSearcher.Models
{
    public class SearchModel
    {
        public string InputText { get; set; } = string.Empty;
        public Dictionary<string, List<SearchResult>?> Response = new Dictionary<string, List<SearchResult>?>();
    }
}
