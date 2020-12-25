using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class Container
    {
        public Container()
        {
            DosingSources = new HashSet<DosingSource>();
        }

        public long Id { get; set; }
        public string Abbreviation { get; set; }
        public long? ContainerTypeId { get; set; }
        public long? CurrentMaterialId { get; set; }
        public string Name { get; set; }

        public virtual ContainerType ContainerType { get; set; }
        public virtual Material CurrentMaterial { get; set; }
        public virtual ICollection<DosingSource> DosingSources { get; set; }
    }
}
