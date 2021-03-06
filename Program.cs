using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;

internal static class Program
{
    [Flags]
    private enum SearchModes
    {
        Files = 0b01,
        Directories = 0b10,
        Content = 0b11
    }
    
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        if (args.Length == 0 || args.Contains("--help"))
        {
            PrintHelp();
            
            return;
        }

        string entryPoint = Directory.GetCurrentDirectory();

        string regexQuery = args[0];
        SearchModes searchMode = SearchModes.Files | SearchModes.Directories;
        bool searchForHidden = false;

        if (args.Contains("--files") || args.Contains("-f"))
            searchMode = SearchModes.Files;

        if (args.Contains("--directories") || args.Contains("-d"))
            searchMode = SearchModes.Directories;

        if (args.Contains("--content") || args.Contains("-c"))
          searchMode = SearchModes.Content;

        if (args.Contains("--hidden") || args.Contains("-h"))
            searchForHidden = true;

        SearchAndPrintFiles(entryPoint, regexQuery, searchMode, searchForHidden);
    }

    private static void SearchAndPrintFiles(string directory, 
                                            string regexQuery, 
                                            SearchModes searchMode, 
                                            bool includeHidden = false)
    {
        SearchAndPrintFiles(directory, new Regex(regexQuery, RegexOptions.Compiled), searchMode, includeHidden);

        void SearchAndPrintFiles(string directory, 
                                 Regex regexQuery, 
                                 SearchModes searchMode, 
                                 bool includeHidden = false)
        {
            try
            {
                
                foreach (string filename in Directory.EnumerateFiles(directory))
                {
                    FileInfo fileInfo = new FileInfo(filename);

                    if ((fileInfo.Attributes.HasFlag(FileAttributes.Hidden) && !includeHidden))
                    {
                        continue;
                    }

                    if (searchMode.HasFlag(SearchModes.Files))
                    {
                        if (regexQuery.IsMatch(fileInfo.Name))
                        {
                            Console.WriteLine(filename);
                        }
                    }

                    if (searchMode.HasFlag(SearchModes.Content))
                    {
                        string[] fileLines = File.ReadAllLines(filename);

                        for (int lineIndex = 0; lineIndex < fileLines.Length; ++lineIndex)
                        {
                            foreach (Match regexMatch in regexQuery.Matches(fileLines[lineIndex]))
                            {
                              Console.WriteLine($"{filename}:{lineIndex} ({regexMatch.Index})");
                            }
                        }
                    }
                }

                Parallel.ForEach(Directory.EnumerateDirectories(directory), (subDirectory) => {
                    DirectoryInfo directoryInfo = new DirectoryInfo(subDirectory);

                    if ((directoryInfo.Attributes.HasFlag(FileAttributes.Hidden) && !includeHidden))
                    {
                        return;
                    }

                    if (searchMode.HasFlag(SearchModes.Directories) &&
                        regexQuery.IsMatch(directoryInfo.Name))
                    {
                        Console.WriteLine(subDirectory);
                    }

                    SearchAndPrintFiles(subDirectory, regexQuery, searchMode, includeHidden);
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                #if DEBUG
                Console.WriteLine(ex);
                #endif
            }
            catch (DirectoryNotFoundException ex)
            {
                #if DEBUG
                Console.WriteLine(ex);
                #endif
            }
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Usage: serafor REGEX-QUERY [OPTIONS]...");
        Console.WriteLine("Search files or directories using regex query");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine($"{"  -f, --files",       -25}Search for files only.");
        Console.WriteLine($"{"  -d, --directories", -25}Search for directories only.");
        Console.WriteLine($"{"  -c  --content",     -25}Search in files content only.");
        Console.WriteLine($"{"  -h, --hidden",      -25}Include hidden files and directories in the search.");
    }
}