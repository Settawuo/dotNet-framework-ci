using System;

namespace WBBExternalService.Customization
{
    public class MainPackageNotFoundException : Exception
    {
        public MainPackageNotFoundException()
        { }

        public MainPackageNotFoundException(string message)
            : base(message)
        {
        }

        public MainPackageNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}