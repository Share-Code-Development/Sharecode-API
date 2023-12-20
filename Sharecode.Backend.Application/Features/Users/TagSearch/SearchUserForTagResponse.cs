using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Application.Features.Users.TagSearch
{
    public class SearchUserForTagResponse  : ListResponse<SearchUsersForTagCommand, TagUserResponse>
    {
        public SearchUserForTagResponse(IEnumerable<User> users)
        {
            AddUsers(users);
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

    public sealed class TagUserResponse(User user)
    {
        private Guid UserId { get; set; } = user.Id;

        public string Name { get; set; } = user.FullName;

        public string EmailAddress { get; set; } = user.EmailAddress;

        public string? ProfilePicture { get; set; } = user.ProfilePicture;

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (TagUserResponse) obj;
            return UserId.Equals(other.UserId);
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }
    }
}