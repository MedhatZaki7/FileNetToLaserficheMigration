namespace MedhatFileNet.Models
{
    public class LaserficheDocument
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string TemplateName { get; set; }
        public string ParentEntryId { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
