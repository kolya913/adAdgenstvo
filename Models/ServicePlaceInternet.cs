namespace adAdgenstvo.Models
{
    public class ServicePlaceInternet
    {
        public int Id { get; set; }
        public int IdService { get; set; }
        public int IdPlace { get; set; }
        public int IdInternet { get; set; }
        public Service Service { get; set; }
        public Place Place { get; set; }
        public Internet Internet { get; set; }
    }

}
