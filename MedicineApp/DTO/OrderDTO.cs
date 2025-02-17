using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MedicineServer.Models;

namespace MedicineServer.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public MedicineDTO Medicine { get; set; }
        public AppUser User { get; set; }
        public string Receiver { get; set; }
        public string Sender { get; set; }

        public OrderDTO() { }
        public OrderDTO(Models.Order modelOrder)
        {
            this.Id = modelOrder.OrderId;
            MedicineDTO medicine = new MedicineDTO(modelOrder.Medicine);
            this.Medicine = medicine;
            AppUser user = new AppUser(modelOrder.User);
            this.User = user;
            this.Sender = modelOrder.Sender;
            this.Receiver = modelOrder.Receiver;
        }
    }
}