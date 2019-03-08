using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.Models.Context;
using Inventory.Models.Entity;

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
