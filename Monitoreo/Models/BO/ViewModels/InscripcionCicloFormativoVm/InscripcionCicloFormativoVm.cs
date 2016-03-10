using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.ViewModels.InscripcionCicloFormativoVm
{
    public class InscripcionCicloFormativoVm
    {
        public int Id { get; set; }
        public string cedula { get; set; }
        public string nombre { get; set; }
        public string grupo { get; set; }
        public string rol { get; set; }
    }
}