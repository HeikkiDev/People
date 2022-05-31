using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace People
{
    public class Program
    {
        public static readonly string PeoplePath = Path.Combine(Directory.GetCurrentDirectory(), "People");
        private const string TextFileExtension = ".txt";
        private const int NumberOfDifferentRandoms = 100;
        private const int NumberOfPeople = 2000000;
        private static readonly PeopleList[] LinesOfPeopleLists = new PeopleList[NumberOfDifferentRandoms];

        static void Main(string[] args)
        {
            SetUpLocalStorage();
            var twoMillionPeople = GeneratePeopleSet();
            SplitPeopleToSpecificFiles(twoMillionPeople);
            RemoveFilesUnderAverageFileSize();
        }
        
        public static void SetUpLocalStorage()
        {
            if (Directory.Exists(PeoplePath))
            {
                var directory = new DirectoryInfo(PeoplePath);
                foreach (var file in directory.GetFiles()) file.Delete();
                foreach (var subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
            }
            else
            {
                Directory.CreateDirectory(PeoplePath);
            }
            
            Parallel.For(0, NumberOfDifferentRandoms, i =>
            {
                var filePath = Path.Combine(PeoplePath, i + TextFileExtension);
                var file = File.Create(filePath);
                file.Close();
                LinesOfPeopleLists[i] = new PeopleList(filePath);
            });
        }

        public static void SplitPeopleToSpecificFiles(List<Person> twoMillionPeople)
        {
            for (int i = 0; i < NumberOfDifferentRandoms; i++)
            {
                var randomNumber = i;
                LinesOfPeopleLists[i].AddRange(twoMillionPeople.AsParallel().Where(x => x.RandomNumber == randomNumber)
                    .Select(person =>
                        $"{person.Id};{person.Name};{person.Surname};{person.DateOfBirth:yyyyMMddHHmmss}"));
            }

            twoMillionPeople.Clear();
            twoMillionPeople = null;
            
            Parallel.ForEach(LinesOfPeopleLists, (peopleList) => { peopleList.WriteNewSetToFile(); });
        }
        
        public static void RemoveFilesUnderAverageFileSize()
        {
            var fileSizes = new List<FileInfo>();
            for (int i = 0; i < NumberOfDifferentRandoms; i++)
            {
                var filePath = Path.Combine(PeoplePath, i + TextFileExtension);
                if (!File.Exists(filePath))
                {
                    continue;
                }
                fileSizes.Add(new FileInfo(filePath));
            }
            
            var averageFileSize = fileSizes.Select( x => x.Length).Average();
            
            foreach (var fileInfo in fileSizes.Where(x => x.Length < averageFileSize))
            {
                File.Delete(fileInfo.FullName);
            }
        }
        
        private static List<Person> GeneratePeopleSet()
        {
            var twoMillionPeople = new List<Person>(NumberOfPeople);
            var random = new Random();
            for (int i = 0; i < NumberOfPeople; i++)
            {
                twoMillionPeople.Add(new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "CoolName",
                    Surname = "CoolSurname",
                    DateOfBirth = DateTime.Now,
                    RandomNumber = random.Next(0, NumberOfDifferentRandoms)
                });
            }

            return twoMillionPeople;
        }
    }
}
