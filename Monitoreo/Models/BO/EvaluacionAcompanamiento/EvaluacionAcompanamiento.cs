using Monitoreo.Models.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.EvaluacionAcompanamiento
{

    public enum TipoEvaluacionAcompanamiento : int
    {
        Acompanamientoaula = 1, AcompanamientoTutorial = 2, ClaseModelo = 3, GrupoPedagogico = 4
    }

    public class EvaluacionAcompanamiento
    {

        public int Id { get; set; }

        [Required]
        [Display(Name = "Titulo", ResourceType = typeof(Resources.T))]
        public string Titulo { get; set; }


        [Display(Name = "TipoEvaluacion")]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public TipoEvaluacionAcompanamiento TipoEvaluacionAcomp { get; set; }



        public virtual List<SuperCicloFormativo> SuperCicloFormativo { get; set; }

               

        [Display(Name = "Creado Por")]
        public string creadoPor { get; set; }

        private List<EvaluacionAcompanamientoPregunta> fPreguntas;
        [NotMapped]
        [Display(Name = "Preguntas", ResourceType = typeof(Resources.T))]
        public virtual List<EvaluacionAcompanamientoPregunta> PreguntasAcomp
        {

            get
            {

                MonitoreoContext db = new MonitoreoContext();
                fPreguntas = new List<EvaluacionAcompanamientoPregunta>();

                foreach (var item in db.EvaluacionAcompanamientoPreguntas)
                {
                    if ((item.EvaluacionAcompId == this.Id || item.EvaluacionAcompId == null))
                    {
                        fPreguntas.Add(item);
                    }
                }


                return fPreguntas;
            }
        }



        [Display(Name = "Respuestas", ResourceType = typeof(Resources.T))]
        public virtual List<EvaluacionAcompanamientoRespuesta> RespuestasAcomp { get; set; }


    }
}