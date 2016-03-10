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
using Monitoreo.Models.BO.ViewModels.CalendarioCicloFormativoVm;


namespace Monitoreo.Controllers
{
    //[Authorize(Roles = "Administrador, Acompanante")]
    [Authorize]
    public class CalendarioCicloFormativoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();


        //Parial view Ciclocalendario
        public ActionResult Ciclocalendario(int CicloFormativoId)
        {

            ViewBag.MasterType = "CicloFormativo";
            ViewBag.MasterId = CicloFormativoId;


            List<CalendarioCicloFormativoVM> calendarioCiclo = new List<CalendarioCicloFormativoVM>();
            var calendarios = db.CalendarioCicloFormativoes.Select(x => new {x.Id, x.CicloFormativoID, x.Fecha, x.horas}).Where(c => c.CicloFormativoID == CicloFormativoId).ToList(); 
            foreach(var cal in calendarios){
                calendarioCiclo.Add(new CalendarioCicloFormativoVM { Id = cal.Id, CicloFormativoID = cal.CicloFormativoID, Fecha = cal.Fecha, horas = cal.horas });
            }

            //List<CalendarioCicloFormativo> calendariocicloformativoes = new List<CalendarioCicloFormativo>();
            //calendariocicloformativoes = db.CalendarioCicloFormativoes.Where(c => c.CicloFormativoID == CicloFormativoId).ToList();
            return PartialView(calendarioCiclo);
        }




        // GET: CalendarioCicloFormativos
        [Route("CalendarioCicloFormativos")]
        public ActionResult Index()
        {            
            List<CalendarioCicloFormativo> calendariocicloformativoes = new List<CalendarioCicloFormativo>();
            calendariocicloformativoes = db.CalendarioCicloFormativoes.Include(c => c.CicloFormativo).ToList();           
            return View(calendariocicloformativoes.ToList());
        }



        [HttpPost]
        [Route("CalendarioCicloFormativos/GeDatesCalendar")]
        public JsonResult Events(DateTime? start, DateTime? end)
        {
            List<CalendarioCicloFormativo> calendariocicloformativoes = new List<CalendarioCicloFormativo>();
            List<Inscripcion> inscripciones = new List<Inscripcion>();
            if (User.IsInRole("Administrador") || User.IsInRole("Coordinador") || User.IsInRole("AdministradorTransversal"))
            {
                calendariocicloformativoes = db.CalendarioCicloFormativoes.Include(c => c.CicloFormativo).ToList();
            }
            if (User.IsInRole("Acompanante") || User.IsInRole("Formador")) {
                Persona participante = new Persona();
                try
                {
                    participante = db.Personas.Where(p => p.Cedula == User.Identity.Name).SingleOrDefault();
                    if (participante != null)
                    {
                        inscripciones = db.Inscripciones.Where(p => p.Participante.Id == participante.Id).Include(p => p.Participante).ToList();
                        List<CicloFormativo> ciclosFormativos = inscripciones.Select(c => c.CicloFormativo).ToList();
                        foreach (CicloFormativo c in ciclosFormativos)
                        {
                            List<CalendarioCicloFormativo> cals = db.CalendarioCicloFormativoes.Where(d => d.CicloFormativoID == c.Id).ToList();
                            calendariocicloformativoes.AddRange(cals);
                        }                      
                    }
                }
                catch (Exception e)
                {
                    var msj = e.Message;
                }
            }

            var data = calendariocicloformativoes.ToList();
            var result = data.Select(x => new { id = x.Id.ToString(), start = x.Fecha.Date.ToString("yyyy-MM-dd"), end = x.Fecha.Date.ToString("yyyy-MM-dd"), title = string.Concat( " - " ,x.CicloFormativo.Tema) });
          
            var jsonData = new {
                result
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        } 



        // POST: CalendarioCicloFormativos
        [Route("CalendarioCicloFormativos/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {

            List<CalendarioCicloFormativo> calendariocicloformativoes = new List<CalendarioCicloFormativo>();
            List<Inscripcion> inscripciones = new List<Inscripcion>();
            if (User.IsInRole("Administrador") || User.IsInRole("Coordinador") || User.IsInRole("AdministradorTransversal"))
            {                
                calendariocicloformativoes = db.CalendarioCicloFormativoes.Include(c => c.CicloFormativo).ToList();
            }
            if (User.IsInRole("Acompanante") || User.IsInRole("Formador")) {
                Persona participante = new Persona();
                try
                {
                    participante = db.Personas.Where(p => p.Cedula == User.Identity.Name).SingleOrDefault();
                    if (participante != null)
                    {
                        inscripciones = db.Inscripciones.Where(p => p.Participante.Id == participante.Id).Include(p => p.Participante).ToList();
                        List<CicloFormativo> ciclosFormativos = inscripciones.Select(c => c.CicloFormativo).ToList();
                        foreach (CicloFormativo c in ciclosFormativos)
                        {
                            List<CalendarioCicloFormativo> cals = db.CalendarioCicloFormativoes.Where(d => d.CicloFormativoID == c.Id).ToList();
                            calendariocicloformativoes.AddRange(cals);
                        }                   
                    }

                }
                catch (Exception e)
                {
                    var msj = e.Message;
                }
            }
             
            var recordsTotal = calendariocicloformativoes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = calendariocicloformativoes.Select(x => new { DT_RowId = x.Id, Fecha = x.Fecha.ToString(), x.horas,  CicloFormativo = x.CicloFormativo.Tema });

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
                data = data.OrderBy(s => s.Fecha);
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

        // GET: /CalendarioCicloFormativo/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalendarioCicloFormativo calendarioCicloFormativo = db.CalendarioCicloFormativoes.Find(id);
            if (calendarioCicloFormativo == null)
            {
                return HttpNotFound();
            }
            return View(calendarioCicloFormativo);
        }

        // GET: CalendarioCicloFormativos/Create
        [Route("CalendarioCicloFormativos/Create")]
        public ActionResult Create(int cicloId)
        {
            ViewBag.MasterId = cicloId;
            //ViewBag.ActividadFormativaID = new SelectList(db.ActividadesFormativas, "Id", "Id");
            //ViewBag.CicloFormativoID = new SelectList(db.CiclosFormativos, "Id", "Tema");
            return View();
        }

        // POST: CalendarioCicloFormativos/Create
        [Route("CalendarioCicloFormativos/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult Create([Bind(Include="Id,Fecha,horas,ActividadFormativaID,CicloFormativoID")] CalendarioCicloFormativo calendarioCicloFormativo)
        {
            if (ModelState.IsValid)
            {
                db.CalendarioCicloFormativoes.Add(calendarioCicloFormativo);
                db.SaveChanges();
                return RedirectToAction("Details", "CicloFormativo", new { id = calendarioCicloFormativo.CicloFormativoID});
            }

            return View(calendarioCicloFormativo);
        }

        // GET: /CalendarioCicloFormativo/5/Edit
        [Authorize(Roles = "Administrador, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult Edit(int? id, int cicloId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalendarioCicloFormativo calendarioCicloFormativo = db.CalendarioCicloFormativoes.Find(id);
            if (calendarioCicloFormativo == null)
            {
                return HttpNotFound();
            }
            ViewBag.MasterId = cicloId;
            //ViewBag.ActividadFormativaID = new SelectList(db.ActividadesFormativas, "Id", "Id", calendarioCicloFormativo.ActividadFormativaID);
            //ViewBag.CicloFormativoID = new SelectList(db.CiclosFormativos, "Id", "Tema", calendarioCicloFormativo.CicloFormativoID);
            return View(calendarioCicloFormativo);
        }

        // POST: /CalendarioCicloFormativo/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult Edit([Bind(Include="Id,Fecha,horas,ActividadFormativaID,CicloFormativoID")] CalendarioCicloFormativo calendarioCicloFormativo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(calendarioCicloFormativo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "CicloFormativo", new { id = calendarioCicloFormativo.CicloFormativoID });
            }
            //ViewBag.ActividadFormativaID = new SelectList(db.ActividadesFormativas, "Id", "Id", calendarioCicloFormativo.ActividadFormativaID);
            //ViewBag.CicloFormativoID = new SelectList(db.CiclosFormativos, "Id", "Tema", calendarioCicloFormativo.CicloFormativoID);
            return View(calendarioCicloFormativo);
        }

        // GET: /CalendarioCicloFormativo/5/Delete
        public ActionResult Delete(int? id, int cicloId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalendarioCicloFormativo calendarioCicloFormativo = db.CalendarioCicloFormativoes.Find(id);
            if (calendarioCicloFormativo == null)
            {
                return HttpNotFound();
            }
            ViewBag.MasterId = cicloId;
            return View(calendarioCicloFormativo);
        }

        // POST: /CalendarioCicloFormativo/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CalendarioCicloFormativo calendarioCicloFormativo = db.CalendarioCicloFormativoes.Find(id);
            db.CalendarioCicloFormativoes.Remove(calendarioCicloFormativo);
            db.SaveChanges();
            return RedirectToAction("Details", "CicloFormativo", new { id = calendarioCicloFormativo.CicloFormativoID });
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
