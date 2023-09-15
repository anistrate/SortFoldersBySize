using SortFoldersBySize.Models;
using SortFoldersBySize.Services;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace SortFoldersBySize.Tests
{
    public class FolderServiceShould
    {

        private FolderService _folderService;
        private const string existingPath1 = @"c:\\MyFolder1\\";
        private const string existingPath2 = @"c:\\MyFolder2\\MyFolder2\\MyFolder2\\MyFolder2";
        private const string test_file1 = @"F:\Github\SortFoldersBySize\SortFoldersBySize.Tests\Test Data\test_data1.txt";
        private const string test_file2 = @"F:\Github\SortFoldersBySize\SortFoldersBySize.Tests\Test Data\test_data2.txt";
        private const string test_file3 = @"F:\Github\SortFoldersBySize\SortFoldersBySize.Tests\Test Data\test_data3.txt";
        private const string test_file4 = @"F:\Github\SortFoldersBySize\SortFoldersBySize.Tests\Test Data\test_data4.txt";

        private const string testFilePath1 = @"c:\\MyFolder1\\Test Data\\test_data1.txt";
        private const string testFilePath2 = @"c:\\MyFolder1\\Test Data\\test_data2.txt";
        private const string testFilePath3 = @"c:\\MyFolder1\\Test Data\\Folder2\\test_data3.txt";
        private const string testFilePath4 = @"c:\\MyFolder1\\Test Data\\Folder2\\test_data4.txt";
        private const string testEmptyFolder = @"c:\\MyFolder1\\Test Data\\Folder3\\";


        private MockFileSystem _fileSystem;

        [OneTimeSetUp]
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
                {testFilePath4, mockFile4 },
                {testEmptyFolder, new MockDirectoryData() }
            });
            _folderService = new FolderService(_fileSystem);

        }

        [TestCase(@"c:\\MyFolder1\\")]
        [TestCase(@"c:\\MyFolder2\\")]
        [TestCase(@"c:\\MyFolder2\\MyFolder2\\MyFolder2\\MyFolder2")]
        public void FolderExists_Right(string path)
        {
            var actual = _folderService.FolderExists(path);

            Assert.That(actual.IsSuccess, Is.EqualTo(true));
        }

        [TestCase(@"c:\\NonExistentFolder2\\")]
        [TestCase(null)]
        public void FolderExists_Wrong(string path)
        {
            var actual = _folderService.FolderExists(path);

            Assert.That(actual.IsSuccess, Is.EqualTo(false));
            Assert.That(actual.Error, Is.EqualTo(string.Format(ErrorMessages.InvalidPath, path)));
        }

        [TestCase(@"c:\\MyFolder1\\Test Data\\", 12749)]
        [TestCase(@"c:\\MyFolder1\\Test Data\\Folder2\\", 4657)]
        [TestCase(@"c:\\MyFolder1\\Test Data\\Folder3\\", 0)]
        public void GetFolderSize_Right(string path, long expectedValue)
        {
            var actual = _folderService.GetFolderSize(path);

            Assert.That(actual, Is.EqualTo(expectedValue));
        }

        [TestCase(null)]
        public void GetFolderSize_Wrong_ParamIsNull(string path)
        {
            Assert.Throws<ArgumentNullException>(() => _folderService.GetFolderSize(path));
        }

        [TestCase(@"c:\\MyFolder1\\NonExistentFolder\\")]
        public void GetFolderSize_Wrong_NonExistentFolder(string path)
        {
            Assert.Throws<DirectoryNotFoundException>(() => _folderService.GetFolderSize(path));
        }

        [TestCase(@"c:\\MyFolder1\\Test Data\\")]
        public void GetSizesForFoldersAtPath_Right(string path)
        {
            var expected = new Dictionary<string, long>()
            {
                { "Folder2", 4657 },
                { "Folder3", 0 }
            };

            var actual = _folderService.GetSizesForFoldersAtPath(path);

            Assert.That(actual, Is.EqualTo(expected));
        }

    }
}
