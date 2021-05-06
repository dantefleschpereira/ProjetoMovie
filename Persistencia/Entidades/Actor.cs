using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Persistencia.Entidades
{
    public class Actor

    {

        public int ActorId { get; set; }

        [Display(Name = "Ator")]
        [StringLength(80, MinimumLength = 3)]
        [Required]
        public String Name { get; set; }

        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime DateBirth { get; set; }

        public virtual ICollection<ActorMovie> Characters { get; set; }

    }
}
