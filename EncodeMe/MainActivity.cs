using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/ic_launcher", MainLauncher = true, NoHistory = true,
        Theme = "@style/Theme.Splash", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Activity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Client.Start();
            // Set our view from the "main" layout resource
            //SetContentView(Resource.Layout.Main);

            var students = await Db.GetStudentsAsync();
            if (students.Count == 0)
            {
                var pref = PreferenceManager.GetDefaultSharedPreferences(this);
               //if (pref.GetInt("intro", 1) == 2)
               //     StartActivity(new Intent(Application.Context, typeof(EditStudentActivity)));
               //else
                    StartActivity(new Intent(Application.Context, typeof(StudentIntroActivity)));
            }
        }
    }
}

