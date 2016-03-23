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
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.UI;
using System.Text.RegularExpressions;
using Monitoreo.Models.BO.ViewModels.Acompanante;

namespace Monitoreo.Controllers
{
    [Authorize]
    public class AcompananteController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();
        IdentityDbContext dbcontexIde = new IdentityDbContext();



        // GET: Acompanantes
        //Pagina de inicio del acompañante
        public ActionResult AcompananteDashboard()
        {
            return View();
        }

        // GET: Acompanantes
        //Lista de Docentes del acompañante
        //[OutputCache(Duration = 1800, VaryByCustom = "User", VaryByParam = "*", Location = OutputCacheLocation.Server)] // Se almacena en cahce por 30 minutos
        public async Task<ActionResult> AcompananteDocentes()
        {
            string cedula = User.Identity.Name;
            var acompanante = db.Acompanantes.Select(x => new { cedula = x.Persona.Cedula, x.centroId }).Where(p => p.cedula == cedula).SingleOrDefault();
            List<AcompananteDocentesVM> acompananteDocentes = new List<AcompananteDocentesVM>();

            if (acompanante == null) {
                ModelState.AddModelError("Error 45454", "Error de usuario, favor contactar a su administrador!");
                return View();
            }
            var docentesAcompanante = await db.Docentes.Select(x => new { x.Id, x.Persona.Cedula, x.Persona.Nombres, x.Persona.PrimerApellido, x.Persona.SegundoApellido, x.CentroId, x.Materias }).Where(c => c.CentroId == acompanante.centroId).ToListAsync();
            var nombreCentro = db.Centros.Select(x => new { x.Id, x.Nombre }).Where(i => i.Id == acompanante.centroId).FirstOrDefault();
            foreach (var docente in docentesAcompanante)
            {

                var inscripcionesCicloActual = db.InscripcionesActividadesAcompanamiento.AsNoTracking()
                                                     .Include(a => a.ActividadAcompanamiento)
                                                    .Where(s => s.ActividadAcompanamiento.SuperCicloFormativo.CategoriaSuperCiclo == CategoriaSuperCiclo.Docentes)
                                                    .Where(f => f.ActividadAcompanamiento.SuperCicloFormativo.FechaInicio < DateTime.Now)
                                                    .Where(f => f.ActividadAcompanamiento.SuperCicloFormativo.FechaFinalizacion > DateTime.Now)
                                                    .OrderBy(f => f.ActividadAcompanamiento.SuperCicloFormativo.FechaInicio.Year)
                                                    .ThenBy(y => y.ActividadAcompanamiento.SuperCicloFormativo.FechaInicio.Month)
                                                    .ThenBy(y => y.ActividadAcompanamiento.SuperCicloFormativo.FechaInicio.Day);
                int horasAcompanadasCicloActual = 0;
                if(inscripcionesCicloActual != null){
                    var inscripcionesCount = inscripcionesCicloActual.Count();
                    if(inscripcionesCount > 0){
                        horasAcompanadasCicloActual = inscripcionesCicloActual.Sum(h => h.horas);
                    }
                }

                AcompananteDocenteHorasAcompanadaVMs horasTotalesAcompanadas = new AcompananteDocenteHorasAcompanadaVMs();
                horasTotalesAcompanadas.horasAcompanamientoTotal =   horasAcompanadasCicloActual;

                acompananteDocentes.Add(new AcompananteDocentesVM { cedula = docente.Cedula, Id = docente.Id, Materias = docente.Materias, nombre = docente.Nombres + " " + docente.PrimerApellido + " " + docente.SegundoApellido, horasAcompanadas = horasTotalesAcompanadas });
            }
            ViewBag.Centro = nombreCentro.Nombre;
            return View(acompananteDocentes);
        }



        //Lista de Personal Administrativo del acompañante
        //[OutputCache(Duration = 1800, VaryByCustom = "User", VaryByParam = "*", Location = OutputCacheLocation.Server)] // Se almacena en cahce por 30 minutos
        public async Task<ActionResult> AcompanantePersonalAdministrativo()
        {
            string cedula = User.Identity.Name;
            List<PersonalAdministrativo> personalAdmin = new List<PersonalAdministrativo>();
            Centro centro = new Centro();
            var acompanante = await db.Acompanantes.Select(x => new { x.Persona, x.centroId }).Where(p => p.Persona.Cedula == cedula).SingleOrDefaultAsync();
            if (acompanante != null)
            {
                personalAdmin = await db.PersonalAdministrativo.Include(d => d.Persona).Where(c => c.CentroId == acompanante.centroId).ToListAsync();
                centro = await db.Centros.FindAsync(acompanante.centroId);
            }
            ViewBag.Centro = centro;
            return View(personalAdmin);
        }


        // GET: Acompanantes
        [Authorize(Roles = "Administrador")]
        //[OutputCache(Duration = 43200, VaryByParam = "none")]
        public ActionResult Index()
        {
            return View();
        }

        // POST: Acompanantes
        [Route("Acompanante/GetDataJson")]
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<JsonResult> GetDataJson(DatatablesParams values)
        {

            int acompanantesCount = await db.Acompanantes.Select(x => new { x.Id }).CountAsync();
            
            var recordsTotal = acompanantesCount;
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;
            var acompanantes =  db.Acompanantes.Select(x => new { x.Id, cedula = x.Persona.Cedula, nombres = x.Persona.Nombres, apellido = x.Persona.PrimerApellido, x.Email, centro = x.Centro.Nombre }).OrderBy(n => n.nombres).Skip(from).Take(limit);

            // Seleccionando
            var data = acompanantes.Select(x => new { DT_RowId = x.Id, Persona = x.cedula, NombrePersona = x.nombres + " " + x.apellido, Email = x.Email, Centro = x.centro });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x =>
                        x.Persona.Contains(searchValue) || x.NombrePersona.Contains(searchValue)
                    );

                    recordsFiltered = data.Count();
                }
            }
            

            // Preparando respuesta y ejecutando consulta
            var jsonData = new
            {
                draw = values.raw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data.ToList()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // GET: /Acompanante/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acompanante acompanante = db.Acompanantes.Find(id);
            if (acompanante == null)
            {
                return HttpNotFound();
            }
            return View(acompanante);
        }

        // GET: Acompanantes/Create
        [Route("Acompanante/Create")]
        [Authorize(Roles = "Administrador")]
        //[OutputCache(Duration = 43200, VaryByParam = "none")]
        public ActionResult Create()
        {
            ViewBag.centroId = new SelectList( db.Centros.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
            ViewBag.PersonaId = new SelectList( db.Personas.Select(x => new { x.Id, x.Cedula }), "Id", "Cedula");
            return View();
        }

        // POST: Acompanantes/Create
        [Route("Acompanante/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Email,PersonaId,centroId")] Acompanante acompanante)
        {

            string createUser = Request.Form["CreateUser"];

            if (ModelState.IsValid)
            {
                Persona persona = await db.Personas.FindAsync(acompanante.PersonaId);
                if (createUser == "1")
                { // crear un usuario con contraseña por defecto
                    var passwordHash = new PasswordHasher();
                    string password = passwordHash.HashPassword(persona.Cedula + "123");
                    var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
                    var userManager = new UserManager<ApplicationUser>(userStore);

                    if (userManager.FindByName(persona.Cedula) == null && persona.Cedula != "")
                    {
                        var userToInsert = new ApplicationUser { UserName = persona.Cedula, PasswordHash = password, SecurityStamp = "Abcdefg" };
                        await userStore.AddToRoleAsync(userToInsert, "Acompanante");
                        await userStore.CreateAsync(userToInsert);
                    }
                }

                // Si el acompanante no esta repertido
                bool repeated = await db.Acompanantes.Select(x => new { x.PersonaId}).AnyAsync(p => p.PersonaId == persona.Id);
                if (!repeated)
                {
                    db.Acompanantes.Add(acompanante);
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }

            ViewBag.centroId = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre}), "Id", "Nombre", acompanante.centroId);
            ViewBag.PersonaId = new SelectList(db.Personas.Select(x => new {x.Id, x.Cedula }), "Id", "Cedula", acompanante.PersonaId);
            return View(acompanante);
        }

        public ActionResult CreateModal()
        {
            ViewBag.CentroId = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
            ViewBag.PersonaId = new SelectList(db.Personas.Select(x => new { x.Id, x.Cedula }), "Id", "Cedula");
            ViewBag.Sexo = Enum.GetValues(typeof(PersonaSexo)).Cast<PersonaSexo>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            return PartialView();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateModal(Acompanante acompanante)
        {
            string result = "OK";
            //Crea una nueva persona si no existe
            string cedula = "";
            string nombre = "";
            string apellido = "";
            string sexo = "";
            string fechaNacimiento = "";
            bool ok = true;
            cedula = Request.Form["cedula"];
            nombre = Request.Form["nombres"];
            apellido = Request.Form["apellido"];
            sexo = Request.Form["sexo"];
            fechaNacimiento = Request.Form["fechaNacimiento"];
            string createUser = Request.Form["CreateUser"];
            Persona persona = new Persona();
            if (cedula != null && cedula != "")
            {
                ModelState.Remove("PersonaId");
                //Verifica la cedula
                Regex regex = new Regex("^\\d{3}-\\d{7}-\\d{1}$$");
                Match match = regex.Match(cedula);
                if (!match.Success)
                {
                    result = "ERROR_CEDULA";
                    ok = false;
                    var data = new { result };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            if (ModelState.IsValid && ok == true)
            {
                bool isRepeated = false;
                if (cedula != null && cedula != "")
                {
                    
                    persona.Cedula = cedula;
                    persona.Nombres = (nombre == null ? " " : nombre);
                    persona.Sexo = (sexo == null ? PersonaSexo.Femenino : (PersonaSexo)Enum.Parse(typeof(PersonaSexo), sexo));
                    persona.FechaNacimiento = (fechaNacimiento == null ? DateTime.Now : Convert.ToDateTime(fechaNacimiento));
                    persona.PrimerApellido = (apellido == null ? "-" : apellido); ;
                    persona.SegundoApellido = " ";
                    bool isRepeatedPersona = await db.Personas.AsNoTracking().Select(x => new { x.Cedula }).AnyAsync(c => c.Cedula == cedula);
                    if (!isRepeatedPersona)
                    {
                        db.Personas.Add(persona);
                        await db.SaveChangesAsync();                        
                        acompanante.PersonaId = persona.Id;
                    }
                    else
                    {
                        persona = await db.Personas.AsNoTracking().Where(c => c.Cedula == cedula).SingleOrDefaultAsync();
                        acompanante.PersonaId = persona.Id;
                    }
                }

                isRepeated = await db.Acompanantes.AsNoTracking().Where(d => d.PersonaId == acompanante.PersonaId).AnyAsync(c => c.centroId == acompanante.centroId);
                if (!isRepeated)  //Verifica que el docente no esta repetido en el mismo centro
                {
                    db.Acompanantes.Add(acompanante);
                    await db.SaveChangesAsync();

                    // ******* Para crear el usuario nuevo del acompañante ********
                    if (createUser == "1")
                    { // crear un usuario con contraseña por defecto
                        var passwordHash = new PasswordHasher();
                        string password = passwordHash.HashPassword(persona.Cedula + "123");
                        var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
                        var userManager = new UserManager<ApplicationUser>(userStore);

                        if (userManager.FindByName(persona.Cedula) == null && persona.Cedula != "")
                        {
                            var userToInsert = new ApplicationUser { UserName = persona.Cedula, PasswordHash = password, SecurityStamp = "Abcdefg" };
                            await  userStore.AddToRoleAsync(userToInsert, "Acompanante");
                            await  userStore.CreateAsync(userToInsert);
                        }
                    }
                    //*********************************************

                }
                else
                {
                    result = "ERROR_DOCENTE_REPETIDO";
                }
            }
            else
            {
                result = "ERROR";
            }

            ViewBag.CentroId = new SelectList(db.Centros.OrderBy(n => n.Nombre), "Id", "Nombre", acompanante.centroId);
            ViewBag.Sexo = Enum.GetValues(typeof(PersonaSexo)).Cast<PersonaSexo>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            var jsonData = new
            {
                result = result,
            };
            return Json(jsonData);
        }








        // GET: /Acompanante/5/Edit
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acompanante acompanante = db.Acompanantes.Find(id);
            if (acompanante == null)
            {
                return HttpNotFound();
            }
            ViewBag.centro = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre}), "Id", "Nombre", acompanante.centroId);
            ViewBag.Persona = new SelectList(db.Personas.Select(x => new { x.Id, x.Cedula}), "Id", "Cedula", acompanante.PersonaId);
            return View(acompanante);
        }

        // POST: /Acompanante/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit([Bind(Include = "Id,Email,PersonaId,centroId")] Acompanante acompanante)
        {
            if (ModelState.IsValid)
            {
                db.Entry(acompanante).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.centro = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre", acompanante.centroId);
            ViewBag.Persona = new SelectList(db.Personas.Select(x => new { x.Id, x.Cedula }), "Id", "Cedula", acompanante.PersonaId);
            return View(acompanante);
        }

        // GET: /Acompanante/5/Delete
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acompanante acompanante = db.Acompanantes.Find(id);
            if (acompanante == null)
            {
                return HttpNotFound();
            }
            return View(acompanante);
        }

        // POST: /Acompanante/5/Delete
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Acompanante acompanante = db.Acompanantes.Find(id);
            string deleteUser = Request.Form["CreateUser"];
            if (ModelState.IsValid)
            {

                if (deleteUser == "1")
                {
                    var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
                    var userManager = new UserManager<ApplicationUser>(userStore);
                    var userToDelete = userManager.FindByName(acompanante.Persona.Cedula);
                    if (userToDelete != null)
                    {
                         userStore.DeleteAsync(userToDelete);
                    }
                }

                db.Acompanantes.Remove(acompanante);
                db.SaveChanges();


            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(acompanante);
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
