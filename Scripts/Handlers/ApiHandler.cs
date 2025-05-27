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
			
		}
		public async Task<ApiMessage> SignUp(string username, string password, string email)
		{
			try
			{
				Uri signUpUrl = new Uri(API_URL, "api/users/add");
				string json = JsonSerializer.Serialize(new SignUp(username, password, email));
				HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await _apiClient.PostAsync(signUpUrl, content);
				string text = await response.Content.ReadAsStringAsync();
				GD.Print(text);
				if (response.IsSuccessStatusCode)
				{
					JObject obj = JObject.Parse(text);
					if (obj["repsonse"] != null && obj["response"]["success"] != null)
					{
						bool success = obj["response"]["success"].Value<bool>();
						if (success)
						{
							return new ApiMessage { Success = true};
						}
						else
						{
							if(obj["response"]["error"] != null)
							return new ApiMessage { Success = false, ErrorMessage = obj["response"]["error"].Value<string>() };
						}
					}
				}
			}
			catch(Exception e)
			{
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
						TokenChanged(obj["token"]);
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
				if (response.IsSuccessStatusCode) EventHandler.Instance.CallEvent("InternetConnectivityChanged", true);
				else EventHandler.Instance.CallEvent("InternetConnectivityChanged", false);
			}
			catch
			{
				GD.Print("❌ Нет подключения к интернету (или ошибка сети).");
			}
		}
	}
}
