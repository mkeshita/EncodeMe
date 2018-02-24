using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Views.InputMethods;
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
        private ISharedPreferences _pref;
        
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Subjects);

            _pref = PreferenceManager.GetDefaultSharedPreferences(this);

            _subjectsView = FindViewById<ListView>(Resource.Id.SubjectsListView);
            _submitButton = FindViewById<Button>(Resource.Id.SubmitButton);
            _addButton = FindViewById<Button>(Resource.Id.AddSubjectButton);
            _progress = FindViewById<ProgressBar>(Resource.Id.Progress);

            _progress.Visibility = ViewStates.Gone;

            //_schedules = Client.ClassSchedules;// await Db.GetAll<ClassSchedule>();
            _submitButton.Enabled = !Client.EnrollmentCommited;
            _subjectsView.Adapter = new SubjectsAdapter(this, Client.ClassSchedules);
            
            RefreshSubjects();
            
            _student = Client.CurrentStudent;
            if (_student == null)
            {
                StartActivity(typeof(StudentIntroActivity));
                Finish();
            }

            if (_pref.GetBoolean(Constants.ENROLLMENT_PROCESSING, false))
            {
                StartActivity(typeof(StatusActivity));
                Finish();
            }
            
            _addButton.Click += (s, args) =>
            {
                 StartActivityForResult(typeof(ScheduleBrowserActivity),7);
            };
            
            _submitButton.Click+= SubmitButtonOnClick;
        }

        private void RefreshSubjects()
        {
            _subjectsView.ItemClick += (sender, args) =>
            {
                Toast.MakeText(this, "Long press to remove", ToastLength.Short).Show();
            };

            _subjectsView.ItemLongClick += async (sender, args) =>
            {
                _progress.Visibility = ViewStates.Visible;
                _subjectsView.Enabled = false;
                _submitButton.Enabled = false;
                _addButton.Enabled = false;
                var sched = ((SubjectsAdapter) _subjectsView.Adapter)[args.Position];
                var res = await Client.RemoveSchedule(sched.ClassId);

                _progress.Visibility = ViewStates.Gone;
                _subjectsView.Enabled = true;
                _submitButton.Enabled = true;
                _addButton.Enabled = true;

                if(res?.Success ?? false)
                {
                    Toast.MakeText(this, "Class removed", ToastLength.Short).Show();
                    _subjectsView.Adapter = new SubjectsAdapter(this, Client.ClassSchedules);
                    if (Client.ClassSchedules.Count == 0)
                    {
                        _submitButton.Enabled = false;
                    }
                } else
                {
                    if(res != null)
                    {
                        Toast.MakeText(this, res.ErrorMessage, ToastLength.Short).Show();
                    } else
                    {
                        try
                        {
                            if(CurrentFocus != null)
                            {
                                var imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                                imm.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);
                            }
                            var dlg = new Android.App.AlertDialog.Builder(this);
                            dlg.SetTitle("Disconnected from server");
                            dlg.SetMessage("You are disconnection from server. Please try again later.");
                            dlg.SetPositiveButton("Exit", (s, a) =>
                            {
                                FinishAffinity();
                            });
                            dlg.SetCancelable(false);
                            dlg.Show();

                        } catch(Exception e)
                        {
                            FinishAffinity();
                        }
                    }
                }
            };

        }

        private async void SubmitButtonOnClick(object sender, EventArgs eventArgs)
        {
            var edit = _pref.Edit();
            edit.PutBoolean("Subjects_Processing", true);
            edit.PutBoolean("Enrollment_Accepted", false);
            edit.Commit();

            _subjectsView.Enabled = false;
            _submitButton.Enabled = false;
            _addButton.Enabled = false;
            _progress.Visibility = ViewStates.Visible;
            
            var result = await Client.CommitEnrollment();
            
            edit = _pref.Edit();
            edit.PutBoolean("Subjects_Processing", true);
            edit.PutLong("QueueNumber", result.QueueNumber);
                
            edit.Commit();
            
            _subjectsView.Enabled = true;
            _addButton.Enabled = true;
            _submitButton.Enabled = true;
            _progress.Visibility = ViewStates.Gone;
            
            if (result?.Success ?? false)
            {
                StartActivity(typeof(StatusActivity));
                Finish();
            }
            else
            {
                Client.ShowDisconnectedDialog(this);
            }
            
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode != 7 || resultCode != Result.Ok) return;

            var id = data.GetLongExtra("id", 0);
            if (id == 0) return;
            var sched = Client.PendingAddition;
            if (sched == null) return;
            

            _progress.Visibility = ViewStates.Visible;
            _submitButton.Enabled = false;
            _addButton.Enabled = false;
            _subjectsView.Enabled = false;
            
            var res = await Client.AddSchedule(sched);

            _progress.Visibility = ViewStates.Gone;
            _submitButton.Enabled = true;
            _addButton.Enabled = true;
            _subjectsView.Enabled = true;

            if (res?.Success ?? false)
            {
                var remove = Client.ClassSchedules.FirstOrDefault(x => x.ClassId == res.ReplacedId);
                if (remove != null)
                    Client.ClassSchedules.Remove(remove);
                if (Client.ClassSchedules.All(x => x.ClassId != sched.ClassId))
                    Client.ClassSchedules.Add(sched);


            }
            else
            {
                var dlg = new AlertDialog.Builder(this);
                dlg.SetTitle(res == null ? "Can not find server" : res.ErrorMessage);
                dlg.SetPositiveButton("Okay", (o, args) =>
                {

                });

                dlg.Show();
            }
            

            _subjectsView.Adapter = new SubjectsAdapter(this, Client.ClassSchedules);
            
            _submitButton.Enabled = true;
        }
        
        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            
            RefreshSubjects();
            _subjectsView.Adapter = new SubjectsAdapter(this, Client.ClassSchedules);
        }
    }
    
}