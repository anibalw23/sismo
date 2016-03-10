using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class PersonalMateria
    {
        public int ID { get; set; }

        public int DocenteId { get; set; }
        public Docente Docente { get; set; }

        //PersonalMateria
        public virtual PeriodoEscolar PeriodoEscolar { get; set; }
        public int PeriodoEscolarID { get; set; }

        //Area
        public int MateriaId { get; set; }
        public virtual Materia Materia { get; set; }

        public virtual ICollection<SeccionPersonalMateria> SeccionesPersonalMateria { get; set; }

    }
}