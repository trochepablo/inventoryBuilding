using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Inventory.Models.Context;
using Inventory.Models.Entity;
using Inventory.Services;
using OfficeOpenXml;

namespace Inventory.Controllers
{
    public class EdificiosController : Controller
    {
        private InventoryContext db = new InventoryContext();

        // GET: Edificios
        public ActionResult Index()
        {
            return View(db.Edificios.ToList());
        }

        // GET: Edificios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Edificio edificio = db.Edificios.Find(id);
            if (edificio == null)
            {
                return HttpNotFound();
            }
            return View(edificio);
        }

        public ActionResult Associate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Edificio edificio = db.Edificios.Find(id);

            ViewBag.MaterialesOptions = db.Materiales.ToList()
             .Select(x => new SelectListItem { Text = string.Format("{0} - {1}", x.Codigo, x.Nombre), Value = x.Id.ToString() })
             .ToList();

            if (edificio == null)
            {
                return HttpNotFound();
            }

            return View(edificio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Associate(Edificio edificio, string submitButton, int? materialId)
        {
            GetAllOptionsMaterial();


            switch (submitButton)
            {
                case "Agregar Material":
                    handlerAsocciateMaterial(edificio, false);
                    break;
                case "Guardar":
                    handlerUpdateAsocciate(edificio);
                    return RedirectToAction("Index");
                case "Agregar todo":
                    handlerAsocciateMaterial(edificio, true);
                    break;
                case null:
                    var materialToDelete = edificio.EdificiosMateriales.Find(c => c.MaterialId == materialId);
                    if (materialToDelete != null)
                    {
                        db.Entry(materialToDelete).State = EntityState.Deleted;
                        db.SaveChanges();
                    }
                    break;
                default:
                    break;
            }


            return View(db.Edificios.Find(edificio.Id));
        }

        private void handlerUpdateAsocciate(Edificio edificio)
        {
            foreach (var item in edificio.EdificiosMateriales)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        private void handlerAsocciateMaterial(Edificio edificio, bool addAll)
        {
            if (addAll)
            {
                foreach (var item in db.Materiales.ToList())
                {
                    if (!existMaterialEnEdificio(item.Id, edificio.Id))
                    {
                        new EdificiosMaterialesService().Insert(edificio.Id, item.Id);
                    }
                }
            }
            else
            {
                int idMaterial = Convert.ToInt32(Request.Form["MaterialesOptions"]);
                if (!existMaterialEnEdificio(idMaterial, edificio.Id))
                {
                    new EdificiosMaterialesService().Insert(edificio.Id, idMaterial);
                }
                else
                {
                    ModelState.AddModelError("Error", "Ya existe material.");
                }
            }
        }

        private void GetAllOptionsMaterial()
        {
            ViewBag.MaterialesOptions = db.Materiales.ToList()
             .Select(x => new SelectListItem { Text = string.Format("{0} - {1}", x.Codigo, x.Nombre), Value = x.Id.ToString() })
             .ToList();
        }

        private void GetAllOptionsTecnicos()
        {
            ViewBag.TecnicosOptions = db.Personas.ToList()
             .Select(x => new SelectListItem { Text = string.Format("{0} - {1}", x.Nombre, x.Cargo), Value = x.Id.ToString() })
             .ToList();
        }

        private bool existMaterialEnEdificio(int idMaterial, int ediId)
        {
            return db.EdificiosMateriales.Where(c => c.EdificioId == ediId).Any(c => c.MaterialId == idMaterial);
        }

        [HttpGet]
        public FileContentResult ExportIndex()
        {
            var req = Request.QueryString;

            var list = new List<Edificio>();
            string param1 = "Todos";

            if (req.Count > 0 && !string.IsNullOrEmpty(param1))
            {
                param1 = req["estado-desc-starts-with"].ToLower();

                bool param = param1 == "cumplido" ? true : false;

                list = db.Edificios.Where(c => c.Estado == param).ToList();
            }
            else
            {
                list = db.Edificios.ToList();
            }
            return CreateExcel(list, param1);

        }

        public FileContentResult CreateExcel(IEnumerable<Edificio> data, string estado)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 4;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");

                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                sheet.Cells[1, 1].Value = "Estado edificio: ";

                sheet.Cells[3, 1].Value = "Material";
                sheet.Cells[3, 2].Value = "Cantidad";


                List<EdificiosMateriales> result = data.SelectMany(c => c.EdificiosMateriales)
                    .GroupBy(l => l.MaterialId)
                    .Select(cl => new EdificiosMateriales
                    {
                        MaterialId = cl.First().MaterialId,
                        Cantidad = cl.Sum(c => c.Cantidad),
                    }).ToList();

                sheet.Cells[1, 2].Value = estado;

                foreach (var item in result)
                {

                    sheet.Cells[row, 1].Value = db.Materiales.Find(item.MaterialId).Nombre;
                    sheet.Cells[row, 2].Value = item.Cantidad;

                    row++;
                }

                return File(package.GetAsByteArray(), "application/unknown", "CantidadMaterialesPorEstado_" + DateTime.Now.ToShortDateString() + ".xlsx");
            }
        }
        // GET: Edificios/Create
        public ActionResult Create()
        {
            GetAllOptionsTecnicos();
            return View();
        }

        // POST: Edificios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Edificio edificio, string submitButton, int? tecnicoId)
        {
            GetAllOptionsTecnicos();

            if (ModelState.IsValid)
            {
                switch (submitButton)
                {
                    case "Crear":
                        List<Persona> tecBase = new List<Persona>();
                        foreach (var item in edificio.Tecnicos)
                        {
                            tecBase.Add(db.Personas.Find(item.Id));
                        }
                        edificio.Tecnicos = tecBase;
                        db.Edificios.Add(edificio);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    case "Agregar Técnico":
                        Persona p = db.Personas.Find(Convert.ToInt32(Request.Form["TecnicosOptions"]));
                        edificio.Tecnicos.Add(p);
                        break;
                    case null:
                        edificio.Tecnicos = edificio.Tecnicos.Where(c => c.Id != tecnicoId).ToList();
                        break;

                }
            }

            return View(edificio);
        }

        //private void AgregarTecnico(Edificio edificio)
        //{
        //    int idTecnico = Convert.ToInt32(Request.Form["TecnicosOptions"]);
        //    //if (!existMaterialEnEdificio(idMaterial, edificio.Id))
        //    //{
        //    //    new EdificiosMaterialesService().Insert(edificio.Id, idMaterial);
        //    //}
        //    //else
        //    //{
        //    //    ModelState.AddModelError("Error", "Ya existe técnico.");
        //    //}
        //}

        // GET: Edificios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Edificio edificio = db.Edificios.Find(id);
            if (edificio == null)
            {
                return HttpNotFound();
            }
            return View(edificio);
        }

        // POST: Edificios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Edificio edificio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(edificio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(edificio);
        }

        // GET: Edificios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Edificio edificio = db.Edificios.Find(id);
            if (edificio == null)
            {
                return HttpNotFound();
            }
            return View(edificio);
        }

        // POST: Edificios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Edificio edificio = db.Edificios.Find(id);
            db.Edificios.Remove(edificio);
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
