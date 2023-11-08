namespace adAdgenstvo.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public int OrderId { get; set; }
        public string CommentText { get; set; }
        public string TypeAuthor { get; set; }
        public int AnswerTo { get; set; }
        public Order Order { get; set; }
    }

}
