using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class AsistenciaReporteViewModel
    {
        public int ID { get; set; }
        public string cedula { get; set; }
        public string nombre { get; set; }
        public string centroPertenece { get; set; }
        public string redPertenece { get; set; }
        public string cicloTema { get; set; }
        public string cicloSeccion { get; set; }
        public double porcentajeAsistencia { get; set; }
        public int horasAsistidas { get; set; }
        public int totalDias { get; set; }
        public int totalHoras { get; set; }

    }
}