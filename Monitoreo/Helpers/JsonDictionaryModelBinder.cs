using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Monitoreo.Helpers
{
    public class JsonDictionary : Dictionary<string, object>
    {
        public JsonDictionary() { }

        public void Add(JsonDictionary jsonDictionary)
        {
            if (jsonDictionary != null)
            {
                foreach (var k in jsonDictionary.Keys)
                {
                    this.Add(k, jsonDictionary[k]);
                }
            }
        }
    }

    public class JsonDictionaryModelBinder : IModelBinder
    {
        #region IModelBinder Members

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            //if (bindingContext.Model == null) { 
            //    bindingContext.Model = new JsonDictionary(); 
            //}

            var model = new JsonDictionary();

            if (bindingContext.ModelType == typeof(JsonDictionary))
            {
                // Deserialize each form/querystring item specified in the "includeProperties"
                // parameter that was passed to the "UpdateModel" method call

                // Check/Add Form Collection
                this.addRequestValues(
                    model,
                    controllerContext.RequestContext.HttpContext.Request.Form,
                    controllerContext, bindingContext);

                // Check/Add QueryString Collection
                this.addRequestValues(
                    model,
                    controllerContext.RequestContext.HttpContext.Request.QueryString,
                    controllerContext, bindingContext);
            }

            return model;
        }

        #endregion

        private void addRequestValues(JsonDictionary model, NameValueCollection nameValueCollection, ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            foreach (string key in nameValueCollection.Keys)
            {
                if (bindingContext.PropertyFilter(key))
                {
                    var jsonText = nameValueCollection[key];
                    var newModel = deserializeJson(jsonText);
                    // Add the new JSON key/value pairs to the Model
                    model.Add(newModel);
                }
            }
        }

        private JsonDictionary deserializeJson(string json)
        {
            // Must Reference "System.Web.Extensions" in order to use the JavaScriptSerializer
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Deserialize<JsonDictionary>(json);
        }
    }
}