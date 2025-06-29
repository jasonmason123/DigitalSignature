using DigitalSignature_Web.DTOs;
using DigitalSignature_Web.Models;
using X.PagedList;

namespace DigitalSignature_Web.Repository.IRepositories
{
    public interface IDocHashtagRepository
    {
        public Task<DocHashtag> GetAsync(int id);
        public IPagedList<DocHashtag> GetList(int pageNumber, int pageSize);
        public IPagedList<DocHashtag> Search(string searchString, int pageNumber, int pageSize);
        public Task<DocHashtag> CreateAsync(DocHashtag document);
        public Task<DocHashtag> UpdateAsync(DocHashtag document);
        public Task<bool> DeleteAsync(int id);
    }
}
