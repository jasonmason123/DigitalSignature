using DigitalSignature_Web.Utils.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalSignature_Web.Models
{
    [Table("AppUser")]
    public class AppUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public FlagBoolean FlagDel { get; set; } = FlagBoolean.FALSE;
        public ICollection<PublicKey>? PublicKeys { get; set; } = new List<PublicKey>();
        public ICollection<Document>? DocumentsOwned { get; set; } = new List<Document>();
        public ICollection<Document>? SharedDocuments { get; set; } = new List<Document>();
    }
}
