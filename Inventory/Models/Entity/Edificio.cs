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
            EdificiosMateriales = new List<EdificiosMateriales>();
            Tecnicos = new List<Persona>();
        }

        public int Id { get; set; }
        [Required]
        public int Codigo { get; set; }
        [Required]
        public string Direccion { get; set; }

        public bool Estado { get; set; }

        public virtual List<Persona> Tecnicos { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Fecha { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaCumplido { get; set; }

        public virtual ICollection<Material> Materiales { get; set; }

        public virtual List<EdificiosMateriales> EdificiosMateriales { get; set; }

        [NotMapped]
        public int CantidadMateriales => CalculateMaterials();

        private int CalculateMaterials() {
            return this.EdificiosMateriales.Sum(c => c.Cantidad);
        }

        [NotMapped]
        public string EstadoDesc
        {
            get
            {
                switch (Estado)
                {
                    case false: return "PreCumplido";
                    case true: return "Cumplido";
                    default: return string.Empty;
                }
            }
        }

    }
}