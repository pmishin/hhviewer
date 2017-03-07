using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using hhModel;

namespace Test.Model
{
	public class VacancyModel
	{
		private string _description;
		public string Id { get; set; }
		public string Alternate_Url { get; set; }
		public string Description
		{
			get { return _description; }
			set { _description = StripHtmlTagsUsingRegex(value); }
		}
		public string Name { get; set; }
		public Experience Experience { get; set; }
		public Area Area { get; set; }
		public Salary Salary { get; set; }
		public Schedule Schedule { get; set; }
		static string StripHtmlTagsUsingRegex(string inputString)
		{
			return Regex.Replace(inputString, @"<[^>]*>", String.Empty).Replace("&quot;", "\"").Trim();
		}

		public static List<string> GetVacancyList()
		{
			List<string> items = new List<string>();
			using (WebClient webClient = new WebClient())
			{
				webClient.Encoding = Encoding.UTF8;
				webClient.Headers.Add(HttpRequestHeader.UserAgent, "test-app");
				string json = webClient.DownloadString(ConfigurationManager.AppSettings["RequestURL"]);
				JsonConvert.DeserializeObject<RootObject>(json).items.ForEach(o => items.Add(o.id));

			}
			return items;
		}

		public static async Task<List<VacancyModel>> GetVacancyInfoAsync(List<string> urlList)
		{
			return await Task<List<VacancyModel>>.Factory.StartNew(() =>
			{
				List<VacancyModel> resList = new List<VacancyModel>();
				List<string> temp = new List<string>();
				foreach (var item in urlList)
				{
					using (WebClient wc = new WebClient())
					{
						wc.Encoding = Encoding.UTF8;
						wc.Headers.Add(HttpRequestHeader.UserAgent, "test-app");
						temp.Add(wc.DownloadString(new Uri("https://api.hh.ru/vacancies/" + item)));
					}
				}
				temp.ForEach(o => resList.Add(JsonConvert.DeserializeObject<VacancyModel>(o)));
				return resList;
			});
		}

	}

	public class Salary
	{
		public int _from { get; set; }
		public int _to { get; set; }
		public int? From { get { return _from; } set { _from = value ?? 0; } }
		public int? To { get { return _to; } set { _to = value ?? 0; } }
		public string TotalSalary { get
		{
			return From != 0 && To != 0 ? $"ОТ {From} ДО {To} {Currency}":"Не указано";
		
		} }
		public string Currency { get; set; }
	}
	public class Schedule
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}
	public class Experience
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}
	public class Area
	{
		public string Url { get; set; }
		public string Id { get; set; }
		public string Name { get; set; }
	}
}

