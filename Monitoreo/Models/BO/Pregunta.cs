using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public enum TipoDeObjeto
    {
        [Display(Name = "Ciclo", ResourceType = typeof(Resources.T))]
        Ciclo = 1,
        [Display(Name = "Regional", ResourceType = typeof(Resources.T))]
        Regional,
        [Display(Name = "Distrito", ResourceType = typeof(Resources.T))]
        Distrito,
        [Display(Name = "Red", ResourceType = typeof(Resources.T))]
        Red,
        [Display(Name = "Centro", ResourceType = typeof(Resources.T))]
        Centro,
        [Display(Name = "Personal", ResourceType = typeof(Resources.T))]
        Personal,
        [Display(Name = "Docente", ResourceType = typeof(Resources.T))]
        Docente,
        [Display(Name = "Estudiante", ResourceType = typeof(Resources.T))]
        Estudiante
    }

    public enum NivelDominio
    {
        [Display(Name = "Contenido", ResourceType = typeof(Resources.T))]
        Contenido = 1,
        [Display(Name = "Actitudinal", ResourceType = typeof(Resources.T))]
        Actitudinal,
        [Display(Name = "Procedimental", ResourceType = typeof(Resources.T))]
        Procedimental
    }

    public enum TipoEvaluacion
    {
        [Display(Name = "PreTest", ResourceType = typeof(Resources.T))]
        PreTest = 1,
        [Display(Name = "Acompanamiento  Aula")]
        AcompanamientoAula = 2,
        [Display(Name = "PostTest")]
        PostTest = 3,
        [Display(Name = "Clase Modelo")]
        AcompanamientoClaseModelo =4,
        [Display(Name = "Grupo Pedagógico")]
        AcompanamientoGrupoPedagogico = 5,
    }

    public class PreguntaOpcion
    {
        public int Id { get; set; }

        public int PreguntaId { get; set; }

        [Display(Name = "Pregunta", ResourceType = typeof(Resources.T))]
        public virtual Pregunta Pregunta { get; set; }

        [Display(Name = "Titulo", ResourceType = typeof(Resources.T))]
        [Required]
        public string Titulo { get; set; }

        [Display(Name = "Valor", ResourceType = typeof(Resources.T))]
        [Required]
        public string Valor { get; set; }

        [Display(Name = "Correcta", ResourceType = typeof(Resources.T))]
        public bool Correcta { get; set; }
    }

    public class Pregunta
    {
        public Pregunta()
        {
            Opciones = new List<PreguntaOpcion>();
        }

        public int Id { get; set; }

        [Display(Name = "Descripcion", ResourceType = typeof(Resources.T))]
        [Required]
        public string Descripcion { get; set; }

        [Display(Name = "NivelDominio", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public NivelDominio NivelDominio { get; set; }

        //Esto lo quite yo ahora
        //[Display(Name = "TipoEvaluacion", ResourceType = typeof(Resources.T))]
        //[System.ComponentModel.DataAnnotations.UIHint("Enum")]
        //public TipoEvaluacion TipoEvaluacion { get; set; }

        //[Display(Name = "TipoDeObjeto", ResourceType = typeof(Resources.T))]
        //[System.ComponentModel.DataAnnotations.UIHint("Enum")]
        //public TipoDeObjeto TipoDeObjeto { get; set; }

        public int? CicloFormativoId { get; set; }
        [Display(Name = "CicloFormativo", ResourceType = typeof(Resources.T))]
        public virtual CicloFormativo CicloFormativo { get; set; }

        public int? EvaluacionId { get; set; }
        [Display(Name = "Evaluacion")]
        public virtual Evaluacion Evaluacion { get; set; }
        
        public virtual List<PreguntaOpcion> Opciones { get; set; }
    }
}