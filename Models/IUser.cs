namespace adAdgenstvo.Models
{
    public interface IUser
    {
        int Id { get; set; }
        string Login { get; set; }
        string Password { get; set; }
        public int IdRole { get; set; }
        public Role Role { get; set; }
    }
}
