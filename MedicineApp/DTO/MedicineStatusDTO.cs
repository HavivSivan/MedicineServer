System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MedicineServer.Models;

namespace MedicineServer.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public int MedicineId { get; set; }
        public int UserId { get; set; }
        public string Receiver { get; set; }

        public string Sender { get; set; }

        public OrderDTO() { }
        public OrderDTO(Models.Order modelOrder)
        {
            this.Id = modelOrder.OrderId;
            this.MedicineId = modelOrder.MedicineId;
            this.UserId = modelOrder.UserId;
            this.Sender = modelOrder.Sender;
            this.Receiver = modelOrder.Receiver;
        }
    }
}