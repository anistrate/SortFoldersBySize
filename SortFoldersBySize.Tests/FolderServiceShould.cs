using SortFoldersBySize.Models;
using SortFoldersBySize.Services;
using System.IO.Abstractions.TestingHelpers;

namespace SortFoldersBySize.Tests
{
    public class FolderServiceShould
    {

        private FolderService _folderService;
        private const string existingPath1 = @"c:\\MyFolder1\\";
        private const string existingPath2 = @"c:\\MyFolder2\\MyFolder2\\MyFolder2\\MyFolder2";
        private const string veryLongExistingPath = @"c:\\MyFolder1\\Test Data\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath\\VeryLongPath";
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
                {testEmptyFolder, new MockDirectoryData() },
                {veryLongExistingPath,new  MockDirectoryData() }
            });
            _folderService = new FolderService(_fileSystem);

        }

        [TestCase(@"c:\\MyFolder1\\")]
        [TestCase(@"c:\\MyFolder2\\")]
        [TestCase(@"c:\\MyFolder2\\MyFolder2\\MyFolder2\\MyFolder2")]
        [TestCase(veryLongExistingPath)]
        public void FolderExists_Right(string path)
        {
            var actual = _folderService.FolderExists(path);

            Assert.That(actual.IsSuccess, Is.EqualTo(true));
        }

        [TestCase(@"c:\\NonExistentFolder2\\")]
        [TestCase(null)]
        [TestCase("")]
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

        [TestCase("")]
        public void GetFolderSize_Wrong_EmptyString(string path)
        {
            Assert.Throws<ArgumentException>(() => _folderService.GetFolderSize(path));
        }

        [TestCase(@"c:\\MyFolder1\\Test Data\\")]
        public void GetSizesForFoldersAtPath_Right(string path)
        {
            var expected = new Dictionary<string, long>()
            {
                { "c:\\MyFolder1\\Test Data\\Folder2", 4657 },
                { "c:\\MyFolder1\\Test Data\\Folder3", 0 },
                { "c:\\MyFolder1\\Test Data\\VeryLongPath", 0 }
            };

            var actual = _folderService.GetSizesForFoldersAtPath(path);

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [TestCase(@"c:\\MyFolder1\\NonexistentFolder\\")]
        [TestCase(veryLongExistingPath + "//NonExistentFolder")]
        public void GetSizesForFoldersAtPath_Wrong_NonExistentFolder(string path)
        {
            Assert.Throws<DirectoryNotFoundException>(() => _folderService.GetSizesForFoldersAtPath(path));
        }

        [TestCase("")]
        public void GetSizesForFoldersAtPath_Wrong_Empty(string path)
        {
            Assert.Throws<ArgumentException>(() => _folderService.GetSizesForFoldersAtPath(path));
        }

        [TestCase(null)]
        public void GetSizesForFoldersAtPath_Wrong_Null(string path)
        {
            Assert.Throws<ArgumentNullException>(() => _folderService.GetSizesForFoldersAtPath(path));
        }


    }
}
