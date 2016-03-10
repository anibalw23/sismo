using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    public class Distrito
    {
        public Distrito()
        {
            this.Telefono = new Telefono();
        }

        public int Id { get; set; }

        [Display(Name = "Codigo", ResourceType = typeof(Resources.T))]
        public string Codigo { get; set; }

        [Display(Name = "Nombre", ResourceType = typeof(Resources.T))]
        public string Nombre { get; set; }

        public int RegionalId { get; set; }

        [Display(Name = "Regional", ResourceType = typeof(Resources.T))]
        public virtual Regional Regional { get; set; }

        public int? ProvinciaId { get; set; }

        [Display(Name = "Provincia", ResourceType = typeof(Resources.T))]
        public virtual Provincia Provincia { get; set; }

        public int? MunicipioId { get; set; }

        [Display(Name = "Municipio", ResourceType = typeof(Resources.T))]
        public virtual Municipio Municipio { get; set; }

        [Display(Name = "Sector", ResourceType = typeof(Resources.T))]
        public string Sector { get; set; }

        [Display(Name = "Calle", ResourceType = typeof(Resources.T))]
        public string Calle { get; set; }

        [Display(Name = "Telefono", ResourceType = typeof(Resources.T))]
        public Telefono Telefono { get; set; }

        [Display(Name = "CorreoElectronico", ResourceType = typeof(Resources.T))]
        public string CorreoElectronico { get; set; }

        [Display(Name = "SitioWeb", ResourceType = typeof(Resources.T))]
        public string SitioWeb { get; set; }

        [Display(Name = "Redes", ResourceType = typeof(Resources.T))]
        public virtual ICollection<Red> Redes { get; set; }

        [Display(Name = "Centros", ResourceType = typeof(Resources.T))]
        public virtual ICollection<Centro> Centros { 
            get {
                List<Centro> result = new List<Centro>();

                if (this.Redes != null)
                {
                    foreach (var red in this.Redes)
                    {
                        if (red.Centros != null)
                        {
                            result.AddRange(red.Centros);
                        }
                    }
                }

                return result;
            } 
        }

        enum TipoPersonalEnum { Funcionario, TecnicoDocente, PersonalAdministrativo }
        bool FlagIsSet (int flags, int flag)
        {
            return ((flags & flag) == flag);
        }

        TipoPersonalEnum TipoPersonal(PersonalFuncion funciones)
        {
            if (FlagIsSet((int)funciones, (int)PersonalFuncion.Directora)) return Distrito.TipoPersonalEnum.Funcionario;
            if (FlagIsSet((int)funciones, (int)PersonalFuncion.SubDirectora)) return Distrito.TipoPersonalEnum.Funcionario;
            if (FlagIsSet((int)funciones, (int)PersonalFuncion.CoordinadoraDocente))  return Distrito.TipoPersonalEnum.TecnicoDocente;
            if (FlagIsSet((int)funciones, (int)PersonalFuncion.SecretariaDocente))  return Distrito.TipoPersonalEnum.TecnicoDocente;
            if (FlagIsSet((int)funciones, (int)PersonalFuncion.Docente)) return Distrito.TipoPersonalEnum.TecnicoDocente;

            return TipoPersonalEnum.PersonalAdministrativo;
        }
        List<Personal> GetPersonalList (TipoPersonalEnum tipo)
        {
            List<Personal> result = new List<Personal>();

            if (this.Personal != null)
            {
                foreach (var personal in this.Personal)
                {
                    if (TipoPersonal(personal.FuncionesEjerce) == tipo)
                    {
                        result.Add(personal);
                    }
                }
            }

            return result;
        }

        [Display(Name = "Funcionarios", ResourceType = typeof(Resources.T))]
        [NotMapped]
        public virtual ICollection<Personal> Funcionarios { get { return GetPersonalList(TipoPersonalEnum.Funcionario); } }

        [Display(Name = "TecnicosDocente", ResourceType = typeof(Resources.T))]
        [NotMapped]
        public virtual ICollection<Personal> TecnicosDocente { get { return GetPersonalList(TipoPersonalEnum.TecnicoDocente); } }

        [Display(Name = "PersonalAdministrativo", ResourceType = typeof(Resources.T))]
        [NotMapped]
        public virtual ICollection<Personal> PersonalAdministrativo { get { return GetPersonalList(TipoPersonalEnum.PersonalAdministrativo); } }

        [Display(Name = "Personal", ResourceType = typeof(Resources.T))]
        public virtual ICollection<Personal> Personal { get; set; }

        public int? CentroSedeId { get; set; }

        [Display(Name = "CentroSede", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.ReadOnly(true)]
        public virtual Centro CentroSede { get; set; }
    }
}