using adAdgenstvo.Models.EditModels;
using adAdgenstvo.Models.RegisterModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("user")]
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Patronymic { get; set; }
        public string Lastname { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Inn { get; set; }
        public string? NameCompany { get; set; }
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public int? PositionId { get; set; }
        public Position? Position { get; set; }

        public User()
        {
        }

        public User(ClientRM client)
        {
            Name = client.Name;
            Patronymic = client.Patronymic;
            Lastname = client.LastName;
            Email = client.Email;
            PhoneNumber = client.PhoneNumber;
            Inn = client.Inn;
            NameCompany = client.NameCompany;
            Password = client.Password;
            if(client.PositionId == null)
            {
                RoleId = 3;
            }
            else
            {
                RoleId = (int)client.PositionId;
                PositionId = client.PositionId;
            }
        }

        public User UpdateUser(UserEM userEM)
        {
            Id = userEM.Id;

            if (userEM.Name != null)
            {
                Name = userEM.Name;
            }

            if (userEM.Patronymic != null)
            {
                Patronymic = userEM.Patronymic;
            }

            if (userEM.Lastname != null)
            {
                Lastname = userEM.Lastname;
            }

            if (userEM.Password != null)
            {
                Password = userEM.Password;
            }

            if (userEM.Email != null)
            {
                Email = userEM.Email;
            }

            if (userEM.PhoneNumber != null)
            {
                PhoneNumber = userEM.PhoneNumber;
            }

            if (userEM.Inn != null)
            {
                Inn = userEM.Inn;
            }

            if (userEM.NameCompany != null)
            {
                NameCompany = userEM.NameCompany;
            }

            if (userEM.RoleId != null)
            {
                RoleId = (int)userEM.RoleId;
            }

            if (userEM.PositionId != null)
            {
                PositionId = userEM.PositionId;
            }



            return this;
        }


    }

}
