using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DigitalSignature_Web.Utils.Enums;

namespace DigitalSignature_Web.Models
{
    [Table("Certificate")]
    public class Certificate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Thumbprint { get; set; }
        public string SerialNumber { get; set; }
        public string Subject { get; set; }
        public string Issuer { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsRevoked { get; set; } = false;
        public string SignatureAlgorithm { get; set; }
        public string Signature { get; set; }
        public string CertificatePath { get; set; }
        [ForeignKey("PublicKey")]
        public Guid PublicKeyId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public FlagBoolean FlagDel { get; set; } = FlagBoolean.FALSE;
        public PublicKey? PublicKey { get; set; }
    }
}
