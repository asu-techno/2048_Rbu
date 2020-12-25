using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class RecipeType
    {
        public RecipeType()
        {
            RecipeMaterialTypes = new HashSet<RecipeMaterialType>();
            Recipes = new HashSet<Recipe>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<RecipeMaterialType> RecipeMaterialTypes { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
