using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MedicineServer.Models;

namespace MedicineServer.DTO
{
    public class PharmacyDTO
    {
        public int Id { get; set; } 

        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public AppUser User { get; set; } 
        public string Phone { get; set; }

        public PharmacyDTO() { }
        public PharmacyDTO(Pharmacy modelPharma)
        {
            this.Id = modelPharma.PharmacyId;
            this.Name = modelPharma.PharmacyName;
            this.Address = modelPharma.Adress;
            this.Phone=modelPharma.Phone;
            AppUser user = new AppUser(modelPharma.User);
            this.User=user;
        }
    }
}