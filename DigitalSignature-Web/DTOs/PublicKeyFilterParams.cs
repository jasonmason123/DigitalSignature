using DigitalSignature_Web.Utils;
using DigitalSignature_Web.Utils.Enums;

namespace DigitalSignature_Web.DTOs
{
    public class PublicKeyFilterParams
    {
        public bool? IsRevoked { get; set; }
        public FlagBoolean? FlagDel { get; set; } = FlagBoolean.FALSE;
        public int PageNumber { get; set; } = AppConst.DEFAULT_PAGE_NUMBER;
        public int PageSize { get; set; } = AppConst.DEFAULT_PAGE_SIZE;
    }
}
