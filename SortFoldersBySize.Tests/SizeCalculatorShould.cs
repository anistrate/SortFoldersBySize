using SortFoldersBySize.Models;
using SortFoldersBySize.Services;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortFoldersBySize.Tests
{
    public class SizeCalculatorShould
    {
        private CommandExecutor sizeCalculator;
        private string existingPath1 = @"c:\\MyFolder1\\";
        private string existingPath2 = @"c:\\MyFolder2\\MyFolder2\\MyFolder2\\MyFolder2";

        private MockFileSystem _fileSystem;

        [SetUp]
        public void Setup()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {existingPath1, new MockDirectoryData() },
                {existingPath2, new MockDirectoryData() }
            });
            sizeCalculator = new CommandExecutor(fileSystem);

            _fileSystem = new MockFileSystem();
        }

        [TestCase(@"c:\\MyFolder1\\")]
        [TestCase(@"c:\\MyFolder2\\")]
        [TestCase(@"c:\\MyFolder2\\MyFolder2\\MyFolder2\\MyFolder2")]
        public void FolderExists_validArgs_ReturnSuccess(string path)
        {
            var actual = sizeCalculator.FolderExists(path);

            Assert.That(actual.IsSuccess, Is.EqualTo(true));
        }

        [TestCase(@"c:\\NonExistentFolder2\\")]
        [TestCase(null)]
        public void FolderExists_invalidArgs_ReturnFailure(string path)
        {
            var actual = sizeCalculator.FolderExists(path);

            Assert.That(actual.IsSuccess, Is.EqualTo(false));
            Assert.That(actual.Error, Is.EqualTo(string.Format(ErrorMessages.InvalidPath, path)));
        }

    }
}
