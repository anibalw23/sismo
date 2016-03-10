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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Monitoreo.Models.BO;
using System.Threading.Tasks;

namespace Monitoreo.Controllers
{
    [Authorize(Roles = "Administrador,Coordinador")]
    public class CoordinadorController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        public ActionResult CoordinadorRedes()
        {
            List<Red> redes = new List<Red>();
            List<CoordinadorRedes> redesCoordinador = new List<CoordinadorRedes>();
            try
            {
                Persona persona = db.Personas.Where(u => u.Cedula == User.Identity.Name).SingleOrDefault();
                if (persona != null)
                {
                    redesCoordinador = db.CoordinadoresRedes.Where(c => c.Coordinador.PersonaId == persona.Id).ToList();
                    foreach (var red in redesCoordinador)
                    {
                        redes.Add(red.Red);
                    }
                }
            }
            catch (Exception e)
            {
                var msj = e.Message;
            }
            return View(redes);
        }


        public ActionResult CoordinadorDocentes(int id)
        {
            List<Docente> docentes = new List<Docente>();
            Centro centro = new Centro();
            string cedula = User.Identity.Name;
            Coordinador coordinador = new Coordinador();

            coordinador = db.Coordinadors.Where(p => p.Persona.Cedula == cedula).SingleOrDefault();
            if (coordinador != null)
            {
                if (coordinador.nivel == NivelEducativo.Inicial)
                {
                    docentes = db.Docentes.Include(m => m.Materias).Include(d => d.Persona).Where(c => c.CentroId == id).Where(d => d.Materias.Any(m => m.Nivel == NivelEducativo.Inicial)).ToList();
                }
                else
                {
                    docentes = db.Docentes.Include(m => m.Materias).Include(d => d.Persona).Where(c => c.CentroId == id).ToList();
                }
                centro = db.Centros.Find(id);
            }

            ViewBag.Centro = centro;
            ViewBag.Coordinador = coordinador;
            return View(docentes);
        }


        // GET: Coordinadors
        [Route("Coordinador")]
        public ActionResult Index()
        {
            List<Coordinador> coordinadors = new List<Coordinador>();
            return View(coordinadors.ToList());
        }

        // POST: Coordinadors
        [Route("Coordinador/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var coordinadors = db.Coordinadors.Include(c => c.Persona).Include(c => c.CoordinadorRedes);
            var recordsTotal = coordinadors.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = coordinadors.Select(x => new { DT_RowId = x.Id, Email = x.Email, Persona = x.Persona.Cedula });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x =>
                        x.Email.Contains(searchValue)
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
                        case "Email":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.Email);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.Email);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.Email);
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

        // GET: /Coordinador/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coordinador coordinador = db.Coordinadors.Find(id);
            if (coordinador == null)
            {
                return HttpNotFound();
            }
            return View(coordinador);
        }

        // GET: Coordinadors/Create
        [Route("Coordinador/Create")]
        public ActionResult Create()
        {
            ViewBag.PersonaId = new SelectList(db.Personas.Select(x => new { x.Id, x.Cedula}), "Id", "Cedula");
            ViewBag.redId = new SelectList(db.Redes.Select(x => new {x.Id, x.Nombre }), "Id", "Nombre");
            return View();
        }

        // POST: Coordinadors/Create
        [Route("Coordinador/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Coordinador coordinador)
        {

            string createUser = Request.Form["CreateUser"];
            string redes = Request.Form["Redes"];
            String[] redesID = redes.Split(',');

            if (ModelState.IsValid)
            {
                Persona persona = await db.Personas.FindAsync(coordinador.PersonaId);
                bool repeated = await db.Coordinadors.Select(x => new { x.PersonaId}).AnyAsync(p => p.PersonaId == persona.Id);
                if (!repeated)
                {
                    //Crea el coordinador
                    db.Coordinadors.Add(coordinador);
                    await db.SaveChangesAsync();

                    //Asigna el coordinador a su Red o Redes
                    foreach (var red in redesID)
                    {
                        CoordinadorRedes coordinadorRed = new CoordinadorRedes();
                        coordinadorRed.RedID = Convert.ToInt32(red);
                        coordinadorRed.CoordinadorID = coordinador.Id;
                        bool isRepeatedCoordinadorRed = await db.CoordinadoresRedes.Select(x => new { x.CoordinadorID, x.RedID }).Where(r => r.RedID == coordinadorRed.RedID).AnyAsync(c => c.CoordinadorID == coordinadorRed.CoordinadorID);
                        if (!isRepeatedCoordinadorRed)
                        {
                            db.CoordinadoresRedes.Add(coordinadorRed);
                            await db.SaveChangesAsync();
                        }
                    }
                    //Crea el usuario
                    if (createUser == "1")
                    {
                        createCoordinadorUser(persona);
                    }
                }

                return RedirectToAction("Index");
            }

            ViewBag.PersonaId = new SelectList(db.Personas.Select(x => new {x.Id, x.Cedula }), "Id", "Cedula", coordinador.PersonaId);
            ViewBag.redId = new SelectList(db.Redes.Select(x => new { x.Id, x.Nombre}), "Id", "Nombre");
            return View(coordinador);
        }


        public void createCoordinadorUser(Persona persona)
        {
            var passwordHash = new PasswordHasher();
            string password = passwordHash.HashPassword(persona.Cedula + "123");
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(userStore);

            if (userManager.FindByName(persona.Cedula) == null && persona.Cedula != "")
            {
                var userToInsert = new ApplicationUser { UserName = persona.Cedula, PasswordHash = password, SecurityStamp = "Abcdefg" };
                userStore.AddToRoleAsync(userToInsert, "Coordinador");
                userStore.CreateAsync(userToInsert);
            }

        }
        // GET: /Coordinador/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coordinador coordinador = db.Coordinadors.Find(id);
            if (coordinador == null)
            {
                return HttpNotFound();
            }
            List<CoordinadorRedes> coordinadorRedes = new List<CoordinadorRedes>();
            var coordinadorRedesSelect = db.Redes.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nombre,
                Selected = db.CoordinadoresRedes.Where(p => p.CoordinadorID == id).Any(r => r.RedID == c.Id),
            }).ToList();


            ViewBag.CoordinadorRedes = coordinadorRedesSelect;
            ViewBag.Persona = new SelectList(db.Personas.Select(x => new {x.Id, x.Cedula }), "Id", "Cedula", coordinador.PersonaId);
            ViewBag.red = new SelectList(db.Redes.Select(x => new { x.Id, x.Nombre}), "Id", "Nombre");
            return View(coordinador);
        }

        // POST: /Coordinador/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task  <ActionResult> Edit(Coordinador coordinador)
        {

            string redes = Request.Form["Redes"];
            String[] redesID = redes.Split(',');
            if (ModelState.IsValid)
            {
                db.Entry(coordinador).State = EntityState.Modified;
                await db.SaveChangesAsync();

                List<CoordinadorRedes> coordinadorRedesTemp = await db.CoordinadoresRedes.Where(c => c.CoordinadorID == coordinador.Id).ToListAsync();
                foreach (var coo in coordinadorRedesTemp)
                {
                    bool isBorrable = true;
                    foreach (var red in redesID)
                    {
                        if (coo.RedID == Convert.ToInt32(red))
                        {
                            isBorrable = false;
                            break;
                        }
                    }
                    if (isBorrable == true)
                    {
                        db.CoordinadoresRedes.Remove(coo);
                        await db.SaveChangesAsync();
                    }

                }


                //############## Edita las redes a que pertenece ###############
                foreach (var red in redesID)
                {
                    CoordinadorRedes coordinadorRed = new CoordinadorRedes();
                    coordinadorRed.RedID = Convert.ToInt32(red);
                    coordinadorRed.CoordinadorID = coordinador.Id;

                    bool isRepeatedCoordinadorRed = await db.CoordinadoresRedes.Select(x => new {x.RedID, x.CoordinadorID }).Where(r => r.RedID == coordinadorRed.RedID).AnyAsync(c => c.CoordinadorID == coordinadorRed.CoordinadorID);
                    if (!isRepeatedCoordinadorRed)
                    {
                        db.CoordinadoresRedes.Add(coordinadorRed);
                        await db.SaveChangesAsync();
                    }

                }
                //###############################################################

                return RedirectToAction("Index");
            }
            List<CoordinadorRedes> coordinadorRedes = new List<CoordinadorRedes>();
            var coordinadorRedesSelect = db.Redes.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nombre,
                Selected = db.CoordinadoresRedes.Where(p => p.CoordinadorID == coordinador.Id).Any(r => r.RedID == c.Id),
            }).ToList();

            ViewBag.CoordinadorRedes = coordinadorRedesSelect;
            ViewBag.Persona = new SelectList(db.Personas, "Id", "Cedula", coordinador.PersonaId);
            ViewBag.red = new SelectList(db.Redes.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
            return View(coordinador);
        }

        // GET: /Coordinador/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coordinador coordinador = db.Coordinadors.Find(id);
            if (coordinador == null)
            {
                return HttpNotFound();
            }
            return View(coordinador);
        }

        // POST: /Coordinador/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coordinador coordinador = db.Coordinadors.Find(id);
            if (ModelState.IsValid)
            {
                db.Coordinadors.Remove(coordinador);
                db.SaveChanges();
            }
            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(coordinador);
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
