using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedicineServer.Models;

public partial class Pharmacy
{
    [Key]
    public int PharmacyId { get; set; }

    [StringLength(255)]
    public string PharmacyName { get; set; } = null!;

    [StringLength(255)]
    public string Adress { get; set; } = null!;

    [StringLength(15)]
    public string Phone { get; set; } = null!;

    public int UserId { get; set; }

    [InverseProperty("Pharmacy")]
    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();

    [ForeignKey("UserId")]
    [InverseProperty("Pharmacies")]
    public virtual User User { get; set; } = null!;
}
