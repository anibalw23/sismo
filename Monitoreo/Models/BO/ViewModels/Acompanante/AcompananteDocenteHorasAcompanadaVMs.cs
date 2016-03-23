using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.ViewModels.Acompanante
{
    public class AcompananteDocenteHorasAcompanadaVMs
    {
        public int horasAcompananmientoAula { get; set; }
        public int horasAcompananmientoTutorial { get; set; }
        public int horasAcompananmientoModelo { get; set; }
        public int horasAcompananmientoGrupoPedagogico { get; set; }
        public int horasAcompanamientoTotal { get; set; }
    }
}