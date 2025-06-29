using DigitalSignature_Web.DTOs;
using DigitalSignature_Web.Models;
using X.PagedList;

namespace DigitalSignature_Web.Repository.IRepositories
{
    public interface IPublicKeyRepository
    {
        public Task<PublicKey> GetAsync(Guid id);
        public IPagedList<PublicKey> GetList(PublicKeyFilterParams filterParams, int pageNumber, int pageSize);
        public IPagedList<PublicKey> Search(string searchString, int pageNumber, int pageSize);
        public Task<PublicKey> AddAsync(PublicKey document);
        public Task<PublicKey> UpdateAsync(PublicKey document);
        public Task<bool> ActivateAsync(Guid id);
        public Task<bool> DeactivateAsync(Guid id);
        public Task<bool> DeleteAsync(Guid id);
    }
}
