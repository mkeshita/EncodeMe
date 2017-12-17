using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;

namespace NORSU.EncodeMe
{
    [Activity(Label = "@string/app_name",Icon = "@drawable/ic_launcher", Theme = "@style/Theme.FullScreen",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class EditStudentActivity : Activity
    {
        private ProgressBar _progressBar;
        private EditText _firstname;
        private EditText _lastname;
        private EditText _course;
        private Button _nextButton;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EditStudent);

            _progressBar = FindViewById<ProgressBar>(Resource.Id.Progress);
            _firstname = FindViewById<EditText>(Resource.Id.FirstName);
            _lastname = FindViewById<EditText>(Resource.Id.LastName);
            _course = FindViewById<EditText>(Resource.Id.Course);
            _nextButton = FindViewById<Button>(Resource.Id.NextButton);
            
        }

        private void BackButtonOnClick(object sender, EventArgs eventArgs)
        {
            OnBackPressed();
        }
    }
}