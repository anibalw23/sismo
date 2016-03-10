using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class PeriodoEscolar
    {
        public int ID { get; set; }
        public string nombre { get; set; }


        [Display(Name = "Fecha Inicio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime fechaInicio { get; set; }

        [Display(Name = "Fecha Fin")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime fechaFin { get; set; }

    }
}