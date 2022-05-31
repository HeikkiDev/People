using System.Collections.Generic;
using System.IO;

namespace People
{
    public class PeopleList : List<string>
    {
        private readonly string _filePath;

        public PeopleList(string filePath)
        {
            _filePath = filePath;
        }
        
        public void WriteNewSetToFile()
        {
            using var fileStream = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            using var streamWriter = new StreamWriter(fileStream);
            ForEach(newLine => streamWriter.WriteLine(newLine));
            Clear();
            streamWriter.Close();
            fileStream.Close();
        }
    }
}