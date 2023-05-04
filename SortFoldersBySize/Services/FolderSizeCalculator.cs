using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortFoldersBySize.Services
{
    public class FolderSizeCalculator
    {
        private readonly IFileSystem _fileSystem;

        public FolderSizeCalculator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Dictionary<string, long> CalculateFolderSizes(string path)
        {
            var directoriesDictionary = new Dictionary<string, long>();
            string[] directories = _fileSystem.Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                directoriesDictionary.Add(directory, GetFolderSize(directory));
            }
            return directoriesDictionary;
        }

        public long GetFolderSize(string folderPath)
        {
            long folderSize = 0;
            var dirInfo = _fileSystem.DirectoryInfo.New(folderPath);
            foreach (var fi in dirInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                folderSize += fi.Length;
            }
            return folderSize;
        }
    }
}
