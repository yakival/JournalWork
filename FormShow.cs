using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JournalWork
{
    public partial class FormShow : Form
    {
        public class SecurityQuestion
        {
            public string name { get; set; }
            public string email { get; set; }
            public string password { get; set; }
            public string state { get; set; }
        }

        public FormShow()
        {
            InitializeComponent();

            var client = new RestClient("http://localhost:8000/api/userinfo/check");
            var request = new RestRequest(Method.POST);
            //request.AddHeader("X-Token-Key", "dsds-sdsdsds-swrwerfd-dfdfd");
            request.AddParameter("application/json",
                "{ \"name\": \"yakival\", \"password\": \"615350\" }", // <- your JSON string
                ParameterType.RequestBody);
            var response = client.Execute<SecurityQuestion>(request);
            SecurityQuestion content = response.Data; // raw content as string
            //var json = JsonConvert.DeserializeObject(content);
            label1.Text = "yakival";
            //JObject customerObjJson = jsonData.CustomerObj;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
