using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedicineApp.Models;

public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(255)]
    public string? FirstName { get; set; }

    [StringLength(255)]
    public string? LastName { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? UserName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? UserPass { get; set; }

    public int? UserRank { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();

    [InverseProperty("User")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
