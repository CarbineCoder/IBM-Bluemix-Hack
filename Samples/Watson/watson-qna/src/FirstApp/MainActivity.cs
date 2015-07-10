using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Json;
using QandA;
using System.Threading.Tasks;
using System.Net.Http;
using ModernHttpClient;
using System.Net;
using System.Text;

namespace FirstApp
{
	[Activity (Label = "Ask Watson", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		//Along with Healthcare you can ask questions on "Travel". Replace healthcare with travel in the URL
		private readonly String baseURL = "https://gateway.watsonplatform.net/question-and-answer-beta/api/v1/question/healthcare";
		//Create Bluemix account for Username/password
		private readonly String username = "";
		private readonly String password = "";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			var question=FindViewById<EditText>(Resource.Id.questionText);
		
			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.fetchAnswerButton);
			
			button.Click += async (sender, e) =>{
				var QnA=new QuestionAndAnswers();
				JsonValue json =await AnswerHttp(question.Text);//QnA.FetchAnswer (question.Text);
				ParseAndDisplay(json);
			};
		}

		private void ParseAndDisplay (JsonValue json)
		{
			
			TextView answerTextView =FindViewById<TextView>(Resource.Id.answerText);
			answerTextView.MovementMethod = new Android.Text.Method.ScrollingMovementMethod();

		
			JsonArray evidenceResults =(JsonArray) json;

			foreach (var item in evidenceResults) {
				var result = item ["question"] ["evidencelist"] [0] ["text"];
				answerTextView.Text = result != null ? result.ToString () : string.Empty;
			}


		}

		//Using ModernHTTPClient
		private async Task<JsonValue> AnswerHttp(string question)
		{
			JsonValue jsonDoc = null;
			const string dataset = "travel";
			var questionJson = new JsonObject();
			questionJson.Add("questionText", question);
			var evidenceRequest = new JsonObject();
			evidenceRequest.Add("items", 1);
			questionJson.Add("evidenceRequest", evidenceRequest);
			var postData = new JsonObject();
			postData.Add("question", questionJson);

			string text = postData.ToString();
			var _nativehandler = new NativeMessageHandler ();

			using (var content = new StringContent(text, Encoding.UTF8, "application/json"))
			using (var client = new HttpClient(_nativehandler))
			{
			
				client.DefaultRequestHeaders.Authorization=new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", 
					Convert.ToBase64String(
						System.Text.ASCIIEncoding.ASCII.GetBytes(
							string.Format("{0}:{1}", username,password))));
				client.DefaultRequestHeaders.Accept.Add (new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue ("application/json"));
				var response = await client.PostAsync(baseURL, content).ConfigureAwait(false);
				if (response.IsSuccessStatusCode)
				{
					var jsonOut = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					jsonDoc = await Task.Run (() => JsonObject.Parse (jsonOut));
				}
			}

			return jsonDoc;
		}
	}
}


