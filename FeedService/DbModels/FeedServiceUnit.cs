using FeedService.DbModels.Interfaces;

namespace FeedService.DbModels
{
    public class FeedServiceUnit: IFeedServiceUoW
    {
        IRepository<Collection> _collections;
        IRepository<Feed> _feeds;
        IRepository<User> _users;
        IRepository<CollectionFeed> _collectionsFeeds;

        public FeedServiceUnit(IRepository<Collection> collections, IRepository<Feed> feeds, IRepository<User> users, IRepository<CollectionFeed> collectionFeeds)
        {
            _collections = collections;
            _feeds = feeds;
            _users = users;
            _collectionsFeeds = collectionFeeds;
        }

        public IRepository<Collection> Collections => _collections;
        public IRepository<Feed> Feeds => _feeds;
        public IRepository<User> Users => _users;
        public IRepository<CollectionFeed> CollectionsFeeds => _collectionsFeeds;
    }
}
