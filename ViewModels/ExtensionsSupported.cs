public static class ExtensionsSupported
{
    public const string AllFiles = "All";
    public const string Images = "Images";
    public const string Videos = "Videos";
    public const string Audios = "Audios";
    public const string Docs = "Docs";
    public static (string, string)[] extensions = { (Videos, "*.exe, *.mkv, *.flv"), (Images, "*.jpg, *.png, *.gif, *.jpeg"), (Audios, "*.mp3"), (Docs, "*.pdf, *.doc, *.docx, *.txt"), (AllFiles, "") };
}