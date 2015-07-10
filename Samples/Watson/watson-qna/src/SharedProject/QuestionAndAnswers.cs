using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Json;
using System.Threading.Tasks;

namespace QandA
{
	public class QuestionAndAnswers
	{
		private readonly String baseURL = "https://gateway.watsonplatform.net/question-and-answer-beta/api/v1/question/travel";
		private readonly String username = "";
		private readonly String password = "";


		public async Task<JsonValue> FetchAnswer(string question)
		{
			
			const string dataset = "travel";
			var questionJson = new JsonObject();
			questionJson.Add("questionText", question);
			var evidenceRequest = new JsonObject();
			evidenceRequest.Add("items", 1);
			questionJson.Add("evidenceRequest", evidenceRequest);
			var postData = new JsonObject();
			postData.Add("question", questionJson);

			string text = postData.ToString();

			HttpWebRequest profileRequest = (HttpWebRequest)WebRequest.Create(baseURL);
			string _auth = string.Format("{0}:{1}", username, password);
			string _enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(_auth));
			string _cred = string.Format("{0} {1}", "Basic", _enc);
			profileRequest.Headers[HttpRequestHeader.Authorization] = _cred;
			profileRequest.Accept = "application/json";
			profileRequest.ContentType = "application/json";
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			profileRequest.Method = "POST";
			profileRequest.ContentLength = bytes.Length;
			using (Stream requeststream = profileRequest.GetRequestStream())
			{
				requeststream.Write(bytes, 0, bytes.Length);
				requeststream.Close();
			}

			// Send the request to the server and wait for the response:
			using (WebResponse response = await profileRequest.GetResponseAsync ())
			{
				// Get a stream representation of the HTTP web response:
				using (Stream stream = response.GetResponseStream ())
				{
					// Use this stream to build a JSON document object:
					JsonValue jsonDoc = await Task.Run (() => JsonObject.Load (stream));
					Console.Out.WriteLine("Response: {0}", jsonDoc.ToString ());

					// Return the JSON document:
					return jsonDoc;
				}
			}
		}

	



	}
}