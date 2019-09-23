namespace SpaceAnalyzer.ViewModels
{
    public class FileModel
    {
        public string Size { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
    }
    public static class CurrentSelectedDrive
    {
        public static string Path { get; set; } = string.Empty;
    }
}