using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class RecipeGroup
    {
        public RecipeGroup()
        {
            RecipeGroups = new HashSet<Recipe>();
            RecipeRecipeGroups = new HashSet<Recipe>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Recipe> RecipeGroups { get; set; }
        public virtual ICollection<Recipe> RecipeRecipeGroups { get; set; }
    }
}
