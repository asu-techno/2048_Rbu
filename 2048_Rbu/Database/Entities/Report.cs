using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class Report
    {
        public Report()
        {
            Batches = new HashSet<Batch>();
        }

        public long Id { get; set; }
        public DateTime StartDt { get; set; }
        public DateTime FinishDt { get; set; }
        public long? TaskId { get; set; }

        public virtual Task Task { get; set; }
        public virtual ICollection<Batch> Batches { get; set; }
    }
}
