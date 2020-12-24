using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class TaskQueueItem
    {
        public long Id { get; set; }
        public long? TaskId { get; set; }
        public int Order { get; set; }

        public virtual Task Task { get; set; }
    }
}
