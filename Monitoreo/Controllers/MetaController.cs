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
    public class MetaController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();


        [HttpPost]
        public JsonResult GetMetasByObjetivo(int objetivoId)
        {

            List<Meta> metas = new List<Meta>();
            ViewBag.ObjetivoID = objetivoId;

            try
            {
                metas = db.Metas.Where(o =>  o.ObjetivoId == objetivoId).ToList();

            }
            catch (Exception e)
            {
                var msj = e.Message;
            }

            var jsonData = new
            {
                data = metas.Select(y => new
                {
                    id = y.ID,
                    codigo = y.codigo,
                    nombre = y.nombre,
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }






        // GET: Metas
        [Route("Meta")]
        public ActionResult Index()
        {
            var metas = db.Metas.Include(m => m.Objetivo).Include(m => m.PeriodoMeta);
            return View(metas.ToList());
        }

        // POST: Metas
        [Route("Meta/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var metas = db.Metas.Include(m => m.Objetivo).Include(m => m.PeriodoMeta);
            var recordsTotal = metas.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = metas.Select(x => new { DT_RowId = x.ID, x.codigo, x.nombre, Objetivo = x.Objetivo.nombre, PeriodoMeta = x.PeriodoMeta.periodo });

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

        // GET: /Meta/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meta meta = db.Metas.Find(id);
            if (meta == null)
            {
                return HttpNotFound();
            }
            return View(meta);
        }

        // GET: Metas/Create
        [Route("Meta/Create")]
        public ActionResult Create()
        {
            ViewBag.ObjetivoId = new SelectList(db.Objetivoes, "ID", "nombre");
            ViewBag.PeriodoMetaId = new SelectList(db.PeriodoMetas, "ID", "periodo");
            return View();
        }

        // POST: Metas/Create
        [Route("Meta/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,codigo,nombre,ObjetivoId,PeriodoMetaId")] Meta meta)
        {
            if (ModelState.IsValid)
            {
                db.Metas.Add(meta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ObjetivoId = new SelectList(db.Objetivoes, "ID", "nombre", meta.ObjetivoId);
            ViewBag.PeriodoMetaId = new SelectList(db.PeriodoMetas, "ID", "periodo", meta.PeriodoMetaId);
            return View(meta);
        }

        // GET: /Meta/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meta meta = db.Metas.Find(id);
            if (meta == null)
            {
                return HttpNotFound();
            }
            ViewBag.ObjetivoId = new SelectList(db.Objetivoes, "ID", "nombre", meta.ObjetivoId);
            ViewBag.PeriodoMetaId = new SelectList(db.PeriodoMetas, "ID", "periodo", meta.PeriodoMetaId);
            return View(meta);
        }

        // POST: /Meta/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,codigo,nombre,ObjetivoId,PeriodoMetaId")] Meta meta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(meta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ObjetivoId = new SelectList(db.Objetivoes, "ID", "nombre", meta.ObjetivoId);
            ViewBag.PeriodoMetaId = new SelectList(db.PeriodoMetas, "ID", "periodo", meta.PeriodoMetaId);
            return View(meta);
        }

        // GET: /Meta/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meta meta = db.Metas.Find(id);
            if (meta == null)
            {
                return HttpNotFound();
            }
            return View(meta);
        }

        // POST: /Meta/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Meta meta = db.Metas.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.Metas.Remove(meta);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(meta);
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
