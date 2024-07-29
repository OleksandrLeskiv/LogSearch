using System.Text.RegularExpressions;

namespace LogParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Incorrect application using. First argument path to files folder, second one is wildcard");
                return;
            }

            string directoryPath = args[0];
            string searchPattern = args[1];

            try
            {
                string[] files = Directory.GetFiles(directoryPath, "*.log");

                foreach (string file in files)
                {
                    SearchFile(file, searchPattern);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void SearchFile(string filePath, string searchPattern)
        {
            string regexPattern = ConvertWildcardToRegex(searchPattern);

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (Regex.IsMatch(line, regexPattern, RegexOptions.IgnoreCase))
                    {
                        Console.WriteLine($"{filePath}: {line}");
                    }
                }
            }
        }

        static string ConvertWildcardToRegex(string pattern)
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
