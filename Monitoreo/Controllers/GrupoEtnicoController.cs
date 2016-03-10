using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Monitoreo.Models;
using Monitoreo.Models.DAL;
using System.Collections.ObjectModel;

namespace Monitoreo.Controllers
{
    public static class TypeHelper
    {
        public static object GetPropertyValue(object fromObject, string propertyName)
        {
            object propertyValue = null;

            foreach (System.Reflection.PropertyInfo p in fromObject.GetType().GetProperties())
            {
                if (p.Name == propertyName)
                {
                    propertyValue = p.GetValue(fromObject, null);
                    break;
                }
            }

            return propertyValue;
        }
    }

    public class DatatablesParams
    {
        /// <summary>
        /// Draw counter. This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by DataTables (Ajax requests are asynchronous and thus can return out of sequence). This is used as part of the draw return parameter (see below).
        /// </summary>
        public int raw { get; set; }

        /// <summary>
        /// Paging first record indicator. This is the start point in the current data set (0 index based - i.e. 0 is the first record).
        /// </summary>
        public int start { get; set; }
        
        /// <summary>
        /// Number of records that the table can display in the current draw. It is expected that the number of records returned will be equal to this number, unless the server has fewer records to return. Note that this can be -1 to indicate that all records should be returned (although that negates any benefits of server-side processing!)
        /// </summary>
        public int length { get; set; }

        /// <summary>
        /// search[value]: Global search value. To be applied to all columns which have searchable as true.
        /// search[regex]: true if the global filter should be treated as a regular expression for advanced searching, false otherwise. Note that normally server-side processing scripts will not perform regular expression searching for performance reasons on large data sets, but it is technically possible and at the discretion of your script.
        /// </summary>
        public Dictionary<string,object> search { get; set; }

        /// <summary>
        /// order[i][column]: Column to which ordering should be applied. This is an index reference to the columns array of information that is also submitted to the server.
        /// order[i][dir]: Ordering direction for this column. It will be asc or desc to indicate ascending ordering or descending ordering, respectively.
        /// </summary>
        public Dictionary<string, object>[] order { get; set; }

        /// <summary>
        /// columns[i][data]: Column's data source, as defined by columns.dataDT.
        /// columns[i][name]: Column's name, as defined by columns.nameDT.
        /// columns[i][searchable]: Flag to indicate if this column is searchable (true) or not (false). This is controlled by columns.searchableDT.
        /// columns[i][orderable]: Flag to indicate if this column is orderable (true) or not (false). This is controlled by columns.orderableDT.
        /// columns[i][search][value]: Search value to apply to this specific column.
        /// columns[i][search][regex]: Flag to indicate if the search term for this column should be treated as regular expression (true) or not (false). As with global search, normally server-side processing scripts will not perform regular expression searching for performance reasons on large data sets, but it is technically possible and at the discretion of your script.
        /// </summary>
        public Dictionary<string, object>[] columns { get; set; }
    }

    [Authorize(Roles = "Administrador")]
    public class GrupoEtnicoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: GruposEtnicos
        [Route("GruposEtnicos")]
        public ActionResult Index()
        {
            //int limit = 10;

            //ViewBag.LoadedCount = limit;
            //ViewBag.RecordsTotal = db.GruposEtnicos.Count();

            //return View(db.GruposEtnicos.Take(limit));
            return View();
        }

        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var recordsTotal = db.GruposEtnicos.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = db.GruposEtnicos.Select(x => new { DT_RowId = x.Id, Nombre = x.Nombre });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];

                //".Contains(searchValue) || x."

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.Nombre.Contains(searchValue.Trim())
                        );

                    recordsFiltered = data.Count();
                }
            }

            // Ordenando
            var sorting = false;
            if (values.order != null && values.order.Count() > 0)
            {
                foreach (var item in values.order)
                {
                    string sortById = (item["column"] as string[])[0];
                    string sortBy = (values.columns[int.Parse(sortById)]["data"] as string[])[0];

                    switch (sortBy)
                    {
                        case "Nombre":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.Nombre);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.Nombre);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.Nombre);
            }

            // Preparando respuesta y ejecutando consulta
            var jsonData = new {
                draw = values.raw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data.Skip(from).Take(limit).ToList()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // GET: /GrupoEtnico/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrupoEtnico grupoetnico = db.GruposEtnicos.Find(id);
            if (grupoetnico == null)
            {
                return HttpNotFound();
            }
            return View(grupoetnico);
        }

        // GET: GruposEtnicos/Create
        [Route("GruposEtnicos/Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: GruposEtnicos/Create
        [Route("GruposEtnicos/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Nombre")] GrupoEtnico grupoetnico)
        {
            if (ModelState.IsValid)
            {
                db.GruposEtnicos.Add(grupoetnico);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(grupoetnico);
        }

        // GET: /GrupoEtnico/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrupoEtnico grupoetnico = db.GruposEtnicos.Find(id);
            if (grupoetnico == null)
            {
                return HttpNotFound();
            }
            return View(grupoetnico);
        }

        // POST: /GrupoEtnico/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Nombre")] GrupoEtnico grupoetnico)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grupoetnico).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(grupoetnico);
        }

        // GET: /GrupoEtnico/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrupoEtnico grupoetnico = db.GruposEtnicos.Find(id);
            if (grupoetnico == null)
            {
                return HttpNotFound();
            }
            return View(grupoetnico);
        }

        // POST: /GrupoEtnico/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GrupoEtnico grupoetnico = db.GruposEtnicos.Find(id);

            // Seteando en null el grupo etnico
            var personas = db.Personas.Where(x => x.GrupoEtnicoId == id);
            foreach (var item in personas)
            {
                item.GrupoEtnicoId = null;
                db.Entry(item).State = EntityState.Modified;
            }

            db.GruposEtnicos.Remove(grupoetnico);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
