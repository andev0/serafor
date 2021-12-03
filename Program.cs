using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

if (args.Length == 0)
{
    Console.WriteLine("Usage: serafor REGEX_QUERY");
    
    return;
}

string entryPoint = Directory.GetCurrentDirectory();
string searchQuery = searchQuery = args[0];

SearchAndPrintFiles(entryPoint, searchQuery);

static void SearchAndPrintFiles(string directory, string searchQuery)
{
    Regex regex = new Regex(searchQuery, RegexOptions.Compiled);

    try
    {
        foreach (string filename in Directory.EnumerateFiles(directory))
        {
            if (regex.IsMatch(Path.GetFileName(filename)))
            {
                Console.WriteLine(filename);
            }
        }

        Parallel.ForEach(Directory.EnumerateDirectories(directory), (subDirectory) => 
            SearchAndPrintFiles(subDirectory, searchQuery)
        );
    }
    catch (UnauthorizedAccessException) { }
    catch (DirectoryNotFoundException) { }
}