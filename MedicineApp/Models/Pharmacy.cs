﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedicineApp.Models;

public partial class Pharmacy
{
    [Key]
    public int PharmacyId { get; set; }

    [StringLength(255)]
    public string? PharmacyName { get; set; }

    public int? Phone { get; set; }

    [InverseProperty("Pharmacy")]
    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}
