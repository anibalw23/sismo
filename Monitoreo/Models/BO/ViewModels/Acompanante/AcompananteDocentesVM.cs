using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.ViewModels.Acompanante
{
    public class AcompananteDocentesVM
    {
        public int Id { get; set; }
        public string cedula { get; set; }
        public string nombre { get; set; }
        public List<DocenteMateria> Materias { get; set; }
    }
}