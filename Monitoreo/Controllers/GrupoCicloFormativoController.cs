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
using Monitoreo.Models;

namespace Monitoreo.Controllers
{
    //[Authorize(Roles = "Administrador")]
    [Authorize]
    public class GrupoCicloFormativoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: GrupoCicloFormativos
        [Route("GrupoCicloFormativo")]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador,EspecialistaCurricular")]
        public ActionResult Index()
        {
            var gruposciclosformativos = db.GruposCiclosFormativos.Include(g => g.CicloFormativo);
            return View(gruposciclosformativos.ToList());
        }

        // POST: GrupoCicloFormativos
        [Authorize(Roles = "Administrador,Acompanante,Coordinador,EspecialistaCurricular")]
        [Route("GrupoCicloFormativo/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var gruposciclosformativos = db.GruposCiclosFormativos.Include(g => g.CicloFormativo).Include(f => f.Centro);
            var recordsTotal = gruposciclosformativos.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = gruposciclosformativos.Select(x => new { DT_RowId = x.ID, x.Centro.Nombre, x.CicloFormativoId, CicloFormativo = x.CicloFormativo.Tema });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.Nombre.Contains(searchValue)
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

        // GET: /GrupoCicloFormativo/5/Details
        [Authorize(Roles = "Administrador,Acompanante,Coordinador,EspecialistaCurricular")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrupoCicloFormativo grupoCicloFormativo = db.GruposCiclosFormativos.Find(id);
            List<Inscripcion> inscripciones = grupoCicloFormativo.inscripciones.ToList();
            ViewBag.inscripcionesList = inscripciones;

            if (grupoCicloFormativo == null)
            {
                return HttpNotFound();
            }
            return View(grupoCicloFormativo);
        }

        // GET: GrupoCicloFormativos/Create
        [Route("GrupoCicloFormativo/Create")]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador,EspecialistaCurricular")]
        public ActionResult Create()
        {
            ViewBag.CentrosID = new SelectList(db.Centros, "Id", "Nombre");
            return View();
        }

        // POST: GrupoCicloFormativos/Create
        [Route("GrupoCicloFormativo/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GrupoCicloFormativo grupoCicloFormativo)
        {
            var cicloFormativoString = Request.QueryString["cicloId"];
            if (cicloFormativoString != "" && cicloFormativoString != null) {
                int cicloFormativoID = Convert.ToInt32(cicloFormativoString);
                grupoCicloFormativo.CicloFormativoId = cicloFormativoID;

                if (ModelState.IsValid)
                {
                    bool isRepeated = db.GruposCiclosFormativos.Where(i => i.CicloFormativoId == cicloFormativoID).Any(p => p.Centro.Id == grupoCicloFormativo.CentroID);
                    if (!isRepeated)
                    {
                        db.GruposCiclosFormativos.Add(grupoCicloFormativo);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Details", "CicloFormativo", new { id = cicloFormativoID });
                }

            }

            ViewBag.CentrosID = new SelectList(db.Centros, "Nombre", "Nombre");
            return View(grupoCicloFormativo);
        }

        // GET: /GrupoCicloFormativo/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrupoCicloFormativo grupoCicloFormativo = db.GruposCiclosFormativos.Find(id);
            if (grupoCicloFormativo == null)
            {
                return HttpNotFound();
            }
            ViewBag.CentrosID = new SelectList(db.Centros, "Nombre", "Nombre");
            return View(grupoCicloFormativo);
        }

        // POST: /GrupoCicloFormativo/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GrupoCicloFormativo grupoCicloFormativo)
        {

            var cicloFormativoString = Request.QueryString["cicloId"];
            if (cicloFormativoString !="" && cicloFormativoString != null)
            {
                int cicloFormativoID = Convert.ToInt32(cicloFormativoString);
                if (ModelState.IsValid)
                {

                    //grupoCicloFormativo.codigo = db.Centros.Where(n => n.Nombre == grupoCicloFormativo.nombre).SingleOrDefault().Id.ToString();

                    grupoCicloFormativo.CicloFormativoId = cicloFormativoID;
                    db.Entry(grupoCicloFormativo).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "CicloFormativo", new { id = cicloFormativoID});
                }
            }
            

            ViewBag.CentrosID = new SelectList(db.Centros, "Nombre", "Nombre");
            return View(grupoCicloFormativo);
        }

        // GET: /GrupoCicloFormativo/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrupoCicloFormativo grupoCicloFormativo = db.GruposCiclosFormativos.Find(id);
            if (grupoCicloFormativo == null)
            {
                return HttpNotFound();
            }
            return View(grupoCicloFormativo);
        }

        // POST: /GrupoCicloFormativo/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            var cicloFormativoString = Request.QueryString["cicloId"];
            int cicloFormativoID = 0;
            if (cicloFormativoString != "" && cicloFormativoString != null)
            {
                 cicloFormativoID = Convert.ToInt32(cicloFormativoString);
            }

            GrupoCicloFormativo grupoCicloFormativo = db.GruposCiclosFormativos.Find(id);
            db.GruposCiclosFormativos.Remove(grupoCicloFormativo);
            db.SaveChanges();
            return RedirectToAction("Details", "CicloFormativo", new { id = cicloFormativoID });
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
