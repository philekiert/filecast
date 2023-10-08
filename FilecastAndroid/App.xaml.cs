using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Drive.v3;



namespace FilecastAndroid
{
    public partial class App : Application
    {
        

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        public void AuthenticateGoogleDrive()
        {

        }
    }
}