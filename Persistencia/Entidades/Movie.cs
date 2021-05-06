using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Persistencia.Entidades
{
    public class Movie
    {
        public int MovieId { get; set; }

        [Display(Name = "Filme")]
        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Diretor")]
        [StringLength(80, MinimumLength = 3)]
        [Required]
        public string Director { get; set; }

        [Display(Name = "Lançamento")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Faturamento")]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^(?!0|\.00)[0-9]+(,\d{3})*(.[0-9]{0,2})$")]
        public decimal Gross { get; set; }

        [Display(Name = "Nota")]
        [RegularExpression(@"^(?!0|\.00)[0-9]+(,\d{3})*(.[0-9]{0,2})$")] 
        [Required]
        public double Rating { get; set; }

        public int GenreID { get; set; }
        public virtual Genre Genre { get; set; }

        public virtual ICollection<ActorMovie> Characters { get; set; }
    }

}
