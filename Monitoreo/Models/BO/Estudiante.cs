using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    public class Estudiante
    {
        public int Id { get; set; }

        [Required]
        public int PersonaId { get; set; }
        public virtual Persona Persona { get; set; }

        [Required]
        [Display(Name = "Matricula", ResourceType = typeof(Resources.T))]
        public string Matricula { get; set; }

        [Required]
        public int CentroId { get; set; }
        public virtual Centro Centro { get; set; }

        public int? SeccionId { get; set; }
        [Display(Name = "Seccion", ResourceType = typeof(Resources.T))]
        public Seccion Seccion { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public NivelesCentro Nivel { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public DocenteCiclo Ciclo { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public TandasCentro Tanda { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public Grado Grado { get; set; }

        //public virtual ICollection<Inscripcion> Inscripciones { get; set; }
    }
}