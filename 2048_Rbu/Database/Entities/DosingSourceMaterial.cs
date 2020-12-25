using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class DosingSourceMaterial
    {
        public long Id { get; set; }
        public decimal StartWeightDosage { get; set; }
        public decimal FinishWeightDosage { get; set; }
        public DateTime StartDosage { get; set; }
        public DateTime FinishDosage { get; set; }
        public long? BatcherMaterialId { get; set; }
        public long? MaterialId { get; set; }
        public long? ContainerId { get; set; }
        public decimal? SetVolume { get; set; }

        public virtual BatcherMaterial BatcherMaterial { get; set; }
    }
}
