using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace BabyLambda
{
	public class Function
	{

		/// <summary>
		/// A simple function that takes a string and does a ToUpper
		/// </summary>
		/// <param name="input"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public string FunctionHandler(JObject input, ILambdaContext context)
		{
			var birthday = new DateTimeOffset(2018, 2, 4, 14, 4, 0, TimeSpan.FromHours(-5));
			var now = DateTimeOffset.Now;
			var delta = now - birthday;

			var sb = new System.Text.StringBuilder();
			sb.Append("<html><body><div align='center'>");
			sb.Append("Arthur Elliott Robinson-Nevill's age is currently:");
			sb.Append("<br/>");
			sb.Append("<br/>");
			sb.AppendFormat("{0} seconds", (int)delta.TotalSeconds); sb.Append("<br/>");
			sb.AppendFormat("{0} hours", (int)delta.TotalHours); sb.Append("<br/>");
			sb.AppendFormat("{0} days", (int)delta.TotalDays); sb.Append("<br/>");
			sb.AppendFormat("{0:0.0} fortnights", delta.TotalDays / 14); sb.Append("<br/>");
			sb.AppendFormat("{0} months", (int)(delta.TotalDays / 30)); sb.Append("<br/>");
			sb.Append("<br/>");
			sb.Append("</div></body></html>");

			SendEmail("Arthur date check!", sb.ToString());
			context.Logger.LogLine("yep, it ran");
			return "done";
		}

		private static void SendEmail( string subject, string body )
		{
			MailMessage msg = new MailMessage();
			msg.From = new MailAddress("charles.nevill@gmail.com");
			msg.To.Add("charles.nevill@gmail.com");
			msg.Subject = subject;
			msg.Body = body;
			msg.IsBodyHtml = true;
			SmtpClient client = new SmtpClient("email-smtp.us-east-1.amazonaws.com");
			client.Credentials = new System.Net.NetworkCredential("AKIAJWRDK23TASKCF2FA", "AvdVDUuV3h5HNt1IeARGjSl8Ex1627c/1bVFS5vJSSLm");
			client.EnableSsl = true;
			client.Send(msg);
		}
	}
}

