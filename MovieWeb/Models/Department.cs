using Persistencia.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Persistencia.ViewModels
{
    public class Department
    {
        public int MovieID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Filme { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "Faturamento")]
        public decimal Faturamento { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Lançamento")]
        public DateTime Lançamento { get; set; }

        public int? GenreID { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        // qual alteração fazer
        public Genre Administrator { get; set; }
        public ICollection<Movie> Movies { get; set; }

    }
}
