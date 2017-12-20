using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe
{
    [Activity(Label = "Schedule Browser", Theme = "@style/Theme")]
    public class ScheduleBrowserActivity : Activity
    {
        private Button _goButton;
        private EditText _subjectCode;
        private ListView _schedulesListView;
        private ProgressBar _progress;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Create your application here
            SetContentView(Resource.Layout.ScheduleBrowser);

            _goButton = FindViewById<Button>(Resource.Id.GoButton);
            _subjectCode = FindViewById<EditText>(Resource.Id.SubjectText);
            _schedulesListView = FindViewById<ListView>(Resource.Id.SchedulesListView);
            _progress = FindViewById<ProgressBar>(Resource.Id.Progress);

            _schedulesListView.ItemClick += SchedulesListViewOnItemClick;
            
            _progress.Visibility = ViewStates.Gone;
            _goButton.Click += GoButtonOnClick;
        }

        private async void GoButtonOnClick(object sender, EventArgs eventArgs)
        {
            if (string.IsNullOrWhiteSpace(_subjectCode.Text)) return;

            _progress.Visibility = ViewStates.Visible;
            _schedulesListView.Enabled = false;
            
            var result = await Client.GetSchedules(_subjectCode.Text);

            _progress.Visibility = ViewStates.Gone;
            _schedulesListView.Enabled = true;

            if (result == null) return;

            _schedulesListView.Adapter = new SchedulesAdapter(this, result.Schedules);

            if (result.Result != ResultCodes.Success)
            {
                var dlg = new AlertDialog.Builder(this);

                switch (result.Result)
                {
                    case ResultCodes.Offline:
                        dlg.SetMessage("You are not connected to the server.");
                        break;
                    case ResultCodes.Timeout:
                        dlg.SetMessage("Request timeout.");
                        break;
                    case ResultCodes.NotFound:
                        dlg.SetMessage($"{_subjectCode.Text} does not exists.");
                        break;
                    case ResultCodes.Error:
                        dlg.SetTitle("Request failed.");
                        break;
                }
                dlg.SetPositiveButton("OK", (o, args) => { });
                dlg.Show();

            }
            
            
        }

        private void SchedulesListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var adapter = _schedulesListView.Adapter as SchedulesAdapter;
            if (adapter == null) return;
            var sched = adapter[e.Position];
            
            SubjectsActivity.AddSchedule(sched);
            
            //var result = new Intent();
            
            //result.PutExtra("Class", sched);
            Finish();
        }
    }
}