using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.ViewModels
{
    public class AssitenciaVM
    {
        public int participanteID { get; set; }
        public string participanteCedula { get; set; }
        public string participanteNombre { get; set; }
        public bool  asistio { get; set; }
    }
}