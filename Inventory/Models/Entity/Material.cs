using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models.Entity
{
    public class Material
    {
        public int Id { get; set; }
        public int Codigo { get; set; }
        public string Nombre { get; set; }
    }
}