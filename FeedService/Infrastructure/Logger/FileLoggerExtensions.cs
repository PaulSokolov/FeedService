using Microsoft.Extensions.Logging;

namespace FeedService.Infrastructure.Logger
{
    public static class FileLoggerExtensions
    {
        public static ILoggerFactory AddFile(this ILoggerFactory loggerFactory, string fileParh)
        {
            loggerFactory.AddProvider(new FileLoggerProvider(fileParh));
            return loggerFactory;
        }
    }
}
