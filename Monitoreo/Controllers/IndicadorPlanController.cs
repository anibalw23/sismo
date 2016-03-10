using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Monitoreo.Models.BO.PlanMejora;
using Monitoreo.Models.DAL;

namespace Monitoreo.Controllers
{
    public class IndicadorPlanController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();



        [HttpPost]
        public JsonResult GetIndicadoresByMeta(int metaId)
        {

            List<IndicadorPlan> indicadores = new List<IndicadorPlan>();
            ViewBag.MetaID = metaId;

            try
            {
                indicadores = db.IndicadorPlans.Where(o => o.MetaId == metaId).ToList();

            }
            catch (Exception e)
            {
                var msj = e.Message;
            }

            var jsonData = new
            {
                data = indicadores.Select(y => new
                {
                    id = y.ID,
                    codigo = y.codigo,
                    nombre = y.indicador,
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }





        // GET: IndicadorPlans
        [Route("IndicadorPlan")]
        public ActionResult Index()
        {
            var indicadorplans = db.IndicadorPlans.Include(i => i.Meta);
            return View(indicadorplans.ToList());
        }

        // POST: IndicadorPlans
        [Route("IndicadorPlan/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var indicadorplans = db.IndicadorPlans.Include(i => i.Meta);
            var recordsTotal = indicadorplans.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = indicadorplans.Select(x => new { DT_RowId = x.ID, x.codigo, x.indicador, Meta = x.Meta.nombre });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.codigo.Contains(searchValue) || x.indicador.Contains(searchValue)
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
                        case "indicador":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.indicador);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.indicador);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.codigo);
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

        // GET: /IndicadorPlan/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicadorPlan indicadorPlan = db.IndicadorPlans.Find(id);
            if (indicadorPlan == null)
            {
                return HttpNotFound();
            }
            return View(indicadorPlan);
        }

        // GET: IndicadorPlans/Create
        [Route("IndicadorPlan/Create")]
        public ActionResult Create()
        {
            ViewBag.MetaId = new SelectList(db.Metas, "ID", "nombre");
            return View();
        }

        // POST: IndicadorPlans/Create
        [Route("IndicadorPlan/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,codigo,indicador,MetaId")] IndicadorPlan indicadorPlan)
        {
            if (ModelState.IsValid)
            {
                db.IndicadorPlans.Add(indicadorPlan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MetaId = new SelectList(db.Metas, "ID", "nombre", indicadorPlan.MetaId);
            return View(indicadorPlan);
        }

        // GET: /IndicadorPlan/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicadorPlan indicadorPlan = db.IndicadorPlans.Find(id);
            if (indicadorPlan == null)
            {
                return HttpNotFound();
            }
            ViewBag.MetaId = new SelectList(db.Metas, "ID", "nombre", indicadorPlan.MetaId);
            return View(indicadorPlan);
        }

        // POST: /IndicadorPlan/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,codigo,indicador,MetaId")] IndicadorPlan indicadorPlan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indicadorPlan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MetaId = new SelectList(db.Metas, "ID", "nombre", indicadorPlan.MetaId);
            return View(indicadorPlan);
        }

        // GET: /IndicadorPlan/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicadorPlan indicadorPlan = db.IndicadorPlans.Find(id);
            if (indicadorPlan == null)
            {
                return HttpNotFound();
            }
            return View(indicadorPlan);
        }

        // POST: /IndicadorPlan/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IndicadorPlan indicadorPlan = db.IndicadorPlans.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.IndicadorPlans.Remove(indicadorPlan);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(indicadorPlan);
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
