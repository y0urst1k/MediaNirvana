using Infrastructure.EF.Entity.IndependentEntity;
using Infrastructure.Interface;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service
{
    public class TagService : Service<Tag>
    {
        public TagService(IRepository<Tag> repo, ILogger<Service<Tag>> logger)
        : base(repo, logger) { }

        public async Task<List<Tag>> GetOrCreateTagsAsync(List<string> tagNames)
        {
            var existingTags = await GetItemsAsync();
            var result = new List<Tag>();

            foreach (var name in tagNames)
            {
                var tag = existingTags.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (tag == null)
                {
                    tag = new Tag { Name = name };
                    await CreateItemAsync(tag);
                }
                result.Add(tag);
            }
            return result;
        }
    }
}