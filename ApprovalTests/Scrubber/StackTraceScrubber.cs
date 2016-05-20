using System;
using System.Text.RegularExpressions;

namespace ApprovalTests.Scrubber
{
	public static class StackTraceScrubber
	{
        static Regex windowsPathRegex = new Regex(@"\b\w:[\\\w.\s-]+\\");
        static Regex unixPathRegex = new Regex(@"\/[\/\w.\s-]+\/");

        public static string ScrubAnonymousIds(string source)
		{
			var regex = new Regex(@"\w+__\w+");
			return regex.Replace(source, string.Empty);
		}

		public static string ScrubLineNumbers(string source)
		{
			var regex = new Regex(@":line \d+");
			return regex.Replace(source, string.Empty);
		}

		public static string ScrubPaths(string source)
		{
			var result = windowsPathRegex.Replace(source, "...\\");
			result = unixPathRegex.Replace(result, ".../");
			return result;
		}
		
		public static string ScrubStackTrace(this string text)
		{
			return ScrubberUtils.Combine(ScrubAnonymousIds, ScrubPaths, ScrubLineNumbers)(text);
		}

		public static string Scrub(this Exception exception)
		{
			return ("" + exception).ScrubStackTrace();
		}
	}
}