using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{

    public enum TipoAcompanamiento : int
    {
        [Display(Name = "Acompañamiento en Aula")]
        AcompanamientoAula = 1,
        [Display(Name = "Acompañamiento Tutorial")] //Acompañamiento Tutorial
        AcompanamientoTutorial = 2,
        [Display(Name = "Clase Modelo")]
        ClaseModelo = 3,
        [Display(Name = "Comunidad de Aprendizaje")] // Grupos Pedagogicos
        GrupoPedagogico = 4       
    }


    public class ActividadAcompanamiento
    {

        public int ID { get; set; }

        public virtual SuperCicloFormativo SuperCicloFormativo { get; set; }
        public int SuperCicloFormativoId { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public TipoAcompanamiento TipoAcompanamiento { get; set; }

        public DocenteArea Area { get; set; }

        public virtual ICollection<InscripcionActividadAcompanamiento> inscripcionesActividadesacompanamiento { get; set; }

    }
}