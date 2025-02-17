using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedicineServer.Models;

public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    public int MedicineId { get; set; }

    public int UserId { get; set; }

    [StringLength(255)]
    public string Receiver { get; set; } = null!;

    [StringLength(255)]
    public string Sender { get; set; } = null!;

    [ForeignKey("MedicineId")]
    [InverseProperty("Orders")]
    public virtual Medicine Medicine { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual User User { get; set; } = null!;
}
