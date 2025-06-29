using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalSignature_Web.Models
{
    [Table("DocHashtag")]
    [Index(nameof(Hashtag), IsUnique = true)]
    public class DocHashtag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Hashtag { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
