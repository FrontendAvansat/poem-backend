using System;

namespace DataLayer
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string text): base(text)
        {

        }
    }
}
