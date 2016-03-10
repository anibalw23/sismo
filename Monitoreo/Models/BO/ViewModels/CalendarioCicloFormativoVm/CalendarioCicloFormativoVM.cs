using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.ViewModels.CalendarioCicloFormativoVm
{
    public class CalendarioCicloFormativoVM
    {
        public int Id { get; set; } //calendarioId
        public DateTime Fecha { get; set; }
        public int horas { get; set; }
        public int CicloFormativoID { get; set; }
        
    }
}