using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using People;
using Program = People.Program;

namespace PeopleTests
{
    [TestClass]
    public class PeopleTests
    {
        [TestMethod]
        public void NumberOfLinesInFilesShouldMatchNumberOfPeople()
        {
            int numberOfPeople = 100000;
            var twoMillionPeople = new List<Person>(numberOfPeople);
            
            // Generate 50.000 items for RandomNumber = 0, and 25.000 for each RandomNumber = 1 and 2
            for (int i = 0; i < numberOfPeople / 2; i++)
            {
                twoMillionPeople.Add(new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "CoolName",
                    Surname = "CoolSurname",
                    DateOfBirth = DateTime.Now,
                    RandomNumber = 0
                });
            }
            
            for (int i = 0; i < numberOfPeople / 4; i++)
            {
                twoMillionPeople.Add(new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "CoolName",
                    Surname = "CoolSurname",
                    DateOfBirth = DateTime.Now,
                    RandomNumber = 1
                });
                twoMillionPeople.Add(new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "CoolName",
                    Surname = "CoolSurname",
                    DateOfBirth = DateTime.Now,
                    RandomNumber = 2
                });
            }
            
            Program.SetUpLocalStorage();
            Program.SplitPeopleToSpecificFiles(twoMillionPeople);
            
            var directoryFiles = new DirectoryInfo(Program.PeoplePath);
            foreach (var fileInfo in directoryFiles.GetFiles())
            {
                if (fileInfo.Length == 0)
                {
                    fileInfo.Delete();
                }
            }
            
            int totalLines = 0;
            for (int i = 0; i < 3; i++)
            {
                int linesOnFile = File.ReadLines(Program.PeoplePath + "/" + i + ".txt").Count();
                totalLines += linesOnFile;
            }
            
            Assert.AreEqual(numberOfPeople, totalLines);
            
            Program.RemoveFilesUnderAverageFileSize();
            
            var numberOfFiles = directoryFiles.GetFiles().Length;
            
            Assert.AreEqual(1, numberOfFiles);

            var zeroFile = directoryFiles.GetFiles().First();
            Assert.AreEqual("0.txt", zeroFile.Name);
        }
    }
}