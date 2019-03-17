using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Inventory.Models.Entity
{
    public class Edificio
    {
        public Edificio()
        {
            this.Materiales = new HashSet<Material>();
        }

        public int Id { get; set; }
        public int Codigo { get; set; }
        public string Direccion { get; set; }
        public string Tecnico { get; set; }
        public int Estado { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Fecha { get; set; }
        public virtual ICollection<Material> Materiales { get; set; }

        public virtual ICollection<EdificiosMateriales> EdificiosMateriales { get; set; }

        [NotMapped]
        public string EstadoDesc
        {
            get
            {
                switch (Estado)
                {
                    case 0: return "PreCumplido";
                    case 1: return "Cumplido";
                    default: return string.Empty;
                }
            }
        }

    }
}