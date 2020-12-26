namespace JigsawPuzzle.Models
{
    public class FileList
    {
        public string DirectoryName;
        public string[] FilesName;

        public FileList(string directoryName, string[] filesName)
        {
            DirectoryName = directoryName ?? string.Empty;
            FilesName = filesName ?? new string[0];
        }
    }
}