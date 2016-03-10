using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.ViewModels
{
    public class ResumenEvalsByCiclo
    {
        public string ciclo { get; set; }
        public string centro { get; set; }
        public string pregunta { get; set; }
        public double respuestaAverage { get; set; }
    }
}