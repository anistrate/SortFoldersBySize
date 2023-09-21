using SortFoldersBySize.Services;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortFoldersBySize.Tests
{
    public class FolderTaggingServiceShould
    {

        private FolderTaggingService _folderTaggingService;
        private MockFileSystem _mockFileSystem;

        [OneTimeSetUp]
        public void SetUp()
        {
            using var stream1 = new StreamReader(test_file1);
            var mockFile1 = new MockFileData(stream1.ReadToEnd());

            using var stream2 = new StreamReader(test_file2);
            var mockFile2 = new MockFileData(stream2.ReadToEnd());

            using var stream3 = new StreamReader(test_file3);
            var mockFile3 = new MockFileData(stream3.ReadToEnd());

            using var stream4 = new StreamReader(test_file4);
            var mockFile4 = new MockFileData(stream4.ReadToEnd());

            var mockFolder = new MockDirectoryData();

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {existingPath1, new MockDirectoryData() },
                {existingPath2, new MockDirectoryData() },
                {testFilePath1, mockFile1 },
                {testFilePath2, mockFile2 },
                {testFilePath3, mockFile3 },
                {testFilePath4, mockFile4 },
                {testEmptyFolder, new MockDirectoryData() },
                {veryLongExistingPath,new  MockDirectoryData() }
            });
            _folderService = new FolderService(_fileSystem);
        }
    }
}
