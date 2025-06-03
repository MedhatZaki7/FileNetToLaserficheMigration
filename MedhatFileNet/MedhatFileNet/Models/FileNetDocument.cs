namespace MedhatFileNet.Models
{
    public class FileNetDocument
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
