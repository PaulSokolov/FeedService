namespace FeedService.Infrastructure.Response
{

    public class ErrorObject
    {
        public string Error { get; }
        public dynamic ModelState { get; set; }

        public ErrorObject(string message)
        {
            Error = message;
        }
    }


}
