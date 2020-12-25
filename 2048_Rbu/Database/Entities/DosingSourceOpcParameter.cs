using System;
using System.Collections.Generic;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class DosingSourceOpcParameter
    {
        public long Id { get; set; }
        public long? DosingSourceId { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public int CharactersAmount { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }

        public virtual DosingSource DosingSource { get; set; }
    }
}
