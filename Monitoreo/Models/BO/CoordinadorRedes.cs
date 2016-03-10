using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class CoordinadorRedes
    {
        public int ID { get; set; }

        public int CoordinadorID { get; set; }
        public virtual Coordinador Coordinador { get; set; }

        public int RedID { get; set; }
        public virtual Red Red { get; set; }

    }
}