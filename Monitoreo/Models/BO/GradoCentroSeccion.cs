using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class GradoCentro
    {
        public int Id { get; set; }

        public virtual Centro Centro { get; set; }
        public int CentroId { get; set; }

        public virtual GradoLookup GradoLookup { get; set; }
        public int GradoLookupId { get; set; }

        public virtual ICollection<SeccionAula> SeccionesAulas { get; set; }

    }
}