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
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Monitoreo.Models.BO.ViewModels;
using Monitoreo.Models.BO.ViewModels.SuperCicloFormativoVm;

namespace Monitoreo.Controllers
{
    //[Authorize(Roles = "Administrador")]
    [Authorize]
    public class PersonalAdministrativoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: PersonalAdministrativo
        [Route("PersonalAdministrativo")]
        [Authorize(Roles = "Administrador")]
        public ActionResult Index()
        {
            //var personals = db.PersonalAdministrativo.Include(p => p.Centro).Include(p => p.Persona);
            return View();
        }


        // POST: Redes
        [Route("Redes/GetDataJson")]
        [HttpPost]
        [Authorize(Roles = "Administrador, Acompanante,Coordinador")]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var personals = db.PersonalAdministrativo.Select(x => new { DT_RowId = x.Id, Centro = x.Centro.Nombre, Cedula = x.Persona.Cedula, NombrePersona = x.Persona.Nombres });
            var recordsTotal = personals.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = personals;

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x =>
                        x.NombrePersona.Contains(searchValue) || x.Cedula.Contains(searchValue)
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
                data = data.OrderBy(n => n.NombrePersona).Skip(from).Take(limit).ToList()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> PersonalAdministrativoDetailsAcompanante(int id)
        {
            DocenteDetailsAcompananteVM docenteDetails = new DocenteDetailsAcompananteVM();
            string cedula = User.Identity.Name;
            List<Inscripcion> inscripciones = new List<Inscripcion>();
            List<CicloFormativo> ciclosDocente = new List<CicloFormativo>();
            Acompanante acompanante = new Acompanante();

            List<AsistenciaAcompanamientoVM> asistenciasActividadesAcompanamiento = new List<AsistenciaAcompanamientoVM>();
            docenteDetails.asistenciasAcompanamientos = asistenciasActividadesAcompanamiento;


            PersonalAdministrativo personal = await db.PersonalAdministrativo.FindAsync(id);  //Cambie esto ahora (probar *)            
            if (personal != null)
            {
                docenteDetails.cedula = personal.Persona.Cedula;
                docenteDetails.nombreDocente = personal.Persona.NombreCompleto;
                docenteDetails.sexo = personal.Persona.Sexo.ToString();
                docenteDetails.tanda = personal.Tanda.ToString();
                docenteDetails.telefono = personal.Persona.Telefono.Celular;
                docenteDetails.edad = personal.Persona.Edad;

            }
            else
            {
                ModelState.AddModelError("Error2345", "no se encontró el usuario");
            }

            var ciclosFormativos = await db.SuperCicloFormativoes.AsNoTracking().Select(x => new { x.Id, x.nombre, x.CategoriaSuperCiclo, x.FechaFinalizacion, x.FechaInicio, x.CiclosFormativos }).Where(s => s.CategoriaSuperCiclo == CategoriaSuperCiclo.Gestion).Where(f => f.FechaInicio < DateTime.Now).Where(f => f.FechaFinalizacion > DateTime.Now).OrderBy(f => f.FechaInicio.Year).ThenBy(y => y.FechaInicio.Month).ThenBy(y => y.FechaInicio.Day).ToListAsync();
            List<SuperCicloFormativoVM> ciclosFormativosVm = new List<SuperCicloFormativoVM>();

            /*Esto lo agrege para poner solo los ciclos formativos en que esta inscrito el docente*/
            List<int> superIds = new List<int>();
            var inscripcionesParticipante = personal != null ? db.Inscripciones.AsNoTracking().Select(x => new { x.CicloFormativo.SuperCicloFormativoId, x.ParticipanteId }).Where(p => p.ParticipanteId == personal.PersonaId).ToList() : null;

            if (inscripcionesParticipante != null)
            {
                var superCiclosIds = inscripcionesParticipante.Select(x => new { x.SuperCicloFormativoId }).Distinct().ToList();
                foreach (var super in superCiclosIds)
                {
                    superIds.Add(super.SuperCicloFormativoId);
                }
            }
            /*End*/

            foreach (var ciclo in ciclosFormativos.Where(s => superIds.Contains(s.Id)))
            {
                ciclosFormativosVm.Add(new SuperCicloFormativoVM { Id = ciclo.Id, nombre = ciclo.nombre });
            }
            docenteDetails.ciclosFormativos.AddRange(ciclosFormativosVm);

            return View(docenteDetails);
        }



        [Route("CentroPersonal")]
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult CentroPersonal(int CentroId)
        {
            var personals = db.PersonalAdministrativo.Include(p => p.Centro).Include(p => p.Persona).Where(m => m.CentroId == CentroId);
            return PartialView(personals.ToList());
        }

        // GET: /PersonalAdministrativo/5/Details
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonalAdministrativo personalAdministrativo = db.PersonalAdministrativo.Find(id);
            if (personalAdministrativo == null)
            {
                return HttpNotFound();
            }
            return View(personalAdministrativo);
        }

        // GET: PersonalAdministrativo/Create
        [Route("PersonalAdministrativo/Create")]
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Nombre");
            ViewBag.PersonaId = new SelectList(db.Personas, "Id", "NombreCompleto");

            if (Request.Params["Modal"] != null)
            {
                return PartialView();
            }

            return View();
        }

        // POST: PersonalAdministrativo/Create
        [Route("PersonalAdministrativo/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Create([Bind(Include = "Id,Codigo,PersonaId,CentroId,FechaContratacion,FuncionesEjerce,Tanda")] PersonalAdministrativo personalAdministrativo)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];

            if (ModelState.IsValid)
            {
                db.PersonalAdministrativo.Add(personalAdministrativo);
                db.SaveChanges();

                if (ViewBag.MasterType != null)
                {
                    return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
                }

                return RedirectToAction("Index");
            }

            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Nombre", personalAdministrativo.CentroId);
            ViewBag.PersonaId = new SelectList(db.Personas, "Id", "NombreCompleto", personalAdministrativo.PersonaId);
            return View(personalAdministrativo);
        }


        public ActionResult CreateModal()
        {
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Nombre");
            ViewBag.PersonaId = new SelectList(db.Personas, "Id", "NombreCompleto");
            ViewBag.Sexo = Enum.GetValues(typeof(PersonaSexo)).Cast<PersonaSexo>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateModal(PersonalAdministrativo personalAdmin)
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
                    Persona persona = new Persona();
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
                        personalAdmin.PersonaId = persona.Id;
                    }
                    else
                    {
                        persona = await db.Personas.AsNoTracking().Where(c => c.Cedula == cedula).SingleOrDefaultAsync();
                        personalAdmin.PersonaId = persona.Id;
                    }
                }
                isRepeated = await db.PersonalAdministrativo.AsNoTracking().Where(d => d.PersonaId == personalAdmin.PersonaId).AnyAsync(c => c.CentroId == personalAdmin.CentroId);

                if (!isRepeated)  //Verifica que el personal administrativo no esta repetido en el mismo centro
                {
                    db.PersonalAdministrativo.Add(personalAdmin);
                    await db.SaveChangesAsync();
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

            ViewBag.CentroId = new SelectList(db.Centros.OrderBy(n => n.Nombre), "Id", "Nombre", personalAdmin.CentroId);
            ViewBag.Sexo = Enum.GetValues(typeof(PersonaSexo)).Cast<PersonaSexo>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            var jsonData = new
            {
                result = result,
                url = new UrlHelper(Request.RequestContext).Action("Index", "PersonalAdministrativo")
            };
            return Json(jsonData);
        }




        // GET: /PersonalAdministrativo/5/Edit
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonalAdministrativo personalAdministrativo = db.PersonalAdministrativo.Find(id);
            if (personalAdministrativo == null)
            {
                return HttpNotFound();
            }

            if (Request.Params["Modal"] != null)
            {
                return PartialView(personalAdministrativo);
            }

            return View(personalAdministrativo);
        }

        // POST: /PersonalAdministrativo/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(PersonalAdministrativo personalAdministrativo)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];

            if (ModelState.IsValid)
            {
                db.Entry(personalAdministrativo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Centro", new { id = personalAdministrativo.CentroId });

            }
            return View(personalAdministrativo);
        }

        // GET: /PersonalAdministrativo/5/Delete
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonalAdministrativo personalAdministrativo = db.PersonalAdministrativo.Find(id);
            if (personalAdministrativo == null)
            {
                return HttpNotFound();
            }
            return View(personalAdministrativo);
        }

        // POST: /PersonalAdministrativo/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult DeleteConfirmed(int id)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {

                PersonalAdministrativo personalAdministrativo = db.PersonalAdministrativo.Find(id);
                db.PersonalAdministrativo.Remove(personalAdministrativo);
                db.SaveChanges();

            }
            return RedirectToAction("Details", "Centro", new { id = ViewBag.MasterId });


        }


        [HttpPost]
        [Authorize(Roles = "Administrador, Acompanante,Coordinador, AdministradorTransversal")]
        public JsonResult GetPersonalAdminFunciones()
        {
            Array values = Enum.GetValues(typeof(PersonalFuncion));
            List<ListItem> items = new List<ListItem>(values.Length);

            foreach (var i in values)
            {
                items.Add(new ListItem
                {
                    Text = Enum.GetName(typeof(PersonalFuncion), i),
                    Value = ((int)i).ToString()
                });
            }


            var jsonData = new
            {
                data = items.Select(y => new
                {
                    y.Text,
                    y.Value

                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
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
