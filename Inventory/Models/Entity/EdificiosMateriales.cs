using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Inventory.Models.Entity
{
    public class EdificiosMateriales
    {
        // Set the column order so it appears nice in the database
        [Key, Column(Order = 0)]
        public int EdificioId { get; set; }

        [Key, Column(Order = 1)]
        public int MaterialId { get; set; }

        // Add the navigation properties
        public virtual Edificio Edificio { get; set; }

        public virtual Material Material { get; set; }

        // Add any additional fields you need
        public int Cantidad { get; set; }
    }
}