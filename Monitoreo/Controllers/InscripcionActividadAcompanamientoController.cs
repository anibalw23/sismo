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
using Monitoreo.Models.BO.EvaluacionAcompanamiento;
using Monitoreo.Helpers;
using System.Text;
using Monitoreo.Models;
using System.Threading.Tasks;

namespace Monitoreo.Controllers
{
    [Authorize]
    public class InscripcionActividadAcompanamientoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        Logger Logger = new Logger();

        // GET: InscripcionActividadAcompanamientos
        [Route("InscripcionActividadAcompanamientos")]
        public ActionResult Index()
        {               
            return View();
        }

        // POST: InscripcionActividadAcompanamientos
        [Route("InscripcionActividadAcompanamientos/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var inscripcionesactividadesacompanamiento = db.InscripcionesActividadesAcompanamiento.Include(i => i.ActividadAcompanamiento).Include(i => i.Personal);
            var recordsTotal = inscripcionesactividadesacompanamiento.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = inscripcionesactividadesacompanamiento.Select(x => new { DT_RowId = x.ID, AreaAcomp = x.ActividadAcompanamiento.Area, Fecha = x.fecha.ToString(), Horas = x.horas, PersonaNombre = x.Personal.Persona.Nombres, CicloFormativo = x.ActividadAcompanamiento.SuperCicloFormativo.nombre, x.actividadAcompanamientoID, ActividadAcompanamiento = x.ActividadAcompanamiento.TipoAcompanamiento.ToString(), x.personalID, CentroNombre = x.Personal.Centro.Nombre, Cedula = x.Personal.Persona.Cedula });


         
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

            //// Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.personalID);
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

        // GET: /InscripcionActividadAcompanamiento/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InscripcionActividadAcompanamiento inscripcionActividadAcompanamiento = db.InscripcionesActividadesAcompanamiento.Find(id);
            if (inscripcionActividadAcompanamiento == null)
            {
                return HttpNotFound();
            }
            return View(inscripcionActividadAcompanamiento);
        }



        // GET: InscripcionActividadAcompanamientos/Create
        public ActionResult CreateModal(int actividadAcompanamientoID, int personalID, string tipo)
        {
            ViewBag.actividadAcompanamientoID = actividadAcompanamientoID;
            ViewBag.personalID = personalID;
            ViewBag.tipo = tipo;
            return PartialView();
        }

        [HttpPost]
        public async Task<JsonResult> CreateModal(InscripcionActividadAcompanamiento inscripcion)
        {
            int cicloId = 0;
            int docenteId = 0;
            string tipo = Request.QueryString["tipo"];
            if (ModelState.IsValid)
            {
                try {

                    inscripcion.horas = Math.Abs(inscripcion.horas); // para evitar valores negativos
                    db.InscripcionesActividadesAcompanamiento.Add(inscripcion);                     
                    await db.SaveChangesAsync();
                    ActividadAcompanamiento actividad = await db.ActividadAcompanamientoes.FindAsync(inscripcion.actividadAcompanamientoID);
                    cicloId = actividad.SuperCicloFormativoId;
                    tipo = actividad.TipoAcompanamiento.ToString();
                    docenteId = inscripcion.personalID;

                    //Enviar por email
                    Personal personal = await db.Personal.FindAsync(inscripcion.personalID);
                    string tituloEmail = inscripcion.ActividadAcompanamiento.TipoAcompanamiento.ToString();
                    StringBuilder textoEmail = new StringBuilder();
                    textoEmail.AppendLine("<h1>Ciclo Formativo: " + inscripcion.ActividadAcompanamiento.SuperCicloFormativo.nombre + " " + inscripcion.ActividadAcompanamiento.TipoAcompanamiento.ToString() + "</h1>");
                    textoEmail.AppendLine("<h2>Creado Por: " + User.Identity.Name + "</h2>");
                    textoEmail.AppendLine("<p>" + "Nombre: " + personal.Persona.Nombres + " fecha:" + inscripcion.fecha + " horas: " + inscripcion.horas + " Area: " + inscripcion.Area + "</p>");
                    await Logger.LogEvent(User.Identity.Name, "Actividad de " + tituloEmail + " creada" + User.Identity.Name, textoEmail.ToString(), "", DateTime.Now);
                    //End Enviar por email

                }
                catch(Exception e){
                    var msj = e.Message;
                    return Json(new { success = false });
                }
                
               // return RedirectToAction("DocenteDetailsAcompanante", "Docente", new { id = inscripcion.personalID});
            }
            return Json(new { 
                success = true,
                cicloId = cicloId,
                docenteId = docenteId,
                tipo = tipo
            });
            //return View(inscripcion);
        }








        // GET: InscripcionActividadAcompanamientos/Create
        [Route("InscripcionActividadAcompanamientos/Create")]
        public ActionResult Create()
        {
            ViewBag.actividadAcompanamientoID = new SelectList(db.ActividadAcompanamientoes, "ID", "ID");
            ViewBag.personalID = new SelectList(db.Personal, "Id", "Codigo");
            return View();
        }
       


        // POST: InscripcionActividadAcompanamientos/Create
        [Route("InscripcionActividadAcompanamientos/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,personalID,actividadAcompanamientoID,fecha,horas")] InscripcionActividadAcompanamiento inscripcionActividadAcompanamiento)
        {
            if (ModelState.IsValid)
            {
                db.InscripcionesActividadesAcompanamiento.Add(inscripcionActividadAcompanamiento);
                db.SaveChanges();               
                return RedirectToAction("Index");
            }

            ViewBag.actividadAcompanamientoID = new SelectList(db.ActividadAcompanamientoes, "ID", "ID", inscripcionActividadAcompanamiento.actividadAcompanamientoID);
            ViewBag.personalID = new SelectList(db.Personal, "Id", "Codigo", inscripcionActividadAcompanamiento.personalID);
            return View(inscripcionActividadAcompanamiento);
        }

        // GET: /InscripcionActividadAcompanamiento/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InscripcionActividadAcompanamiento inscripcionActividadAcompanamiento = db.InscripcionesActividadesAcompanamiento.Find(id);
            if (inscripcionActividadAcompanamiento == null)
            {
                return HttpNotFound();
            }
            ViewBag.actividadAcompanamiento = new SelectList(db.ActividadAcompanamientoes.Select(x => new { x.ID, SuperCiclo = x.SuperCicloFormativo.nombre + "-" + x.TipoAcompanamiento.ToString() }), "ID", "SuperCiclo", inscripcionActividadAcompanamiento.actividadAcompanamientoID);
            ViewBag.personalID = inscripcionActividadAcompanamiento.personalID;
            return View(inscripcionActividadAcompanamiento);
        }

        // POST: /InscripcionActividadAcompanamiento/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,personalID,actividadAcompanamientoID,fecha,horas,Area")] InscripcionActividadAcompanamiento inscripcionActividadAcompanamiento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inscripcionActividadAcompanamiento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.actividadAcompanamiento = new SelectList(db.ActividadAcompanamientoes.Select(x => new { x.ID, SuperCiclo = x.SuperCicloFormativo.nombre + "-" + x.TipoAcompanamiento.ToString() }), "ID", "SuperCiclo", inscripcionActividadAcompanamiento.actividadAcompanamientoID);
            ViewBag.personalID = new SelectList(db.Personal, "Id", "Cedula", inscripcionActividadAcompanamiento.personalID);
            return View(inscripcionActividadAcompanamiento);
        }


        public async Task<JsonResult> DeleteModal(int? inscripcionId)
        {
            var response = "OK";
            int cicloId = 0;
            string tipo = "";
            string superCicloFormativoNombre = "";
            int docenteId = 0;
            if (inscripcionId == null)
            {
                response = "ERROR";
            }
            InscripcionActividadAcompanamiento inscripcionActividadAcompanamiento = await db.InscripcionesActividadesAcompanamiento.FindAsync(inscripcionId);
            if (inscripcionActividadAcompanamiento == null)
            {
                response = "ERROR";
            }
            try {

                bool hasRespuestasEvaluaciones = await db.EvaluacionAcompanamientoRespuestas.AsNoTracking().Select(x => new { x.InscripcionActividadAcompanamientoId, x.Id }).AnyAsync(p => p.InscripcionActividadAcompanamientoId == inscripcionId);
                if (hasRespuestasEvaluaciones)
                {
                    List<EvaluacionAcompanamientoRespuesta> respuestasAcompanamiento = new List<EvaluacionAcompanamientoRespuesta>();
                    respuestasAcompanamiento = await db.EvaluacionAcompanamientoRespuestas.Where(p => p.InscripcionActividadAcompanamientoId == inscripcionId).ToListAsync();
                    foreach(var respA in respuestasAcompanamiento){
                        db.EvaluacionAcompanamientoRespuestas.Remove(respA);
                        await db.SaveChangesAsync();
                    }
                }

                ActividadAcompanamiento actividad = await db.ActividadAcompanamientoes.FindAsync(inscripcionActividadAcompanamiento.actividadAcompanamientoID);
                cicloId = actividad.SuperCicloFormativoId;
                tipo = actividad.TipoAcompanamiento.ToString();
                docenteId = inscripcionActividadAcompanamiento.personalID;
                superCicloFormativoNombre = actividad.SuperCicloFormativo.nombre;

                db.InscripcionesActividadesAcompanamiento.Remove(inscripcionActividadAcompanamiento);
                await db.SaveChangesAsync();

                //Enviar por email
                Personal personal = await db.Personal.FindAsync(inscripcionActividadAcompanamiento.personalID);
                string tituloEmail = tipo;
                StringBuilder textoEmail = new StringBuilder();
                textoEmail.AppendLine("<h1>Ciclo Formativo: " + superCicloFormativoNombre + " " + tipo + "</h1>");
                textoEmail.AppendLine("<p>" + "Nombre: " + personal.Persona.Nombres + " fecha:" + inscripcionActividadAcompanamiento.fecha + " horas: " + inscripcionActividadAcompanamiento.horas + " Area: " + inscripcionActividadAcompanamiento.Area + "</p>");
                textoEmail.AppendLine("<h2>Borrada Por: " + User.Identity.Name + "</h2>");
                await Logger.LogEvent(User.Identity.Name, "Actividad de " + tituloEmail + " Borrada" + User.Identity.Name, textoEmail.ToString(), "", DateTime.Now);
                //End Enviar por email
                
            }
            catch(Exception e){
                response = "ERROR";
                var mj = e.Message;
            }
            var jsonData = new
            {
               response,
               cicloId = cicloId,
               tipo = tipo,
               docenteId = docenteId,
               horas = inscripcionActividadAcompanamiento.horas

            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }







        // GET: /InscripcionActividadAcompanamiento/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InscripcionActividadAcompanamiento inscripcionActividadAcompanamiento = db.InscripcionesActividadesAcompanamiento.Find(id);
            if (inscripcionActividadAcompanamiento == null)
            {
                return HttpNotFound();
            }
            return View(inscripcionActividadAcompanamiento);
        }

        // POST: /InscripcionActividadAcompanamiento/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InscripcionActividadAcompanamiento inscripcionActividadAcompanamiento = db.InscripcionesActividadesAcompanamiento.Find(id);

            try
            {
                
                if (ModelState.IsValid)
                {
                    db.InscripcionesActividadesAcompanamiento.Remove(inscripcionActividadAcompanamiento);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(inscripcionActividadAcompanamiento);
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
