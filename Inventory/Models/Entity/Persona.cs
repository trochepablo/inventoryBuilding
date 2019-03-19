using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models.Entity
{
    public class Persona
    {
        public int Id { get; set; }
        public int Dni { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Cargo { get; set; }

        public virtual List<Edificio> Edificios { get; set; }
    }
}