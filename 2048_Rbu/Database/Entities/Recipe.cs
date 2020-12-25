using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class Recipe
    {
        public Recipe()
        {
            RecipeMaterials = new HashSet<RecipeMaterial>();
            Tasks = new HashSet<Task>();
        }

        public long Id { get; set; }
        public long? GroupId { get; set; }
        public long? RecipeTypeId { get; set; }
        public decimal MixingTime { get; set; }
        public decimal PercentOpenGate { get; set; }
        public decimal TimePartialUnload { get; set; }
        public decimal TimeFullUnload { get; set; }
        public long? RecipeGroupId { get; set; }
        public string Name { get; set; }

        public virtual RecipeGroup Group { get; set; }
        public virtual RecipeGroup RecipeGroup { get; set; }
        public virtual RecipeType RecipeType { get; set; }
        public virtual ICollection<RecipeMaterial> RecipeMaterials { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
