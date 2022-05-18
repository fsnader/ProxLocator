using System.Collections.Generic;
using System.Linq;

namespace ProxLocator.Engine.Entities
{
    public class CheckPoint
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public List<Tag> Tags { get; set; }

        public CheckPoint(string id,
            IEnumerable<long> tagIds,
            string description = null)
        {
            Id = id;
            Description = description;
            Tags = tagIds
                .Select(tagId => new Tag(tagId))
                .ToList();
        }

        /// <summary>
        /// Receives new messages and save in the correct queue
        /// </summary>
        /// <param name="message"></param>
        public void ReceiveMessage(LocatorMessage message)
        {
            var tag = Tags
                .SingleOrDefault(x => x.Id == message.TagId);

            tag?.EnqueueMessage(message);
        }

        /// <summary>
        /// Returns the oldest message received from the indicated tag
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public LocatorMessage DequeueMessageFromTag(long tagId)
        {
            var tag = Tags
                .SingleOrDefault(x => x.Id == tagId);

            return tag?.DequeueMessage();
        }

        /// <summary>
        /// Clear all tag queues
        /// </summary>
        public void Clear()
        {
            foreach (var tag in Tags)
                tag.Clear();
        }
    }
}
