using SortFoldersBySize.Services;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortFoldersBySize.Tests
{
    public class FolderSizeCalculatorShould
    {

        private FolderSizeCalculator folderSizeCalculator;
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
            folderSizeCalculator = new FolderSizeCalculator(fileSystem);

            _fileSystem = new MockFileSystem();
        }

    }
}
