using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Inventory.Models.Context;
using Inventory.Models.Entity;
using Inventory.Services;

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

        private bool existMaterialEnEdificio(int idMaterial, int ediId)
        {
            return db.EdificiosMateriales.Where(c => c.EdificioId == ediId).Any(c => c.MaterialId == idMaterial);
        }

        // GET: Edificios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Edificios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Codigo,Direccion,Tecnico,Estado,Fecha")] Edificio edificio)
        {
            if (ModelState.IsValid)
            {
                db.Edificios.Add(edificio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(edificio);
        }

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
        public ActionResult Edit([Bind(Include = "Id,Codigo,Direccion,Tecnico,Estado,Fecha")] Edificio edificio)
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
