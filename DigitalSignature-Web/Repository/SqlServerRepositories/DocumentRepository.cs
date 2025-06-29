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
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserRetriever _currentUserRetriever;

        public DocumentRepository(AppDbContext context, ICurrentUserRetriever currentUserRetriever)
        {
            _context = context;
            _currentUserRetriever = currentUserRetriever;
        }

        public async Task<Document> GetAsync(int id)
        {
            return await _context.Documents
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && d.OwnerId == _currentUserRetriever.UserId);
        }

        public IPagedList<Document> GetList(DocumentFilterParams filterParams, int pageNumber = AppConst.DEFAULT_PAGE_NUMBER, int pageSize = AppConst.DEFAULT_PAGE_SIZE)
        {
            var query = _context.Documents
                .AsNoTracking()
                .Where(d => d.OwnerId == _currentUserRetriever.UserId);

            if (filterParams.FlagDel != null)
            {
                query.Where(x => x.FlagDel == filterParams.FlagDel);
            }

            if (filterParams.Hashtags != null && filterParams.Hashtags.Any())
            {
                query = query
                    .Include(x => x.Hashtags)
                    .Where(d => d.Hashtags.Any(h => filterParams.Hashtags.Equals(h.Hashtag)));
            }

            return query
                .OrderByDescending(d => d.CreatedAt)
                .ToPagedList(pageNumber, pageSize);
        }

        public IPagedList<Document> Search(string searchString, int pageNumber = AppConst.DEFAULT_PAGE_NUMBER, int pageSize = AppConst.DEFAULT_PAGE_SIZE)
        {
            return _context.Documents
                .AsNoTracking()
                .Include(d => d.Hashtags)
                .Where(d => d.OwnerId == _currentUserRetriever.UserId &&
                    (
                        d.DocumentName.Contains(searchString) ||
                        d.Hashtags.Any(h => h.Hashtag.Contains(searchString))
                    ))
                .OrderByDescending(x => x.CreatedAt)
                .ToPagedList(pageNumber, pageSize);
        }

        public async Task<Document> AddAsync(Document document)
        {
            document.OwnerId = _currentUserRetriever.UserId;
            await _context.Documents.AddAsync(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<Document> UpdateAsync(Document document)
        {
            var existingDocument = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == document.Id && d.OwnerId == _currentUserRetriever.UserId);

            if (existingDocument == null)
            {
                return null;
            }

            // Check the document status, only editable if it's pending
            if (existingDocument.DocumentStatus != DocumentStatus.PENDING)
            {
                throw new InvalidOperationException("Cannot update a document that is ready to be signed.");
            }

            if (existingDocument.DocumentName != document.DocumentName)
            {
                existingDocument.DocumentName = document.DocumentName;
            }

            if(existingDocument.DocumentPath != document.DocumentPath)
            {
                existingDocument.DocumentPath = document.DocumentPath;
            }

            if(existingDocument.HashingAlgorithm != document.HashingAlgorithm)
            {
                existingDocument.HashingAlgorithm = document.HashingAlgorithm;
            }

            if(existingDocument.IsPublic != document.IsPublic)
            {
                existingDocument.IsPublic = document.IsPublic;
            }

            var currentTagNames = existingDocument.Hashtags.Select(h => h.Hashtag).OrderBy(x => x).ToList();
            var newTagNames = document.Hashtags.Select(h => h.Hashtag).OrderBy(x => x).ToList();

            if (!currentTagNames.SequenceEqual(newTagNames))
            {
                existingDocument.Hashtags.Clear();

                foreach (var newTag in document.Hashtags)
                {
                    existingDocument.Hashtags.Add(newTag);
                }
            }

            existingDocument.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return existingDocument;
        }

        public async Task<bool> ActivateAsync(int id)
        {
            var result = await _context.Documents
                .Where(pk => pk.Id == id && pk.OwnerId == _currentUserRetriever.UserId && pk.FlagDel == FlagBoolean.TRUE)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.FlagDel, FlagBoolean.FALSE));

            if (result <= 0)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var result = await _context.Documents
                .Where(pk => pk.Id == id && pk.OwnerId == _currentUserRetriever.UserId && pk.FlagDel == FlagBoolean.FALSE)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.FlagDel, FlagBoolean.TRUE));

            if (result <= 0)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _context.Documents
                .Where(pk => pk.Id == id && pk.OwnerId == _currentUserRetriever.UserId)
                .ExecuteDeleteAsync();

            if (result <= 0)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateViewersAsync(int documentId, List<string> viewerIds)
        {
            var document = await _context.Documents
                .Include(d => d.Viewers)
                .FirstOrDefaultAsync(d => d.Id == documentId && d.OwnerId == _currentUserRetriever.UserId);

            if (document == null) 
            {
                return false;
            }

            document.Viewers.Clear();
            foreach (var viewerId in viewerIds)
            {
                var viewer = await _context.AppUsers.FindAsync(viewerId);
                if (viewer != null)
                {
                    document.Viewers.Add(viewer);
                }
            }

            document.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateSignerAsync(int documentId, List<Signature> signatures)
        {
            var document = await _context.Documents
                .Include(d => d.Signatures)
                .FirstOrDefaultAsync(d => d.Id == documentId && d.OwnerId == _currentUserRetriever.UserId);

            if (document == null)
            {
                return false;
            }

            // Check the document status, only editable if it's pending
            if (document.DocumentStatus != DocumentStatus.PENDING)
            {
                throw new InvalidOperationException("Cannot update signer for a document that is ready to be signed.");
            }

            document.Signatures.Clear();
            foreach(var signature in signatures)
            {
                // Ensure the signature is associated with the document
                signature.DocumentId = documentId;
                signature.IsSigned = FlagBoolean.FALSE;
                document.Signatures.Add(signature);
            }

            document.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateDocumentVisibility(int documentId, bool isPublic)
        {
            var result = await _context.Documents
                .Where(pk => pk.Id == documentId && pk.OwnerId == _currentUserRetriever.UserId && pk.FlagDel == FlagBoolean.FALSE)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.IsPublic, isPublic ? FlagBoolean.TRUE : FlagBoolean.FALSE));

            if (result <= 0)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> SwitchToReadyToBeSigned(int documentId)
        {
            var result = await _context.Documents
                .Where(pk => pk.Id == documentId && pk.OwnerId == _currentUserRetriever.UserId && pk.FlagDel == FlagBoolean.FALSE)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.DocumentStatus, DocumentStatus.READY_TO_BE_SIGNED));


            if (result <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
