using DigitalSignature_Web.Utils.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalSignature_Web.Models
{
    [Table("PublicKey")]
    [Index(nameof(KeyName), nameof(OwnerId), IsUnique = true)]
    public class PublicKey
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string KeyName { get; set; } = string.Empty;
        [ForeignKey("AppUser")]
        public string OwnerId { get; set; }
        public string KeyBase64 { get; set; }
        public KeyAlgorithm KeyAlgorithm { get; set; }
        [ForeignKey("Certificate")]
        public int CertificateId { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public FlagBoolean FlagDel { get; set; } = FlagBoolean.FALSE;
        public AppUser? Owner { get; set; }
        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    }
}
