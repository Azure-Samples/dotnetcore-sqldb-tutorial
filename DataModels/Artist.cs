using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreSqlDb.DataModels
{
    [DebuggerDisplay("{Name} (ArtistId = {ArtistId})")]
    public class Artist
    {
        [Key]
        public int ArtistId { get; set; }

        [Required, MaxLength(120)]
        public string Name { get; set; }
    }
}
