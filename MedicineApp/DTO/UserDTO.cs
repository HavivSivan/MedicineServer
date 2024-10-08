using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MedicineServer.Models;

namespace MedicineServer.DTO
{
    public class AppUser
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!;
        public string FirstName { get; set; } 

        public string LastName { get; set; } 

        public string UserEmail { get; set; } = null!;

        public string UserPassword { get; set; } = null!;

        public int Rank { get; set; }

        public AppUser() { }
        public AppUser(Models.User modelUser)
        {
            this.Id = modelUser.UserId;
            this.UserName = modelUser.UserName;
            this.LastName = modelUser.LastName;
            this.FirstName = modelUser.FirstName;
            this.UserEmail = modelUser.Email;
            this.UserPassword = modelUser.UserPass;
            this.Rank = modelUser.UserRank;
        }
    }
}