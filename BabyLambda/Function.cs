using System;
using System.Collections.Generic;
using System.Globalization;
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
			// Time zone tomfoolery here is to deal with the fact that the server time zone is unknown; as long as
			// we use proper offset-qualified values everywhere, it works out correctly.
			var birthdayExact = new DateTimeOffset(2018, 2, 4, 14, 4, 0, TimeSpan.FromHours(-5));
			var birthdayRound = new DateTimeOffset(2018, 2, 4, 0, 0, 0, TimeSpan.Zero);
			var nowExact = DateTimeOffset.UtcNow;
			var nowRound = new DateTimeOffset(nowExact.Year, nowExact.Month, nowExact.Day, 0, 0, 0, TimeSpan.Zero);
			var deltaExact = nowExact - birthdayExact;
			var deltaRound = nowRound - birthdayRound;

			// How many months old? This is going by calendar dates, so Feb 4 -> March 4 is one month,
			// and Feb 4 -> April 4 is two months. Since months are not all the same length, it's not
			// so easy to calculate.
			int months = (nowRound.Year - birthdayRound.Year) * 12
						+ (nowRound.Month - birthdayRound.Month);
			if (nowRound.Day < birthdayRound.Day)
				months -= 1;

			// Create the full version.
			var sb = new System.Text.StringBuilder();
			sb.Append("<html><div align='center' style='background-color: #cccccc'>");
			sb.Append("<br/>");
			sb.Append("Arthur Elliott Robinson-Nevill's age is currently:");
			sb.Append("<br/>");
			sb.Append("<br/>");
			sb.AppendFormat("{0} seconds", (int)deltaExact.TotalSeconds); sb.Append("<br/>");
			sb.AppendFormat("{0} hours", (int)deltaExact.TotalHours); sb.Append("<br/>");
			sb.AppendFormat("{0} earth days", (int)deltaRound.TotalDays); sb.Append("<br/>");
			sb.AppendFormat("{0} martian days", (int)(deltaRound.TotalMinutes / 1477)); sb.Append("<br/>");
			sb.AppendFormat("{0} weeks", (int)(deltaRound.TotalDays / 7)); sb.Append("<br/>");
			sb.AppendFormat("{0:0.0} fortnights", deltaRound.TotalDays / 14); sb.Append("<br/>");
			sb.AppendFormat("{0:0.0} lunar months (synodic)", deltaRound.TotalDays / 29.53); sb.Append("<br/>");
			sb.AppendFormat("{0:0} earth months", months); sb.Append("<br/>");
			sb.AppendFormat("{0:0.00} years", deltaRound.TotalDays / 365); sb.Append("<br/>");
			sb.Append("<br/>");
			sb.AppendFormat("Server time: {0:o}", nowExact); sb.Append("<br/>");
			sb.Append("</div></html>");

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

