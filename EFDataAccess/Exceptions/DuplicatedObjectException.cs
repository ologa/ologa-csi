using System;

namespace EFDataAccess.Exceptions
{
    public class DuplicatedObjectException : SystemException
    {
        public DuplicatedObjectException() {}

        public DuplicatedObjectException(string message) : base(message) {}

        public DuplicatedObjectException(string message, SystemException inner) : base(message, inner) {}
    }
}
