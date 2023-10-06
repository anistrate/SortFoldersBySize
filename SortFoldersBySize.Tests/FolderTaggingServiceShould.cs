using SortFoldersBySize.Models;
using SortFoldersBySize.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private const string existingDesktopIniPath = @"c:\\MyFolder\desktop.ini";
        private const string existingPath1 = @"c:\\MyFolder1\\";
        private const string existingPath2 = @"c:\\MyFolder2\\MyFolder2\\MyFolder2\\MyFolder2";
        private const string desktopIniFile = "\\desktop.ini";
        
        [OneTimeSetUp]
        public void SetUp()
        {
            /*    
            using var stream1 = new StreamReader(test_file1);
            var mockFile1 = new MockFileData(stream1.ReadToEnd());

            using var stream2 = new StreamReader(test_file2);
            var mockFile2 = new MockFileData(stream2.ReadToEnd());

            using var stream3 = new StreamReader(test_file3);
            var mockFile3 = new MockFileData(stream3.ReadToEnd());

            using var stream4 = new StreamReader(test_file4);
            var mockFile4 = new MockFileData(stream4.ReadToEnd());

            var mockFolder = new MockDirectoryData();
            */

            var desktopIniContent = FolderTaggingHelper.GetDesktopIniFileContent(30000, MagiGStrings.ForCreatedFiles);
            var stringBuilder = new StringBuilder();
            for(int i= 0; i<desktopIniContent.Length; i++)
            {
                stringBuilder.AppendLine(desktopIniContent[i]);
                //else stringBuilder.Append(desktopIniContent[i]);
            }

            var mockDesktopFile = new MockFileData(stringBuilder.ToString());

            _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {existingPath1, new MockDirectoryData() },
                {existingPath2, new MockDirectoryData() },
                {existingDesktopIniPath, mockDesktopFile }
                /*,
                {testFilePath1, mockFile1 },
                {testFilePath2, mockFile2 },
                {testFilePath3, mockFile3 },
                {testFilePath4, mockFile4 },
                {testEmptyFolder, new MockDirectoryData() },
                {veryLongExistingPath,new  MockDirectoryData() }*/
            });
            _folderTaggingService = new FolderTaggingService(_mockFileSystem);
            
        }

        [TestCase(@"c:\\MyFolder1\\desktop.ini", 30000)]
        [TestCase(@"c:\\MyFolder2\\desktop.ini", 0)]
        public void CreateNewDesktopIniForFolder_Right(string path, long size)
        {
            _folderTaggingService.CreateNewDesktopIniForFolder(path, size);

            var a1 = _mockFileSystem.GetFile(existingDesktopIniPath).TextContents;
            var a2 = _mockFileSystem.GetFile(path).TextContents;

            Assert.That(_mockFileSystem.FileExists(path), Is.True);
            Assert.That(a1, Is.EqualTo(a2));
        }

        [TestCase(null, 30000)]
        public void CreateNewDesktopIniForFolder_Wrong_PathNull(string path, long size)
        {
            Assert.Throws<ArgumentNullException>(() => _folderTaggingService.CreateNewDesktopIniForFolder(path,size));
        }

        [TestCase(@"c:\\MyFolder1\\NonExistentFolder\\desktop.ini", 30000)]
        public void CreateNewDesktopIniForFolder_Wrong_NonExistentPath(string path, long size)
        {
            Assert.Throws<DirectoryNotFoundException>(() => _folderTaggingService.CreateNewDesktopIniForFolder(path, size));
        }

        [TestCase(@"c:\\MyFolder1\\", 30000)]
        public void CreateNewDesktopIniForFolder_Wrong_NoFileName(string path, long size)
        {
            Assert.Throws<UnauthorizedAccessException>(() => _folderTaggingService.CreateNewDesktopIniForFolder(path, size));
        }


    }
}
