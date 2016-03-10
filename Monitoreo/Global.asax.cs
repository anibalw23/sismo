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
