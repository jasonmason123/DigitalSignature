using DigitalSignature_Web.Context;
using DigitalSignature_Web.DTOs;
using DigitalSignature_Web.Models;
using DigitalSignature_Web.Repository.IRepositories;
using DigitalSignature_Web.Utils;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using X.PagedList;
using X.PagedList.Extensions;

namespace DigitalSignature_Web.Repository.SqlServerRepositories
{
    public class DocHashtagRepository : IDocHashtagRepository
    {
        private readonly AppDbContext _context;

        public DocHashtagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DocHashtag> GetAsync(int id)
        {
            return await _context.DocHashtags
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public IPagedList<DocHashtag> GetList(int pageNumber = AppConst.DEFAULT_PAGE_NUMBER, int pageSize = AppConst.DEFAULT_PAGE_SIZE)
        {
            return _context.DocHashtags
                .AsNoTracking()
                .OrderByDescending(d => d.CreatedAt)
                .ToPagedList(pageNumber, pageSize);
        }

        public IPagedList<DocHashtag> Search(string searchString, int pageNumber = AppConst.DEFAULT_PAGE_NUMBER, int pageSize = AppConst.DEFAULT_PAGE_SIZE)
        {
            return _context.DocHashtags
                .AsNoTracking()
                .Where(d => d.Hashtag.Contains(searchString))
                .OrderByDescending(x => x.CreatedAt)
                .ToPagedList(pageNumber, pageSize);
        }

        public async Task<DocHashtag> CreateAsync(DocHashtag hashtag)
        {
            hashtag.CreatedAt = DateTime.UtcNow;
            await _context.DocHashtags.AddAsync(hashtag);
            await _context.SaveChangesAsync();
            return hashtag;
        }

        public async Task<DocHashtag> UpdateAsync(DocHashtag hashtag)
        {
            var result = await _context.DocHashtags
                .Where(x => x.Id == hashtag.Id)
                .ExecuteUpdateAsync(d => d
                    .SetProperty(x => x.Hashtag, hashtag.Hashtag)
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow));

            if (result <= 0)
            {
                return null;
            }
            return hashtag;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _context.DocHashtags
                .Where(pk => pk.Id == id)
                .ExecuteDeleteAsync();

            if (result <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
