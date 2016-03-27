using FluentSecurity;
using Monitoreo.Controllers;
using Monitoreo.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Monitoreo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
          
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            // Register custom flag enum model binder
            ModelBinders.Binders.DefaultBinder = new Monitoreo.Utility.CustomModelBinder();
            //ModelBinders.Binders.Add(typeof(JsonDictionary), new JsonDictionaryModelBinder());
            MvcHandler.DisableMvcResponseHeader = true; // Esto loa agrege para la seguridad
            ConfigureSecurity(); // Fluent Security Filter
        }


        protected void ConfigureSecurity() {           

            SecurityConfigurator.Configure(configuration =>
            {
                // Let FluentSecurity know how to get the authentication status of the current user
                configuration.GetAuthenticationStatusFrom(() => HttpContext.Current.User.Identity.IsAuthenticated);               
                configuration.GetRolesFrom(() =>
                   getUserRoles()
                );
                configuration.DefaultPolicyViolationHandlerIs(() => new LocalOnlyPolicyViolationHandler());
                configuration.Advanced.IgnoreMissingConfiguration();

                //HomeController
                configuration.For<HomeController>().DenyAnonymousAccess();
                //AccountController
                configuration.For<AccountController>(x => x.Login(default(string))).Ignore(); //Account Login
                configuration.For<AccountController>(x => x.CreateUser()).DenyAnonymousAccess(); //Account Create User
                configuration.For<AccountController>(x => x.LogOff()).DenyAnonymousAccess(); //Account Logoff
                //Manejo de Roles y Usuarios
                configuration.For<AccountController>(x => x.RoleList()).RequireAnyRole(new[] { "Administrador" });
                configuration.For<AccountController>(x => x.CreateRole(default(string))).RequireAnyRole("Administrador");
                configuration.For<AccountController>(x => x.DeleteRol()).RequireAnyRole("Administrador");
                configuration.For<AccountController>(x => x.EditRol(default(string))).RequireAnyRole("Administrador");
                configuration.For<AccountController>(x => x.GetUsersJson(null)).RequireAnyRole("Administrador");

                //DocenteController
                configuration.For<DocenteController>(x => x.Index()).RequireAnyRole(new[] { "Administrador", "Acompanante", "EspecialistaCurricular" });
                configuration.For<DocenteController>(x => x.GetDataJson(null)).RequireAnyRole(new[] { "Administrador", "Acompanante", "EspecialistaCurricular" });
                configuration.For<DocenteController>(x => x.DocentesFilterByCriteria()).RequireAnyRole(new[] { "Administrador","Acompanante","AdministradorTransversal","Coordinador","Visualizador","EspecialistaCurricular" });
                configuration.For<DocenteController>(x => x.GetDocenteByCentrosAreasGrados(null, null, null, true)).RequireAnyRole(new[] { "Administrador","Acompanante","AdministradorTransversal","Coordinador","Visualizador","EspecialistaCurricular" });
                configuration.For<DocenteController>(x => x.GetActividadesAcompompanamientoIds(0)).RequireAnyRole(new[] { "Administrador","Acompanante","Coordinador" });
                configuration.For<DocenteController>(x => x.GetActividadesPresencialesByCicloByPersona(0,0)).RequireAnyRole(new[] { "Administrador","Acompanante","Coordinador" });
                configuration.For<DocenteController>(x => x.GetInscripcionesActividadByCicloPersonaId(0, 0, 0, 0)).RequireAnyRole(new[] { "Administrador","Acompanante","Coordinador" });
                configuration.For<DocenteController>(x => x.InsripcionActividadesAcompanamientoByCicloPersonaId(0, 0, null, null, 0)).RequireAnyRole(new[] { "Administrador","Acompanante","Coordinador" });
                configuration.For<DocenteController>(x => x.DocenteDetailsAcompanante(0)).RequireAnyRole(new[] { "Acompanante","Coordinador" });
                configuration.For<DocenteController>(x => x.DocenteDetailsCoordinador(0)).RequireAnyRole(new[] { "Acompanante","Coordinador" });
                configuration.For<DocenteController>(x => x.DocenteDetailsCoordinadorInicial(0, 0)).RequireAnyRole(new[] { "Acompanante","Coordinador" });
                configuration.For<DocenteController>(x => x.CentroDocentes(0)).RequireAnyRole(new[] { "Administrador","EspecialistaCurricular" });
                configuration.For<DocenteController>(x => x.Details(0)).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular", "Acompanante" });
                configuration.For<DocenteController>(x => x.CreateModal()).RequireAnyRole(new[] { "Administrador" });
                configuration.For<DocenteController>(x => x.CreateModal(null)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<DocenteController>(x => x.Create()).RequireAnyRole(new[] { "Administrador" });
                configuration.For<DocenteController>(x => x.Create(null)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<DocenteController>(x => x.CreatePersonalMateria(0)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<DocenteController>(x => x.CreatePersonalMateria(null)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<DocenteController>(x => x.EditPersonalMateria(null, null)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<DocenteController>(x => x.EditModal(0)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<DocenteController>(x => x.setActive(0, false)).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });
                configuration.For<DocenteController>(x => x.Delete(null, null)).RequireAnyRole(new[] { "Administrador" });

                //CentrosController
                configuration.For<CentroController>(x => x.Index()).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });
                configuration.For<CentroController>(x => x.GetDataJson(null)).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });
                configuration.For<CentroController>(x => x.DistritoCentros(0)).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });
                configuration.For<CentroController>(x => x.Details(0)).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });
                configuration.For<CentroController>(x => x.Create()).RequireAnyRole(new[] { "Administrador" });
                configuration.For<CentroController>(x => x.Edit(0)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<CentroController>(x => x.Delete(0)).RequireAnyRole(new[] { "Administrador" });
                
                //RedesController
                configuration.For<RedController>(x => x.Index()).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });
                configuration.For<RedController>(x => x.GetDataJson(null)).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });
                configuration.For<RedController>(x => x.DistritoRedes(0)).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });
                configuration.For<RedController>(x => x.Details(0)).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });
                configuration.For<RedController>(x => x.Edit(0)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<RedController>(x => x.Create()).RequireAnyRole(new[] { "Administrador" });
                configuration.For<RedController>(x => x.Delete(null)).RequireAnyRole(new[] { "Administrador" });
                //DistritoController
                configuration.For<DistritoController>(x => x.Index()).RequireAnyRole(new[] { "Administrador" });
                configuration.For<DistritoController>(x => x.GetDataJson(null)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<DistritoController>(x => x.Details(null)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<DistritoController>(x => x.Create(null)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<DistritoController>(x => x.Edit(0)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<DistritoController>(x => x.Delete(0)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<DistritoController>(x => x.DeleteConfirmed(0)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<DistritoController>(x => x.getPersonalAdmin()).RequireAnyRole(new[] { "Administrador", "Acompanante", "Coordinador", "EspecialistaCurricular", "AdministradorTransversal" });
                configuration.For<DistritoController>(x => x.getPersonalAdminByCentro(0)).RequireAnyRole(new[] { "Administrador", "Acompanante", "Coordinador", "EspecialistaCurricular", "AdministradorTransversal" });
                configuration.For<DistritoController>(x => x.getPersonalAdminByRed(0)).RequireAnyRole(new[] { "Administrador", "Acompanante", "Coordinador", "EspecialistaCurricular", "AdministradorTransversal" });
                configuration.For<DistritoController>(x => x.getPersonalAdminByCentro(0)).RequireAnyRole(new[] { "Administrador", "Acompanante", "Coordinador", "EspecialistaCurricular", "AdministradorTransversal" });
                configuration.For<DistritoController>(x => x.getPersonalAdminByRed(0)).RequireAnyRole(new[] { "Administrador", "Acompanante", "Coordinador", "EspecialistaCurricular", "AdministradorTransversal" });
                configuration.For<DistritoController>(x => x.getDocentesPorGradoByRed(0)).RequireAnyRole(new[] { "Administrador", "Acompanante", "Coordinador", "EspecialistaCurricular", "AdministradorTransversal" });

                //AcompananteController
                configuration.For<AcompananteController >(x => x.Index()).RequireAnyRole(new[] { "Administrador" });
                configuration.For<AcompananteController>(x => x.AcompananteDocentes()).RequireAnyRole(new[] { "Administrador", "Acompanante", "Coordinador" });
                configuration.For<AcompananteController>(x => x.AcompanantePersonalAdministrativo()).RequireAnyRole(new[] { "Administrador", "Acompanante", "Coordinador" });
                configuration.For<AcompananteController>(x => x.GetDataJson(null)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<AcompananteController>(x => x.Create(null)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<AcompananteController>(x => x.CreateModal(null)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<AcompananteController>(x => x.CreateModal()).RequireAnyRole(new[] { "Administrador" });
                configuration.For<AcompananteController>(x => x.Edit(0)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<AcompananteController>(x => x.Delete(0)).RequireAnyRole(new[] { "Administrador" });

                //CentroController
                configuration.For<CentroController>(x => x.Index()).RequireAnyRole(new[] { "Administrador" });
                configuration.For<CentroController>(x => x.RedCentros(0)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<CentroController>(x => x.DistritoCentros(0)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<CentroController>(x => x.GetCentrosByRedesIds(new int[] { })).RequireAnyRole(new[] { "Administrador", "Acompanante", "Coordinador", "EspecialistaCurricular", "AdministradorTransversal" });
                configuration.For<CentroController>(x => x.Details(0)).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });
                configuration.For<CentroController>(x => x.Create()).RequireAnyRole(new[] { "Administrador" });
                configuration.For<CentroController>(x => x.Edit(0)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<CentroController>(x => x.Delete(0)).RequireAnyRole(new[] { "Administrador" });
                configuration.For<CentroController>(x => x.DeleteConfirmed(0)).RequireAnyRole(new[] { "Administrador" });

                //EvaluacionAcompanamientoController
                configuration.For<EvaluacionAcompanamientoController>(x => x.Index()).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });
                configuration.For<EvaluacionAcompanamientoController>(x => x.Details(0)).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });
                configuration.For<EvaluacionAcompanamientoController>(x => x.Create()).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });
                configuration.For<EvaluacionAcompanamientoController>(x => x.Edit(0)).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });
                configuration.For<EvaluacionAcompanamientoController>(x => x.Delete(0)).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });
                configuration.For<EvaluacionAcompanamientoController>(x => x.DeleteConfirmed(0)).RequireAnyRole(new[] { "Administrador", "EspecialistaCurricular" });

            });

            GlobalFilters.Filters.Add(new HandleSecurityAttribute(), 0);
        
        }


        //Esto lo agrege para la seguridad
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            if (app != null && app.Context != null)
            {
                app.Context.Response.Headers.Remove("Server");
            }
        }


        public string[] getUserRoles() { 
            string[] result = {};
            List<string> userRol = new List<string>();
            RolesConstants rols = new RolesConstants();

            foreach (var rol in rols.getRolesConstants()) {
                if (HttpContext.Current.User.IsInRole(rol))
                {
                    userRol.Add(rol);
                }            
            }
            result = userRol.ToArray<string>();
            return result;
        }
           
        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            if (arg.Equals("User", StringComparison.InvariantCultureIgnoreCase))
            {              
                return User.Identity.Name;
            }

            return base.GetVaryByCustomString(context, arg);
        }




    }
}
