using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
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

            var stud = (await Db.GetAll<Student>()).FirstOrDefault();
            if(stud==null)
                StartActivity(new Intent(Application.Context, typeof(StudentIntroActivity)));
            else
                StartActivity(new Intent(Application.Context, typeof(SubjectsActivity)));
            Finish();
        }
    }
}

