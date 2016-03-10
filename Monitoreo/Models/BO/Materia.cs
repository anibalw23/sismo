using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class Materia
    {
        public int ID { get; set; }
        public string nombreMateria { get; set; }
        public bool esBasica { get; set; }
        public bool esTecnica { get; set; }
    }
}