using System;

namespace ApprovalTests.Scrubber
{
    public class ScrubberNotRegisteredException : Exception
    {
        public ScrubberNotRegisteredException(string message, Type type) : base(string.Format(message, type))
        {
        }
    }
}