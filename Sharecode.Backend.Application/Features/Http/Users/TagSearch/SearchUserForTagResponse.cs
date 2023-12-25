using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Application.Features.Http.Users.TagSearch
{
    public class SearchUserForTagResponse  : ListResponse<SearchUsersForTagCommand, TagUserResponse>
    {
        public SearchUserForTagResponse(IEnumerable<User> users)
        {
            AddUsers(users);
        }

        public SearchUserForTagResponse()
        {
        }

        private void AddUsers(IEnumerable<User> users)
        {
            HashSet<TagUserResponse> responses = new HashSet<TagUserResponse>();
            foreach (var user in users)
            {
                responses.Add(new TagUserResponse(user));
            }
            AddRecords(responses, true);
        }
    }

    public class TagUserResponse
    {
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string? ProfilePicture { get; set; }

        public TagUserResponse()
        {
            // parameterless constructor
        }

        public TagUserResponse(User? user)
        {
            UserId = user?.Id;
            Name = user.FullName;
            EmailAddress = user.EmailAddress;
            ProfilePicture = user.ProfilePicture;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var other = (TagUserResponse)obj;
            return EmailAddress.Equals(other.EmailAddress);
        }

        public override int GetHashCode()
        {
            return EmailAddress.GetHashCode();
        }
    }
}