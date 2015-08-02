using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WatsonLanguageTranslation
{
	public partial class StartPage : ContentPage
	{
		public StartPage ()
		{
			InitializeComponent ();
            this.btnTranslate.Clicked += btnTranslate_Clicked;
            GetSupportedLanguages();
           
		}
       
        private readonly string url = "https://gateway.watsonplatform.net/language-translation/api/v2/translate?"; 
        //Create Bluemix account for Username/password
        private readonly string username = "d5d54445-2bd8-4951-84b3-3442c70175eb";
        private readonly string password = "5ScEAqjHNMum";
        async void btnTranslate_Clicked(object sender, EventArgs e)
        {
            //https://gateway.watsonplatform.net/language-translation/api/v2/identifiable_languages
            StringBuilder builder = new StringBuilder();
            builder.Append(url).Append("source=").Append(SupportedLanguages.languages[cboFrom.SelectedIndex].language).
                Append("&target=").Append(SupportedLanguages.languages[cboTo.SelectedIndex].language).
                Append("&text=")
                .Append(txtConversionString.Text);
            WebClient client = new WebClient();
            client.Headers["Authorization"] = " Basic " + Convert.ToBase64String(
                        System.Text.Encoding.UTF8.GetBytes(
                            string.Format("{0}:{1}", username, password)));
            client.DownloadStringCompleted += client_DownloadStringCompleted;
            string UrlString = builder.ToString();
            client.DownloadStringAsync(new Uri(UrlString, UriKind.Absolute));

           
        }
        RootObject SupportedLanguages = new RootObject();
        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var data = e.Result.ToString();
            txtResult.Text = data; 
        }
        public void GetSupportedLanguages()
        {
            WebClient client = new WebClient();
            client.Headers["Authorization"] = " Basic " + Convert.ToBase64String(
                        System.Text.Encoding.UTF8.GetBytes(
                            string.Format("{0}:{1}", username, password)));
            
            client.DownloadStringCompleted +=client_DownloadLanguageStringCompleted;
            client.DownloadStringAsync(new Uri("https://gateway.watsonplatform.net/language-translation/api/v2/identifiable_languages", UriKind.Absolute));        

        }
        void client_DownloadLanguageStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var jsonText= e.Result.ToString();
            SupportedLanguages = JsonConvert.DeserializeObject<RootObject>(jsonText);
            foreach (var item in SupportedLanguages.languages)
            {
                cboFrom.Items.Add(item.language);
                cboTo.Items.Add(item.language);
            }
            cboFrom.Title = "language";
        }
        public class Language
        {
            public string language { get; set; }
            public string name { get; set; }
        }

        public class RootObject
        {
            public List<Language> languages { get; set; }
        }
	}
}
