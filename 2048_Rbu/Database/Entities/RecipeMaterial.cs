using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class RecipeMaterial
    {
        public long Id { get; set; }
        public int Order { get; set; }
        public decimal Volume { get; set; }
        public long? MaterialId { get; set; }
        public long? RecipeId { get; set; }

        public virtual Material Material { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
