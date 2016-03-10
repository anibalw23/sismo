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
    public class CentroGradoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();


        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult CentroGrado(int CentroId)
        {
            var aulas = db.CentroGradoes.Include(g => g.GradoLookup).Where(s => s.CentroId == CentroId);
            ViewBag.MasterType = "Centro";
            ViewBag.MasterId = CentroId;
            return PartialView(aulas.ToList());
        }

        // GET: CentroGrados
        [Route("CentroGrado")]
        public ActionResult Index()
        {
            var centrogradoes = db.CentroGradoes.Include(c => c.Centro).Include(c => c.GradoLookup);
            return View(centrogradoes.ToList());
        }

        // POST: CentroGrados
        [Route("CentroGrado/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var centrogradoes = db.CentroGradoes.Include(c => c.Centro).Include(c => c.GradoLookup);
            var recordsTotal = centrogradoes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = centrogradoes.Select(x => new { DT_RowId = x.ID, x.CentroId, x.GradoLookupId });

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
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.CentroId);
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

        // GET: /CentroGrado/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CentroGrado centroGrado = db.CentroGradoes.Find(id);
            if (centroGrado == null)
            {
                return HttpNotFound();
            }
            return View(centroGrado);
        }

        // GET: CentroGrados/Create
        [Route("CentroGrado/Create")]
        public ActionResult Create()
        {
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo");
            ViewBag.GradoLookupId = new SelectList(db.GradoLookups, "Id", "grado");

            ViewBag.Secciones = new SelectList(db.Secciones, "Id", "Numero");

            if (Request.Params["Modal"] != null)
            {
                return PartialView();
            }

            return View();
        }

        // POST: CentroGrados/Create
        [Route("CentroGrado/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,CentroId,GradoLookupId")] CentroGrado centroGrado)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];
            string Secciones = Request.Form["seccion.Id"];
            string[] SeccionesArr = Secciones.Split(',');
            List<string> seccionesStrTemp = SeccionesArr.Skip(1).ToList();
            int centroID = Convert.ToInt32(ViewBag.MasterId);

            int tempCentroGradoId = 0;

            if (ModelState.IsValid)
            {
                bool isGradoRepeated = db.CentroGradoes.Where(c => c.CentroId == centroID).Any(g => g.GradoLookupId == centroGrado.GradoLookupId);


                if (!isGradoRepeated)
                {
                    centroGrado.CentroId = centroID;
                    db.CentroGradoes.Add(centroGrado);
                    db.SaveChanges();
                }
                else {
                    try {
                     tempCentroGradoId = db.CentroGradoes.Where(c => c.CentroId == centroID).Where(g => g.GradoLookupId == centroGrado.GradoLookupId).SingleOrDefault().ID;
                    }
                    catch(Exception e){
                        var msg = e.Message;
                    }
                    
                }


                //Añadir Secciones
                List<SeccionAula> seccionesAula = new List<SeccionAula>();
                foreach (var seccion in SeccionesArr.Skip(1))
                {
                    SeccionAula s = new SeccionAula();
                    s.SeccionId = Convert.ToInt32(seccion);
                    if (!isGradoRepeated)
                        s.gradoId = centroGrado.ID;
                    else
                        s.gradoId = tempCentroGradoId;
                    bool isSeccionAulaRepeated = db.SeccionesAula.Where(a => a.gradoId == s.gradoId).Any(g => g.SeccionId == s.SeccionId);
                    if (!isSeccionAulaRepeated)
                    {
                        seccionesAula.Add(s);
                    }
                }
                db.SeccionesAula.AddRange(seccionesAula);
                db.SaveChanges();

                if (ViewBag.MasterType != null)
                {
                    return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
                }






                return RedirectToAction("Index");
            }

            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", centroGrado.CentroId);
            ViewBag.GradoLookupId = new SelectList(db.GradoLookups, "Id", "grado", centroGrado.GradoLookupId);
            return View(centroGrado);
        }

        // GET: /CentroGrado/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CentroGrado centroGrado = db.CentroGradoes.Find(id);
            if (centroGrado == null)
            {
                return HttpNotFound();
            }
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", centroGrado.CentroId);
            ViewBag.GradoLookupId = new SelectList(db.GradoLookups, "Id", "grado", centroGrado.GradoLookupId);
            return View(centroGrado);
        }

        // POST: /CentroGrado/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,CentroId,GradoLookupId")] CentroGrado centroGrado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(centroGrado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", centroGrado.CentroId);
            ViewBag.GradoLookupId = new SelectList(db.GradoLookups, "Id", "grado", centroGrado.GradoLookupId);
            return View(centroGrado);
        }

        // GET: /CentroGrado/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CentroGrado centroGrado = db.CentroGradoes.Find(id);
            if (centroGrado == null)
            {
                return HttpNotFound();
            }
            return View(centroGrado);
        }

        // POST: /CentroGrado/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CentroGrado centroGrado = db.CentroGradoes.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.CentroGradoes.Remove(centroGrado);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(centroGrado);
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
