using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Monitoreo.Models.BO;
using Monitoreo.Models.DAL;

namespace Monitoreo.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class IndicadorController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Indicadors
        [Route("Indicadors")]
        public ActionResult Index()
        {
            return View(db.Indicadors.ToList());
        }

        // POST: Indicadors
        [Route("Indicadors/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var indicadors = db.Indicadors;
            var recordsTotal = indicadors.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = indicadors.Select(x => new { DT_RowId = x.ID, x.nombre, x.codigo, x.tipo, x.valorLineaBase, x.descripcion, x.formula, x.Frecuencia });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.nombre.Contains(searchValue) || x.codigo.Contains(searchValue) || x.descripcion.Contains(searchValue) || x.formula.Contains(searchValue)
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
                        case "nombre":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.nombre);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.nombre);
                            }
                            sorting = true;
                            break;
                        case "codigo":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.codigo);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.codigo);
                            }
                            sorting = true;
                            break;
                        case "descripcion":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.descripcion);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.descripcion);
                            }
                            sorting = true;
                            break;
                        case "formula":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.formula);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.formula);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.nombre);
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

        // GET: /Indicador/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicador indicador = db.Indicadors.Find(id);
            if (indicador == null)
            {
                return HttpNotFound();
            }
            return View(indicador);
        }

        // GET: Indicadors/Create
        [Route("Indicadors/Create")]
        public ActionResult Create()
        {
            List<IndicadorFecha> fechasIndicador = db.IndicadorFechas.ToList();            
            ViewBag.IndicadorFechaID = new SelectList(db.IndicadorFechas, "Id", "Id");
            return View();
        }

        // POST: Indicadors/Create
        [Route("Indicadors/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,nombre,codigo,tipo,valorLineaBase,descripcion,formula,Frecuencia")] Indicador indicador)
        {
            if (ModelState.IsValid)
            {
                db.Indicadors.Add(indicador);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(indicador);
        }

        // GET: /Indicador/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicador indicador = db.Indicadors.Find(id);
            if (indicador == null)
            {
                return HttpNotFound();
            }
            return View(indicador);
        }

        // POST: /Indicador/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,nombre,codigo,tipo,valorLineaBase,descripcion,formula,Frecuencia")] Indicador indicador)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indicador).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(indicador);
        }

        // GET: /Indicador/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicador indicador = db.Indicadors.Find(id);
            if (indicador == null)
            {
                return HttpNotFound();
            }
            return View(indicador);
        }

        // POST: /Indicador/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Indicador indicador = db.Indicadors.Find(id);
            db.Indicadors.Remove(indicador);
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
