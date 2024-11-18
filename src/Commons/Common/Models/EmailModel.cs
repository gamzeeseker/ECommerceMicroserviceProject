namespace Common.Models
{
    public class EmailNotifyModel
    {
        public List<string> Addresses { get; set; }
        public List<string> CC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Attachment { get; set; }
    }

    public class SmsNotifyModel
    {
        public List<string> Addresses { get; set; }

        public string Body { get; set; }
    }
}
