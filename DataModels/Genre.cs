using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace DotNetCoreSqlDb.DataModels
{
    [DebuggerDisplay("{Name} (GenreId = {GenreId})")]
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }

        [MaxLength(120)]
        public string Name { get; set; }
    }
}
