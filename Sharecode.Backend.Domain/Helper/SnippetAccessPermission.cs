using Sharecode.Backend.Domain.Dto.Snippet;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Domain.Helper
{
    public class SnippetAccessPermission
    {
        public Guid SnippetId { get; set; }
        public Guid AccessorId { get; set; }
        private bool Read { get; set; }
        private bool Write { get; set; }
        private bool Manage { get; set; }
        public bool IsPublicSnippet { get; set;}
        public bool IsPrivateSnippet => !IsPublicSnippet;

        // Constructor
        public SnippetAccessPermission(Guid snippetId, Guid accessorId, bool read, bool write, bool manage, bool snippetPublic)
        {
            SnippetId = snippetId;
            AccessorId = accessorId;
            Read = read || write || manage;
            Write = write || manage;
            Manage = manage;
            IsPublicSnippet = snippetPublic;
        }

        public SnippetAccessPermission()
        {
            
        }

        // Factory method for creating SnippetAccessPermission with no permissions
        public static SnippetAccessPermission NoPermission(Guid snippetId, Guid accessorId) =>
            new SnippetAccessPermission(snippetId, accessorId, false, false, false, true);

        // Factory method for creating SnippetAccessPermission with an error state
        public static SnippetAccessPermission Error =>
            new SnippetAccessPermission(Guid.Empty, Guid.Empty, false, false, false, true);

        public bool Any()
        {
            return Any(SnippetAccess.Read, SnippetAccess.Write, SnippetAccess.Manage);
        }

        public bool Any(params SnippetAccess[] accesses)
        {
            foreach (var access in accesses)
            {
                switch (access)
                {
                    case SnippetAccess.Read when Read:
                    case SnippetAccess.Write when Write:
                    case SnippetAccess.Manage when Manage:
                        return true;
                }
            }

            return false;
        }

        public bool All()
        {
            return All(SnippetAccess.Read, SnippetAccess.Write, SnippetAccess.Manage);
        }

        public bool All(params SnippetAccess[] accesses)
        {
            foreach (var access in accesses)
            {
                switch (access)
                {
                    case SnippetAccess.Read when !Read:
                    case SnippetAccess.Write when !Write:
                    case SnippetAccess.Manage when !Manage:
                        return false;
                }
            }

            return true;
        }

        public SnippetAccessControlDto ToControlModel()
        {
            return new SnippetAccessControlDto()
            {
                UserId = AccessorId,
                Manage = Manage,
                Read = Read,
                Write = Write
            };
        }
    }
}