

namespace FeedService.DbModels.Interfaces
{
    public interface IFeedServiceUoW
    {
        IRepository<Collection> Collections { get; }
        IRepository<Feed> Feeds { get; }
        IRepository<User> Users { get; }
        IRepository<CollectionFeed> CollectionsFeeds { get; }
    }
}
