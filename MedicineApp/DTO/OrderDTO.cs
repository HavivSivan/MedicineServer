using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MedicineServer.Models;

namespace MedicineServer.DTO
{
    public class MedicineStatusDTO
    {
        public int Id { get; set; }

        public int MedicineId { get; set; }
        public int UserId { get; set; }
        public string Receiver { get; set; }

        public string Sender { get; set; }

        public MedicineStatusDTO() { }
        public MedicineStatusDTO(Models.MedicineStatus modelStatus)
        {
            
        }
    }
}