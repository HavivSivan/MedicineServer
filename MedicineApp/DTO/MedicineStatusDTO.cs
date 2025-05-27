using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MedicineServer.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Medicine.DTO
{
    public class MedicineStatusDTO
    {
        public int StatusId { get; set; }
        public string Mstatus { get; set; }
        public string Notes { get; set; }
        public MedicineStatusDTO() { }
        public MedicineStatusDTO(MedicineStatus status) 
        {
            if (status!=null)
            {
                this.StatusId=status.StatusId;
                this.Mstatus= status.Mstatus;
                this.Notes=status.Notes;
            }
        }
    }
}