using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.ViewModels
{
    public class EvaluacionPersonalVM
    {
        public string nombreEvaluacion { get; set; }
        public string tipoEvaluacion { get; set; }
        public List<Pregunta> preguntas { get; set; }
    }
}