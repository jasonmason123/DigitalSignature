using DigitalSignature_Web.Context;
using DigitalSignature_Web.DTOs;
using DigitalSignature_Web.Infrastructure.CurrentUserRetriever;
using DigitalSignature_Web.Models;
using DigitalSignature_Web.Repository.IRepositories;
using DigitalSignature_Web.Utils;
using DigitalSignature_Web.Utils.Enums;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace DigitalSignature_Web.Repository.SqlServerRepositories
{
    public class PublicKeyRepository : IPublicKeyRepository
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserRetriever _currentUserRetriever;

        public PublicKeyRepository(AppDbContext context, ICurrentUserRetriever currentUserRetriever)
        {
            _context = context;
            _currentUserRetriever = currentUserRetriever;
        }

        public async Task<PublicKey> GetAsync(Guid id)
        {
            return await _context.PublicKeys
                .AsNoTracking()
                .FirstOrDefaultAsync(pk => pk.Id == id && pk.OwnerId == _currentUserRetriever.UserId);
        }

        public IPagedList<PublicKey> GetList(PublicKeyFilterParams filterParams, int pageNumber = AppConst.DEFAULT_PAGE_NUMBER, int pageSize = AppConst.DEFAULT_PAGE_SIZE)
        {
            var query = _context.PublicKeys
                .AsNoTracking()
                .Where(pk => pk.OwnerId == _currentUserRetriever.UserId);

            if(filterParams.FlagDel != null)
            {
                query.Where(x => x.FlagDel == filterParams.FlagDel);
            }

            if(filterParams.IsRevoked != null)
            {
                query = query.Where(x => x.IsRevoked == filterParams.IsRevoked);
            }

            return query
                .OrderByDescending(x => x.CreatedAt)
                .ToPagedList(filterParams.PageNumber, filterParams.PageSize);
        }

        public IPagedList<PublicKey> Search(string searchString, int pageNumber = AppConst.DEFAULT_PAGE_NUMBER, int pageSize = AppConst.DEFAULT_PAGE_SIZE)
        {
            var query = _context.PublicKeys
                .AsNoTracking()
                .Where(pk => pk.OwnerId == _currentUserRetriever.UserId &&
                    pk.KeyName.Contains(searchString));

            return query
                .OrderByDescending(x => x.CreatedAt)
                .ToPagedList(pageNumber, pageSize);
        }

        public async Task<bool> ActivateAsync(Guid id)
        {
            var result = await _context.PublicKeys
                .Where(pk => pk.Id == id && pk.OwnerId == _currentUserRetriever.UserId && pk.FlagDel == FlagBoolean.TRUE)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.FlagDel, FlagBoolean.FALSE));

            if(result <= 0)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeactivateAsync(Guid id)
        {
            var result = await _context.PublicKeys
                .Where(pk => pk.Id == id && pk.OwnerId == _currentUserRetriever.UserId && pk.FlagDel == FlagBoolean.FALSE)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.FlagDel, FlagBoolean.TRUE));

            if (result <= 0)
            {
                return false;
            }
            return true;
        }

        public async Task<PublicKey> AddAsync(PublicKey publicKey)
        {
            publicKey.OwnerId = _currentUserRetriever.UserId;
            await _context.PublicKeys.AddAsync(publicKey);
            return publicKey;
        }

        public async Task<PublicKey> UpdateAsync(PublicKey publicKey)
        {
            var result = await _context.PublicKeys
                .Where(pk => pk.Id == publicKey.Id && pk.OwnerId == _currentUserRetriever.UserId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.KeyName, publicKey.KeyName)
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow));

            if (result <= 0)
            {
                return null;
            }
            return publicKey;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var result = await _context.PublicKeys
                .Where(pk => pk.Id == id && pk.OwnerId == _currentUserRetriever.UserId)
                .ExecuteDeleteAsync();

            if (result <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
