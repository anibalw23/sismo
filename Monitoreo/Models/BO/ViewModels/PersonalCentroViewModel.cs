using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class PersonalCentroViewModel
    {
        //public int ID { get; set; }
        public string centro { get; set; } 
        public string cedula { get; set; }
        public string nombres { get; set; }
        public string telefono { get; set; }
        //public string distrito { get; set; }
        //public string red { get; set; }               
        public string sexo { get; set; }
        public string area { get; set; }
        public string nivel { get; set; }
        public string ciclo { get; set; }
        public string tanda { get; set; }
        public string grado { get; set; }
        public string seccion { get; set; }
        public string tipo { get; set; }
        public string funcionEjerce { get; set; }
        public string formacionAcademica { get; set; }
    }
}