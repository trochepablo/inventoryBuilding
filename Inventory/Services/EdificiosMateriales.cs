using Inventory.Models.Context;
using Inventory.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Services
{
    public class EdificiosMaterialesService
    {
        private InventoryContext db = new InventoryContext();

        public void Insert(int idEdificio, int idMaterial)
        {
            db.EdificiosMateriales.Add(new EdificiosMateriales { EdificioId = idEdificio, MaterialId = idMaterial, Cantidad = 0 });
            db.SaveChanges();
        }
    }
}