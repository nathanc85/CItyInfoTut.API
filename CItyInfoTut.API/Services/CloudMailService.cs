using System;
using System.Diagnostics;
namespace CItyInfoTut.API.Services
{
    public class CloudMailService: IMailService
    {
		private string _mailFrom = Startup.Configuration["mailSettings:mailFromAddress"];
		private string _mailTo = Startup.Configuration["mailSettings:mailToAddress"];

		public void Send(string subject, string message)
		{
			Debug.WriteLine($"Mail from {_mailFrom} sent to {_mailTo} with CloudMailService.");
			Debug.WriteLine($"Mail subject: {subject}");
			Debug.WriteLine($"Mail message: {message}");
		}

        public CloudMailService()
        {
        }
    }
}
