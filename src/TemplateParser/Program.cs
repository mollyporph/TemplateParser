using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TemplateParser
{
    public class Program
    {
        static string tagStart = "<#REFERENCE=";
        static string tagEnd = "#>";
        static Regex regex = new Regex($@"(?:{tagStart}((?:[\w]:|\\)?(?:(?:\\|\/)?[a-z_\-\s0-9\.]+)+(?:\.[\w\d_-]+)?){tagEnd})", RegexOptions.Compiled);
        public static void Main(string[] args)
        {
            string inputFile, outputFile;
          
            if (args.Length <1 || args.Length >2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Wrong amount of parameters. Accepted parameters are -InputFile <filepath> [required] and -OutputFile <filepath> [optional]");
                Console.ResetColor();
                return;
            }
            inputFile = args[0];
            outputFile = $"output{Path.GetExtension(inputFile)}";
            if(args.Length == 2)
            {
                outputFile = args[1];
            }
            try
            {
                var strg = ResolveReferences(File.ReadAllText(inputFile),"");
                File.WriteAllText(outputFile, strg, Encoding.UTF8);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Successfully parsed file, output available in {outputFile}");
                Console.ResetColor();
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Exception occured : {e.Message}");
                Console.ResetColor();
            }

        }
        public static string ResolveReferences(string fileString, string currentPath)
        {
            var sb = new StringBuilder();
            sb.Append(fileString);
            foreach(Match match in regex.Matches(fileString))
            {
                var matchString = match.Groups[0].Value;
                var fileRelativePath = match.Groups[1].Value;
                var filePath = $"{currentPath}/{fileRelativePath}".TrimStart('/');
                var files = File.GetAttributes(filePath).HasFlag(FileAttributes.Directory) ? Directory.GetFiles(filePath) : Directory.GetFiles(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
                if (files.Length > 1) {
                    var sbInner = new StringBuilder();
                    foreach (var file in files)
                    {
                        sbInner.Append(ResolveReferences(File.ReadAllText(file), Path.GetDirectoryName(filePath)));
                        sbInner.Append(",");
                        sbInner.Append(Environment.NewLine);
                    }
                    var res = sbInner.ToString().TrimEnd(',');
                    sb.Replace(matchString,res);
                }
                else
                {
                    sb.Replace(matchString, ResolveReferences(File.ReadAllText(filePath), Path.GetDirectoryName(filePath)));
                }
            }
            return sb.ToString();
        }
    }
}
