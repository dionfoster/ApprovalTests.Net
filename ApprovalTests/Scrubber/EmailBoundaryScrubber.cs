using System.Collections.Generic;
using System.Linq;
using ApprovalUtilities.Utilities;

namespace ApprovalTests.Scrubber
{
    public static class EmailBoundaryScrubber
    {
        private static string[] FindBoundaries(string emailText)
        {
            var startPoint = 0;
            var boundaries = new HashSet<string>();

            while ((startPoint = emailText.IndexOf("boundary=--", startPoint)) != -1)
            {
                var preamble = "boundary=--boundary_0_".Length;
                var guid = "7ddc4a25-b0f6-44d4-bcb0-03f577170c19".Length;

                boundaries.Add(emailText.Substring(preamble + startPoint, guid));

                startPoint++;
            }

            return boundaries.ToArray();
        }

        public static string ScrubBoundaries(string emailText)
        {
            var boundaries = FindBoundaries(emailText);

            emailText = ScrubBoundaries(emailText, boundaries);

            return emailText;
        }

        private static string ScrubBoundaries(string emailText, string[] boundaries)
        {
            var count = 0;
            var guid = "--boundary_{0}_00000000-0000-0000-0000-00000000000{0}";

            foreach (var b in boundaries)
            {
                emailText = emailText.Replace(b, guid.FormatWith(count++));
            }

            return emailText;
        }
    }
}