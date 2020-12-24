using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class MaterialType
    {
        public MaterialType()
        {
            ContainerTypes = new HashSet<ContainerType>();
            Materials = new HashSet<Material>();
            RecipeMaterialTypes = new HashSet<RecipeMaterialType>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ContainerType> ContainerTypes { get; set; }
        public virtual ICollection<Material> Materials { get; set; }
        public virtual ICollection<RecipeMaterialType> RecipeMaterialTypes { get; set; }
    }
}
