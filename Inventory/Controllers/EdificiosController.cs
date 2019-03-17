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

            Edificio edificio = BuildModelEdificio(id, false);

            ViewBag.MaterialesOptions = db.Materiales.ToList()
             .Select(x => new SelectListItem { Text = string.Format("{0} - {1}", x.Codigo, x.Nombre), Value = x.Id.ToString() })
             .ToList();

            if (edificio == null)
            {
                return HttpNotFound();
            }

            return View(edificio);
        }

        private Edificio BuildModelEdificio(int? id, bool isUpdate)
        {
            Edificio edificio = db.Edificios.Find(id);

            if (isUpdate || edificio.Materiales == null || edificio.Materiales.Count == 0)
            {
                if (isUpdate)
                {
                    edificio.Materiales.Clear();
                }
                var edificioMateriales = db.EdificiosMateriales.Where(c => c.EdificioId == id);

                foreach (var item in edificioMateriales)
                {
                    edificio.Materiales.Add(new Material { Codigo = item.Material.Codigo, Id = item.Material.Id, Nombre = item.Material.Nombre });
                }
            }

            return edificio;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Associate([Bind(Include = "Id,Codigo,Direccion,Tecnico,Estado,Fecha,Materiales")] Edificio edificio, string submitButton)
        {
            bool isUpdate = false;
            ViewBag.MaterialesOptions = db.Materiales.ToList()
             .Select(x => new SelectListItem { Text = string.Format("{0} - {1}", x.Codigo, x.Nombre), Value = x.Id.ToString() })
             .ToList();

            if (ModelState.IsValid)
            {
                switch (submitButton)
                {
                    case "Agregar Material":
                        int idMaterial = Convert.ToInt32(Request.Form["MaterialesOptions"]);
                        if (!existMaterialEnEdificio(idMaterial, edificio.Id))
                        {
                            new EdificiosMaterialesService().Insert(edificio.Id, idMaterial);
                            edificio.Materiales = null;
                            isUpdate = true;
                        }
                        else {
                            ModelState.AddModelError("Error", "Ya existe material.");
                        }
                        break;
                    case "Guardar":
                        break;
                    default:
                        break;
                }
            }

            return View(BuildModelEdificio(edificio.Id, isUpdate));
        }

        private bool existMaterialEnEdificio(int idMaterial, int ediId)
        {
            Edificio edi = BuildModelEdificio(ediId, false);
            return edi.Materiales.Any(c => c.Id == idMaterial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMaterial([Bind(Include = "Id,Codigo,Direccion,Tecnico,Estado,Fecha,Materiales")] Edificio edificio, string submitButton)
        {
            if (ModelState.IsValid)
            {

            }

            return View(edificio);
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
