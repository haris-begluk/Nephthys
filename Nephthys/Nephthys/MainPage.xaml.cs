using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Nephthys
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        OidcClient _client;
        LoginResult _result;
        static string AccessToken;
        public static string BaseAddress = Device.RuntimePlatform == Device.Android ? "https://10.0.2.2:44389" : "https://localhost:44389";
        public static string ApiBaseAddress = Device.RuntimePlatform == Device.Android ? "https://10.0.2.2:44356" : "https://localhost:44356";
        Lazy<HttpClient> _apiClient = new Lazy<HttpClient>(() => new HttpClient());
        public MainPage()
        {
            InitializeComponent();

            Login.Clicked += Login_Clicked;
            CallApi.Clicked += CallApi_Clicked;

            var browser = DependencyService.Get<IBrowser>();

            var options = new OidcClientOptions
            {
                Authority = BaseAddress,
                ClientId = "xamarin.mobile",
                Scope = "openid profile nephthys-api",
                RedirectUri = "xamarinformsclients://callback",
                Browser = browser,

                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect
            };

            options.BackchannelHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (message, certificate, chain, sslPolicyErrors) =>
                {
                    if (certificate.Issuer.Equals("CN=localhost"))
                        return true;
                    return sslPolicyErrors == System.Net.Security.SslPolicyErrors.None;
                }
            };
            _client = new OidcClient(options);
            _apiClient.Value.BaseAddress = new Uri(BaseAddress);
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            _result = await _client.LoginAsync(new LoginRequest());

            if (_result.IsError)
            {
                OutputText.Text = _result.Error;
                return;
            }
            AccessToken = _result?.AccessToken ?? "";
            var sb = new StringBuilder(128);
            foreach (var claim in _result.User.Claims)
            {
                sb.AppendFormat("{0}: {1}\n", claim.Type, claim.Value);
            }

            sb.AppendFormat("\n{0}: {1}\n", "refresh token", _result?.RefreshToken ?? "none");
            sb.AppendFormat("\n{0}: {1}\n", "access token", _result.AccessToken);

            OutputText.Text = sb.ToString();

            _apiClient.Value.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _result?.AccessToken ?? "");
        }

        private async void CallApi_Clicked(object sender, EventArgs e)
        {
            Lazy<HttpClient> _api2Client = new Lazy<HttpClient>(() => new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (message, certificate, chain, sslPolicyErrors) =>
                {
                    if (certificate.Issuer.Equals("CN=localhost"))
                        return true;
                    return sslPolicyErrors == System.Net.Security.SslPolicyErrors.None;
                }
            }));

            _api2Client.Value.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            _api2Client.Value.BaseAddress = new Uri(ApiBaseAddress);

            var result = await _api2Client.Value.GetAsync("/WeatherForecast");

            if (result.IsSuccessStatusCode && !result.StatusCode.Equals(401))
            {
                OutputText.Text = JArray.Parse(await result.Content.ReadAsStringAsync()).ToString();
            }
            else
            {

                if (result.StatusCode == HttpStatusCode.Unauthorized)
                    OutputText.Text = "Unauthorized";
                else
                    OutputText.Text = result.ReasonPhrase;
            }

        }

    }
}
