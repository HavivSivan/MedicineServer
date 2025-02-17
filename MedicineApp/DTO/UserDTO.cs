using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MedicineServer.Models;
using System.Text.Json.Serialization;

namespace MedicineServer.DTO
{
    public class AppUser
    {
        public string Email { get; set; } 
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserPassword { get; set; }

        public int Rank { get; set; }

        public int Id { get; set; }

        public AppUser() { }
        public AppUser(Models.User modelUser)
        {
            this.Id = modelUser.UserId;
            this.UserName = modelUser.UserName;
            this.LastName = modelUser.LastName;
            this.FirstName = modelUser.FirstName;
            this.Email = modelUser.Email;
            this.UserPassword = modelUser.UserPass;
            this.Rank = modelUser.UserRank;
        }
    }
}
