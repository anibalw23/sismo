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
using System.Threading.Tasks;
using Monitoreo.Models.BO;

namespace Monitoreo.Controllers
{
    [Authorize]
    public class SuperCicloFormativoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: SuperCicloFormativos
        [Route("SuperCiclosFormativos")]
        [AllowAnonymous]
        public ActionResult Index()
        {
            var supercicloformativoes = new List<SuperCicloFormativo>();

            if (User.IsInRole("Coordinador") || User.IsInRole("Administrador") || User.IsInRole("EspecialistaCurricular"))
            {
                if (Request.QueryString["q"] == "1")
                {
                    supercicloformativoes = db.SuperCicloFormativoes.OrderBy(f => f.FechaInicio.Year).ThenBy(y => y.FechaInicio.Month).ThenBy(y => y.FechaInicio.Day).ToList();  
                }
                if (Request.QueryString["q"] == "0" || Request.QueryString["q"] == null)
                {
                    supercicloformativoes = db.SuperCicloFormativoes.Where(u => u.CreadoPor == User.Identity.Name).OrderBy(f => f.FechaInicio.Year).ThenBy(y => y.FechaInicio.Month).ThenBy(y => y.FechaInicio.Day).ToList();
                }
               
            }
             if (User.IsInRole("AdministradorTransversal")){
                supercicloformativoes = db.SuperCicloFormativoes.Where(x => x.CategoriaSuperCiclo == CategoriaSuperCiclo.Transversal).OrderBy(f => f.FechaInicio.Year).ThenBy(y => y.FechaInicio.Month).ThenBy(y => y.FechaInicio.Day).ToList();  
             }

             return View(supercicloformativoes.OrderBy(c => c.nombre).ToList());
        }

       






        // POST: SuperCicloFormativos
        [Route("SuperCiclosFormativos/GetDataJson")]
        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var supercicloformativoes =  new List<SuperCicloFormativo>();

            if(User.IsInRole("Coordinador")){
                supercicloformativoes = db.SuperCicloFormativoes.Where(c => c.CreadoPor == User.Identity.Name).ToList();
            }

            if (User.IsInRole("Administrador") )
            {
                 supercicloformativoes = db.SuperCicloFormativoes.ToList();
            }

            if (User.IsInRole("Acompanante") || User.IsInRole("Formador"))
            {
                List<Inscripcion> inscripciones = new List<Inscripcion>();
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
                            SuperCicloFormativo superCiclo = new SuperCicloFormativo();
                            superCiclo.Id = c.SuperCicloFormativoId;
                            superCiclo.Area = c.SuperCicloFormativo.Area;
                            superCiclo.Nivel = c.SuperCicloFormativo.Nivel;
                            superCiclo.Ciclo = c.SuperCicloFormativo.Ciclo;
                            superCiclo.nombre = c.SuperCicloFormativo.nombre;
                            superCiclo.CreadoPor = c.SuperCicloFormativo.CreadoPor;
                            supercicloformativoes.Add(superCiclo);
                        }
                    }

                }
                catch (Exception e)
                {
                    var msj = e.Message;
                }

            }
                

            var recordsTotal = supercicloformativoes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = supercicloformativoes.Select(x => new { DT_RowId = x.Id, x.nombre, Nivel = x.Nivel.ToString(), Area =  x.Area, Ciclo = x.Ciclo });

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

        // GET: /SuperCicloFormativo/5/Details
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {

            if (User.IsInRole("Administrador") || User.IsInRole("Coordinador") || User.IsInRole("AdministradorTransversal") || User.IsInRole("EspecialistaCurricular"))
            {
                ViewBag.CiclosFormativos = db.CiclosFormativos.AsNoTracking().Where(sc => sc.SuperCicloFormativoId == id).ToList();
                ViewBag.ActividadesAcompanamiento = db.ActividadAcompanamientoes.AsNoTracking().Where(sc => sc.SuperCicloFormativoId == id).ToList();
            }
        
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SuperCicloFormativo superCicloFormativo = db.SuperCicloFormativoes.Find(id);
            if (superCicloFormativo == null)
            {
                return HttpNotFound();
            }
            return View(superCicloFormativo);
        }

        // GET: SuperCicloFormativos/Create
        [Route("SuperCiclosFormativos/Create")]
        [Authorize(Roles = "Administrador,EspecialistaCurricular")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: SuperCicloFormativos/Create
        [Route("SuperCiclosFormativos/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,EspecialistaCurricular")]
        public ActionResult Create([Bind(Include = "Id,nombre,Nivel,Area,Ciclo,FechaInicio,FechaFinalizacion,CategoriaSuperCiclo")] SuperCicloFormativo superCicloFormativo)
        {

            superCicloFormativo.CreadoPor = User.Identity.Name;
            string createActividadesAcompanamiento = Request.Form["createActividadesAcompanamiento"];            

            if (ModelState.IsValid)
            {
                if (createActividadesAcompanamiento != null) //Crea las actividades de Acompañamiento Correspondientes
                {
                    var tiposAcompanamientos = Enum.GetValues(typeof(TipoAcompanamiento)).Cast<TipoAcompanamiento>();
                    superCicloFormativo.ActividadesAcompanamiento = new List<ActividadAcompanamiento>();
                    foreach (var tipo in tiposAcompanamientos)
                    {
                        ActividadAcompanamiento actividad = new ActividadAcompanamiento { TipoAcompanamiento = tipo, SuperCicloFormativo = superCicloFormativo };
                        superCicloFormativo.ActividadesAcompanamiento.Add(actividad);
                    }
                }                
                db.SuperCicloFormativoes.Add(superCicloFormativo);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(superCicloFormativo);
        }

        // GET: /SuperCicloFormativo/5/Edit
        [Authorize(Roles = "Administrador,EspecialistaCurricular")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SuperCicloFormativo superCicloFormativo = db.SuperCicloFormativoes.Find(id);
            if (superCicloFormativo == null)
            {
                return HttpNotFound();
            }
            return View(superCicloFormativo);
        }

        // POST: /SuperCicloFormativo/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,EspecialistaCurricular")]
        public ActionResult Edit([Bind(Include = "Id,nombre,Nivel,Area,Ciclo,CreadoPor,FechaInicio,FechaFinalizacion,CategoriaSuperCiclo")] SuperCicloFormativo superCicloFormativo)
        {
            if (ModelState.IsValid)
            {
                string creadoPor = db.SuperCicloFormativoes.AsNoTracking().Where(s => s.Id == superCicloFormativo.Id).SingleOrDefault().CreadoPor;
                superCicloFormativo.CreadoPor = creadoPor;
                db.Entry(superCicloFormativo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(superCicloFormativo);
        }

        // GET: /SuperCicloFormativo/5/Delete
        [Authorize(Roles = "Administrador,EspecialistaCurricular")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SuperCicloFormativo superCicloFormativo = db.SuperCicloFormativoes.Find(id);
            if (superCicloFormativo == null)
            {
                return HttpNotFound();
            }
            return View(superCicloFormativo);
        }

        // POST: /SuperCicloFormativo/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,EspecialistaCurricular")]
        public ActionResult DeleteConfirmed(int id)
        {
            SuperCicloFormativo superCicloFormativo = db.SuperCicloFormativoes.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.SuperCicloFormativoes.Remove(superCicloFormativo);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error5016", " Favor Borrar las Actividades formativas, Evaluaciones y Grupos relacionados con este Ciclo Formativo!");
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(superCicloFormativo);
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
