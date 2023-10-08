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

        private const string existingDesktopIniPath30mb = @"c:\\MyFolder\desktop.ini";
        private const string existingDesktopIniPath0kb = @"c:\\MyFolder10\desktop.ini";

        private const string existingPath1 = @"c:\\MyFolder1\\";
        private const string existingPath2 = @"c:\\MyFolder2\\MyFolder2\\MyFolder2\\MyFolder2";
        private const string desktopIniFile = "\\desktop.ini";
        
        [OneTimeSetUp]
        public void SetUp()
        {
            var mockDesktopFile30mb = Stubs.GetMockDesktopIniFile(30000);
            var mockDesktopFile0mb = Stubs.GetMockDesktopIniFile(0);

            _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {existingPath1, new MockDirectoryData() },
                {existingPath2, new MockDirectoryData() },
                {existingDesktopIniPath30mb, mockDesktopFile30mb },
                {existingDesktopIniPath0kb, mockDesktopFile0mb }
            });
            _folderTaggingService = new FolderTaggingService(_mockFileSystem);
            
        }

        [TestCase(@"c:\\MyFolder1\\desktop.ini", 30000 , existingDesktopIniPath30mb)]
        [TestCase(@"c:\\MyFolder2\\desktop.ini", 0, existingDesktopIniPath0kb)]
        public void CreateNewDesktopIniForFolder_Right(string path, long size, string existingPath)
        {
            var result = _folderTaggingService.CreateNewDesktopIniForFolder(path, size);

            var actual = _mockFileSystem.GetFile(existingPath).TextContents;
            var expected = _mockFileSystem.GetFile(path).TextContents;

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_mockFileSystem.FileExists(path), Is.True);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(@"c:\\MyFolder1\\", 30000, "Access to the path 'c:\\\\MyFolder1\\\\' is denied.")]
        [TestCase(@"c:\\MyFolder1\\NonExistentFolder\\desktop.ini", 30000, "Could not find a part of the path 'c:\\\\MyFolder1\\\\NonExistentFolder\\\\desktop.ini'.")]
        public void CreateNewDesktopIniForFolder_Wrong_NoFileName(string path, long size, string expectedErrorMessage)
        {
            var result = _folderTaggingService.CreateNewDesktopIniForFolder(path, size);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(expectedErrorMessage));
        }

        [TestCase(null, 30000)]
        public void CreateNewDesktopIniForFolder_Wrong_PathNull(string path, long size)
        {
            Assert.Throws<ArgumentNullException>(() => _folderTaggingService.CreateNewDesktopIniForFolder(path,size));
        }




    }
}
