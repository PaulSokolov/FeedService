namespace FeedService.Intrefaces
{
    public interface IFeedItem
    {
        string Title { get; set; }
        string Description { get; set; }
        string PublishedDate { get; set; }
    }
}
