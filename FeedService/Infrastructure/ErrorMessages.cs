using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FeedService.Infrastructure
{
    public abstract class ErrorMessages
    {
        public const string SERVER_ERROR = "Server error. Try later.";
        public const string COLLECTION_NOT_FOUND_ERROR = "Collection not found.";
        public const string USER_NOT_FOUND_ERROR = "User not found.";
        public const string FEED_NOT_FOUND_ERROR = "Feed not found.";
        public const string BAD_REQUEST_ERROR = "Invalid request parameters.";
        public const string USER_ALREADY_EXISTS_ERROR = "User with such login already exists";
        public const string WRONG_PASSWORD_LENGTH = "Password should be at least 6 symbols";
        public const string INVALID_USERNAME_OR_PASSWORD_ERROR = "Invalid username or password.";
        public const string FEED_ALREADY_EXISTS_ERROR = "This feed already exists in this collection.";
    }

    public abstract class SuccessMessages
    {
        public const string REGISTRATION_SUCCESS = "registered successfully.";
        public const string COLLECTION_EDITED_SUCCESSFULLY = "Collection edited successfully.";
        public const string FEED_ADDED_SUCCESSFULLY = "Feed added to collection successfully.";
        public const string COLLECTION_DELETED_SUCCESSFULLY = "Collection deleted succesfully";
    }

    public class ErrorObject
    {
        public string Error { get;}
        public ModelStateDictionary ModelState { get; set; }

        public ErrorObject(string message)
        {
            Error = message;
        }
    }

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
