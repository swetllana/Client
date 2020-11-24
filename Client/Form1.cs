using System;
using System.IO;
using System.IO.Compression;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Client
{
    public partial class frmClient : Form
    {
        string address = "127.0.0.1";
        // SSL server port
        int port = 2222;
        // Create and prepare a new SSL client context
        SslContext context = new SslContext();
        // Create a new SSL chat client
        ChatClient client = new ChatClient();

        public frmClient()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            context = new SslContext(SslProtocols.Tls12, new X509Certificate2("client.pfx", "qwerty"), (senderr, certificate, chain, sslPolicyErrors) => true);
            client = new ChatClient(context, address, port);

            // Connect the client
            label1.Text = "Client connecting...";
            client.ConnectAsync();
            label1.Text = "Done!";
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Data data = new Data();
            data.Name = txtName.Text;
            data.Surname = txtSurname.Text;
            data.Town = txtTown.Text;
            data.PostalCode = txtPostalCode.Text;
            data.ProgramVersion = txtProgramVersion.Text;
            data.Email = txtEmail.Text;
            data.Music = txtMusic.Text;
            data.Singer = txtSinger.Text;
            data.Year = txtYear.Text;
            data.Hour = txtHour.Text;

            //serialize info
            string jsonString = JsonConvert.SerializeObject(data);

            //gzip
            var message = "";
            byte[] inputBytes = Encoding.UTF8.GetBytes(jsonString);
            using (var outputStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
                    gZipStream.Write(inputBytes, 0, inputBytes.Length);
                var outputBytes = outputStream.ToArray();
                var outputbase64 = Convert.ToBase64String(outputBytes);
                message = outputbase64;
            }

            client.SendAsync(message);
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            // Disconnect the client
            label1.Text = "Client disconnecting...";
            client.DisconnectAndStop();
            label1.Text = "Done!";
        }

        private void frmClient_Load(object sender, EventArgs e)
        {

        }
    }
}
