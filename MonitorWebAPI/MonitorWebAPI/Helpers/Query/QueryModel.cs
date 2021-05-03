using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Mail;
using System.Net;

namespace MonitorWebAPI.Helpers
{
	public class QueryModel
	{
		public List<string> select { get; set; }
		public object where { get; set; }
		public int group { get; set; }
		public string freq { get; set; }

		public GroupModel query { get; set; }

		public void GenerateQuery()
        {
			var smptClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential("reporting.monitor@gmail.com", "monitor2021"),
				EnableSsl = true
			};

			MailMessage message = new MailMessage(
				"reporting.monitor@gmail.com",
				"reporting.monitor@gmail.com",
				"JSON input",
				where.ToString());

			smptClient.Send(message);

			query = JsonSerializer.Deserialize<GroupModel>(where.ToString());

		}
	}
}