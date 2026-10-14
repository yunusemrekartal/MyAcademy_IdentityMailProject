namespace IdentityMail.Web.Entities
{
    public class UserMessage
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SendDate { get; set; }
        public bool IsRead { get; set; }
        public bool IsImportant { get; set; }
        public AppUser Sender { get; set; }
        public int SenderId { get; set; }
        public AppUser Receiver { get; set; }
        public int ReceiverId { get; set; }
        public bool IsDraft { get; set; }
        public bool IsTrashed { get; set; }
        public DateTime? TrashedDate { get; set; }
        public int? CategoryId { get; set; }
        public MessageCategory? Category { get; set; }

    }
}
