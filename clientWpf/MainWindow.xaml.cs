using IdentityModel.Client;
using System.Net.Http;
using System.Windows;

namespace wfpClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _accessToken;
        private DiscoveryDocumentResponse _disco;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnRequestToken_Click(object sender, RoutedEventArgs e)
        {
            // discovery endpoint
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5001");
            if (disco.IsError)
            {
                MessageBox.Show(disco.Error);
                return;
            }
            _disco = disco;
            // request response
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "wpfClient",
                ClientSecret = "secret",
                Scope = "scope1 openid profile address phone email",

                UserName = txtUser.Text,
                Password=txtPass.Text
            }) ;
            if (tokenResponse.IsError)
            {
                MessageBox.Show(tokenResponse.Error);
                return;
            }
            _accessToken = tokenResponse.AccessToken;
            txtToken.Text = tokenResponse.Json.ToString();
        }
        // 请求API资源
        private async void AccessApi_Click(object sender, RoutedEventArgs e)
        {
            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(_accessToken);
            //var response = await apiClient.GetAsync(disco.UserInfoEndpoint);
            var response = await apiClient.GetAsync("http://localhost:5002/identity");
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.StatusCode.ToString());
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                txtApiStr.Text = content;
            }
        }
        // 请求IdentityServer资源
        private async void btnRequestIdtResource_Click(object sender, RoutedEventArgs e)
        {
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(_accessToken);
            var response = await apiClient.GetAsync(_disco.UserInfoEndpoint);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.StatusCode.ToString());
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
               txtIdtStr.Text = content;
            }
        }
    }
}
