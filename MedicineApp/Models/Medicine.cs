using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedicineServer.Models;

public partial class Medicine
{
    [Key]
    public int MedicineId { get; set; }

    public int PharmacyId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string MedicineName { get; set; } = null!;

    [StringLength(100)]
    public string BrandName { get; set; } = null!;

    public bool NeedsPrescription { get; set; }

    public int StatusId { get; set; }

    public int UserId { get; set; }

    [InverseProperty("Medicine")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("PharmacyId")]
    [InverseProperty("Medicines")]
    public virtual Pharmacy Pharmacy { get; set; } = null!;

    [ForeignKey("StatusId")]
    [InverseProperty("Medicines")]
    public virtual MedicineStatus Status { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Medicines")]
    public virtual User User { get; set; } = null!;
}
