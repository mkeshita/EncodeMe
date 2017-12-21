using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/ic_launcher", Theme = "@style/Theme")]
    class SubjectsActivity : Activity
    {
        private ListView _subjectsView;
        private Button _submitButton;
        private Button _addButton;
        private ProgressBar _progress;
        private Student _student;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Subjects);

            _subjectsView = FindViewById<ListView>(Resource.Id.SubjectsListView);
            _submitButton = FindViewById<Button>(Resource.Id.SubmitButton);
            _addButton = FindViewById<Button>(Resource.Id.AddSubjectButton);
            _progress = FindViewById<ProgressBar>(Resource.Id.Progress);
            
            _schedules = await Db.GetAll<ClassSchedule>();
            _submitButton.Enabled = _schedules.Any(d=>d.Sent);
            _subjectsView.Adapter = new SubjectsAdapter(this,_schedules);

            _student = (await Db.GetAll<Student>()).FirstOrDefault();
            if (_student == null)
            {
                StartActivity(typeof(StudentIntroActivity));
                Finish();
            }
            
            _addButton.Click += (s, args) =>
            {
                 StartActivityForResult(typeof(ScheduleBrowserActivity),7);
            };
            
            _submitButton.Click+= SubmitButtonOnClick;
        }

        private async void SubmitButtonOnClick(object sender, EventArgs eventArgs)
        {
            var pref = PreferenceManager.GetDefaultSharedPreferences(this);
            var edit = pref.Edit();
            edit.PutBoolean("Subjects_Processing", true);
            edit.PutBoolean("Enrollment_Accepted", false);
            edit.Commit();

            _subjectsView.Enabled = false;
            _submitButton.Enabled = false;
            _addButton.Enabled = false;
            _progress.Visibility = ViewStates.Visible;
            
            var result = await Client.Enroll(_student.StudentId, _schedules.Where(x => !x.Sent).ToList());

            edit = pref.Edit();
            edit.PutBoolean("Subjects_Processing", true);
            edit.Commit();
            
            _subjectsView.Enabled = true;
            _addButton.Enabled = true;
            _progress.Visibility = ViewStates.Gone;

            if (result.Result == ResultCodes.Success)
            {
                edit = pref.Edit();
                edit.PutInt("QueueNumber", result.QueueNumber);
                edit.Commit();
                
                StartActivity(typeof(StatusActivity));
                Finish();
            }
            
            var dlg = new AlertDialog.Builder(this);

            if (result.Result == ResultCodes.Offline)
                dlg.SetMessage("Server is unavailable.");
            else if (result.Result == ResultCodes.Timeout)
                dlg.SetMessage("Request timeout");
            else
                dlg.SetMessage("Request failed.");
            
            dlg.SetPositiveButton("OK", (o, args) => { });
            dlg.Show();
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode != 7 || resultCode != Result.Ok) return;

            var id = data.GetLongExtra("id", 0);
            if (id == 0) return;
            var scheds = await Db.GetAll<ClassSchedule>("cache.db");
            var sched = scheds.FirstOrDefault(d => d.ClassId == id);
            if (sched == null) return;
            AddSchedule(sched);
        }

        private List<ClassSchedule> _schedules = new List<ClassSchedule>();

        private void AddSchedule(ClassSchedule schedule)
        {
            Db.Save(schedule);
            _schedules.Add(schedule);
            _subjectsView.Adapter = new SubjectsAdapter(this, _schedules);
            _submitButton.Enabled = true;
        }

       // protected override async void OnRestoreInstanceState(Bundle savedInstanceState)
       // {
       //     base.OnRestoreInstanceState(savedInstanceState);

           // _schedules = await Db.GetAll<ClassSchedule>();
            
       // }
    }
    
}