using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    public enum Estado : int
    {
        Activo = 1, Cancelado
    }

    public enum TipoCursoTaller : int
    {
        Presencial = 1, Virtual
    }

    [Flags]
    public enum Grado : int
    {
        NivelInicial = 1,
        Primero = 2,
        Segundo = 4,
        Tercero = 8,
        Cuarto = 16,
        Quinto = 32,
        Sexto = 64,
        Septimo = 128,
        Octavo = 256,
        PrimeroSecundaria = 512,
        SegundoSecundaria = 1024,
        TerceroSecundaria = 2048,
        CuartoSecundaria = 4096
    }

    public class CursoTaller
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public Grado Grado { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public Seccion Seccion { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public TipoCursoTaller Tipo { get; set; }

        public int CicloFormativoId { get; set; }
        public virtual CicloFormativo CicloFormativo { get; set; }

        public int CentroId { get; set; }
        public virtual Centro Centro { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public Estado Estado { get; set; }

        public virtual ICollection<Inscripcion> Inscripciones { get; set; }
    }
}