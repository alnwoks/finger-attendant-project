using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;





namespace Finger_ATM
{

    public partial class index : Form
    {

        public index()
        {
            InitializeComponent();
        }
        // Enter your API key here.
        // Get an API key by making a free account at:
        //      http://home.openweathermap.org/users/sign_in
        private const string API_KEY = "b13399d144051bb578929282dccb62a6";

        // Query URLs. Replace @LOC@ with the location.
        private const string CurrentUrl =
            "https://newsapi.org/v2/everything?" +
            "@QUERY@=lagos&mode=xml&units=imperial&APPID=" + API_KEY;
        private const string ForecastUrl =
            "http://api.openweathermap.org/data/2.5/forecast?" +
            "@QUERY@=lagos&mode=xml&units=imperial&APPID=" + API_KEY;

        // Query codes.
        private string[] QueryCodes = { "q", "zip", "id", };

        // Fill in query types. These should match the QueryCodes.

        private void index_Load(object sender, EventArgs e)
        {
            cboQuery.Items.Add("City");
            cboQuery.SelectedIndex = 0;
            comboBox1.SelectedText = "--select--";
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Compose the query URL.
            string url = ForecastUrl.Replace("@LOC@", label2.Text);
            url = url.Replace("@QUERY@", QueryCodes[cboQuery.SelectedIndex]);

            // Create a web client.
            using (WebClient client = new WebClient())
            {
                // Get the response string from the URL.
                try
                {
                    DisplayForecast(client.DownloadString(url));
                }
                catch (WebException ex)
                {
                    DisplayError(ex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unknown error\n" + ex.Message);
                }
            }
        }
        // Display the forecast.
        private void DisplayForecast(string xml)
        {
            // Load the response into an XML document.
            XmlDocument xml_doc = new XmlDocument();
            xml_doc.LoadXml(xml);

            // Get the city, country, latitude, and longitude.
            XmlNode loc_node = xml_doc.SelectSingleNode("weatherdata/location");
            label5.Text = loc_node.SelectSingleNode("name").InnerText;
            label9.Text = loc_node.SelectSingleNode("country").InnerText;
            XmlNode geo_node = loc_node.SelectSingleNode("location");
             label2.Text = geo_node.Attributes["latitude"].Value;
            label10.Text = geo_node.Attributes["longitude"].Value;
            label13.Text = geo_node.Attributes["geobaseid"].Value;
          
            lvwForecast.Items.Clear();
            char degrees = (char)176;

            foreach (XmlNode time_node in xml_doc.SelectNodes("//time"))
            {
                // Get the time in UTC.
                DateTime time =
                    DateTime.Parse(time_node.Attributes["from"].Value,
                        null, DateTimeStyles.AssumeUniversal);

                // Get the temperature.
                XmlNode temp_node = time_node.SelectSingleNode("temperature");
                string temp = temp_node.Attributes["value"].Value;

               ListViewItem item =   lvwForecast.Items.Add(time.DayOfWeek.ToString());
               item.SubItems.Add(time.ToShortTimeString());
                item.SubItems.Add(temp + degrees);
            }
        }

        // Display an error message.
        private void DisplayError(WebException exception)
        {
            try
            {
                StreamReader reader = new StreamReader(exception.Response.GetResponseStream());
                XmlDocument response_doc = new XmlDocument();
                response_doc.LoadXml(reader.ReadToEnd());
                XmlNode message_node = response_doc.SelectSingleNode("//message");
                MessageBox.Show(message_node.InnerText);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unknown error\n" + ex.Message);
            }
        }

        private void LOGIN_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminLogin am = new AdminLogin();
            am.Show();
        }

       
        private void button2_Click(object sender, EventArgs e)
        {
            

            string baseurl = "https://newsapi.org/v2/";
            string id = Convert.ToString(comboBox1.SelectedItem);
            WebClient client = new WebClient();
            string pagecode = client.DownloadString(baseurl + "top-headlines?country=NG&apiKey=ca601829d3c74e8591e1edaa5f586867&category=" + id);

            JObject googleSearch = JObject.Parse(pagecode);

            // get JSON result objects into a list
            IList<JToken> results = googleSearch["articles"].Children().ToList();

            // serialize JSON results into .NET objects
            IList<SearchResult> searchResults = new List<SearchResult>();
            foreach (JToken result in results)
            {
                SearchResult searchResult = JsonConvert.DeserializeObject<SearchResult>(result.ToString());
                searchResults.Add(searchResult);
            }
            String data = "";
            // List the properties of the searchResults IList
            foreach (SearchResult item in searchResults)
            {
                data+= "Title=  " + item.title+ Environment.NewLine;
                data += "Author=  " + item.author + Environment.NewLine;
                data += "Content=  " + item.content + Environment.NewLine+Environment.NewLine+Environment.NewLine;
               

                //richTextBox1.AppendText(Environment.NewLine + "Author=  " + item.author);
                //richTextBox1.AppendText(Environment.NewLine + "Url=  " + item.url);
                //richTextBox1.AppendText(Environment.NewLine + "Content=  " + item.content);
                //richTextBox1.AppendText(Environment.NewLine + "Description=  " + item.description);


            }
            
            richTextBox1.Text =data;


        }

        
    }

    

}

