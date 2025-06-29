using DigitalSignature_Web.Utils.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalSignature_Web.Models
{
    [Table("Signature")]
    public class Signature
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("AppUser")]
        public string SignerId { get; set; }
        [ForeignKey("Document")]
        public int DocumentId { get; set; }
        public string SignerRole { get; set; }       // e.g., "Reviewer", "Approver", etc.
        public string? SignerAuthority { get; set; } // e.g., "Manager", "Director", etc.
        public string SignatureBase64 { get; set; } = string.Empty;
        public string PublicKeyBase64 { get; set; } = string.Empty;
        public KeyAlgorithm Algorithm { get; set; }
        public FlagBoolean IsSigned { get; set; } = FlagBoolean.FALSE;
        public DateTime? SignedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
