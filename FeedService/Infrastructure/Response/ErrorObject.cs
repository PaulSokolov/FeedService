namespace FeedService.Infrastructure.Response
{

    public class ErrorObject
    {
        public string Error { get; }
        public object ModelState { get; set; }

        public ErrorObject(string message)
        {
            Error = message;
        }
    }

   
    }
