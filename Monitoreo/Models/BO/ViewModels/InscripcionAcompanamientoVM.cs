using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.ViewModels
{
    public class InscripcionAcompanamientoVM
    {
        public string cedula { get; set; }
        public string nombre { get; set; }
        public int horas { get; set; }
        public string tipoActividad { get; set; }
    }
}