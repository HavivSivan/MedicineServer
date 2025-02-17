using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedicineServer.Models;

public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string FirstName { get; set; } = null!;

    [StringLength(255)]
    public string LastName { get; set; } = null!;

    [StringLength(30)]
    [Unicode(false)]
    public string UserName { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string UserPass { get; set; } = null!;

    public int? UserRank { get; set; }

    public bool Active { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();

    [InverseProperty("User")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("User")]
    public virtual ICollection<Pharmacy> Pharmacies { get; set; } = new List<Pharmacy>();
}
