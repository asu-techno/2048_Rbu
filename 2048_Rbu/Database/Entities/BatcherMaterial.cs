using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class BatcherMaterial
    {
        public BatcherMaterial()
        {
            DosingSourceMaterials = new HashSet<DosingSourceMaterial>();
        }

        public long Id { get; set; }
        public decimal StartWeight { get; set; }
        public decimal FinishWeight { get; set; }
        public DateTime StartLoading { get; set; }
        public DateTime FinishLoading { get; set; }
        public long? BatchId { get; set; }

        public virtual Batch Batch { get; set; }
        public virtual ICollection<DosingSourceMaterial> DosingSourceMaterials { get; set; }
    }
}
