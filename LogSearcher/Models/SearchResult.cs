namespace LogSearcher.Models
{
    public class SearchResult
    {
        public string LogText { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;
        public long PositionInFile { get; set; }
    }
}
