using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Data.Entities
{
    public class CaseItem
    {
        public Guid CaseId { get; set; }
        public Guid ItemId { get; set; }
        public Case Case { get; set; } = null!;
        public Item Item { get; set; } = null!;
    } 
}

