using DigitalSignature_Web.Utils.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalSignature_Web.Models
{
    [Table("Document")]
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("AppUser")]
        public string OwnerId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public HashingAlgorithm HashingAlgorithm { get; set; }
        public FlagBoolean IsPublic { get; set; } = FlagBoolean.FALSE;
        public DocumentStatus DocumentStatus { get; set; } = DocumentStatus.PENDING;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public FlagBoolean FlagDel { get; set; } = FlagBoolean.FALSE;
        public AppUser? Owner { get; set; }
        public ICollection<DocHashtag> Hashtags { get; set; } = new List<DocHashtag>();
        public ICollection<Signature> Signatures { get; set; } = new List<Signature>();
        public ICollection<AppUser> Viewers { get; set; } = new List<AppUser>();
    }
}
