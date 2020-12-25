using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class BatcherOpcParameter
    {
        public long Id { get; set; }
        public long? BatcherId { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public int CharactersAmount { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }

        public virtual Batcher Batcher { get; set; }
    }
}
