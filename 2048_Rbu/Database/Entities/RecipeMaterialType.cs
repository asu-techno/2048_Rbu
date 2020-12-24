using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class RecipeMaterialType
    {
        public long Id { get; set; }
        public int Order { get; set; }
        public long? RecipeTypeId { get; set; }
        public long? MaterialTypeId { get; set; }

        public virtual MaterialType MaterialType { get; set; }
        public virtual RecipeType RecipeType { get; set; }
    }
}
