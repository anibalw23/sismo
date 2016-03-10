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
using System.IO;
using System.Data.OleDb;
using Monitoreo.Models.BO;
using Monitoreo.Models.BO.ViewModels;
using System.Text;
using Monitoreo.Helpers;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;

namespace Monitoreo.Controllers
{
    [Authorize]
    public class CicloFormativoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();
        Logger Logger = new Logger();

        // GET: CiclosFormativos
        [Route("CiclosFormativos")]
        [Authorize(Roles = "Administrador, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult Index()
        {
            var ciclosformativos = db.CiclosFormativos;
            return View(ciclosformativos.ToList());
        }

        // GET: /CicloFormativo/5/Details
       
        [Route("CicloFormativo/Details")]
        [Authorize(Roles = "Administrador, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CicloFormativo cicloFormativo = db.CiclosFormativos.Find(id);
            if (cicloFormativo == null)
            {
                return HttpNotFound();
            }
            ViewBag.Grupos = db.GruposCiclosFormativos.AsNoTracking().Include(c => c.Centro).Include(c => c.CicloFormativo).Where(c => c.CicloFormativoId == id).ToList();
               
            return View(cicloFormativo);
        }

        // GET: CiclosFormativos/Create
        [Route("CiclosFormativos/Create")]
        [Authorize(Roles = "Administrador, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult Create()
        {
            ViewBag.SuperCicloFormativoId = new SelectList(db.SuperCicloFormativoes.Select(x => new { x.Id, x.nombre }), "Id", "nombre");
            //ViewBag.CentrosId = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
            var tipoModuloFormativo = new Dictionary<int, string>();
            foreach (var item in Enum.GetValues(typeof(TipoModuloFormativo)))
            {
                int val = (int)item;
                string b = Enum.GetName(typeof(TipoModuloFormativo), item);
                tipoModuloFormativo.Add((int)val, b);
            }
            ViewBag.ModuloFormativoList = new SelectList(tipoModuloFormativo, "Key", "Value");

            return View();
        }

        // POST: CiclosFormativos/Create
        [Authorize(Roles = "Administrador, AdministradorTransversal,EspecialistaCurricular")]
        [Route("CiclosFormativos/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CicloFormativo cicloFormativo)
        {

            CalendarioCicloFormativo calendarioCiclo = new CalendarioCicloFormativo();

            List<Inscripcion> inscripcionesDocentes = new List<Inscripcion>();

            var superCicloId = Request.QueryString["superCicloId"];
            //var escuelaID = Request.Form["CentrosId"];

            if (superCicloId != "")
            {
                cicloFormativo.SuperCicloFormativoId = Convert.ToInt32(superCicloId);
            }

            if (ModelState.IsValid)
            {

                    cicloFormativo.CreadoPor = User.Identity.Name;
                    db.CiclosFormativos.Add(cicloFormativo);
                    await  db.SaveChangesAsync();

                    calendarioCiclo.CicloFormativoID = cicloFormativo.Id;
                    calendarioCiclo.Fecha = cicloFormativo.FechaInicio;
                    calendarioCiclo.horas = cicloFormativo.DuracionTotal;
                    calendarioCiclo.TipoEvento = cicloFormativo.tipo;
                    db.CalendarioCicloFormativoes.Add(calendarioCiclo);
                    await db.SaveChangesAsync();

                    //if (escuelaID != "" && escuelaID != null)
                    //{
                    //    var escuelasID = escuelaID.Split(',');
                    //    foreach (var esc in escuelasID)
                    //    {
                    //        int idEscuela = Convert.ToInt32(esc);
                    //        GrupoCicloFormativo grupo = new GrupoCicloFormativo();
                    //        grupo.CentroID = idEscuela;
                    //        grupo.CicloFormativoId = cicloFormativo.Id;


                    //        bool isRepeated = await db.GruposCiclosFormativos.AsNoTracking().Select(x => new {x.ID,  x.CicloFormativoId, x.CentroID}).Where(i => i.CicloFormativoId == cicloFormativo.Id).AnyAsync(p => p.CentroID == grupo.CentroID);
                    //        if (!isRepeated)
                    //        {
                    //            db.GruposCiclosFormativos.Add(grupo);
                    //            await db.SaveChangesAsync();
                    //        }

                    //    }

                    //}
               ViewBag.SuperCicloFormativoId = new SelectList(db.SuperCicloFormativoes.Select(x => new { x.Id, x.nombre }), "Id", "nombre");
               //ViewBag.CentrosId = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");

                //db.SaveChanges();

                return Redirect("/SuperCicloFormativo/Details/" + superCicloId);
            }

            return View(cicloFormativo);
        }

        // GET: /CicloFormativo/5/Edit
        [Authorize(Roles = "Administrador, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CicloFormativo cicloFormativo = db.CiclosFormativos.Find(id);
            if (cicloFormativo == null)
            {
                return HttpNotFound();
            }

            ViewBag.SuperCicloFormativoId = new SelectList(db.SuperCicloFormativoes.Select(x => new { x.Id, x.nombre}), "Id", "nombre");
            //ViewBag.CentrosId = new SelectList(db.Centros, "Id", "Nombre");

            var tipoModuloFormativo = new Dictionary<int, string>();
            foreach (var item in Enum.GetValues(typeof(TipoModuloFormativo)))
            {
                int val = (int)item;
                string b = Enum.GetName(typeof(TipoModuloFormativo), item);
                tipoModuloFormativo.Add((int)val, b);
            }
            int moduloTipoSelectedValue = (int)db.CiclosFormativos.Find(cicloFormativo.Id).tipo;
            ViewBag.ModuloFormativoTipo = new SelectList(tipoModuloFormativo, "Key", "Value", moduloTipoSelectedValue.ToString());

            ViewBag.ModuloFormativoTipo = new SelectList(tipoModuloFormativo, "Key", "Value");
            return View(cicloFormativo);
        }

        [Authorize(Roles = "Administrador, AdministradorTransversal,EspecialistaCurricular")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CicloFormativo cicloFormativo, int? superCicloID)
        {
            var superCicloId = Request.QueryString["superCicloID"];
            int idSuper = 0;

            GrupoCicloFormativo grupo = new GrupoCicloFormativo();

            if (superCicloId != "")
            {
                idSuper = Convert.ToInt32(superCicloId);
                cicloFormativo.SuperCicloFormativoId = idSuper;
            }

            if (ModelState.IsValid)
            {

                CalendarioCicloFormativo cal =  db.CalendarioCicloFormativoes.Where(c => c.CicloFormativoID == cicloFormativo.Id).FirstOrDefault();
                if(cal != null){
                    cal.Fecha = cicloFormativo.FechaInicio;
                    db.Entry(cal).State = EntityState.Modified;
                    db.SaveChanges();
                }

                db.Entry(cicloFormativo).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect("/SuperCicloFormativo/Details/" + idSuper);
            }

            ViewBag.SuperCicloFormativoId = new SelectList(db.SuperCicloFormativoes, "Id", "nombre");
            //ViewBag.CentrosId = new SelectList(db.Centros, "Id", "Nombre");

            var tipoModuloFormativo = new Dictionary<int, string>();
            foreach (var item in Enum.GetValues(typeof(TipoModuloFormativo)))
            {
                int val = (int)item;
                string b = Enum.GetName(typeof(TipoModuloFormativo), item);
                tipoModuloFormativo.Add((int)val, b);
            }
            int moduloTipoSelectedValue = (int)db.CiclosFormativos.Find(cicloFormativo.Id).tipo;
            ViewBag.ModuloFormativoTipo = new SelectList(tipoModuloFormativo, "Key", "Value", moduloTipoSelectedValue);

            return View(cicloFormativo);
        }

        // GET: /CicloFormativo/5/Delete
        [Authorize(Roles = "Administrador, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CicloFormativo cicloFormativo = db.CiclosFormativos.Find(id);
            if (cicloFormativo == null)
            {
                return HttpNotFound();
            }
            return View(cicloFormativo);
        }

        // POST: /CicloFormativo/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult DeleteConfirmed(int id)
        {

            var superCicloId = Request.QueryString["superCicloID"];
            CicloFormativo cicloFormativo = new CicloFormativo();

            try
            {
                if (User.IsInRole("Administrador") || User.IsInRole("Coordinador"))
                {
                    cicloFormativo = db.CiclosFormativos.Find(id);
                    List<GrupoCicloFormativo> gruposCiclo = db.GruposCiclosFormativos.Where(c => c.CicloFormativoId == cicloFormativo.Id).ToList();
                    foreach (GrupoCicloFormativo g in gruposCiclo)
                    {
                        db.GruposCiclosFormativos.Remove(g);
                    }

                    db.CiclosFormativos.Remove(cicloFormativo);
                }
                if (User.IsInRole("Acompanante") || User.IsInRole("Formador"))
                {
                    cicloFormativo = db.CiclosFormativos.Find(id);
                    db.CiclosFormativos.Remove(cicloFormativo);

                    Persona participante = new Persona();
                    participante = db.Personas.Where(p => p.Cedula == User.Identity.Name).SingleOrDefault();
                    if (participante != null)
                    {
                        GrupoCicloFormativo grupo = db.GruposCiclosFormativos.Where(c => c.CicloFormativoId == cicloFormativo.Id).Where(p => p.inscripciones.Any(u => u.ParticipanteId == participante.Id)).SingleOrDefault();
                        db.GruposCiclosFormativos.Remove(grupo);
                    }
                }

                db.SaveChanges();
                return Redirect("/SuperCicloFormativo/Details/" + superCicloId.ToString());
            }
            catch (Exception e)
            {
                var dummy = e.Message;
                ModelState.AddModelError("1", "Debe eliminar todos los grupos de la actividad formativa antes!!!");
                return View(cicloFormativo);
            }



        }



        //Parial view CicloAsistencia
        public ActionResult CicloAsistencia(int CicloFormativoId)
        {
            List<Inscripcion> inscripciones = new List<Inscripcion>();
            inscripciones = db.Inscripciones.AsNoTracking().Include(i => i.CicloFormativo).Include(i => i.Participante).Include(i => i.GrupoCicloFormativo).Where(s => s.CicloFormativoId == CicloFormativoId).Take(1).ToList();
           
            ViewBag.MasterType = "CicloFormativo";
            ViewBag.MasterId = CicloFormativoId;
            ViewBag.CalendarioId = new SelectList(db.CalendarioCicloFormativoes.Select(x => new {x.CicloFormativoID, x.Id, x.Fecha }).Where(c => c.CicloFormativoID == CicloFormativoId), "Id", "Fecha");

            return PartialView(inscripciones.ToList());
        }

        public ActionResult CicloInscripcionesVerificarDuplicado(int[] docentesIds)
        {
            ViewBag.ciclos = new SelectList(db.SuperCicloFormativoes.Select(x => new { x.Id, x.nombre }), "Id", "nombre");
            ViewBag.DocentesIds = docentesIds;
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> CicloInscripcionesVerificarDuplicados(int[] ciclosIds, int[] docentesIds){

           List<Docente> docentesRepeated = new List<Docente>();
           foreach(var docente in docentesIds){
               Docente docenteTemp = await db.Docentes.FindAsync(docente);
               int participanteId = docenteTemp.PersonaId;
               bool isRepeated = await db.Inscripciones.Select(x => new { x.Id, x.CicloFormativoId, x.ParticipanteId, x.Participante.Cedula }).Where(c => ciclosIds.Contains(c.CicloFormativoId)).AnyAsync(p => p.ParticipanteId ==  participanteId);
               if (isRepeated)
               {
                   docentesRepeated.Add(docenteTemp);
               }              
           }

           return Json(docentesRepeated.Select(x => new {Id =  x.Id, Cedula = x.Persona.Cedula }), JsonRequestBehavior.AllowGet);
        }


        //[Authorize(Roles = "Administrador")]
        public ActionResult CicloInscripcionesBatch(int? id)
        {
            var cicloFormativo = db.CiclosFormativos.Find(id);
            ViewBag.Distrito = new SelectList(db.Distritos.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");                       
            return View(cicloFormativo);
        }

        [HttpPost]
        public async Task<ActionResult> InscribirDocentesBatch(int cicloId,  int[] docentesIds  ) {
            List<GrupoCicloFormativo> grupos = new List<GrupoCicloFormativo>();
            List<Inscripcion> inscripcionesNew = new List<Inscripcion>();
            if (cicloId != 0) {
                var inscripciones = await db.Inscripciones.Where(c => c.CicloFormativoId == cicloId).ToListAsync();
                var docentes = db.Docentes.Where(i => docentesIds.Contains(i.Id));
                var centros =  docentes.Select(x => new { x.Centro, x.CentroId }).Distinct(); //Distintos centros de los docentes a inscribir
                
                //Anade los Grupos al Ciclo Formativo
                foreach (var centro in centros) {
                    bool isGrupoRepeated = inscripciones.Select(x => new { x.GrupoCicloFormativoId, x.GrupoCicloFormativo }).Any(c => c.GrupoCicloFormativo.CentroID == centro.CentroId);
                    if (!isGrupoRepeated) {
                        GrupoCicloFormativo grupo = new GrupoCicloFormativo();
                        grupo.CentroID = centro.CentroId;
                        grupo.CicloFormativoId = cicloId;
                        grupos.Add(grupo);                       
                    }
                }
                db.GruposCiclosFormativos.AddRange(grupos);
                await db.SaveChangesAsync();

                //Anade los Dcoentes a cada grupo del Ciclo Formativo
                foreach(var doc in docentes){
                    bool isRepeatedInscripcion = inscripciones.Any(p => p.ParticipanteId == doc.PersonaId);
                    if (!isRepeatedInscripcion) {
                        inscripcionesNew.Add(new Inscripcion { CicloFormativoId = cicloId, GrupoCicloFormativoId = grupos.Where(c => c.CentroID == doc.CentroId).FirstOrDefault().ID, Rol = InscripcionRol.Participante, ParticipanteId = doc.PersonaId, Fecha = DateTime.Now }); 
                    }
                }
                db.Inscripciones.AddRange(inscripcionesNew);
                await db.SaveChangesAsync();                
                return Json("OK", JsonRequestBehavior.AllowGet);

            }
            return Json("ERROR", JsonRequestBehavior.AllowGet);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador,AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult CicloInscripcionesBatchImport(int? id)
        {

            //var inscripciones = db.Inscripciones.Include(i => i.CicloFormativo).Include(i => i.Participante).Include(i => i.GrupoCicloFormativo);
            //List<Docente> docentes = new List<Docente>();
            //List<Docente> docentesGrado = new List<Docente>();
            //string[] grados;
            //List<int> gradosList = new List<int>(); ;
            //int centroId = 0;
            //int RedId = 0;
            //int gradoId = 0;
            //int SeccionId = 0;
            //GrupoCicloFormativo grupo = new GrupoCicloFormativo();
            //try
            //{

            //    string centroTest = Request.Form["centroId"].ToString();
            //    if (Request.Form["centroId"] != "")
            //    {
            //        centroId = Convert.ToInt32(Request.Form["centroId"].ToString());
            //    }
            //    if (Request.Form["RedId"] != "")
            //    {
            //        RedId = Convert.ToInt32(Request.Form["RedId"].ToString());
            //    }

            //    int areaId = Convert.ToInt32(Request.Form["areaId"].ToString());
            //    int nivelId = Convert.ToInt32(Request.Form["nivelId"].ToString());
            //    int cicloId = Convert.ToInt32(Request.Form["cicloId"].ToString());

            //    if (Request.Form["GrupoId"] != "")
            //    {
            //        SeccionId = Convert.ToInt32(Request.Form["GrupoId"].ToString());
            //    }

            //    if (gradoId != 0)
            //    {
            //        gradoId = Convert.ToInt32(Request.Form["gradoId"].ToString());
            //        string Grado = Enum.GetName(typeof(Grado), gradoId);
            //    }


            //    if (RedId != 0)
            //    {
            //        if (gradoId == 0)
            //        {
            //            docentes = db.Docentes.Where(c => c.Centro.RedId == RedId).Include(dm => dm.Materias).ToList();
            //        }
            //        else
            //        {
            //            //docentes = db.Docentes.Where(c => c.Centro.RedId == RedId)
            //            //                      .Where(d => d.Materias.Any(a => a.Grados.ToString() == Grado)).ToList();
            //        }
            //    }
            //    if (centroId != 0)
            //    {
            //        docentes = db.Docentes.Where(c => c.CentroId == centroId).Include(dm => dm.Materias).ToList();
            //    }

            //    string Area = Enum.GetName(typeof(DocenteArea), areaId);
            //    string Nivel = Enum.GetName(typeof(NivelEducativo), nivelId);
            //    string Ciclo = Enum.GetName(typeof(DocenteCiclo), cicloId);


            //    List<Docente> docentesFilter = docentes.Where(d => d.Materias.Any(a => a.Area.ToString() == Area))
            //                                           .Where(b => b.Materias.Any(c => c.Nivel.ToString() == Nivel))
            //                                           .Where(b => b.Materias.Any(c => c.Ciclo.ToString() == Ciclo))
            //                                           .ToList();


            //    if (SeccionId == 0 && RedId != 0 && cicloId != 0)
            //    {
            //        Red redGrupo = db.Redes.Find(RedId);
            //        grupo.CicloFormativoId = cicloId;
            //        grupo.CentroID = centroId;
            //        bool isRepeated = db.GruposCiclosFormativos.Where(i => i.CicloFormativoId == id).Any(p => p.CentroID == grupo.CentroID);
            //        if (!isRepeated)
            //        {
            //            db.GruposCiclosFormativos.Add(grupo);
            //            db.SaveChanges();
            //        }
            //    }

            //    foreach (var doc in docentesFilter)
            //    {
            //        Inscripcion inscripcion = new Inscripcion();
            //        bool isRepeated = db.Inscripciones.Where(i => i.CicloFormativoId == id).Any(p => p.ParticipanteId == doc.PersonaId);
            //        if (!isRepeated)
            //        {
            //            inscripcion.Fecha = DateTime.Now;
            //            inscripcion.CicloFormativoId = (int)id;

            //            if (SeccionId == 0)
            //            {
            //                inscripcion.GrupoCicloFormativo = grupo;
            //            }
            //            else
            //            {
            //                inscripcion.GrupoCicloFormativoId = SeccionId;
            //            }

            //            inscripcion.ParticipanteId = doc.PersonaId;
            //            inscripcion.Rol = InscripcionRol.Participante;
            //            db.Inscripciones.Add(inscripcion);
            //            db.SaveChanges();
            //        }


            //    }


            //}
            //catch (Exception e)
            //{
            //    var dummy = e.Message;
            //}

            return RedirectToAction("Details/" + id.ToString());
            //return View();
        }

        [Authorize(Roles = "Administrador, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult CicloInscripcionesExcel(int? id)
        {
            var cicloFormativo = db.CiclosFormativos.Find(id);
            return View(cicloFormativo);
        }

        public ActionResult CicloAsistenciaExcel(int? id, int calendarioId)
        {
            var cicloFormativo = db.CiclosFormativos.Find(id);
            ViewBag.calendarioId = calendarioId;
            return PartialView(cicloFormativo);
        }

        [Authorize(Roles = "Administrador, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult CicloInscripcionAdministrativo(int cicloId) {
            CicloFormativo ciclo = db.CiclosFormativos.Find(cicloId);
            ViewBag.Distritos = new SelectList(db.Distritos.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
            return PartialView(ciclo);
        }

        [Authorize(Roles = "Administrador, AdministradorTransversal,EspecialistaCurricular")]
        public async Task<JsonResult> InscribirPersonalAdministrativo(int cicloId, int[] centrosIds, int[] tiposPersonalIds)
        {

            Array personalFunciones = Enum.GetValues(typeof(PersonalFuncion)); 
            List<PersonalAdministrativo> personalInscribir = new List<PersonalAdministrativo>();
            List<Inscripcion> inscripcionesPersonal = new List<Inscripcion>();

            string result = "OK";
            
            //Grupo de Ciclo Formativo
            foreach(var centroId in centrosIds){
                GrupoCicloFormativo grupo = new GrupoCicloFormativo();
                grupo.CentroID = centroId;
                grupo.CicloFormativoId = cicloId;
                var grupoCiclo = await db.GruposCiclosFormativos.AsNoTracking().Select(x => new { x.ID, x.CicloFormativoId, x.CentroID }).Where(i => i.CicloFormativoId == cicloId).Where(p => p.CentroID == grupo.CentroID).SingleOrDefaultAsync();
                if (grupoCiclo == null)
                {
                    db.GruposCiclosFormativos.Add(grupo);
                    await db.SaveChangesAsync();
                }
                else {
                    grupo.ID = grupoCiclo.ID;
                    grupo.CentroID = grupoCiclo.CentroID;
                    grupo.CicloFormativoId = grupoCiclo.CicloFormativoId;
                }

                foreach (var tipo in tiposPersonalIds)
                {
                    personalInscribir = await db.PersonalAdministrativo.AsNoTracking().Where(d => d.CentroId ==  centroId).Where(t => t.FuncionesEjerce.HasFlag((PersonalFuncion)tipo)).ToListAsync();
                    foreach (var personal in personalInscribir)
                    {

                        bool isRepeatedInscripcion =  await db.Inscripciones.Where(c => c.CicloFormativoId == cicloId).Where(g => g.GrupoCicloFormativoId == grupo.ID).AnyAsync(p => p.ParticipanteId == personal.Persona.Id);
                        if (!isRepeatedInscripcion)
                        {
                            Inscripcion inscripcionDocente = new Inscripcion();
                            inscripcionDocente.CicloFormativoId = cicloId;
                            Persona participante = new Persona();
                            inscripcionDocente.ParticipanteId = personal.Persona.Id;
                            inscripcionDocente.Rol = InscripcionRol.Participante;
                            inscripcionDocente.Fecha = DateTime.Now;
                            inscripcionDocente.GrupoCicloFormativoId = grupo.ID;
                            inscripcionesPersonal.Add(inscripcionDocente);
                      
                        }

                        
                    }
                }


            }
            db.Inscripciones.AddRange(inscripcionesPersonal);
            await db.SaveChangesAsync();

            var jsonData = new
            {
                data = result,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Administrador,AdministradorTransversal,EspecialistaCurricular")]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult RegistrarAsistenciaFromExcelFile(HttpPostedFileBase uploadFile)
        {
            List<AssitenciaVM> asistencias = new List<AssitenciaVM>();
            StringBuilder textoEmail = new StringBuilder();
            int contadorFaltantes = 0;

            int cicloId = 0;
            int calendarioId = 0;

                cicloId = Convert.ToInt32(Request["id"]);
                calendarioId = Convert.ToInt32(Request["calendarioId"]);
                if (uploadFile.ContentLength > 0)
                {
                    string filePath = Path.Combine(HttpContext.Server.MapPath("../"), Path.GetFileName(uploadFile.FileName));
                    uploadFile.SaveAs(filePath);
                    DataSet ds = new DataSet();
                    string ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=Excel 12.0;";

                    using (OleDbConnection conn = new System.Data.OleDb.OleDbConnection(ConnectionString))
                    {
                        conn.Open();
                        using (DataTable dtExcelSchema = conn.GetSchema("Tables"))
                        {
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            string query = "SELECT * FROM [" + sheetName + "]";
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                            //DataSet ds = new DataSet();
                            adapter.Fill(ds, "Items");
                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    string cedula = "";
                                    string nombre = "";
                                    string actividadFormativa = "";
                                    string fecha = "";
                                    string asistio = "";
                                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                    {
                                        AssitenciaVM asistencia = new AssitenciaVM();
                                        cedula = ds.Tables[0].Rows[i][0].ToString();
                                        nombre = ds.Tables[0].Rows[i][1].ToString();
                                        actividadFormativa = ds.Tables[0].Rows[i][2].ToString();
                                        fecha = ds.Tables[0].Rows[i][3].ToString();
                                        asistio = ds.Tables[0].Rows[i][4].ToString();

                                        asistencia.participanteCedula = cedula;
                                        asistencia.participanteNombre = nombre;
                                        asistencia.asistio = true;

                                        asistencias.Add(asistencia);
                                    }
                                }
                            }
                        }
                    }

                }
 

            List<Inscripcion> inscripciones = new List<Inscripcion>();
            inscripciones = db.Inscripciones.AsNoTracking().Where(c => c.CicloFormativoId == cicloId).ToList();
            foreach (var inscripcion in inscripciones)
            {
                bool asistio = asistencias.Any(i => i.participanteCedula == inscripcion.Participante.Cedula);
                if (!asistio) //Personas que no asistieron
                {
                    Persona persona = db.Personas.AsNoTracking().Where(c => c.Cedula == inscripcion.Participante.Cedula).SingleOrDefault();
                    if (persona != null)
                    {
                        Ausencia ausencia = new Ausencia();
                        ausencia.Tipo = TipoDeAusencia.Justificada;
                        ausencia.PersonaId = persona.Id;
                        ausencia.CalendarioCicloFormativoId = calendarioId;
                        ausencia.Comentario = "";
                        bool isAusenciaRepeated = db.Ausencias.AsNoTracking().Where(p => p.PersonaId == ausencia.PersonaId).Any(c => c.CalendarioCicloFormativoId == calendarioId);
                        if (!isAusenciaRepeated)
                        {
                            db.Ausencias.Add(ausencia); //Registra la ausencia en la base de datos
                            db.SaveChanges();
                        }
                    }
                    else
                    { // No aparecio en el sistema (notificar por email listado de estas personas)                      

                    }
                }
                else
                { //lista de personas que asistieron
                    Persona persona = db.Personas.AsNoTracking().Where(c => c.Cedula == inscripcion.Participante.Cedula).SingleOrDefault();
                    List<Ausencia> ausenciaPersona = db.Ausencias.Where(p => p.PersonaId == persona.Id).Where(c => c.CalendarioCicloFormativoId == calendarioId).ToList();
                    if (ausenciaPersona.Count() > 0)
                    {
                        db.Ausencias.Remove(ausenciaPersona.SingleOrDefault());
                        db.SaveChanges();
                    }
                }

            }

            //Enviar por email
            string tituloEmail = "Registro Automatico de Asistencia con Excel";
            foreach (var a in asistencias)
            {
                bool isInSismo = db.Personas.Any(c => c.Cedula == a.participanteCedula);
                if (!isInSismo)
                {
                    contadorFaltantes++;
                    textoEmail.AppendLine("<h3>No existe esta persona en Sismo= " + a.participanteCedula + " , " + a.participanteNombre + "</h3>");
                }
            }
            textoEmail.AppendLine("<h3>Contador de Faltantes en sistema= " + contadorFaltantes + "</h3>");
            Logger.LogEvent(User.Identity.Name, tituloEmail, textoEmail.ToString(), "", DateTime.Now);
            //End Enviar por email

            return RedirectToAction("Details", "CicloFormativo", new { id = cicloId });
        }




        [Authorize(Roles = "Administrador,AdministradorTransversal")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ImportFromExcelFile(HttpPostedFileBase uploadFile)
        {
            int cicloId = 0;
            try
            {
                cicloId = Convert.ToInt32(Request["id"]);

                if (uploadFile.ContentLength > 0)
                {
                    string filePath = Path.Combine(HttpContext.Server.MapPath("../"), Path.GetFileName(uploadFile.FileName));
                    uploadFile.SaveAs(filePath);
                    DataSet ds = new DataSet();
                    string ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=Excel 12.0;";

                    using (OleDbConnection conn = new System.Data.OleDb.OleDbConnection(ConnectionString))
                    {
                        conn.Open();
                        using (DataTable dtExcelSchema = conn.GetSchema("Tables"))
                        {
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            string query = "SELECT * FROM [" + sheetName + "]";
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                            //DataSet ds = new DataSet();
                            adapter.Fill(ds, "Items");
                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                    {
                                        Inscripcion inscripcion = new Inscripcion();
                                        Persona persona = new Persona();
                                        GrupoCicloFormativo grupoCiclo = new GrupoCicloFormativo();
                                        string cedula = ds.Tables[0].Rows[i][0].ToString();
                                        string grupo = ds.Tables[0].Rows[i][1].ToString();
                                        string rol = ds.Tables[0].Rows[i][2].ToString();
                                        inscripcion.Rol = InscripcionRol.Participante; //default

                                        if (rol == "Formador")
                                        {
                                            inscripcion.Rol = InscripcionRol.Formador;
                                        }

                                        if (rol == "Acompanante")
                                        {
                                            inscripcion.Rol = InscripcionRol.Acompanante;
                                        }

                                        if (cedula != "" && grupo != "")
                                        {
                                            persona = db.Personas.Where(c => c.Cedula == cedula).SingleOrDefault();
                                            grupoCiclo = db.GruposCiclosFormativos.Where(g => g.Centro.Nombre == grupo).Where(c => c.CicloFormativoId == cicloId).SingleOrDefault();
                                            bool isRepeated = false;
                                            if (grupoCiclo != null && persona != null)
                                            {
                                                isRepeated = db.Inscripciones.Where(g => g.GrupoCicloFormativoId == grupoCiclo.ID).Any(p => p.Participante.Cedula == persona.Cedula);//db.Personas.Any(c => c.Cedula == cedula);
                                                if (isRepeated == false)
                                                {
                                                    inscripcion.ParticipanteId = persona.Id;
                                                    inscripcion.CicloFormativoId = cicloId;
                                                    inscripcion.GrupoCicloFormativoId = grupoCiclo.ID;
                                                    inscripcion.Fecha = DateTime.Now;
                                                    db.Inscripciones.Add(inscripcion);
                                                    db.SaveChanges();
                                                    //    db.Provincias.Add(provincia);
                                                    //    db.SaveChanges();
                                                }
                                            }


                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception exp)
            {
                var dummy = exp.Message;
            }
            return RedirectToAction("Details/" + cicloId);
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
