using System;

namespace Extreal.P2P.Dev
{
    public class HostNameAlreadyExistsException : Exception
    {
        public HostNameAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
