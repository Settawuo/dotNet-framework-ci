using System;

namespace WBBExternalService.Customization
{
    public class SaveOrderResWasNoValue : Exception
    {
        public SaveOrderResWasNoValue()
        { }

        public SaveOrderResWasNoValue(string message)
            : base(message)
        {
        }

        public SaveOrderResWasNoValue(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}