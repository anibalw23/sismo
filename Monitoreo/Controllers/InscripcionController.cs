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
using Monitoreo.Models.BO;
using System.Threading.Tasks;
using System.Text;
using Monitoreo.Helpers;
using Monitoreo.Models.BO.ViewModels.InscripcionCicloFormativoVm;

namespace Monitoreo.Controllers
{
    //[Authorize(Roles = "Administrador")]
    [Authorize]
    public class InscripcionController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();
        Logger Logger = new Logger();


        [Authorize(Roles = "Administrador, Acompanante, Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        [HttpPost]
        public JsonResult GetPersonalByGrupoAndRol(string escuela, string Rol)
        {
            var participantes = new List<Persona>();
            var personal = new List<Personal>();
            var calendarios = new List<CalendarioCicloFormativo>();

            try
            {
                if (Rol == "Participante")
                {
                    //Recordar poner este codigo
                    var personalDocente = db.Personal.Where(s => s.Centro.Nombre == escuela);
                    foreach (var pd in personalDocente)
                    {
                        var participanteDocente = new Persona();
                        participanteDocente.Id = pd.Persona.Id;
                        participanteDocente.Cedula = pd.Persona.Cedula;
                        participanteDocente.Nombres = pd.Persona.NombreCompleto;
                        participantes.Add(participanteDocente);
                    }

                }
                if (Rol == "Formador" || Rol == "Acompanante")
                {
                    var acompanantes = db.Acompanantes.Where(s => s.Centro.Nombre == escuela).ToList();
                    foreach (var p in acompanantes)
                    {
                        var participante = new Persona();
                        participante.Cedula = p.Persona.Cedula;
                        participante.Id = p.Persona.Id;
                        participante.Nombres = p.Persona.NombreCompleto;
                        participantes.Add(participante);
                    }
                }
            }
            catch (Exception exp)
            {
                var dummy = exp.Message;
            }
            var jsonData = new
            {
                data = participantes.Select(c => new
                {
                    label = c.Cedula + " " + c.Nombres,
                    title = c.Cedula,
                    value = c.Id
                })

            };
            return Json(jsonData.data, JsonRequestBehavior.AllowGet);
        }




        // GET: Inscripciones
        [Route("Inscripciones")]
        [Authorize(Roles = "Administrador, Acompanante, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult Index()
        {
            var inscripciones = db.Inscripciones.Include(i => i.CicloFormativo).Include(i => i.Participante).Include(i => i.GrupoCicloFormativo);//Include(i => i.Seccion);
            return View(inscripciones.ToList());
        }

        // GET: RedCentros
        [Authorize(Roles = "Administrador, Acompanante, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult CicloInscripciones(int CicloFormativoId)
        {
            List<Inscripcion> inscripciones = new List<Inscripcion>();

            ViewBag.MasterType = "CicloFormativo";
            ViewBag.MasterId = CicloFormativoId;
            List<SelectListItem> items = new List<SelectListItem>();
            var grupos = db.GruposCiclosFormativos.Select(x => new { x.ID, x.CicloFormativoId, x.Centro.Nombre }).Where(c => c.CicloFormativoId == CicloFormativoId);
            foreach(var grupo in grupos){
                items.Add(new SelectListItem { Text = grupo.Nombre, Value = grupo.ID.ToString() });
            }
            ViewBag.Grupo = new SelectList(items, "Value", "Text");
            //ViewBag.Grupo = new SelectList(db.GruposCiclosFormativos.Select(x => new { x.ID, x.CicloFormativoId,x.Centro.Nombre}).Where(c => c.CicloFormativoId == CicloFormativoId), "Id", "Centro.Nombre");
            //GrupoCicloFormativo grupoc = new GrupoCicloFormativo();
            return PartialView(inscripciones.ToList());
        }


        [Authorize(Roles = "Administrador, Acompanante,Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        public JsonResult CicloInscripcionesAjax(int CicloFormativoId, int grupoId)
        {

            List<Inscripcion> inscripciones = new List<Inscripcion>();
            List<InscripcionCicloFormativoVm> inscripcionesVm = new List<InscripcionCicloFormativoVm>();
            var inscripcionesAnonymous = db.Inscripciones.Select(x => new { x.Id, x.Participante.Cedula, x.Participante.Nombres, x.Participante.PrimerApellido, x.Participante.SegundoApellido, x.Rol, x.GrupoCicloFormativo.Centro.Nombre, x.CicloFormativoId, x.GrupoCicloFormativoId }).Where(s => s.CicloFormativoId == CicloFormativoId).Where(f => f.GrupoCicloFormativoId == grupoId);
           
            foreach(var inscripcion in inscripcionesAnonymous){
                inscripcionesVm.Add(new InscripcionCicloFormativoVm { cedula = inscripcion.Cedula, grupo = inscripcion.Nombre, Id = inscripcion.Id, nombre = inscripcion.Nombres + " " + inscripcion.PrimerApellido + " " + inscripcion.SegundoApellido, rol = inscripcion.Rol.ToString() });
            }
           
            var jsonData = new
            {
                data = inscripcionesVm.Select(y => new
                {
                    id = y.Id,
                    cedula = y.cedula,
                    nombre = y.nombre,
                    grupo = y.grupo,
                    rol = y.rol
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        // POST: Inscripciones
        [Route("Inscripciones/GetDataJson")]
        [Authorize(Roles = "Administrador, Acompanante, AdministradorTransversal,EspecialistaCurricular")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var inscripciones = db.Inscripciones.Include(i => i.CicloFormativo).Include(i => i.Participante).Include(i => i.GrupoCicloFormativo);
            var recordsTotal = inscripciones.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = inscripciones.Select(x => new { DT_RowId = x.Id, x.CicloFormativoId, Grupo = x.GrupoCicloFormativo.Centro.Nombre, Fecha = x.Fecha, Cedula = x.Participante.Cedula, Rol = x.Rol });

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

        // GET: /Inscripcion/5/Details
        [Authorize(Roles = "Administrador, Acompanante, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inscripcion inscripcion = db.Inscripciones.Find(id);
            if (inscripcion == null)
            {
                return HttpNotFound();
            }
            return View(inscripcion);
        }

        public  async Task<ActionResult> CreateModal(int cicloID, int personaID, int? centroID)
        {
            Inscripcion inscripcion = new Inscripcion();
            GrupoCicloFormativo grupo = new GrupoCicloFormativo();
            //List<CicloFormativo> actividadesFormativas = new List<CicloFormativo>();
            int centroId = 0;
            try
            {
                
                if (centroID != 0 && centroID != null)
                {
                    centroId = Convert.ToInt32(centroID);                   
                }
                else {
                    centroId = db.Personal.AsNoTracking().Select(x => new {x.CentroId, x.Id }).Where(p => p.Id == personaID).FirstOrDefault().CentroId;  //camnie esto ahora probar(*)
                }
                inscripcion.CicloFormativoId = cicloID;
                inscripcion.Fecha = DateTime.Now;
                grupo = await db.GruposCiclosFormativos.AsNoTracking().Where(s => s.CicloFormativo.SuperCicloFormativoId == cicloID).Where(c => c.CentroID == centroId).FirstOrDefaultAsync();
                if(grupo != null){
                    inscripcion.GrupoCicloFormativo = grupo;
                    inscripcion.GrupoCicloFormativoId = grupo.ID;
                }               
                inscripcion.Participante = db.Personal.Find(personaID).Persona;
                inscripcion.ParticipanteId = inscripcion.Participante.Id;
                inscripcion.Rol = InscripcionRol.Participante;
                //actividadesFormativas = await  db.CiclosFormativos.AsNoTracking().Where(s => s.SuperCicloFormativoId == cicloID).ToListAsync();
            }
            catch (Exception e)
            {
                var msj = e.Message;
            }

            ViewBag.actividades = new SelectList(db.CiclosFormativos.AsNoTracking().Select(x => new { x.SuperCicloFormativoId, x.Id, x.Tema}).Where(s => s.SuperCicloFormativoId == cicloID), "Id", "Tema");
            ViewBag.centro = centroId;
            return PartialView(inscripcion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateModal(Inscripcion inscripcion)
        {
            //List<CicloFormativo> actividadesFormativas = new List<CicloFormativo>();
          
            if (ModelState.IsValid)
            {
                bool isRepeated = await db.Inscripciones.AsNoTracking().Select(x => new {x.ParticipanteId, x.CicloFormativoId }).Where(p => p.ParticipanteId == inscripcion.ParticipanteId).AnyAsync(c => c.CicloFormativoId == inscripcion.CicloFormativoId);
                if (!isRepeated)
                {
                    try {
                        if(inscripcion.GrupoCicloFormativoId == 0){
                            GrupoCicloFormativo grupoCiclo = new GrupoCicloFormativo();
                            grupoCiclo.CentroID =  Convert.ToInt32(Request.Form["centroId"]);
                            grupoCiclo.CicloFormativoId = inscripcion.CicloFormativoId;
                            db.GruposCiclosFormativos.Add(grupoCiclo);
                            await db.SaveChangesAsync();
                            inscripcion.GrupoCicloFormativoId = grupoCiclo.ID;
                        }

                        db.Inscripciones.Add(inscripcion);
                        await db.SaveChangesAsync();
                        //Enviar por email
                        string tituloEmail = "Inscripción Actividad Presencial";
                        StringBuilder textoEmail = new StringBuilder();
                        CicloFormativo ciclo = db.CiclosFormativos.Find(inscripcion.CicloFormativoId);
                        textoEmail.AppendLine("<h1>Actividad Formativa: " + ciclo.Tema + "</h1>");
                        textoEmail.AppendLine("<h2>Creado Por: " + User.Identity.Name + "</h2>");
                        textoEmail.AppendLine("<p>" + "ParticipanteId: " + inscripcion.ParticipanteId + "</p>");
                        await Logger.LogEvent(User.Identity.Name, "Actividad de " + tituloEmail + " creada " + User.Identity.Name, textoEmail.ToString(), "", DateTime.Now);
                        //End Enviar por email
                    }
                    catch(Exception e){
                        var msj = e.Message;
                    }
                }

                //actividadesFormativas =  await db.CiclosFormativos.Where(i => i.Id == inscripcion.CicloFormativoId).ToListAsync();
            }
            ViewBag.actividades = new SelectList(db.CiclosFormativos.Select(x => new{x.Id, x.Tema }).Where(i => i.Id == inscripcion.CicloFormativoId), "Id", "Tema");
            return View(inscripcion);
        }




        public async Task<ActionResult> DeleteModal(int cicloID, int personaID)
        {
            Inscripcion inscripcion = new Inscripcion();
            List<Inscripcion> inscripciones = new List<Inscripcion>();
            GrupoCicloFormativo grupo = new GrupoCicloFormativo();
            List<CicloFormativo> actividadesFormativas = new List<CicloFormativo>();
            try
            {
                inscripcion.Participante = db.Personal.Find(personaID).Persona;
                inscripciones = await db.Inscripciones.AsNoTracking().Where(s => s.CicloFormativo.SuperCicloFormativoId == cicloID).Where(p => p.Participante.Id == inscripcion.Participante.Id).ToListAsync();
                actividadesFormativas = inscripciones.Select(x => x.CicloFormativo).ToList();
                inscripcion.Fecha = DateTime.Now;               
                inscripcion.Rol = InscripcionRol.Participante;
                inscripcion.ParticipanteId = inscripcion.Participante.Id;
            }
            catch (Exception e)
            {
                var msj = e.Message;
            }

            ViewBag.actividades = new SelectList(inscripciones.Select(x => new { x.CicloFormativo.Tema, x.Id }), "Id", "Tema");
            return PartialView(inscripcion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteModal(Inscripcion inscripcion)
        {
            List<CicloFormativo> actividadesFormativas = new List<CicloFormativo>();
            Inscripcion inscripcionDelete = await db.Inscripciones.FindAsync(inscripcion.Id);
            if (ModelState.IsValid)
            {
                try
                {
                    db.Inscripciones.Remove(inscripcionDelete);
                    await db.SaveChangesAsync();

                    //Enviar por email
                    string tituloEmail = "Desinscripción Actividad Presencial";
                    StringBuilder textoEmail = new StringBuilder();
                    CicloFormativo ciclo = db.CiclosFormativos.Find(inscripcionDelete.CicloFormativoId);
                    textoEmail.AppendLine("<h1>Actividad Formativa: " + ciclo.Tema + "</h1>");
                    textoEmail.AppendLine("<h2>Creado Por: " + User.Identity.Name + "</h2>");
                    textoEmail.AppendLine("<p>" + "Nombre: " + inscripcionDelete.ParticipanteId + "</p>");
                    await Logger.LogEvent(User.Identity.Name, "Actividad de " + tituloEmail + " borrada " + User.Identity.Name, textoEmail.ToString(), "", DateTime.Now);
                    //End Enviar por email
                }
                catch (Exception e)
                {
                    var msj = e.Message;
                }
                //actividadesFormativas = await db.CiclosFormativos.AsNoTracking().Where(i => i.Id == inscripcion.CicloFormativoId).ToListAsync();
            }
            ViewBag.actividades = new SelectList(db.CiclosFormativos.AsNoTracking().Where(i => i.Id == inscripcion.CicloFormativoId).Select(x => new {x.Id, x.Tema }), "Id", "Tema");
            return View(inscripcion);
        }


        // GET: Inscripciones/Create
        [Route("Inscripciones/Create")]
        public ActionResult Create()
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];
            int cicloID = Convert.ToInt32(Request.Params["MasterId"]);

            if (User.IsInRole("Administrador") || User.IsInRole("AdministradorTransversal") || User.IsInRole("EspecialistaCurricular"))
            {
                ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema");
                ViewBag.GrupoCicloFormativoId = new SelectList(db.GruposCiclosFormativos.Where(c => c.CicloFormativoId == cicloID).Select(c => new { Id = c.ID, Nombre = c.Centro.Nombre }), "Id", "Nombre");
            }
            if (User.IsInRole("Acompanante") || User.IsInRole("Formador"))
            {
                ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema");

                List<Inscripcion> inscripciones = new List<Inscripcion>();
                Persona participante = new Persona();
                try
                {
                    participante = db.Personas.Where(p => p.Cedula == User.Identity.Name).SingleOrDefault();
                    if (participante != null)
                    {
                        inscripciones = db.Inscripciones.Where(p => p.Participante.Id == participante.Id).Include(p => p.Participante).ToList();
                        ViewBag.GrupoCicloFormativoId = new SelectList(db.GruposCiclosFormativos.Where(c => c.CicloFormativoId == cicloID).Where(i => i.inscripciones.Any(p => p.ParticipanteId == participante.Id)).Select(c => new { Id = c.ID, Nombre = c.Centro.Nombre }), "Id", "Nombre");

                    }
                }
                catch (Exception e)
                {
                    var msj = e.Message;
                }
            }

            if (Request.Params["Modal"] != null)
            {
                return PartialView();
            }

            return View();
        }




        // POST: Inscripciones/Create
        [Route("Inscripciones/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inscripcion inscripcion)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];
            string personaId = Request.Form["personaId"];
            if (ModelState.IsValid)
            {

                try
                {
                    if (personaId != null)
                    {

                        string[] personasID = personaId.Split(',');
                        for (var ii = 0; ii < personasID.Count(); ii++)
                        {
                            Inscripcion tempInscripcion = new Inscripcion();
                            tempInscripcion.CicloFormativoId = inscripcion.CicloFormativoId;
                            tempInscripcion.GrupoCicloFormativoId = inscripcion.GrupoCicloFormativoId;
                            tempInscripcion.ParticipanteId = Convert.ToInt32(personasID[ii]);
                            tempInscripcion.Fecha = DateTime.Now;
                            tempInscripcion.Rol = inscripcion.Rol;
                            db.Inscripciones.Add(tempInscripcion);
                            db.SaveChanges();
                        }


                        if (ViewBag.MasterType != null)
                        {
                            return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
                        }

                        return RedirectToAction("Index");
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("Error 15625", "Faltan Datos!");
                }

            }

            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema", inscripcion.CicloFormativoId);
            ViewBag.GrupoCicloFormativoId = new SelectList(db.GruposCiclosFormativos.Where(c => c.CicloFormativoId == inscripcion.CicloFormativoId), "Id", "nombre");

            //ViewBag.SeccionId = new SelectList(db.Secciones, "Id", "Numero", inscripcion.SeccionId);
            return View(inscripcion);
        }

        // GET: /Inscripcion/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inscripcion inscripcion = db.Inscripciones.Find(id);
            if (inscripcion == null)
            {
                return HttpNotFound();
            }


            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema", inscripcion.CicloFormativoId);
            ViewBag.GrupoCicloFormativo = new SelectList(db.GruposCiclosFormativos.Where(c => c.CicloFormativoId == inscripcion.CicloFormativoId).Select(c => new { Id = c.ID, Nombre = c.Centro.Nombre }), "Id", "Nombre");

            //ViewBag.GrupoCicloFormativoId = new SelectList(db.GruposCiclosFormativos.Where(c => c.CicloFormativoId == inscripcion.CicloFormativoId), "Id", "nombre");
            return View(inscripcion);
        }

        // POST: /Inscripcion/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Inscripcion inscripcion)
        {
            if (ModelState.IsValid)
            {
                inscripcion.Fecha = DateTime.Now;
                db.Entry(inscripcion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "CicloFormativo", new { Id = inscripcion.CicloFormativoId });
            }


            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema", inscripcion.CicloFormativoId);
            ViewBag.GrupoCicloFormativo = new SelectList(db.GruposCiclosFormativos.Where(c => c.CicloFormativoId == inscripcion.CicloFormativoId), "Id", "nombre");
            //ViewBag.SeccionId = new SelectList(db.Secciones, "Id", "Numero", inscripcion.SeccionId);
            return View(inscripcion);
        }

        // GET: /Inscripcion/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inscripcion inscripcion = db.Inscripciones.Find(id);
            if (inscripcion == null)
            {
                return HttpNotFound();
            }
            return View(inscripcion);
        }

        // POST: /Inscripcion/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inscripcion inscripcion = db.Inscripciones.Find(id);
            db.Inscripciones.Remove(inscripcion);
            db.SaveChanges();
            return RedirectToAction("Details", "CicloFormativo", new { Id = inscripcion.CicloFormativoId });
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
