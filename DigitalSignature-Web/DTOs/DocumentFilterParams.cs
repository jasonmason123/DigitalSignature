using DigitalSignature_Web.Models;
using DigitalSignature_Web.Utils;
using DigitalSignature_Web.Utils.Enums;

namespace DigitalSignature_Web.DTOs
{
    public class DocumentFilterParams
    {
        public FlagBoolean? FlagDel { get; set; } = FlagBoolean.FALSE;
        public List<string>? Hashtags { get; set; } = new List<string>();
    }
}
