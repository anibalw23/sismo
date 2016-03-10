using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class IndicadorFecha
    {
        public int Id { get; set; }
        public int mes { get; set; }
        public int cuarto { get; set; }
        public int anio { get; set; }
    }
}