using DigitalSignature_Web.DTOs;
using DigitalSignature_Web.Models;
using X.PagedList;

namespace DigitalSignature_Web.Repository.IRepositories
{
    public interface IDocumentRepository
    {
        /// <summary>
        /// Get a document by its ID.
        /// </summary>
        public Task<Document> GetAsync(int id);
        /// <summary>
        /// Get a paginated list of documents based on filter parameters.
        /// </summary>
        public IPagedList<Document> GetList(DocumentFilterParams filterParams, int pageNumber, int pageSize);
        /// <summary>
        /// Search documents by a search string and return a paginated list.
        /// </summary>
        public IPagedList<Document> Search(string searchString, int pageNumber, int pageSize);
        /// <summary>
        /// Add a new document.
        /// </summary>
        public Task<Document> AddAsync(Document document);
        /// <summary>
        /// Update an existing document.
        /// </summary>
        public Task<Document> UpdateAsync(Document document);
        /// <summary>
        /// Set a document's flag deleted as false.
        /// </summary>
        public Task<bool> ActivateAsync(int id);
        /// <summary>
        /// Set a document's flag deleted as true.
        /// </summary>
        public Task<bool> DeactivateAsync(int id);
        /// <summary>
        /// Delete a document permanently.
        /// </summary>
        public Task<bool> DeleteAsync(int id);
        /// <summary>
        /// Update the viewer list of a document.
        /// </summary>
        public Task<bool> UpdateViewersAsync(int documentId, List<string> viewerIds);
        /// <summary>
        /// Update the signer (signature) list of a document.
        /// </summary>
        public Task<bool> UpdateSignerAsync(int documentId, List<Signature> signatures);
        /// <summary>
        /// Update the visibility of a document.
        /// </summary>
        public Task<bool> UpdateDocumentVisibility(int documentId, bool isPublic);
        /// <summary>
        /// Switch a document's status to ready to be signed.
        /// </summary>
        public Task<bool> SwitchToReadyToBeSigned(int documentId);
    }
}
