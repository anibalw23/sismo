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
using Postal;
using System.Text.RegularExpressions;

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


        public ActionResult InscripcionesPorFecha()
        {

            Acompanante acompanante = new Acompanante();
            int centroID = 0;
            if (User.Identity.IsAuthenticated)
            {
                Persona persona = db.Personas.Where(c => c.Cedula == User.Identity.Name).FirstOrDefault();
                if (persona != null)
                {
                    acompanante = db.Acompanantes.Where(p => p.PersonaId == persona.Id).FirstOrDefault();
                    if (acompanante != null)
                    {
                        centroID = acompanante.centroId;
                        ViewBag.centroID = centroID;
                    }
                }
            }
            if (centroID == 0)
            {
                ViewBag.centroList = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre }).ToList(), "Id", "Nombre");
            }

            return View();
        }


        public ActionResult MisInscripcionesAcompanamiento()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetInscripcionesAcompanamientoPorFecha(DateTime fecha1, DateTime fecha2, int centroId)
        {

            var inscripciones = await db.InscripcionesActividadesAcompanamiento.Where(p => p.Personal.CentroId == centroId).Where(f => f.fecha > fecha1).Where(f => f.fecha < fecha2).ToListAsync();
            int recordsTotal = inscripciones.Count();
            var data = inscripciones.Select(x => new { DT_RowId = x.ID, AreaAcomp = x.Area.ToString(), Fecha = x.fecha.ToShortDateString(), Horas = x.horas, PersonaNombre = x.Personal.Persona.Nombres, CicloFormativo = x.ActividadAcompanamiento.SuperCicloFormativo.nombre, x.actividadAcompanamientoID, ActividadAcompanamiento = x.ActividadAcompanamiento.TipoAcompanamiento.ToString(), x.personalID, CentroNombre = x.Personal.Centro.Nombre, Cedula = x.Personal.Persona.Cedula }).OrderByDescending(f => f.Fecha);
            var jsonData = new
            {
                data = data.ToList()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }



        [HttpPost]
        public async Task<JsonResult> GetMisAcompanaminetos(DatatablesParams values)
        {
            var recordsTotal = 0; // inscripcionesactividadesacompanamiento.Count();
            var recordsFiltered = 0; //recordsTotal;
            var limit = 0;//values.length > 0 ? values.length : recordsTotal;
            var from = 0; //values.start;


            Persona persona = db.Personas.Where(c => c.Cedula == User.Identity.Name).FirstOrDefault();
            Acompanante acompanante = new Acompanante();
            Centro centro = new Centro();
            List<InscripcionActividadAcompanamiento> inscripciones = new List<InscripcionActividadAcompanamiento>();

            if (persona != null)
            {
                acompanante = db.Acompanantes.Where(p => p.PersonaId == persona.Id).FirstOrDefault();
                if (acompanante != null)
                {
                    centro = acompanante.Centro;
                    if (centro != null)
                    {
                        inscripciones = await db.InscripcionesActividadesAcompanamiento.Where(p => p.Personal.CentroId == centro.Id).ToListAsync();
                        recordsTotal = inscripciones.Count();
                        recordsFiltered = recordsTotal;
                        limit = values.length > 0 ? values.length : recordsTotal;
                        from = values.start;
                    }
                }

            }
            var data = inscripciones.Select(x => new { DT_RowId = x.ID, AreaAcomp = x.Area.ToString(), Fecha = x.fecha.ToShortDateString(), Horas = x.horas, PersonaNombre = x.Personal.Persona.Nombres, CicloFormativo = x.ActividadAcompanamiento.SuperCicloFormativo.nombre, x.actividadAcompanamientoID, ActividadAcompanamiento = x.ActividadAcompanamiento.TipoAcompanamiento.ToString(), x.personalID, CentroNombre = x.Personal.Centro.Nombre, Cedula = x.Personal.Persona.Cedula }).OrderByDescending(f => f.DT_RowId).Skip(from).Take(limit);

            var jsonData = new
            {
                draw = values.raw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data.ToList()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);


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
            var jsonData = new
            {
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
                try
                {
                    inscripcion.userId = User.Identity.Name;
                    inscripcion.horas = Math.Abs(inscripcion.horas); // para evitar valores negativos
                    db.InscripcionesActividadesAcompanamiento.Add(inscripcion);
                    await db.SaveChangesAsync();


                    ActividadAcompanamiento actividad = await db.ActividadAcompanamientoes.FindAsync(inscripcion.actividadAcompanamientoID);
                    cicloId = actividad.SuperCicloFormativoId;
                    tipo = actividad.TipoAcompanamiento.ToString();
                    docenteId = inscripcion.personalID;

                    //Envia por Email Verificacion
                    dynamic email = new Email("EmailActividadAcompanamiento");
                    email.IdInscripcion = inscripcion.ID;
                    email.TipoAcompanamiento = actividad.TipoAcompanamiento;
                    email.cicloFormativo = actividad.SuperCicloFormativo.nombre;

                    var persona = db.Personas.AsNoTracking().Where(p => p.Cedula == User.Identity.Name).Select(x => new { x.Cedula, x.Nombres, x.PrimerApellido, x.mail }).FirstOrDefault();
                    email.acompanante = persona != null ? persona.Nombres + " " + persona.PrimerApellido : "";
                    Personal personal = await db.Personal.FindAsync(inscripcion.personalID);
                    email.docente = persona != null ? personal.Persona.NombreCompleto.ToString() : "";
                    email.escuela = personal.Centro.Nombre;
                    email.fecha = inscripcion.fecha;
                    email.horas = inscripcion.horas;
                    email.area = inscripcion.Area;
                    email.comentario = inscripcion.comentario;
                    email.grado = inscripcion.Grado;
                    email.Subject = "Actividad de Acompañamiento " + actividad.TipoAcompanamiento + " Creada" + " Centro " + personal.Centro.Nombre;
                    if (verifyEmail(persona.mail))
                    {
                        email.To = "sismo@stat5.com" + "," + persona.mail;
                    }
                    else {
                        email.To = "sismo@stat5.com";
                    }                   
                    email.Send();

                }
                catch (Exception e)
                {
                    var msj = e.Message;
                    return Json(new { success = false });
                }

                // return RedirectToAction("DocenteDetailsAcompanante", "Docente", new { id = inscripcion.personalID});
            }
            return Json(new
            {
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
        public ActionResult Create([Bind(Include = "ID,personalID,actividadAcompanamientoID,fecha,horas")] InscripcionActividadAcompanamiento inscripcionActividadAcompanamiento)
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
            ActividadAcompanamiento actividad = inscripcionActividadAcompanamiento.ActividadAcompanamiento;


            if (inscripcionActividadAcompanamiento == null)
            {
                response = "ERROR";
            }
            try
            {

                bool hasRespuestasEvaluaciones = await db.EvaluacionAcompanamientoRespuestas.AsNoTracking().Select(x => new { x.InscripcionActividadAcompanamientoId, x.Id }).AnyAsync(p => p.InscripcionActividadAcompanamientoId == inscripcionId);
                if (hasRespuestasEvaluaciones)
                {
                    List<EvaluacionAcompanamientoRespuesta> respuestasAcompanamiento = new List<EvaluacionAcompanamientoRespuesta>();
                    respuestasAcompanamiento = await db.EvaluacionAcompanamientoRespuestas.Where(p => p.InscripcionActividadAcompanamientoId == inscripcionId).ToListAsync();
                    foreach (var respA in respuestasAcompanamiento)
                    {
                        db.EvaluacionAcompanamientoRespuestas.Remove(respA);
                        await db.SaveChangesAsync();
                    }
                }

                //ActividadAcompanamiento actividad = await db.ActividadAcompanamientoes.FindAsync(inscripcionActividadAcompanamiento.actividadAcompanamientoID);
                cicloId = actividad.SuperCicloFormativoId;
                tipo = actividad.TipoAcompanamiento.ToString();
                docenteId = inscripcionActividadAcompanamiento.personalID;
                superCicloFormativoNombre = actividad.SuperCicloFormativo.nombre;
                db.InscripcionesActividadesAcompanamiento.Remove(inscripcionActividadAcompanamiento);
                await db.SaveChangesAsync();


                //Envia por Email Verificacion
                dynamic email = new Email("EmailActividadBorrar");
                email.IdInscripcion = inscripcionId;
                email.TipoAcompanamiento = actividad.TipoAcompanamiento;
                email.cicloFormativo = actividad.SuperCicloFormativo.nombre;

                var persona = db.Personas.AsNoTracking().Where(p => p.Cedula == User.Identity.Name).Select(x => new { x.Cedula, x.Nombres, x.PrimerApellido, x.mail}).FirstOrDefault();
                email.acompanante = persona != null ? persona.Nombres + " " + persona.PrimerApellido : "";
                Personal personal = await db.Personal.FindAsync(docenteId);
                email.docente = personal.Persona.NombreCompleto.ToString();
                email.escuela = personal.Centro.Nombre;
                email.fecha = inscripcionActividadAcompanamiento.fecha;
                email.horas = inscripcionActividadAcompanamiento.horas;
                email.area = inscripcionActividadAcompanamiento.Area.ToString();
                email.Subject = "Actividad de Acompañamiento " + actividad.TipoAcompanamiento + " Borrada" + " Centro " + personal.Centro.Nombre;

                if (verifyEmail(persona.mail))
                {
                    email.To = "sismo@stat5.com" + "," + persona.mail;
                }
                else
                {
                    email.To = "sismo@stat5.com";
                }
                email.Send();

                //Enviar por email
                //Personal personal = await db.Personal.FindAsync(inscripcionActividadAcompanamiento.personalID);
                //string tituloEmail = tipo;
                //StringBuilder textoEmail = new StringBuilder();
                //textoEmail.AppendLine("<h1>Ciclo Formativo: " + superCicloFormativoNombre + " " + tipo + "</h1>");
                //textoEmail.AppendLine("<p>" + "Nombre: " + personal.Persona.Nombres + " fecha:" + inscripcionActividadAcompanamiento.fecha + " horas: " + inscripcionActividadAcompanamiento.horas + " Area: " + inscripcionActividadAcompanamiento.Area + "</p>");
                //textoEmail.AppendLine("<h2>Borrada Por: " + User.Identity.Name + "</h2>");
                //await Logger.LogEvent(User.Identity.Name, "Actividad de " + tituloEmail + " Borrada" + User.Identity.Name, textoEmail.ToString(), "", DateTime.Now);
                //End Enviar por email

            }
            catch (Exception e)
            {
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


        private bool verifyEmail(string email)
        {
            bool result = true;
            string emailRegex = @"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$";
            Regex re = new Regex(emailRegex);
            if (email == null) {
                return false;            
            }
            if (!re.IsMatch(email))
            {
                return false;
            }
            return result;
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
