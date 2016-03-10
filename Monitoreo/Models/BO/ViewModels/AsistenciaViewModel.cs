using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class AsistenciaViewModel
    {
        public int ID { get; set; }
        public int personaId { get; set; }
        public string cedula { get; set; }
        public int cicloID { get; set; }
        public double porcentajeAsistencia { get; set; }
        public int horasAsistidad { get; set; }

    }
}