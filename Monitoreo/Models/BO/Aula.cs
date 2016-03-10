using Monitoreo.Models.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    /***
     * Aplica a Taller, Conferencia, Etc.
    ***/
    public class CaracteristicaAula
	{
        public int Id { get; set; }

        public int AulaId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public virtual Aula Aula { get; set; }
	}

    public class Aula
    {
        public int Id { get; set; }        
        
        //Centro
        public int CentroId { get; set; }
        public virtual Centro Centro { get; set; }
        //Grado
        //public int GradoId { get; set; }
        //public virtual GradoCentro Grado { get; set; }
        ////Secciones
        public virtual ICollection<SeccionAula> SeccionesAula { get; set; }
        
        public virtual ICollection<CaracteristicaAula> Caracteristicas { get; set; }
    }
}