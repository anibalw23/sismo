using Monitoreo.Models.BO.ViewModels.SuperCicloFormativoVm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.ViewModels
{
    public class DocenteDetailsAcompananteVM
    {
        public DocenteDetailsAcompananteVM() {
            ciclosFormativos = new List<SuperCicloFormativoVM>();
            inscripciones = new List<Inscripcion>();        
        }


        //Datos del Docente
        public string nombreDocente { get; set; }
        public string cedula { get; set; }
        public string sexo { get; set; }
        public int edad { get; set; }
        public string telefono { get; set; }
        public string nivel { get; set; }
        public string grado { get; set; }
        public string tanda { get; set; }


        public List<AsistenciaAcompanamientoVM> asistenciasAcompanamientos { get; set; }
        public List<DocenteMateria> materias { get; set; }
        public List<Inscripcion> inscripciones { get; set; }
        public List<SuperCicloFormativoVM> ciclosFormativos { get; set; }
        public List<CalendarioCicloFormativo> fechasCiclosFormativos { get; set; }
        public List<Ausencia> ausencias { get; set; }

    }
}