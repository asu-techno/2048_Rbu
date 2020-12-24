using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class DosingSource
    {
        public DosingSource()
        {
            DosingSourceOpcParameters = new HashSet<DosingSourceOpcParameter>();
        }

        public long Id { get; set; }
        public string PlcName { get; set; }
        public long? ContainerId { get; set; }
        public long? BatcherId { get; set; }
        public string Name { get; set; }

        public virtual Batcher Batcher { get; set; }
        public virtual Container Container { get; set; }
        public virtual ICollection<DosingSourceOpcParameter> DosingSourceOpcParameters { get; set; }
    }
}
