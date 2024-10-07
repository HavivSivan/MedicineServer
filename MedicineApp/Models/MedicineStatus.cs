using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedicineApp.Models;

public partial class MedicineStatus
{
    [Key]
    public int StatusId { get; set; }

    [Column("MStatus")]
    [StringLength(1)]
    [Unicode(false)]
    public string? Mstatus { get; set; }

    [StringLength(1)]
    public string? Notes { get; set; }

    [InverseProperty("Status")]
    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}
