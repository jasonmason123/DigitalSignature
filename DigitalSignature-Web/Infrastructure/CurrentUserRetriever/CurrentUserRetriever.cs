using DigitalSignature_Web.Models;
using Microsoft.AspNetCore.Identity;

namespace DigitalSignature_Web.Infrastructure.CurrentUserRetriever
{
    public class CurrentUserRetriever : ICurrentUserRetriever
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public CurrentUserRetriever(
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        /// <summary>
        /// sets the UserId to the current user's Id
        /// </summary>
        public string UserId =>
            _userManager.GetUserId(_httpContextAccessor.HttpContext?.User)
            ?? throw new InvalidOperationException("User is not authenticated.");
    }
}
