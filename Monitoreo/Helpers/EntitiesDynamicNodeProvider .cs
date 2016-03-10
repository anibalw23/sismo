using Monitoreo.Models.DAL;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Helpers
{
    public class EntitiesDynamicNodeProvider : DynamicNodeProviderBase
    {
        MonitoreoContext context = new MonitoreoContext();

        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            // Build value 
            var returnValue = new List<DynamicNode>();
            var type = context.GetType();
            var properties = type.GetProperties();

            var catAdministracion = new DynamicNode { Title = Categorias.catAdm, Key = Guid.NewGuid().ToString(), Clickable = false };
            var catOperativo = new DynamicNode { Title = Categorias.catOp, Key = Guid.NewGuid().ToString(), Clickable = false };
            var catSeguimiento = new DynamicNode { Title = Categorias.catSeg, Key = Guid.NewGuid().ToString(), Clickable = false };

            returnValue.Add(catAdministracion);
            returnValue.Add(catOperativo);
            returnValue.Add(catSeguimiento);

            var categorias = returnValue.ToDictionary(x => x.Title, x => x.Key);

            foreach (var entity in properties)
            {
                Type tipo = entity.PropertyType;
                if (tipo.IsGenericType && tipo.GetGenericTypeDefinition() == typeof(System.Data.Entity.DbSet<>))
                {
                    string entityName = entity.Name; ;
                    var innerType = tipo.GetGenericArguments()[0];
                    DynamicNode menuNode = new DynamicNode();
                    menuNode.Title = Resources.Mvc.sitemap.ResourceManager.GetString(entityName);
                    if (string.IsNullOrEmpty(menuNode.Title)) menuNode.Title = entityName;
                    menuNode.Controller = innerType.Name;
                    menuNode.Action = "Index";
                    //menuNode.ParentKey = "Administracion";
                    menuNode.Key = Guid.NewGuid().ToString();

                    object[] browsableAtts = entity.GetCustomAttributes(typeof(System.ComponentModel.BrowsableAttribute), false);
                    if (browsableAtts.Count() > 0) menuNode.Attributes["visibility"] = "SiteMapPathHelper,!*";

                    object[] categoryAtts = entity.GetCustomAttributes(typeof(System.ComponentModel.CategoryAttribute), false);
                    if (categoryAtts.Count() > 0)
                    {
                        string category = (categoryAtts[0] as System.ComponentModel.CategoryAttribute).Category;
                        if (categorias.ContainsKey(category))
                        {
                            menuNode.ParentKey = categorias[category];
                        }
                    }

                    DynamicNode createNewNode = new DynamicNode();
                    createNewNode.Title = "$resources:Mvc.sitemap,CrearNuevo";
                    createNewNode.Controller = innerType.Name;
                    createNewNode.Action = "Create";
                    createNewNode.Attributes["visibility"] = "SiteMapPathHelper,!*";
                    createNewNode.ParentKey = menuNode.Key;

                    DynamicNode detailsNode = new DynamicNode();
                    detailsNode.Title = Resources.Mvc.sitemap.ResourceManager.GetString(innerType.Name);
                    if (string.IsNullOrEmpty(detailsNode.Title)) detailsNode.Title = innerType.Name;
                    //detailsNode.Title = String.Format("$resources:Mvc.sitemap,{0}",innerType.Name);
                    detailsNode.Controller = innerType.Name;
                    detailsNode.Action = "Details";
                    detailsNode.PreservedRouteParameters = new string[]{"id"};
                    detailsNode.Attributes["visibility"] = "SiteMapPathHelper,!*";
                    detailsNode.Key = Guid.NewGuid().ToString();
                    detailsNode.ParentKey = menuNode.Key;

                    DynamicNode editNode = new DynamicNode();
                    editNode.Title = "$resources:Mvc.sitemap,Editar";
                    editNode.Controller = innerType.Name;
                    editNode.Action = "Edit";
                    editNode.PreservedRouteParameters = new string[] { "id" };
                    editNode.Attributes["visibility"] = "SiteMapPathHelper,!*";
                    editNode.ParentKey = detailsNode.Key;

                    DynamicNode deleteNode = new DynamicNode();
                    deleteNode.Title = "$resources:Mvc.sitemap,Borrar";
                    deleteNode.Controller = innerType.Name;
                    deleteNode.Action = "Delete";
                    deleteNode.PreservedRouteParameters = new string[] { "id" };
                    deleteNode.Attributes["visibility"] = "SiteMapPathHelper,!*";
                    deleteNode.ParentKey = detailsNode.Key;

                    returnValue.Add(menuNode);
                    returnValue.Add(createNewNode);
                    returnValue.Add(detailsNode);
                    returnValue.Add(editNode);
                    returnValue.Add(deleteNode);
                }
            }

            // Return 
            return returnValue;
        }
    }
}