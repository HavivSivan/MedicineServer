using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MedicineServer.Models;

namespace MedicineServer.DTO
{
    public class PharmacyDTO
    {
        public int Id { get; set; } 

        public string Name { get; set; } = null!;
        public string Adress { get; set; } = null!;

        public int Phone { get; set; }

        public PharmacyDTO() { }
        public PharmacyDTO(Models.Pharmacy modelPharma)
        {
            this.Id = modelPharma.PharmacyId;
            this.Name = modelPharma.PharmacyName;
            this.Adress = modelPharma.Adress;
            this.Phone=modelPharma.Phone;
        }
    }
}