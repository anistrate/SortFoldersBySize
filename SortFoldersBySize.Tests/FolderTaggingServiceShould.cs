using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using SortFoldersBySize.Models;
using SortFoldersBySize.Services;
using System.IO.Abstractions.TestingHelpers;

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
        private const string existingDesktopIniPathCreatedBySystemAppendedByProgram3 = @"c:\\MyFolder19\desktop.ini";
        private const string existingDesktopIniPathCreatedBySystemAppendedByProgram4 = @"c:\\MyFolder20\desktop.ini";
        private const string fileToDelete1 = @"c:\\MyFolder21\desktop.ini";
        private const string pathTriedToDelete = @"c:\\MyFolder22\";

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
            var mockDesktopFileCreatedByProgramAppendedBySystem2 = Stubs.GetMockDesktopIniCreatedBySystemAfterBeingModified("300000","300", "KB");
            var mockDesktopFileCreatedByProgramAppendedBySystem3 = Stubs.GetMockDesktopIniCreatedBySystemAfterBeingModified("0","0", "KB");
            var mockDesktopFileCreatedByProgramAppendedBySystem4 = Stubs.GetMockDesktopIniCreatedBySystemAfterBeingModified("2000","2", "KB");
            var mockFileToDelete1 = Stubs.GetMockDesktopIniFile(2000,MagiGStrings.ForCreatedFiles);

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
                {existingDesktopIniPathCreatedBySystemAppendedByProgram1, mockDesktopFileCreatedByProgramAppendedBySystem1},
                {existingDesktopIniPathCreatedBySystemAppendedByProgram2, mockDesktopFileCreatedByProgramAppendedBySystem2},
                {existingDesktopIniPathCreatedBySystemAppendedByProgram3, mockDesktopFileCreatedByProgramAppendedBySystem3},
                {existingDesktopIniPathCreatedBySystemAppendedByProgram4, mockDesktopFileCreatedByProgramAppendedBySystem4},
                {fileToDelete1, mockFileToDelete1},
                {pathTriedToDelete, new MockDirectoryData() }
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
            var expected = Stubs.GetMockDesktopIniCreatedBySystemAfterBeingModified((size/1000).ToString(), expectedValue, expectedUnit);

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

        [TestCase(existingDesktopIniPathCreatedBySystemAppendedByProgram2, 4000, "4", "KB")]
        [TestCase(existingDesktopIniPathCreatedBySystemAppendedByProgram3, 50000000000, "47.68", "GB")]
        public void ModifyDesktopIniCreatedBySystemModifiedByProgram_Right(string path, long size,string expectedValue, string unit)
        {
            var result = _folderTaggingService.ModifyDesktopIniCreatedBySystemModifiedByProgram(path, size);

            var actual = _mockFileSystem.GetFile(path).TextContents;
            var expected = Stubs.GetMockDesktopIniCreatedBySystemAfterBeingModified((size/1000).ToString(), expectedValue, unit);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(nonExistingPath, 1000, "Could not find file 'c:\\\\MyFolder1\\\\NonExistentFolder\\\\desktop.ini'.")]
        [TestCase(@"c:\\MyFolder1\\", -1000, "Access to the path 'c:\\\\MyFolder1\\\\' is denied.")]
        public void ModifyDesktopIniCreatedBySystemModifiedByProgram_Wrong_NonExistentPath(string path, long size, string expectedErrorMessage)
        {
            var result = _folderTaggingService.ModifyDesktopIniCreatedBySystemModifiedByProgram(path, size);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(expectedErrorMessage));
        }

        [TestCase(null, 1000)]
        public void ModifyDesktopIniCreatedBySystemModifiedByProgram_Wrong_NullPath(string path, long size)
        {
            Assert.Throws<ArgumentNullException>(() => _folderTaggingService.ModifyDesktopIniCreatedBySystemModifiedByProgram(path, size));
        }

        [TestCase(existingDesktopIniPathCreatedBySystemAppendedByProgram4, -1000)]
        public void ModifyDesktopIniCreatedBySystemModifiedByProgram_Wrong_NegativeFolderSize(string path, long size)
        {
            Assert.Throws<ArgumentException>(() => _folderTaggingService.ModifyDesktopIniCreatedBySystemModifiedByProgram(path, size));
        }

        [TestCase(fileToDelete1)]
        public void RemoveDesktopIniCreatedByProgram_Right(string path)
        {
            Assert.That(_mockFileSystem.FileExists(path), Is.True);
            var result = _folderTaggingService.RemoveDesktopIniCreatedByProgram(path);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_mockFileSystem.FileExists(path), Is.False);
        }

        [TestCase(null)]
        public void RemoveDesktopIniCreatedByProgram_Wrong_NullString(string path)
        {
            Assert.Throws<ArgumentNullException>(() => _folderTaggingService.RemoveDesktopIniCreatedByProgram(path));
        }

        //Nothign happens if pointed to a folder. Weird.
        [TestCase(pathTriedToDelete)]
        public void RemoveDesktopIniCreatedByProgram_Wrong_NoFile_FolderExists(string path)
        {
            Assert.That(_mockFileSystem.Directory.Exists(path), Is.True);
            var result = _folderTaggingService.RemoveDesktopIniCreatedByProgram(path);
            Assert.That(_mockFileSystem.Directory.Exists(path), Is.True);
            Assert.That(result.IsSuccess, Is.True);
        }

        //Nothign happens if pointed to a file that is not there. Weird.
        [TestCase(@"c:\\MyFolder21\desktop3.ini")]
        public void RemoveDesktopIniCreatedByProgram_Wrong_NoFile(string path)
        {
            Assert.That(_mockFileSystem.File.Exists(path), Is.False);
            var result = _folderTaggingService.RemoveDesktopIniCreatedByProgram(path);
            Assert.That(result.IsSuccess, Is.True);
        }


        [TestCase(nonExistingPath, "Could not find a part of the path 'c:\\\\MyFolder1\\\\NonExistentFolder\\\\desktop.ini'.")]
        [TestCase(@"c:\\NonExistingFolder\\", "Could not find a part of the path 'c:\\\\NonExistingFolder\\\\'.")]
        public void RemoveDesktopIniCreatedByProgram_Wrong_NonExistentPath(string path, string expectedErrorMessage)
        {
            var result = _folderTaggingService.RemoveDesktopIniCreatedByProgram(path);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(expectedErrorMessage));
        }


        //check if delete(path) deletes all files from a path, if no specific file is specified. Sounds idiotic but better check
        [TestCase()]
        public void CleanDesktopIniFromProgramTagInfo_Right(string path)
        {

        }
    }
}
