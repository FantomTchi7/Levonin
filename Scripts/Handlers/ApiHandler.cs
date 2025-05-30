using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Godot;
using Levonin.Scripts.Models.API;
using System.Net.Http.Json;
using System.ComponentModel;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Levonin.Scripts.Models;
using System.Net.Http.Headers;

namespace Levonin.Scripts.Handlers
{
	public partial class ApiHandler: Node
	{
		public static ApiHandler Instance { get; private set; }
		public readonly Uri API_URL = new Uri("http://localhost:3000");
		private readonly System.Net.Http.HttpClient _apiClient = new();
		public override void _Ready()
		{
			this._apiClient.DefaultRequestHeaders.Add("x-api-key", "celuj_menja_v_lico");
			EventHandler.Instance.AddListener("TokenChanged", TokenChanged);
			GD.Print("GG WP");
			CheckInternet();
			if(Instance != null && Instance != this)
			{
				QueueFree();
				return;
			}
			Instance = this;

			EventHandler.Instance.AddListener("loggedIn", param =>
			{
				if (param is bool logged) if (!logged) TokenChanged("");
			});
			
		}
		public async Task<ApiMessage> GetChannels()
		{
			ApiMessage apiMessage = new ApiMessage() { Success = false };
			try
			{
				GD.Print("OUUUUUUUUUUU GADAMMMMMMMMNDFGSFGDSF");
				Uri channelsUrl = new Uri(API_URL, "/api/users/channels");
				HttpResponseMessage response = await _apiClient.GetAsync(channelsUrl);
				JObject obj = JObject.Parse(await response.Content.ReadAsStringAsync());
				if (response.IsSuccessStatusCode)
				{
					GD.Print("success ???");
					if (obj["channels"] != null)
					{
						GD.Print("channels not null ???");
						List<Channel> channels = obj["channels"].ToObject<List<Channel>>();
						GD.Print("godamnn ???");
						if(channels != null)
						{
							apiMessage.Success = true;
							apiMessage.Response = channels;
							GD.Print("here we are ???");
						}
					}
					else if (obj["error"] != null)
					{
						GD.Print("error ???");
						apiMessage.ErrorMessage = obj["error"].ToString();
					}
				}
				else
				{
					GD.Print("error!@!!");
					GD.Print(await response.Content.ReadAsStringAsync());
				}

			}
			catch (Exception e)
			{
				GD.Print(e.ToString());
				apiMessage.Success = false;
				apiMessage.ErrorMessage = "Server issue.";
			}
			return apiMessage;
		}
		public async Task<ApiMessage> SignUp(string username, string password, string email)
		{
			try
			{
				Uri signUpUrl = new Uri(API_URL, "api/users/add");
				string json = JsonSerializer.Serialize(new Models.API.SignUp(username, password, email));
				HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await _apiClient.PostAsync(signUpUrl, content);
				string text = await response.Content.ReadAsStringAsync();
				GD.Print(text);
				if (response.IsSuccessStatusCode)
				{
					JObject obj = JObject.Parse(text);
					if (obj["response"]["success"].Value<bool?>().HasValue)
					{
						bool success = obj["response"]["success"].Value<bool>();
						if (success)
						{
							GD.Print("SUCCESS [API HANDLER - SIGNUP ]");
							return new ApiMessage { Success = true };
						}
						else
						{
							GD.Print("FAIL [API HANDLER - SIGNUP]");
							if(obj["response"]["error"] != null)
							return new ApiMessage { Success = false, ErrorMessage = obj["response"]["error"].Value<string>() };
						}
					}
					GD.Print("RESPONSE OR SUCCESS KEY IS NULL [API HANDLER SIGNUP]");
				}
			}
			catch(Exception e)
			{
				GD.Print("Exception has been catched");
				GD.Print(e.ToString());
			}
			return new ApiMessage { Success = false };
		}
		
		public async Task<ApiMessage> Authorize(string username, string password)
		{
			try
			{
				Uri authorizeUrl = new Uri(API_URL, "api/authorize");
				GD.Print("Authorizing");
				string json = JsonSerializer.Serialize(new Authorize(username, password));

				HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await _apiClient.PostAsync(authorizeUrl, content);
				string text = await response.Content.ReadAsStringAsync();
				GD.Print(text);
				if (response.IsSuccessStatusCode)
				{
					GD.Print("response is success");
					JObject obj = JObject.Parse(text);
					if (obj["token"] != null)
					{
						GD.Print("token is not null");
						_apiClient.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("Bearer", obj["token"].ToString());
						return new ApiMessage { Success = true };
					}
					else if (obj["error"] != null)
					{
						GD.Print(obj["error"]);
						return new ApiMessage { Success = false, ErrorMessage = "Invalid data" };
					}
				}
			}
			catch(Exception e)
			{
				GD.Print(e.ToString());
			}
			return new ApiMessage { Success = false, ErrorMessage = "Error" };
		}

		private void TokenChanged(object param)
		{
			if (param is string token)
			{
				_apiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
			}
		}
	 
		public async void CheckInternet()
		{
			try
			{
				using var client = new System.Net.Http.HttpClient
				{
					Timeout = TimeSpan.FromSeconds(3)
				};

				HttpResponseMessage response = await client.GetAsync(API_URL);
				if (response.IsSuccessStatusCode)
				{
					EventHandler.Instance.CallEvent("internetActivityChanged", true);
					EventHandler.Instance.CallEvent("successNotification", "Подключение успешно!");
				}
				else
				{
					EventHandler.Instance.CallEvent("internetActivityChanged", false);
					EventHandler.Instance.CallEvent("successNotification", "Нет подключения к интернету (или ошибка сети).");
				}
			}
			catch
			{
				GD.Print("❌ Нет подключения к интернету (или ошибка сети).");
			}
		}
	}
}
