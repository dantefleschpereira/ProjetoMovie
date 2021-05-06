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
        [StringLength(20, MinimumLength = 3)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(180, MinimumLength = 3)]
        [Required]
        public string Description { get; set; }

        public virtual ICollection<Movie> Movies { get; set; }

    }
}
