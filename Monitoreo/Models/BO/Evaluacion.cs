using Monitoreo.Models.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public enum FormaEvaluacion : int
    {
        Presencial = 1, Acompanamiento, Virtual
    }

    public enum ModoEntradaEvaluacion : int
    {
        Preguntas_Respuesta = 1, Nota_Final = 2
    }


    public class Evaluacion
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Titulo", ResourceType = typeof(Resources.T))]
        public string Titulo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha", ResourceType = typeof(Resources.T))]
        public DateTime Fecha { get; set; }

        [Display(Name = "TipoEvaluacion", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public TipoEvaluacion TipoEvaluacion { get; set; }

        [Display(Name = "FormaEvaluacion", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public FormaEvaluacion FormaEvaluacion { get; set; }

        public int CicloFormativoId { get; set; }
        [Display(Name = "CicloFormativo", ResourceType = typeof(Resources.T))]
        public virtual CicloFormativo CicloFormativo { get; set; }

        [Display(Name = "TipoDeObjeto", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public TipoDeObjeto TipoDeObjeto { get; set; }

        [Display(Name = "Creado Por")]
        public string creadoPor { get; set; }


        [NotMapped]
        [Display(Name = "Participantes", ResourceType = typeof(Resources.T))]
        public virtual List<Persona> Participantes { 
            get 
            {
                List<Persona> resp = new List<Persona>();

                if (this.CicloFormativo != null)
                {
                    foreach (var item in this.CicloFormativo.Participantes)
                    {
                        resp.Add(item.Participante);
                    }
                }

                return resp;
            } 
        }

        private List<Pregunta> fPreguntas;

        [NotMapped]
        [Display(Name = "Preguntas", ResourceType = typeof(Resources.T))]
        public virtual List<Pregunta> Preguntas
        {
            get
            {
                if (fPreguntas == null && this.ModoEntradaEvaluacion != BO.ModoEntradaEvaluacion.Nota_Final) // agregado anibal
                {
                    MonitoreoContext db = new MonitoreoContext();
                    fPreguntas = new List<Pregunta>();

                    foreach (var item in db.Preguntas)
                    {
                        /*if (item.TipoEvaluacion == this.TipoEvaluacion && (item.CicloFormativoId == this.CicloFormativoId || item.CicloFormativoId == null))
                        {
                            fPreguntas.Add(item);
                        }*/
                        if ((item.CicloFormativoId == this.CicloFormativoId || item.CicloFormativoId == null))
                        {
                            fPreguntas.Add(item);
                        }
                    }
                }

                return fPreguntas;
            }
        }

        [Display(Name = "Respuestas", ResourceType = typeof(Resources.T))]
        public virtual List<Respuesta> Respuestas { get; set; }
        
        //**************************************************************************
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public ModoEntradaEvaluacion ModoEntradaEvaluacion { get; set; }

        public virtual List<EvaluacionNotaRaw> EvaluacionNotaRaw { get; set; }
        //**************************************************************************

    }
}