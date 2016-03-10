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
    public class AmbitoObjetivoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: AmbitoObjetivos
        [Route("AmbitoObjetivo")]
        public ActionResult Index()
        {
            return View(db.AmbitoObjetivoes.ToList());
        }

        // POST: AmbitoObjetivos
        [Route("AmbitoObjetivo/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var ambitoobjetivoes = db.AmbitoObjetivoes;
            var recordsTotal = ambitoobjetivoes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = ambitoobjetivoes.Select(x => new { DT_RowId = x.ID, x.codigo, x.nombre });

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

        // GET: /AmbitoObjetivo/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AmbitoObjetivo ambitoObjetivo = db.AmbitoObjetivoes.Find(id);
            if (ambitoObjetivo == null)
            {
                return HttpNotFound();
            }
            return View(ambitoObjetivo);
        }

        // GET: AmbitoObjetivos/Create
        [Route("AmbitoObjetivo/Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: AmbitoObjetivos/Create
        [Route("AmbitoObjetivo/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,codigo,nombre")] AmbitoObjetivo ambitoObjetivo)
        {
            if (ModelState.IsValid)
            {
                db.AmbitoObjetivoes.Add(ambitoObjetivo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ambitoObjetivo);
        }

        // GET: /AmbitoObjetivo/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AmbitoObjetivo ambitoObjetivo = db.AmbitoObjetivoes.Find(id);
            if (ambitoObjetivo == null)
            {
                return HttpNotFound();
            }
            return View(ambitoObjetivo);
        }

        // POST: /AmbitoObjetivo/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,codigo,nombre")] AmbitoObjetivo ambitoObjetivo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ambitoObjetivo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ambitoObjetivo);
        }

        // GET: /AmbitoObjetivo/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AmbitoObjetivo ambitoObjetivo = db.AmbitoObjetivoes.Find(id);
            if (ambitoObjetivo == null)
            {
                return HttpNotFound();
            }
            return View(ambitoObjetivo);
        }

        // POST: /AmbitoObjetivo/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AmbitoObjetivo ambitoObjetivo = db.AmbitoObjetivoes.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.AmbitoObjetivoes.Remove(ambitoObjetivo);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(ambitoObjetivo);
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
