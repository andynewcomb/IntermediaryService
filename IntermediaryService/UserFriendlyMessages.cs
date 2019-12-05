using System;
using System.Collections.Generic;
using System.Text;

namespace IntermediaryService
{
    public static class UserFriendlyMessages
    {
        public const string ErrorProcessingBody = "Could Not Process Body of Request";
        public const string UnhandledException = "Something didn't work right. Please Try Again Later";
        public const string ThirdPartyCommunicationFailure = "Failed to communicate with third Party";
        public const string UnexpectedBodyContent = "The body of the request was not the expected string: 'STARTED'";
        public const string DocumentNotFound = "The resource was not found";

    }
}
