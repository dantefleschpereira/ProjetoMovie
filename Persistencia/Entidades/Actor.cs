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
        public String Name { get; set; }

        [Display(Name = "Data de Nascimento")]
        public DateTime DateBirth { get; set; }

        public virtual ICollection<ActorMovie> Characters { get; set; }

    }
}
