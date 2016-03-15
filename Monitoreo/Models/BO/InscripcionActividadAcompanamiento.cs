using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class InscripcionActividadAcompanamiento
    {
        public int ID { get; set; }

        public int personalID { get; set; }
        public virtual Personal Personal { get; set; }

        public int actividadAcompanamientoID { get; set; }
        public virtual ActividadAcompanamiento ActividadAcompanamiento { get; set; }

        public DateTime fecha { get; set; }
        public int horas { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public DocenteArea Area { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public Grado Grado { get; set; }
        
        //[DataType(DataType.MultilineText)]
        public string comentario { get; set; }

        public string userId { get; set; }

    }
}