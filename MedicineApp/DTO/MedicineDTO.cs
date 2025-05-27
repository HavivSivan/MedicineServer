using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using MedicineServer.Models;
using Medicine.DTO;
namespace MedicineServer.DTO
{
    public class MedicineDTO
    {
       
        public int MedicineId { get; set; }

        public PharmacyDTO Pharmacy { get; set; } 

       
        public string MedicineName { get; set; }

      
        public string BrandName { get; set; }
        
        public bool NeedsPrescription { get; set; }

        public MedicineStatusDTO Status { get; set; }

        public AppUser User { get; set; }
        public MedicineDTO(Models.Medicine medicine) 
        {
            if (medicine!=null)
            {
                this.MedicineId = medicine.MedicineId;
                this.MedicineName = medicine.MedicineName;
                this.Status = new MedicineStatusDTO(medicine.Status);
                PharmacyDTO pharmacy = new PharmacyDTO(medicine.Pharmacy);
                this.Pharmacy = pharmacy;
                this.BrandName = medicine.BrandName;
                AppUser user = new AppUser(medicine.User);
                this.User = user;
                this.NeedsPrescription=medicine.NeedsPrescription;
            }
        }
    }
}
