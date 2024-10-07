using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedicineApp.Models;

public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    public int? MedicineId { get; set; }

    public int? UserId { get; set; }

    [ForeignKey("MedicineId")]
    [InverseProperty("Orders")]
    public virtual Medicine? Medicine { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual User? User { get; set; }
}
