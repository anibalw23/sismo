using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    public class PersonalAdministrativo : Personal
    {
        public int? DistritoId { get; set; }
        public virtual Distrito Distrito { get; set; }

        public int? RegionalId { get; set; }
        public virtual Regional Regional { get; set; }
    }
}