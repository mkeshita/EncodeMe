using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using NORSU.EncodeMe.Network;

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
        private string StudentId;

        private const string FIRST_NAME = "Edit_Firstname";
        private const string LAST_NAME = "Edit_Lastname";
        private const string COURSE = "Edit_Course";
        private const string PROCESSING = "Edit_Processing";
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EditStudent);

            var pref = PreferenceManager.GetDefaultSharedPreferences(this);
            StudentId = pref.GetString("studentId", "");
            
            _progressBar = FindViewById<ProgressBar>(Resource.Id.Progress);
            _firstname = FindViewById<EditText>(Resource.Id.FirstName);
            _lastname = FindViewById<EditText>(Resource.Id.LastName);
            _course = FindViewById<EditText>(Resource.Id.Course);
            _nextButton = FindViewById<Button>(Resource.Id.NextButton);
            
            _nextButton.Click += NextButtonOnClick;
        }

        private async void NextButtonOnClick(object sender, EventArgs eventArgs)
        {
            var stud = new Student()
            {
                StudentId = StudentId,
                FirstName = _firstname.Text,
                LastName = _lastname.Text,
                Course = _course.Text
            };

            _progressBar.Indeterminate = true;
            _firstname.Enabled = false;
            _lastname.Enabled = false;
            _course.Enabled = false;
            _nextButton.Enabled = false;
            
            var result = await Client.Register(stud);

            _progressBar.Indeterminate = false;
            _firstname.Enabled = true;
            _lastname.Enabled = true;
            _course.Enabled = true;
            _nextButton.Enabled = true;

            if (result == ResultCodes.Success)
            {
                await Db.DropTable<Student>();
                await Db.Save(stud);
            }
        }
        
        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString(FIRST_NAME, _firstname.Text);
            outState.PutString(LAST_NAME,_lastname.Text);
            outState.PutString(COURSE,_course.Text);
            outState.PutBoolean(PROCESSING,_progressBar.Indeterminate);
            base.OnSaveInstanceState(outState);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            _firstname.Text = savedInstanceState.GetString(FIRST_NAME, "");
            _lastname.Text = savedInstanceState.GetString(LAST_NAME, "");
            _course.Text = savedInstanceState.GetString(COURSE, "");
            var proc = savedInstanceState.GetBoolean(PROCESSING, false);
            _progressBar.Indeterminate = proc;
            _firstname.Enabled = !proc;
            _lastname.Enabled = !proc;
            _course.Enabled = !proc;
            _nextButton.Enabled = !proc;
            base.OnRestoreInstanceState(savedInstanceState);
        }
    }
}