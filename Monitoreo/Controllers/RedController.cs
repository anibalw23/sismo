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

namespace Monitoreo.Controllers
{
    [Authorize]
    public class RedController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Redes
        [Route("Redes")]
        [Authorize(Roles = "Administrador, Acompanante,Coordinador")]
        public ActionResult Index()
        {
            //var redes = db.Redes.Include(r => r.CentroSede).Include(r => r.Distrito);
            return View();
        }

        // POST: Redes
        [HttpPost]
        [Authorize(Roles = "Administrador, Acompanante,Coordinador")]
        public ActionResult GetDataJson(DatatablesParams values)
        {
            var redes = db.Redes.Select(x => new { DT_RowId = x.Id, x.Codigo, x.Nombre, Distrito = x.Distrito.Nombre, CentroSede = x.CentroSede.Nombre });
            var recordsTotal = redes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = redes;

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.Codigo.Contains(searchValue) || x.Nombre.Contains(searchValue)
                    );

                    recordsFiltered = data.Count();
                }
            }
  


            // Preparando respuesta y ejecutando consulta
            var jsonData = new {
                draw = values.raw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data.OrderBy(n => n.Nombre).Skip(from).Take(limit).ToList()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // GET: RedCentros
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult DistritoRedes(int DistritoId)
        {
            var centros = db.Redes.Include(r => r.Distrito).Where(s => s.DistritoId == DistritoId);

            ViewBag.MasterType = "Distrito";
            ViewBag.MasterId = DistritoId;

            return PartialView(centros.ToList());
        }

        // GET: /Red/5/Details
        [Authorize(Roles = "Administrador, Acompanante,Coordinador")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Red red = db.Redes.Find(id);

            ViewBag.CentrosLatLong = db.Centros.Where(r => r.RedId == red.Id).ToList();

            if (red == null)
            {
                return HttpNotFound();
            }
            return View(red);
        }

        // GET: Redes/Create
        [Route("Redes/Create")]
        public ActionResult Create()
        {
            ViewBag.CentroSedeId = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre}), "Id", "Nombre");
            ViewBag.DistritoId = new SelectList(db.Distritos.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
            return View();
        }

        [Route("Redes/CreateFromMaster")]
        //[Authorize(Roles = "Administrador")]
        public ActionResult CreateFromMaster(string masterType, int masterId)
        {
            ViewBag.MasterType = masterType;
            ViewBag.MasterId = masterId;

            ViewBag.CentroSedeId = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
            ViewBag.DistritoId = new SelectList(db.Distritos.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre", ViewBag.MasterId);
            return PartialView();
        }

        // POST: Redes/Create
        [Route("Redes/Create")]
        //[Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Codigo,Nombre,DistritoId,CentroSedeId")] Red red)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];

            if (ModelState.IsValid)
            {
                db.Redes.Add(red);
                db.SaveChanges();

                if (ViewBag.MasterType != null)
                {
                    return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
                }
                return RedirectToAction("Index");
            }

            ViewBag.CentroSedeId = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre", red.CentroSedeId);
            ViewBag.DistritoId = new SelectList(db.Distritos.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre", red.DistritoId);
            if (ViewBag.MasterType != null)
            {
                return PartialView("CreateFromMaster");
            }
            return View(red);
        }

        // GET: /Red/5/Edit
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Red red = db.Redes.Find(id);
            if (red == null)
            {
                return HttpNotFound();
            }
            ViewBag.CentroSede = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre", red.CentroSedeId);
            ViewBag.Distrito = new SelectList(db.Distritos.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre", red.DistritoId);
            return View(red);
        }

        // POST: /Red/5/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrador")]
        public ActionResult Edit([Bind(Include="Id,Codigo,Nombre,DistritoId,CentroSedeId")] Red red)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];

            if (ModelState.IsValid)
            {
                db.Entry(red).State = EntityState.Modified;
                db.SaveChanges();


                if (ViewBag.MasterType != null)
                {
                    return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
                }
                return RedirectToAction("Index");
            }
            ViewBag.CentroSede = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre", red.CentroSedeId);
            ViewBag.Distrito = new SelectList(db.Distritos.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre", red.DistritoId);
            return View(red);
        }

        // GET: /Red/5/Delete
        //[Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Red red = db.Redes.Find(id);
            if (red == null)
            {
                return HttpNotFound();
            }
            return View(red);
        }

        // POST: /Red/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrador")]
        public ActionResult DeleteConfirmed(int id)
        {
            Red red = db.Redes.Find(id);

            try
            {
                if (db.Centros.Where(x => x.RedId == id).Count() > 0)
                    ModelState.AddModelError("error", "Esta red contiene centros relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.Redes.Remove(red);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (!ModelState.IsValid) return View(red);

            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];

            if (ViewBag.MasterType != null)
            {
                return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
            }

            return RedirectToAction("Index");
        }


        public JsonResult GetCentrosByRedId(int redID)
        {
            var centros = db.Centros.Where(r => r.RedId == redID).Select(x => new { x.Id, x.Nombre, x.Codigo, x.Red });
            var jsonData = new
            {
                data = centros.Select(y => new
                {
                    y.Id,
                    y.Nombre,
                    y.Codigo,
                    Red = y.Red.Nombre
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Authorize(Roles = "Administrador, Acompanante,Coordinador, AdministradorTransversal")]
        public JsonResult GetRedesByDistritosIds(int[] ints)
        {

            if (ints == null) {
                var jsonNoRecords= new
                {
                    data = ""
                };
                return Json(jsonNoRecords, JsonRequestBehavior.AllowGet);
            }
            else {
                var redes = db.Redes.Where(d => ints.Contains(d.DistritoId)).ToList();
                var redesOutList = new List<Red>();

                foreach (var red in redes)
                {
                    redesOutList.Add(red);
                }
                var jsonData = new
                {
                    data = redesOutList.Select(y => new
                    {
                        y.Id,
                        y.Nombre

                    }),
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);            
            }

            
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
