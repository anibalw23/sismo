using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{

    public class CalendarioCicloFormativo
    {
        public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }

        public int horas { get; set; }


        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public TipoModuloFormativo TipoEvento { get; set; }

        //public virtual ActividadFormativa ActividadFormativa { get; set; }
        //public int ActividadFormativaID { get; set; }

        public virtual CicloFormativo CicloFormativo { get; set; }
        public int CicloFormativoID { get; set; }
    }
}

