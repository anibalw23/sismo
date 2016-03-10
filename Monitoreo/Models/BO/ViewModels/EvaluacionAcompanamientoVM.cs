using Monitoreo.Models.BO.EvaluacionAcompanamiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.ViewModels
{
    public class EvaluacionAcompanamientoVM
    {
        public int Id { get; set; }
        public string Titulo { get;set;}
        TipoEvaluacionAcompanamiento TipoEvaluacionAcomp { get; set; }

    }
}