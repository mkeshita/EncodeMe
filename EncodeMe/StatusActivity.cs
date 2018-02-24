using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private Button _cancelButton;
        private ImageView _statusImage;
        private TextView _messageText;
        private ProgressBar _progress;
        private TextView _title;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Client.CurrentStudent == null)
            {
                StartActivity(typeof(StudentIntroActivity));
                Finish();
                return;
            }
            if (!(Client.RequestStatus?.IsSubmitted ?? false))
            {
                StartActivity(typeof(SubjectsActivity));
                Finish();
                return;
            }
            
            base.OnCreate(savedInstanceState);
            
            // Create your application here
            SetContentView(Resource.Layout.Status);

            var pref = PreferenceManager.GetDefaultSharedPreferences(this);
            _statusButton = FindViewById<Button>(Resource.Id.StatusButton);
            _statusImage = FindViewById<ImageView>(Resource.Id.StatusImage);
            _messageText = FindViewById<TextView>(Resource.Id.MessageText);
            _progress = FindViewById<ProgressBar>(Resource.Id.Progress);
            _title = FindViewById<TextView>(Resource.Id.TitleText);
            
            _statusButton.Click += (sender, args) => GetStatus();

            _cancelButton = FindViewById<Button>(Resource.Id.CancelButton);
            
            _statusUpdateHandler = (h,c,i)=>RefreshStatus();
            NetworkComms.AppendGlobalIncomingPacketHandler(StatusResult.GetHeader()+"update", _statusUpdateHandler);
            
            GetStatus();
            
            _cancelButton.Click += CancelButtonOnClick;

            _statusUpdater = Task.Factory.StartNew(async () =>
            {
                while (!_cancelUpdater)
                {
                    await Task.Delay(3333);
                    if (_cancelUpdater) return;
                    GetStatus();
                }
            });
        }

        private bool _cancelUpdater;
        private Task _statusUpdater;

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
                        Client.ShowDisconnectedDialog(this);
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
            if (_activityPaused) return;
            RefreshStatus();
            if (_requestingStatus) return;
            _requestingStatus = true;
            var status = await Client.Instance.GetStatus();
          
            if (status?.Success ?? false)
            {
                _requestingStatus = false;
                RefreshStatus();
            }
            else
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        
                        if (status == null)
                        {
                            Client.ShowDisconnectedDialog(this);
                        }
                        else
                        {
                            var dlg = new AlertDialog.Builder(this).Create();
                            dlg.SetTitle("Request Failed");
                            dlg.SetMessage(status.ErrorMessage);
                            dlg.SetButton("OKAY", (sender, args) =>
                            {
                                _requestingStatus = false;
                            });
                            dlg.Show();
                        }



                        
                    }
                    catch (Exception e)
                    {
                        FinishAffinity();
                    }
                });
            }
                
        }

        private void RefreshStatus()
        {
            RunOnUiThread(() =>
            {
                if (Client.CurrentStudent == null)
                {
                    StartActivity(typeof(StudentIntroActivity));
                    Finish();
                    return;
                }
                if (!(Client.RequestStatus?.IsSubmitted ?? false))
                {
                    StartActivity(typeof(SubjectsActivity));
                    Finish();
                    return;
                }


                _statusButton.Text = Client.RequestStatus.QueueNumber + "";

                switch (Client.RequestStatus.Status)
                {
                    case EnrollmentStatus.Pending:
                        _cancelButton.Visibility = ViewStates.Visible;
                        return;
                    case EnrollmentStatus.Processing:
                        _statusButton.Visibility = ViewStates.Gone;
                        _statusImage.Visibility = ViewStates.Visible;
                        _statusImage.SetImageResource(Resource.Drawable.status_processing);
                        _messageText.Text = "You request is now being processed.";
                        _cancelButton.Visibility = ViewStates.Gone;
                        return;
                    case EnrollmentStatus.Accepted:
                        _progress.Indeterminate = false;
                        _statusButton.Visibility = ViewStates.Gone;
                        _statusImage.Visibility = ViewStates.Visible;
                        _statusImage.SetImageResource(Resource.Drawable.status_accepted);
                        _progress.Progress = 1;
                        _title.Text = "OFFICIALLY ENROLLED";
                        _messageText.Text =
                            "Congratulations! Your are now officially enrolled. Please proceed to the PRINTING AREA and present your Official Receipt and ID.";
                        _cancelButton.Visibility = ViewStates.Gone;
                        
                        break;
                    case EnrollmentStatus.Closed:
                        _progress.Indeterminate = false;
                        _progress.Progress = 0;
                        _statusButton.Visibility = ViewStates.Gone;
                        _statusImage.Visibility = ViewStates.Visible;
                        _statusImage.SetImageResource(Resource.Drawable.status_failed);
                        _title.SetTextColor(Color.Red);
                        _title.Text = "ENROLLMENT FAILED";
                        _messageText.Text = "Problem occured during the process. Some class schedules are closed.";
                        _cancelButton.Visibility = ViewStates.Visible;
                        _cancelButton.Text = "VIEW SUBJECTS";
                        break;
                    case EnrollmentStatus.Conflict:
                        _progress.Indeterminate = false;
                        _progress.Progress = 0;
                        _statusButton.Visibility = ViewStates.Gone;
                        _statusImage.Visibility = ViewStates.Visible;
                        _statusImage.SetImageResource(Resource.Drawable.status_failed);
                        _title.Text = "ENROLLMENT FAILED";
                        _title.SetTextColor(Color.Red);
                        _messageText.Text = "Problem occured during the process. Some class schedules are in conflict.";
                        _cancelButton.Visibility = ViewStates.Visible;
                        _cancelButton.Text = "VIEW SUBJECTS";
                        break;
                }
            });
        }

        protected override void OnDestroy()
        {
            _cancelUpdater = true;
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