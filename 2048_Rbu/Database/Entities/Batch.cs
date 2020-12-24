using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class Batch
    {
        public Batch()
        {
            BatcherMaterials = new HashSet<BatcherMaterial>();
        }

        public long Id { get; set; }
        public DateTime StartDt { get; set; }
        public DateTime FinishDt { get; set; }
        public DateTime StartMixing { get; set; }
        public DateTime FinishMixing { get; set; }
        public DateTime StartUnloading { get; set; }
        public DateTime FinishUnloading { get; set; }
        public long? ReportId { get; set; }

        public virtual Report Report { get; set; }
        public virtual ICollection<BatcherMaterial> BatcherMaterials { get; set; }
    }
}
