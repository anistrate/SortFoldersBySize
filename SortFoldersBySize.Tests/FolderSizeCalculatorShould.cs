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
        private const string existingPath1 = @"c:\\MyFolder1\\";
        private const string existingPath2 = @"c:\\MyFolder2\\MyFolder2\\MyFolder2\\MyFolder2";
        private const string test_file1 = @"\\Test Data\\test_data1.txt";
        private const string test_file2 = @"\\Test Data\\test_data2.txt";
        private const string test_file3 = @"\\Test Data\\test_data3.txt";
        private const string test_file4 = @"\\Test Data\\test_data4.txt";

        private const string testFilePath1 = @"c:\\MyFolder1\\Test Data\\test_data1.txt";
        private const string testFilePath2 = @"c:\\MyFolder1\\Test Data\\test_data2.txt";
        private const string testFilePath3 = @"c:\\MyFolder1\\Test Data\\test_data3.txt";
        private const string testFilePath4 = @"c:\\MyFolder1\\Test Data\\test_data4.txt";


        private MockFileSystem _fileSystem;

        [SetUp]
        public void Setup()
        {
            
            using var stream1 = new StreamReader(test_file1);
            var mockFile1 = new MockFileData(stream1.ReadToEnd());

            using var stream2 = new StreamReader(test_file2);
            var mockFile2 = new MockFileData(stream2.ReadToEnd());

            using var stream3 = new StreamReader(test_file3);
            var mockFile3 = new MockFileData(stream3.ReadToEnd());

            using var stream4 = new StreamReader(test_file4);
            var mockFile4 = new MockFileData(stream4.ReadToEnd());

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {existingPath1, new MockDirectoryData() },
                {existingPath2, new MockDirectoryData() },
                {testFilePath1, mockFile1 },
                {testFilePath2, mockFile2 },
                {testFilePath3, mockFile3 },
                {testFilePath4, mockFile4 }
            });
            folderSizeCalculator = new FolderSizeCalculator(_fileSystem);

        }

        [TestCase("i")]
        [TestCase("incorrect")]
        [TestCase("testtest")]

    }
}
