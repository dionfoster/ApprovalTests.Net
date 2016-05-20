using System;
using System.Collections.Concurrent;

namespace ApprovalTests.Scrubber
{
    public static class ScrubberProvider
    {
        private static readonly ConcurrentDictionary<Type, Func<string, string>> Scrubbers = new ConcurrentDictionary<Type, Func<string, string>>();

        public static void RegisterScrubber<T>(Func<string, string> scrubber)
        {
            Scrubbers.AddOrUpdate(typeof(T), scrubber, (oldval, replacement) => scrubber);
        }

        public static Func<string, string> GetScrubberFor<T>()
        {
            if (!Scrubbers.ContainsKey(typeof(T)))
            {
                throw new ScrubberNotRegisteredException("Cannot find scrubber for type: '{0}'", typeof(T));
            }

            return Scrubbers[typeof (T)];
        }
    }
}