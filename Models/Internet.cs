namespace adAdgenstvo.Models
{
    public class Internet
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int IdOwner { get; set; }
        public string Platform { get; set; }
        public int CountViewers { get; set; }
        public int CountFollowers { get; set; }
        public Worker Owner { get; set; }
    }

}
