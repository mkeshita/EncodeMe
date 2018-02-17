using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe
{
    
    [Activity(Label = "@string/app_name", Icon = "@drawable/ic_launcher", Theme = "@style/Theme.FullScreen")]
    public class StatusActivity : Activity
    {
        private NetworkComms.PacketHandlerCallBackDelegate<StatusResult> _statusUpdateHandler;
        private bool _activityPaused;
        private Button _statusButton;
        private long _queueNumber;
        private Button _editButton;
        private ImageView _statusImage;
        private TextView _messageText;
        private ProgressBar _progress;
        private TextView _title;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Create your application here
            SetContentView(Resource.Layout.Status);

            var pref = PreferenceManager.GetDefaultSharedPreferences(this);
            _statusButton = FindViewById<Button>(Resource.Id.StatusButton);
            _statusImage = FindViewById<ImageView>(Resource.Id.StatusImage);
            _messageText = FindViewById<TextView>(Resource.Id.MessageText);
            _progress = FindViewById<ProgressBar>(Resource.Id.Progress);
            _title = FindViewById<TextView>(Resource.Id.TitleText);
            
            _queueNumber = pref.GetLong("QueueNumber", 0L);
            _statusButton.Text = _queueNumber.ToString();

            _statusButton.Click += (sender, args) => GetStatus();
            
            _editButton = FindViewById<Button>(Resource.Id.EditButton);
            _editButton.Click += (sender, args) => StartActivity(typeof(SubjectsActivity));
            _editButton.Enabled = _queueNumber > 0;
            
            _statusUpdateHandler = (h,c,i)=>Enrolled(i);
            NetworkComms.AppendGlobalIncomingPacketHandler(StatusResult.GetHeader()+"update", _statusUpdateHandler);

            GetStatus();
            _cancelButton.Click += CancelButtonOnClick;
        }

        private async void CancelButtonOnClick(object o, EventArgs eventArgs)
        {
            _cancelButton.Enabled = false;
            var res = await Client.CancelEnrollment();
            if (res?.Success ?? false)
            {
                StartActivity(typeof(SubjectsActivity));
                Finish();
            }
            else
            {
                try
                {
                    var dlg = new AlertDialog.Builder(this);

                    if (res == null)
                    {
                        dlg.SetTitle("Request Timeout");
                        dlg.SetMessage("You are not connected to the server.");
                        dlg.SetPositiveButton("QUIT", (sender, args) =>
                        {
                            FinishAffinity();
                            return;
                        });
                    }
                    else
                    {
                        dlg.SetTitle("Request Failed");
                        dlg.SetMessage(res.ErrorMessage);
                        dlg.SetPositiveButton("OKAY", (sender, args) =>
                        {
                        });
                    }
                    
                    dlg.Show();
                }
                catch (Exception e)
                {
                    FinishAffinity();
                }

            }
        }

        private bool _requestingStatus;
        private async void GetStatus()
        {
            var status = await Client.Instance.GetStatus();
            _editButton.Enabled = status.Status == EnrollmentStatus.Pending;
            if (status.Result == ResultCodes.Success)
                Enrolled(status);
        }

        private async void Enrolled(StatusResult status)
        {
            var pref = PreferenceManager.GetDefaultSharedPreferences(this);
            var edit = pref.Edit();
            edit.PutInt("QueueNumber", status.QueueNumber);
            edit.PutBoolean(Constants.ENROLLMENT_PROCESSING, status.Status>EnrollmentStatus.Processing);
            edit.PutBoolean("Enrollment_Accepted", status.Status == EnrollmentStatus.Accepted);
            edit.Commit();
            _statusButton.Text = status.QueueNumber + "";
            _queueNumber = status.QueueNumber;
            
            _editButton.Enabled = status.Status != EnrollmentStatus.Processing;
            
            switch (status.Status)
            {
                case EnrollmentStatus.Pending:
                    return;
                case EnrollmentStatus.Processing:
                    _statusButton.Visibility = ViewStates.Gone;
                    _statusImage.Visibility = ViewStates.Visible;
                    _messageText.Text = "You request is now being processed.";
                    return;
                case EnrollmentStatus.Accepted:
                    _progress.Indeterminate = false;
                    _progress.Progress = 1;
                    _title.Text = "OFFICIALLY ENROLLED";
                    _messageText.Text = "Congratulations! Your are now officially enrolled.";
                    _editButton.Text = "View Schedule";
                    break;
                case EnrollmentStatus.Closed:
                    _progress.Indeterminate = false;
                    _progress.Progress = 0;
                    _title.Text = "ENROLLMENT FAILED";
                    _messageText.Text = "Some if not al of the classes you are enrolling are closed.";
                    break;
                case EnrollmentStatus.Conflict:
                    _progress.Indeterminate = false;
                    _progress.Progress = 0;
                    _title.Text = "ENROLLMENT FAILED";
                    _messageText.Text = "The schedules of some classes you are enrolling are overlapping.";
                    break;
            }


            var scheds = await Db.GetAll<ClassSchedule>();
            foreach (var stat in status.Schedules)
            {
                var sched = scheds.FirstOrDefault(x => x.ClassId == stat.ClassId);
                if (sched == null) continue;
                sched.Enrolled = stat.Enrolled;
                sched.EnrollmentStatus = stat.Status;
                await Db.Save(sched);
            }

           // if (_activityPaused)
            //{
              //  var notificationManager = (NotificationManager) GetSystemService(Context.NotificationService);
              //  var channel = new NotificationChannel("NORSU_EncodeMe_Channel_07","EncodeMe", NotificationImportance.High);
              //  channel.Description = "Enrollment status updated.";
              //  notificationManager.CreateNotificationChannel(channel);
           // } else
               // StartActivity(typeof(SubjectsActivity));
           // Finish();
        }

        protected override void OnDestroy()
        {
            NetworkComms.RemoveGlobalIncomingPacketHandler(StatusResult.GetHeader()+"update", _statusUpdateHandler);
            base.OnDestroy();
        }

        protected override void OnPause()
        {
            _activityPaused = true;
            base.OnPause();
        }

        protected override void OnResume()
        {
            _activityPaused = false;
            GetStatus();
            //_editButton.Enabled = _queueNumber > 0;
            base.OnResume();
        }
    }
}