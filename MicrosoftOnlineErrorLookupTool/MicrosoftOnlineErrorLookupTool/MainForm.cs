using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MicrosoftOnlineErrorLookupTool
{
    public partial class MainForm : Form
    {
        private const string ErrorUrl = "https://login.microsoftonline.com/error?code=";

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string errorCode = textBoxErrorCodeInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(errorCode))
            {
                MessageBox.Show(@"Please enter an error code to lookup", @"Error - missing error code", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string errorPageUrl = ErrorUrl + WebUtility.UrlEncode(errorCode);

            try
            {
                using (WebClient client = new WebClient())
                {
                    string htmlCode = client.DownloadString(errorPageUrl);
                    string[] errorDetails = ParseErrorDetails(htmlCode);

                    // Set text boxes with returned values
                    //textBoxErrorCodeDetails.Text = errorDetails[0];
                    textBoxMessageDetails.Text = errorDetails[1];
                    textBoxRemediationDetails.Text = errorDetails[2];
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show($@"Error: {ex.Message}", @"Web Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string[] ParseErrorDetails(string html)
        {
            string errorCode = "", message = "", remediation = "";

            // Define regular expressions to match the error code, message, and remediation
            Regex errorCodeRegex = new Regex(@"<td>Error Code<\/td>\s*<td>(.*?)<\/td>");
            Regex messageRegex = new Regex(@"<td>Message<\/td>\s*<td>(.*?)<\/td>");
            Regex remediationRegex = new Regex(@"<td>Remediation<\/td>\s*<td>(.*?)<\/td>");

            // Match the regular expressions in the HTML content
            Match errorCodeMatch = errorCodeRegex.Match(html);
            if (errorCodeMatch.Success)
            {
                errorCode = errorCodeMatch.Groups[1].Value.Trim();
            }

            Match messageMatch = messageRegex.Match(html);
            if (messageMatch.Success)
            {
                message = messageMatch.Groups[1].Value.Trim();
            }

            Match remediationMatch = remediationRegex.Match(html);
            if (remediationMatch.Success)
            {
                remediation = remediationMatch.Groups[1].Value.Trim();
            }

            return new string[] { errorCode, message, remediation };
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}