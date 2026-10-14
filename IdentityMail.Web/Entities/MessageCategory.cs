namespace IdentityMail.Web.Entities
{
    public class MessageCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;   // yeni kategoriler varsayılan olarak aktif
        public List<UserMessage> Messages { get; set; } = new();
    }
}
