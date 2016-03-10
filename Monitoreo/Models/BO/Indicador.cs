using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{

    [Flags]
    public enum tipoIndicador : int
    {
        Impacto = 1,
        Proceso = 2,
    }

    [Flags]
    public enum FrecuenciaMedicion : int
    {
        Mensual = 1,
        Trimestral = 2,
        Cuatrimestral = 4,
        Semestral = 8,
        Anual = 16,
    }

    public class Indicador
    {
        public int ID { get; set; }
        public string nombre { get; set; }
        public string codigo { get; set; }
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public tipoIndicador tipo { get; set; }

        public int valorLineaBase { get; set; }
        public string descripcion { get; set; }
        public string formula { get; set; }
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public FrecuenciaMedicion Frecuencia { get; set; }
        public virtual ICollection<indicadorResultado> Resultados { get; set; } 

    }
}