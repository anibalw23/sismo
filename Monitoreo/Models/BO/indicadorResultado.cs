using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class indicadorResultado
    {
        public int ID { get; set; }
        public int valor { get; set; }
        public int valorEsperado { get; set; }

        public int IndicadorID { get; set; }
        public virtual Indicador Indicador { get; set; }

        public int IndicadorFechaID { get; set; }
        public virtual IndicadorFecha IndicadorFecha { get; set; }

        public int IndicadorDesagregacionID { get; set; }
        public virtual IndicadorDesagregacion IndicadorDesagregacion { get; set; }

    }
}