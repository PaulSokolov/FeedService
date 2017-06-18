namespace FeedService.Infrastructure.Response
{

    public class SuccessObject
    {
        public string Success { get; }
        public object Result { get; set; }

        public SuccessObject()
        {
            Success = "Operation performed successfully.";
        }

        public SuccessObject(string message)
        {
            Success = message;
        }
    }
}