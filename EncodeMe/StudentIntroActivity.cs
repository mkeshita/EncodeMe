using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/ic_launcher", Theme = "@style/Theme.FullScreen",
        ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class StudentIntroActivity : Activity
    {
        private ProgressBar _progressBar;
        private EditText _studentId;
        private Button _next;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.StudentIntro);
            
            _progressBar = FindViewById<ProgressBar>(Resource.Id.Progress);
            _studentId = FindViewById<EditText>(Resource.Id.StudentId);
            _next = FindViewById<Button>(Resource.Id.NextButton);
            
            _next.Click += NextOnClick;
            
            return;
            var pref = PreferenceManager.GetDefaultSharedPreferences(this);
            _progressBar.Indeterminate = pref.GetBoolean("submitted", false);
            _studentId.Enabled = !pref.GetBoolean("submitted", false);
            _next.Enabled = !pref.GetBoolean("submitted", false);
            _studentId.Text = pref.GetString("studentId", "");
        }

        private async void NextOnClick(object sender, EventArgs eventArgs)
        {
            _progressBar.Indeterminate = true;
            _studentId.Enabled = false;
            _next.Enabled = false;

            var pref = PreferenceManager.GetDefaultSharedPreferences(this);
            var edit = pref.Edit();
            edit.PutBoolean("submitted", true);
            edit.PutInt("intro", 2);
            edit.PutString("studentId", _studentId.Text);
            edit.Commit();

            var result = await Client.GetStudentInfo(_studentId.Text);
            if (result.Result == ResultCodes.Success && result.Student != null)
            {
                await Db.Connection().CreateTableAsync<Student>();
                await Db.Connection().InsertOrReplaceAsync(result.Student);
                //Todo: student found
            }
            else if (result.Result == ResultCodes.NotFound)
            {
                StartActivity(new Intent(Application.Context, typeof(EditStudentActivity)));
            }
            else
            {
                var dlg = new AlertDialog.Builder(this);
                dlg.SetMessage("Please make sure you are connected to the network.");
                dlg.SetIcon(Resource.Drawable.ic_error);
                dlg.SetTitle(result.Message);
                dlg.SetCancelable(true);
                dlg.SetNegativeButton("Cancel", (o, args) =>
                {
                    edit = pref.Edit();
                    edit.PutBoolean("submitted", false);
                    edit.PutInt("intro", 1);
                    edit.Commit();
                    _progressBar.Indeterminate = false;
                    _studentId.Enabled = true;
                    _next.Enabled = true;
                });
                dlg.SetPositiveButton("Retry", (o, args) =>
                {
                    NextOnClick(sender,eventArgs);
                });
                dlg.Show();
            }
        }
        
        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            
            var pref = PreferenceManager.GetDefaultSharedPreferences(this);
            _progressBar.Indeterminate = pref.GetBoolean("submitted",false);
            _studentId.Enabled = !pref.GetBoolean("submitted",false);
            _next.Enabled = !pref.GetBoolean("submitted",false);
            _studentId.Text = pref.GetString("studentId", "");
        }
    }
}