using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using ApprovalTests.Scrubber;
using ApprovalTests.Writers;

namespace ApprovalTests.Email
{
    public class EmailApprovals
    {
        static EmailApprovals()
        {
            ScrubberProvider.RegisterScrubber<MailMessage>(EmailBoundaryScrubber.ScrubBoundaries);
        }

        public static void Verify(MailMessage email)
        {
            VerifyScrubbed(email, ScrubberProvider.GetScrubberFor<MailMessage>());
        }

        public static void VerifyScrubbed(MailMessage email, params Func<string, string>[] scrubbers)
        {
            var emailText = CreateEmail(email);

            foreach (var scrubber in scrubbers)
            {
                emailText = scrubber.Invoke(emailText);
            }

            Approvals.Verify(WriterFactory.CreateTextWriter(emailText, "eml"));
        }

        public static string CreateEmail(MailMessage email)
        {
            var tempdir = Path.GetTempFileName();

            File.Delete(tempdir);

            Directory.CreateDirectory(tempdir);

            var client = new SmtpClient("doesntmatter")
            {
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = tempdir
            };

            client.Send(email);

            var emailText = ReadFileWhereLines(GetLatestFile(client.PickupDirectoryLocation), l => !l.StartsWith("Date"));

            return emailText;
        }

        public static string ReadFileWhereLines(string latestFile, Func<string, bool> predicate)
        {
            var latestFileLines = File.ReadAllLines(latestFile).Where(predicate).ToArray();
            var newText = string.Join(Environment.NewLine, latestFileLines);

            return newText;
        }

        public static string GetLatestFile(string dir)
        {
            return new DirectoryInfo(dir)
                .GetFiles("*.eml")
                .OrderBy(f => f.CreationTime)
                .Last()
                .FullName;
        }
    }
}