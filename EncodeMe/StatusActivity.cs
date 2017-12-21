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
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Create your application here
            SetContentView(Resource.Layout.Status);

            var pref = PreferenceManager.GetDefaultSharedPreferences(this);
            _statusButton = FindViewById<Button>(Resource.Id.StatusButton);
            _statusButton.Text = pref.GetInt("QueueNumber",0).ToString();

            _statusButton.Click += (sender, args) => GetStatus();
            
            var btn = FindViewById<Button>(Resource.Id.EditButton);
            btn.Click += (sender, args) => StartActivity(typeof(SubjectsActivity));
            
            _statusUpdateHandler = StatusUpdateHandler;
            NetworkComms.AppendGlobalIncomingPacketHandler(StatusResult.GetHeader()+"update", _statusUpdateHandler);

            GetStatus();
        }

        private async void GetStatus()
        {
            var status = await Client.Instance.GetStatus();
            if (status.Result == ResultCodes.Success)
                Enrolled(status);
        }

        private void StatusUpdateHandler(PacketHeader h, Connection c, StatusResult i)
        {
            Enrolled(i);
        }

        private async void Enrolled(StatusResult status)
        {
            var pref = PreferenceManager.GetDefaultSharedPreferences(this);
            var edit = pref.Edit();
            edit.PutInt("QueueNumber", status.QueueNumber);
            edit.PutBoolean("Enrollment_Processed", status.IsProcessed);
            edit.PutBoolean("Enrollment_Accepted", status.IsAccepted);
            edit.Commit();
            _statusButton.Text = status.QueueNumber + "";

            if (!status.IsProcessed) return;
            var scheds = await Db.GetAll<ClassSchedule>();
            foreach (var stat in status.Schedules)
            {
                var sched = scheds.FirstOrDefault(x => x.ClassId == stat.ClassId);
                if (sched == null) continue;
                sched.Enrolled = stat.Enrolled;
                sched.EnrollmentStatus = stat.Status;
                await Db.Save(sched);
            }

            if (_activityPaused)
            {
              //  var notificationManager = (NotificationManager) GetSystemService(Context.NotificationService);
              //  var channel = new NotificationChannel("NORSU_EncodeMe_Channel_07","EncodeMe", NotificationImportance.High);
              //  channel.Description = "Enrollment status updated.";
              //  notificationManager.CreateNotificationChannel(channel);
            } else
                StartActivity(typeof(SubjectsActivity));
            Finish();
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
            base.OnResume();
        }
    }
}