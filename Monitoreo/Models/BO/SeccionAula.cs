using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class SeccionAula
    {

        public int Id { get; set; }

        public int gradoId { get; set; }
        public virtual CentroGrado Grado { get; set; }

        public int SeccionId { get; set; }
        public virtual Seccion Seccion { get; set; }

        public string tanda { get; set; }

        public virtual ICollection<SeccionPersonalMateria> SeccionesPersonalMateria { get; set; }

    }
}