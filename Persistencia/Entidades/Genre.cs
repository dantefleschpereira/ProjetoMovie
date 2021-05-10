using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Persistencia.Entidades
{
     public class Genre
    {
        public int GenreId { get; set; }

        [Display(Name="Gênero")]
        [StringLength(50, MinimumLength = 3)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(500, MinimumLength = 3)]
        [Required]
        public string Description { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Movie> Movies { get; set; }

    }
}
