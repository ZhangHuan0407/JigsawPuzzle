namespace JigsawPuzzle.Models
{
    public class FileList
    {
        public string DirectoryName;
        public string[] FilesName;
        public bool Multiple;

        public FileList(string directoryName, string[] filesName, bool multiple)
        {
            DirectoryName = directoryName ?? string.Empty;
            FilesName = filesName ?? new string[0];
            Multiple = multiple;
        }
    }
}