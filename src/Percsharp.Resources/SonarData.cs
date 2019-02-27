using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloom.Percsharp.Resources
{
    public static class SonarData
    {
        public static List<string[]> ReadSonarAll()
        {
            List<string[]> result = new List<string[]>();

            var path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "data/sonar.all-data"); 
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = false;

                while (!csvParser.EndOfData)
                {
                    result.Add(csvParser.ReadFields());
                }

                return result;
            }
        }
    }
}
