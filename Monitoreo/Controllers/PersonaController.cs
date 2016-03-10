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
    public class Select2PagedResult
    {
        public int Total { get; set; }
        public List<Select2Result> Results { get; set; }
    }

    public class Select2Result
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    //[Authorize(Roles = "Administrador")]
    [Authorize]
    public class PersonaController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Personas
        [Route("Personas")]
        public ActionResult Index()
        {
            //var personas = db.Personas.Include(p => p.GrupoEtnico).Include(p => p.Municipio);
            //return View(personas.ToList());
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Administrador, Acompanante")]
        public JsonResult GetPersona(int id, string type)
        {
            IQueryable<Persona> personas;

            switch (type)
            {
                case "Estudiante":
                    personas = db.Estudiantes.Include(p => p.Persona).Select(a => a.Persona);
                    break;
                case "Participante":
                    personas = db.Personal.Include(p => p.Persona).Select(a => a.Persona);
                    break;
                default:
                    personas = db.Personas;
                    break;
            }

            var persona = db.Personas.Find(id);

            if (persona != null)
            {
                return new JsonResult
                {
                    Data = new Select2Result { id = persona.Id.ToString(), text = String.Format("{0}, {1}", persona.Cedula, persona.NombreCompleto) },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                return new JsonResult
                {
                    Data = new Select2Result { id = "-1", text = "" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        //Select2
        [HttpGet]
        [Authorize(Roles = "Administrador, Acompanante")]
        public JsonResult GetPersonas(string searchTerm, int pageSize, int pageNum, string type)
        {
            //Get the paged results and the total count of the results for this query.
            searchTerm = searchTerm.ToLower();

            IQueryable<Persona> personas;

            switch (type)
            {
                case "Estudiante":
                    personas = db.Estudiantes.Select(a => a.Persona);
                    break;
                case "Participante":
                    personas = db.Personas;
                    break;
                default:
                    personas = db.Personas;
                    break;
            }

            personas = personas.Where(a => a.Nombres.Contains(searchTerm) || a.PrimerApellido.Contains(searchTerm) || a.SegundoApellido.Contains(searchTerm) || a.Cedula.Contains(searchTerm));

            int personasCount = personas.Count();

            List<Persona> personasList = personas.OrderBy(x => x.Cedula).Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList();

            //Translate the attendees into a format the select2 dropdown expects
            Select2PagedResult pagedPersonas = PersonasToSelect2Format(personasList, personasCount);

            //Return the data as a jsonp result
            return new JsonResult
            {
                Data = pagedPersonas,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        [Authorize(Roles = "Administrador, Acompanante")]
        private Select2PagedResult PersonasToSelect2Format(List<Persona> Personas, int totalPersonas)
        {
            Select2PagedResult jsonPersonas = new Select2PagedResult();
            jsonPersonas.Results = new List<Select2Result>();

            //Loop through our attendees and translate it into a text value and an id for the select list
            foreach (Persona a in Personas)
            {
                jsonPersonas.Results.Add(new Select2Result { id = a.Id.ToString(), text = String.Format("{0}, {1}", a.Cedula, a.NombreCompleto) });
            }
            //Set the total count of the results from the query.
            jsonPersonas.Total = totalPersonas;

            return jsonPersonas;
        }

        // Datatables
        [HttpPost]
        [Authorize(Roles = "Administrador, Acompanante")]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var recordsTotal = db.Personas.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = db.Personas.Select(x => new { DT_RowId = x.Id, Nombre = x.Nombres, Cedula = x.Cedula, FechaNacimiento = x.FechaNacimiento, LugarNacimiento = x.LugarNacimiento, Sexo = x.Sexo.ToString(), TelefonoFijo = x.Telefono.Fijo, TelefonoCelular = x.Telefono.Celular });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];

                //".Contains(searchValue) || x."

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x =>
                        x.Nombre.Contains(searchValue.Trim()) || x.Cedula.Contains(searchValue.Trim())
                        );

                    recordsFiltered = data.Count();
                }
            }

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
                        case "Nombre":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.Nombre);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.Nombre);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.Nombre);
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
		
        // GET: /Persona/5/Details
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = db.Personas.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // GET: Personas/Create
        [Route("Personas/Create")]
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.GrupoEtnicoId = new SelectList(db.GruposEtnicos, "Id", "Nombre");
            ViewBag.ProvinciaId = new SelectList(db.Provincias, "Id", "Nombre");
            ViewBag.MunicipioId = new SelectList(new List<Municipio>(), "Id", "Nombre");
            return View();
        }

        // POST: Personas/Create
        [Route("Personas/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Create(Persona persona)
        {
            if (ModelState.IsValid)
            {

                bool isRepeated = db.Personas.Any(i => i.Cedula == persona.Cedula);
                if (!isRepeated) {
                    db.Personas.Add(persona);
                    db.SaveChanges();
                    ModelState.AddModelError("1","Esta persona ya está registrada!!");
                }
               
                return RedirectToAction("Index");
            }

            ViewBag.GrupoEtnicoId = new SelectList(db.GruposEtnicos, "Id", "Nombre", persona.GrupoEtnicoId);
            ViewBag.ProvinciaId = new SelectList(db.Provincias, "Id", "Nombre", persona.ProvinciaId);
            ViewBag.MunicipioId = new SelectList(db.Municipios.Where(x => x.ProvinciaId == persona.ProvinciaId), "Id", "Nombre", persona.MunicipioId);
            return View(persona);
        }

        // GET: /Persona/5/Edit
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = db.Personas.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            ViewBag.GrupoEtnicoId = new SelectList(db.GruposEtnicos, "Id", "Nombre", persona.GrupoEtnicoId);
            ViewBag.ProvinciaId = new SelectList(db.Provincias, "Id", "Nombre", persona.ProvinciaId);
            ViewBag.MunicipioId = new SelectList(db.Municipios.Where(x => x.ProvinciaId == persona.ProvinciaId), "Id", "Nombre", persona.MunicipioId);
            return View(persona);
        }

        // POST: /Persona/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(Persona persona)
        {
            if (ModelState.IsValid)
            {
                db.Entry(persona).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GrupoEtnicoId = new SelectList(db.GruposEtnicos, "Id", "Nombre", persona.GrupoEtnicoId);
            ViewBag.ProvinciaId = new SelectList(db.Provincias, "Id", "Nombre", persona.ProvinciaId);
            ViewBag.MunicipioId = new SelectList(db.Municipios.Where(x => x.ProvinciaId == persona.ProvinciaId), "Id", "Nombre", persona.MunicipioId);
            return View(persona);
        }

        // GET: /Persona/5/Delete
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = db.Personas.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // POST: /Persona/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult DeleteConfirmed(int id)
        {
            Persona persona = db.Personas.Find(id);
            db.Personas.Remove(persona);
            db.SaveChanges();
            return RedirectToAction("Index");
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
