using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Persistencia.Entidades;
// Nos projetos no estilo SDK como este, vários atributos de assembly que sempre eram
// definidos nesse arquivo agora são adicionados automaticamente durante o build e
// populados com os valores definidos nas propriedades do projeto. Para obter detalhes
// de quais atributos são incluídos e como personalizar esse processo, confira: https://aka.ms/assembly-info-properties


// A definição de ComVisible como false torna os tipos neste assembly invisíveis para
// os componentes do COM. Se for necessário acessar um tipo do COM neste assembly,
// defina o atributo ComVisible como true nesse tipo.

[assembly: ComVisible(false)]

// O GUID a seguir será destinado à ID de typelib se este projeto for exposto ao COM.

[assembly: Guid("85a5d076-0560-4f92-ab6c-837aa2450e65")]



namespace MvcMovie.Models
{
    public class MovieGenreViewModel
    {
        public List<Movie> Movies { get; set; }
        public SelectList Genres { get; set; }
        public string MovieGenre { get; set; }
        public string SearchString { get; set; }
    }
}
