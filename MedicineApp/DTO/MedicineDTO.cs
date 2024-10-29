using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using MedicineServer.Models;
namespace MedicineServer.DTO
{
    public class MedicineDTO
    {
       
        public int MedicineId { get; set; }

        public int? PharmacyId { get; set; }

       
        public string MedicineName { get; set; }

      
        public string BrandName { get; set; }

        public int? StatusId { get; set; }

        public int? UserId { get; set; }
        public MedicineDTO(Models.Medicine medicine) 
        {
            this.MedicineId = medicine.MedicineId;
            this.MedicineName = medicine.MedicineName;
            this.StatusId = medicine.StatusId;
            this.PharmacyId = medicine.PharmacyId;
            this.BrandName = medicine.BrandName;
            this.UserId = medicine.UserId;
        }
    }
}
