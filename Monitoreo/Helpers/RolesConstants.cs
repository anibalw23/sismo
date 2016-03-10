using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq;


namespace Monitoreo.Helpers
{
  
    public class RolesConstants
    {        
        public const string ADMINISTRADOR = "Administrador";
        public const string ACOMPANANTE = "Acompanante";
        public const string COORDINADOR = "Coordinador";
        public const string ADMINISTRADOR_TRANSVERSAL = "AdministradorTransversal";
        public const string VISUALIZADOR = "Visualizador";
        public const string ESPECIALISTA_CURRICULAR = "EspecialistaCurricular";

        public List<string> getRolesConstants() { 
            List<string> result = new List<string>();
            result.Add(ADMINISTRADOR);
            result.Add(ACOMPANANTE);
            result.Add(COORDINADOR);
            result.Add(ADMINISTRADOR_TRANSVERSAL);
            result.Add(VISUALIZADOR);
            result.Add(ESPECIALISTA_CURRICULAR);
            return result;        
        }

    }  

}