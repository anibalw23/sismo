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

namespace Monitoreo.Controllers
{
    [Authorize]
    public class ProvinciaController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Provincias
        [Route("Provincias")]
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Index()
        {
            //return View(db.Provincias.ToList());
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador, Acompanante")]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var recordsTotal = db.Provincias.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = db.Provincias.Select(x => new { DT_RowId = x.Id, Nombre = x.Nombre , Codigo = x.Codigo});

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];

                //".Contains(searchValue) || x."

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x =>
                        x.Nombre.Contains(searchValue.Trim())
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
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult GetMunicipios(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Provincia provincia = db.Provincias.Find(id);
            if (provincia == null)
            {
                return HttpNotFound();
            }

            var result = (from s in provincia.Municipios
                          select new
                          {
                              id = s.Id,
                              name = s.Nombre
                          }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        } 

        // GET: /Provincia/5/Details
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Provincia provincia = db.Provincias.Find(id);
            if (provincia == null)
            {
                return HttpNotFound();
            }
            return View(provincia);
        }

        // GET: Provincias/Create
        [Route("Provincias/Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Provincias/Create
        [Route("Provincias/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Codigo,Nombre")] Provincia provincia)
        {
            if (ModelState.IsValid)
            {
                db.Provincias.Add(provincia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(provincia);
        }

        // GET: /Provincia/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Provincia provincia = db.Provincias.Find(id);
            if (provincia == null)
            {
                return HttpNotFound();
            }
            return View(provincia);
        }

        // POST: /Provincia/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Codigo,Nombre")] Provincia provincia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(provincia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(provincia);
        }

        // GET: /Provincia/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Provincia provincia = db.Provincias.Find(id);
            if (provincia == null)
            {
                return HttpNotFound();
            }
            return View(provincia);
        }

        // POST: /Provincia/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Provincia provincia = db.Provincias.Find(id);

            // Seteando en null lo relacionada
            var personas = db.Personas.Where(x => x.ProvinciaId == id);
            foreach (var item in personas)
            {
                item.ProvinciaId = null;
                db.Entry(item).State = EntityState.Modified;
            }

            db.Provincias.Remove(provincia);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ImportFromExcel() {
            return View();
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ImportFromExcelFile(HttpPostedFileBase uploadFile)
        {
            try{
                if (uploadFile.ContentLength > 0) {
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
                                        Provincia provincia = new Provincia();
                                        provincia.Codigo = ds.Tables[0].Rows[i][0].ToString();
                                        provincia.Nombre = ds.Tables[0].Rows[i][1].ToString();

                                        bool isRepeated = db.Provincias.Any(c => c.Codigo == provincia.Codigo);

                                        if (isRepeated == false && provincia.Codigo != "" && provincia.Codigo != null && provincia.Nombre !="")
                                        {
                                            db.Provincias.Add(provincia);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }

                }            
            }
            catch(Exception exp){
                var dummy = exp.Message;
            }
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
