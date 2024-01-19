using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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
        private const string existingDesktopIniPathCreatedBySystem1 = @"c:\\MyFolder11\desktop.ini";
        private const string existingDesktopIniPathCreatedBySystem2 = @"c:\\MyFolder12\desktop.ini";
        private const string existingDesktopIniPathCreatedBySystem3 = @"c:\\MyFolder13\desktop.ini";
        private const string existingPathWithNoDesktopIni = @"c:\\MyFolder14\";
        private const string existingDesktopIniPathCreatedBySystem4 = @"c:\\MyFolder15\desktop.ini";
        private const string existingDesktopIniPathCreatedByProgram = @"c:\\MyFolder16\desktop.ini";
        private const string existingDesktopIniPathCreatedBySystemAppendedByProgram1 = @"c:\\MyFolder17\desktop.ini";
        private const string nonExistingPath = @"c:\\MyFolder1\\NonExistentFolder\\desktop.ini";
        private const string existingDesktopIniPathCreatedBySystemAppendedByProgram2 = @"c:\\MyFolder18\desktop.ini";

        private const string existingPath1 = @"c:\\MyFolder1\\";
        private const string existingPath2 = @"c:\\MyFolder2\\MyFolder2\\MyFolder2\\MyFolder2";
        private const string desktopIniFile = "\\desktop.ini";
        
        [OneTimeSetUp]
        public void SetUp()
        {
            var mockDesktopFile30mb = Stubs.GetMockDesktopIniFile(30000, MagiGStrings.ForCreatedFiles);
            var mockDesktopFile0mb = Stubs.GetMockDesktopIniFile(0, MagiGStrings.ForCreatedFiles);
            var mockDesktopFileCreatedBySystem1 = Stubs.GetMockDesktopIniFileCreatedBySystem();
            var mockDesktopFileCreatedBySystem2 = Stubs.GetMockDesktopIniFileCreatedBySystem();
            var mockDesktopFileCreatedBySystem3 = Stubs.GetMockDesktopIniFileCreatedBySystem();
            var mockDesktopFileCreatedBySystem4 = Stubs.GetMockDesktopIniFileCreatedBySystem();
            var mockDesktopFileCreatedByProgram1 = Stubs.GetMockDesktopIniFile(10000, MagiGStrings.ForCreatedFiles);
            var mockDesktopFileCreatedByProgramAppendedBySystem1 = Stubs.GetMockDesktopIniFile(10000, MagiGStrings.ForAppendedFiles);
            var mockDesktopFileCreatedByProgramAppendedBySystem2 = Stubs.GetMockDesktopIniFile(100000, MagiGStrings.ForAppendedFiles);

            _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {existingPath1, new MockDirectoryData() },
                {existingPath2, new MockDirectoryData() },
                {existingDesktopIniPath30mb, mockDesktopFile30mb },
                {existingDesktopIniPath0kb, mockDesktopFile0mb },
                {existingDesktopIniPathCreatedBySystem1,  mockDesktopFileCreatedBySystem1},
                {existingDesktopIniPathCreatedBySystem2,  mockDesktopFileCreatedBySystem2},
                {existingDesktopIniPathCreatedBySystem3,  mockDesktopFileCreatedBySystem3},
                {existingPathWithNoDesktopIni, new MockDirectoryData()  },
                {existingDesktopIniPathCreatedBySystem4,mockDesktopFileCreatedBySystem4  },
                {existingDesktopIniPathCreatedByProgram,mockDesktopFileCreatedByProgram1 },
                {existingDesktopIniPathCreatedBySystemAppendedByProgram1, mockDesktopFileCreatedByProgramAppendedBySystem1 },
                { existingDesktopIniPathCreatedBySystemAppendedByProgram2, mockDesktopFileCreatedByProgramAppendedBySystem2}


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
        [TestCase(nonExistingPath, 30000, "Could not find a part of the path 'c:\\\\MyFolder1\\\\NonExistentFolder\\\\desktop.ini'.")]
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

        [TestCase(existingDesktopIniPathCreatedBySystem2, -30000)]
        public void CreateNewDesktopIniForFolder_Wrong_NegativeSize(string path, long size)
        {
            Assert.Throws<ArgumentException>(() => _folderTaggingService.CreateNewDesktopIniForFolder(path, size));
        }

        [TestCase(existingDesktopIniPathCreatedBySystem1, 30000, "30", "KB")]
        [TestCase(existingDesktopIniPathCreatedBySystem2, 0 , "0" , "KB" )]
        public void ModifyDesktopIniFileCreatedbySystem_Correct(string path, long size, string expectedValue, string expectedUnit)
        {
            var result = _folderTaggingService.ModifyDesktopIniCreatedbySystem(path, size);

            var actual = _mockFileSystem.GetFile(path).TextContents;
            var expected = Stubs.GetMockDesktopIniCreatedBySystemAfterBeingModified(expectedValue, expectedUnit);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_mockFileSystem.FileExists(path), Is.True);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(null, 0)]
        public void ModifyDesktopIniFileCreatedbyProgram_Wrong_PathNull(string path, long size)
        {
            Assert.Throws<ArgumentNullException>(() => _folderTaggingService.ModifyDesktopIniCreatedbySystem(path, size));
        }

        [TestCase(existingDesktopIniPathCreatedBySystem3, -2)]
        public void ModifyDesktopIniFileCreatedbyProgram_Wrong_NegativeSize(string path, long size)
        {
            Assert.Throws<ArgumentException>(() => _folderTaggingService.ModifyDesktopIniCreatedbySystem(path, size));
        }

        [TestCase(@"c:\\MyFolder1\\", 30000, "Access to the path 'c:\\\\MyFolder1\\\\' is denied.")]
        [TestCase(nonExistingPath, 30000, "Could not find file 'c:\\\\MyFolder1\\\\NonExistentFolder\\\\desktop.ini'.")]

        public void ModifyDesktopIniFileCreatedbyProgram__Wrong_NoFileName(string path, long size, string expectedErrorMessage)
        {
            var result = _folderTaggingService.ModifyDesktopIniCreatedbySystem(path, size);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(expectedErrorMessage));
        }

        [TestCase(existingPathWithNoDesktopIni, FolderTagCase.DesktopIniNotExist)]
        [TestCase(existingDesktopIniPathCreatedBySystem4, FolderTagCase.DesktopIniCreatedBySystem)]
        [TestCase(existingDesktopIniPathCreatedByProgram, FolderTagCase.DesktopIniCreatedByProgram)]
        [TestCase(existingDesktopIniPathCreatedBySystemAppendedByProgram1, FolderTagCase.DesktopIniCreatedBySystemModifiedByProgram)]
        public void GetFolderTagCase_Right(string path, FolderTagCase expectedTagCase)
        {
            var result = _folderTaggingService.GetFolderTagCase(path);

            Assert.That(result, Is.EqualTo(expectedTagCase));
        }

        //The IFile.Exists() method will return false if the path is invalid, or null, or it does not have enough permissions.
        //The actual cases are treated in the tagging part of the service, this one mostly silently ignores them
        [TestCase(nonExistingPath, FolderTagCase.DesktopIniNotExist)]
        [TestCase(null, FolderTagCase.DesktopIniNotExist)]
        [TestCase("", FolderTagCase.DesktopIniNotExist)]
        public void GetFolderTagCase_Wrong(string path, FolderTagCase expectedTagCase)
        {
            var result = _folderTaggingService.GetFolderTagCase(path);

            Assert.That(result, Is.EqualTo(expectedTagCase));
        }

        [TestCase("", 50000)]
        public void ModifyDesktopIniCreatedBySystemModifiedByProgram_Right(string path, long size)
        {
            var result = _folderTaggingService.ModifyDesktopIniCreatedBySystemModifiedByProgram(path, size);

            var expected = ...;
            Assert.That(result.IsSuccess, Is.True);

        }


    }
}
