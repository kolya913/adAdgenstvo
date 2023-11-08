namespace adAdgenstvo.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int IdPlace { get; set; }
        public int Region { get; set; }
        public int MailCode { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Home { get; set; }
        public Place Place { get; set; }
    }

}
