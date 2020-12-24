using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class Material
    {
        public Material()
        {
            Containers = new HashSet<Container>();
            RecipeMaterials = new HashSet<RecipeMaterial>();
        }

        public long Id { get; set; }
        public string Abbreviation { get; set; }
        public long? MaterialTypeId { get; set; }
        public string Name { get; set; }

        public virtual MaterialType MaterialType { get; set; }
        public virtual ICollection<Container> Containers { get; set; }
        public virtual ICollection<RecipeMaterial> RecipeMaterials { get; set; }
    }
}
