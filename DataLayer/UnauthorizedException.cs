using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string text) : base(text)
        {

        }
    }
}
