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
    [Authorize]
    public class ObjetivoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();



        public ActionResult IndexByPlanCentro()
        {           
            return View();
        }



        [HttpPost]
        public JsonResult GetObjetivosByPlanCentro(int planId)
        {

            List<Objetivo> objetivos = new List<Objetivo>();
            ViewBag.PlanId = planId;

            try
            {
                objetivos = db.Objetivoes.Where(p => p.PlanMejoraCentroId == planId).ToList();

            }
            catch (Exception e)
            {
                var msj = e.Message;
            }

            var jsonData = new
            {
                data = objetivos.Select(y => new
                {
                    id = y.ID,
                    codigo = y.codigo,
                    nombre = y.nombre,
                    AmbitoObjetivo = y.AmbitoObjetivo.nombre,
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }







        [HttpPost]
        public JsonResult IndexByPlanCentro(int planId)
        {

            List<Objetivo> objetivos = new List<Objetivo>();
            ViewBag.PlanId = planId;

            try
            {
                objetivos = db.Objetivoes.Where(p => p.PlanMejoraCentroId == planId).ToList();
                
            }
            catch (Exception e)
            {
                var msj = e.Message;
            }

            var jsonData = new
            {
                data = objetivos.Select(y => new
                {
                    id = y.ID,
                    codigo = y.codigo,
                    nombre = y.nombre,
                    AmbitoObjetivo = y.AmbitoObjetivo.nombre,
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        // GET: Objetivos
        [Route("Objetivo")]
        public ActionResult Index()
        {
            var objetivoes = db.Objetivoes.Include(o => o.AmbitoObjetivo).Include(o => o.PlanMejoraCentro);
            return View(objetivoes.ToList());
        }

        // POST: Objetivos
        [Route("Objetivo/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var objetivoes = db.Objetivoes.Include(o => o.AmbitoObjetivo).Include(o => o.PlanMejoraCentro);
            var recordsTotal = objetivoes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = objetivoes.Select(x => new { DT_RowId = x.ID, x.codigo, x.nombre,PlanMejoraCentro =  x.PlanMejoraCentro.nombre,AmbitoObjetivo = x.AmbitoObjetivo.nombre });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.codigo.Contains(searchValue) || x.nombre.Contains(searchValue)
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

        // GET: /Objetivo/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Objetivo objetivo = db.Objetivoes.Find(id);
            if (objetivo == null)
            {
                return HttpNotFound();
            }
            return View(objetivo);
        }

        public ActionResult CreateByPlanCentro(int planID)
        {
            ViewBag.AmbitoObjetivoId = new SelectList(db.AmbitoObjetivoes, "ID", "nombre");
            ViewBag.planID = planID;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateByPlanCentro([Bind(Include = "ID,codigo,nombre,PlanMejoraCentroId,AmbitoObjetivoId")] Objetivo objetivo)
        {
            if (ModelState.IsValid)
            {
                db.Objetivoes.Add(objetivo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AmbitoObjetivoId = new SelectList(db.AmbitoObjetivoes, "ID", "nombre", objetivo.AmbitoObjetivoId);
            ViewBag.PlanMejoraCentroId = new SelectList(db.PlanMejoraCentroes, "ID", "nombre", objetivo.PlanMejoraCentroId);
            return View(objetivo);
        }
        

        // GET: Objetivos/Create
        [Route("Objetivo/Create")]
        public ActionResult Create()
        {
            ViewBag.AmbitoObjetivoId = new SelectList(db.AmbitoObjetivoes, "ID", "nombre");
            ViewBag.PlanMejoraCentroId = new SelectList(db.PlanMejoraCentroes, "ID", "nombre");
            return View();
        }

        // POST: Objetivos/Create
        [Route("Objetivo/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,codigo,nombre,PlanMejoraCentroId,AmbitoObjetivoId")] Objetivo objetivo)
        {
            if (ModelState.IsValid)
            {
                db.Objetivoes.Add(objetivo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AmbitoObjetivoId = new SelectList(db.AmbitoObjetivoes, "ID", "nombre", objetivo.AmbitoObjetivoId);
            ViewBag.PlanMejoraCentroId = new SelectList(db.PlanMejoraCentroes, "ID", "nombre", objetivo.PlanMejoraCentroId);
            return View(objetivo);
        }

        // GET: /Objetivo/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Objetivo objetivo = db.Objetivoes.Find(id);
            if (objetivo == null)
            {
                return HttpNotFound();
            }
            ViewBag.AmbitoObjetivoId = new SelectList(db.AmbitoObjetivoes, "ID", "nombre", objetivo.AmbitoObjetivoId);
            ViewBag.PlanMejoraCentroId = new SelectList(db.PlanMejoraCentroes, "ID", "nombre", objetivo.PlanMejoraCentroId);
            return View(objetivo);
        }

        // POST: /Objetivo/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,codigo,nombre,PlanMejoraCentroId,AmbitoObjetivoId")] Objetivo objetivo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(objetivo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AmbitoObjetivoId = new SelectList(db.AmbitoObjetivoes, "ID", "nombre", objetivo.AmbitoObjetivoId);
            ViewBag.PlanMejoraCentroId = new SelectList(db.PlanMejoraCentroes, "ID", "nombre", objetivo.PlanMejoraCentroId);
            return View(objetivo);
        }

        // GET: /Objetivo/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Objetivo objetivo = db.Objetivoes.Find(id);
            if (objetivo == null)
            {
                return HttpNotFound();
            }
            return View(objetivo);
        }

        // POST: /Objetivo/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Objetivo objetivo = db.Objetivoes.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.Objetivoes.Remove(objetivo);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(objetivo);
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
