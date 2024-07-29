using LogSearcher.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace LogSearcher.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private const string _logsLocation = "Files";
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            SearchModel searchModel = new SearchModel();
            return View("Index", searchModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Find(SearchModel model)
        {
            if (ModelState.IsValid)
            {
                Dictionary<string, List<SearchResult>?> result = new Dictionary<string, List<SearchResult>?>();
                try
                {
                    string[] files = Directory.GetFiles(_logsLocation, "*.log");

                    foreach (string file in files)
                    {
                        result[file] = ProcessSearch(file, model.InputText);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                model.Response = result;
                return View("Index", model);
            }

            return BadRequest();
        }

        private static List<SearchResult>? ProcessSearch(string filePath, string searchPattern)
        {
            string regexPattern = ConvertWildcardToRegex(searchPattern);

            List<SearchResult> results = new List<SearchResult>();

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (Regex.IsMatch(line, regexPattern, RegexOptions.IgnoreCase))
                    {
                        results.Add(new SearchResult { LogText = line, Pattern = regexPattern, PositionInFile = fs.Position });
                    }
                }
            }

            return results;
        }

        private static string ConvertWildcardToRegex(string pattern)
        {
            string escapedPattern = Regex.Escape(pattern);

            string regexPattern = escapedPattern
                .Replace(@"\*", ".*")
                .Replace(@"\?", ".")
                .Replace(@"\(", "(")
                .Replace(@"\)", ")")
                .Replace(@"\ and \", ".*")
                .Replace(@"\ or\", "|");

            return regexPattern;
        }
    }
}
