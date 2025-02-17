using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedicineServer.Models;

public partial class MedicineStatus
{
    [Key]
    public int StatusId { get; set; }

    [Column("MStatus")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Mstatus { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [InverseProperty("Status")]
    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}
