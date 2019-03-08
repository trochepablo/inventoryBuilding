using Inventory.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Inventory.Models.Context
{
    public class InventoryContext : DbContext
    { 
        public InventoryContext() : base("name=InventoryContext")
        {
        }

        public DbSet<Edificio> Edificios { get; set; }
        public DbSet<Material> Materiales { get; set; }
        public DbSet<EdificiosMateriales> EdificiosMateriales { get; set; }
    }
}
