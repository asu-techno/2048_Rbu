using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class Task
    {
        public Task()
        {
            Reports = new HashSet<Report>();
            TaskQueueItems = new HashSet<TaskQueueItem>();
        }

        public long Id { get; set; }
        public long? CustomerId { get; set; }
        public long? RecipeId { get; set; }
        public decimal Volume { get; set; }
        public decimal BatchesAmount { get; set; }
        public decimal BatchVolume { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Recipe Recipe { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<TaskQueueItem> TaskQueueItems { get; set; }
    }
}
