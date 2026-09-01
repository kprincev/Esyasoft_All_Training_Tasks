namespace RabbitFilePipeline.Common
{
    public class FileMessage
    {
        public string FileName { get; set; }
        public string FileType { get; set; }   // .csv / .json / .xml
        public string FileContent { get; set; }
    }
}
