using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class Batcher
    {
        public Batcher()
        {
            BatcherOpcParameters = new HashSet<BatcherOpcParameter>();
            DosingSources = new HashSet<DosingSource>();
        }

        public long Id { get; set; }
        public string PlcName { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BatcherOpcParameter> BatcherOpcParameters { get; set; }
        public virtual ICollection<DosingSource> DosingSources { get; set; }
    }
}
