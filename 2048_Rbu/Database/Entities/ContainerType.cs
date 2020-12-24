using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class ContainerType
    {
        public ContainerType()
        {
            Containers = new HashSet<Container>();
        }

        public long Id { get; set; }
        public long? MaterialTypeId { get; set; }
        public string Name { get; set; }

        public virtual MaterialType MaterialType { get; set; }
        public virtual ICollection<Container> Containers { get; set; }
    }
}
