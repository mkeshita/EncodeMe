using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/ic_launcher", Theme = "@style/Theme.FullScreen",
        ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class StudentInfo : Activity
    {
        private TextView _fullname, _studentId, _birthDate, _gender, _course, _major, _minor;
        private EditText _address, _scholarship;
        private Spinner _yearLevel, _studentStatus;
        private ImageView _picture;
        private Button _enroll,_status,_save;

        private string[] YearLevels = new[]
        {
            "First Year",
            "Second Year",
            "Third Year",
            "Fourth Year",
            "Fifth Year"
        };

        private string[] Statuses = new[]
        {
            "Ongoing",
            "Returnee",
            "Shiftee",
            "Transferee"
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Client.CurrentStudent == null)
            {
                StartActivity(typeof(StudentIntroActivity));
                Finish();
                return;
            }
            
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.StudentInfo);

            _fullname = FindViewById<TextView>(Resource.Id.Fullname);
            _studentId = FindViewById<TextView>(Resource.Id.StudentId);
            _birthDate = FindViewById<TextView>(Resource.Id.BirthDate);
            _gender = FindViewById<TextView>(Resource.Id.Gender);
            _address = FindViewById<EditText>(Resource.Id.Address);
            _course = FindViewById<TextView>(Resource.Id.Course);
            _major = FindViewById<TextView>(Resource.Id.Major);
            _minor = FindViewById<TextView>(Resource.Id.Minor);
            _scholarship = FindViewById<EditText>(Resource.Id.Scholarship);
            _picture = FindViewById<ImageView>(Resource.Id.Picture);
            _yearLevel = FindViewById<Spinner>(Resource.Id.YearLevel);
            _studentStatus = FindViewById<Spinner>(Resource.Id.Status);
            _enroll = FindViewById<Button>(Resource.Id.EnrollButton);
            _status = FindViewById<Button>(Resource.Id.StatusButton);
            _save = FindViewById<Button>(Resource.Id.SaveButton);
            
            _yearLevel.Adapter = new ArrayAdapter<string>(this,Android.Resource.Layout.SimpleListItem1, YearLevels);
            _studentStatus.Adapter = new ArrayAdapter<string>(this,Android.Resource.Layout.SimpleListItem1, Statuses);

            _studentStatus.ItemSelected += (sender, args) =>
            {
                RefreshSaveButton();
            };
            _yearLevel.ItemSelected += (sender, args) =>
            {
                RefreshSaveButton();
            };
            _address.AfterTextChanged += (sender, args) =>
            {
                RefreshSaveButton();
            };
            _scholarship.AfterTextChanged += (sender, args) =>
            {
                RefreshSaveButton();
            };
            
            SetupValues();
            
            _enroll.Click+= EnrollOnClick;
            _status.Click += StatusOnClick;
            _save.Click += SaveOnClick;
        }

        private async void SaveOnClick(object o, EventArgs eventArgs)
        {
            _enroll.Enabled = false;
            _save.Enabled = false;

            var res = await Client.UpdateStudent(_address.Text, _scholarship.Text, _yearLevel.SelectedItemPosition,
                _studentStatus.SelectedItemPosition);

            _enroll.Enabled = true;
            _save.Enabled = true;

            if (res?.Success ?? false)
            {
                Toast.MakeText(this, "Information changes saved", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Action failed", ToastLength.Short).Show();
            }
        }

        private void RefreshSaveButton()
        {
            if (Client.RequestStatus == null || Client.RequestStatus.IsSubmitted)
            {
                _save.Visibility = ViewStates.Gone;
                return;
            }
            
            _save.Visibility = ViewStates.Visible;
            _save.Enabled = false;
            
            if (_studentStatus.SelectedItemPosition != Client.CurrentStudent.Status) _save.Enabled = true;
            if (_yearLevel.SelectedItemPosition != Client.CurrentStudent.YearLevel)
                _save.Enabled = true;
            if (_address.Text.ToLower() != Client.CurrentStudent.Address.ToLower())
                _save.Enabled = true;
            if (_scholarship.Text.ToLower() != Client.CurrentStudent.Scholarship.ToLower())
                _save.Enabled = true;
        }

        private void StatusOnClick(object sender, EventArgs eventArgs)
        {
            StartActivity(typeof(StatusActivity));
        }

        private void EnrollOnClick(object sender, EventArgs eventArgs)
        {
            StartActivity(typeof(GetReceiptActivity));
        }

        protected override void OnResume()
        {
            base.OnResume();
            SetupValues();
        }

        private void SetupValues()
        {
            if (Client.RequestStatus?.IsSubmitted ?? false)
            {
                _enroll.Visibility = ViewStates.Gone;
                _status.Visibility = ViewStates.Visible;
            }
            else
            {
                _enroll.Visibility = ViewStates.Visible;
                _status.Visibility = ViewStates.Gone;
            }
            _fullname.Text = $"{Client.CurrentStudent.FirstName} {Client.CurrentStudent.LastName}".ToUpper();
            _studentId.Text = Client.CurrentStudent.StudentId?.ToUpper();
            _birthDate.Text = Client.CurrentStudent.BirthDate?.ToString("MMM d, yyyy")??"N/A";
            _gender.Text = Client.CurrentStudent.Male ? "MALE" : "FEMALE";
            _address.Text = Client.CurrentStudent.Address;
            _course.Text = Client.CurrentStudent.Course;
            _major.Text = Client.CurrentStudent.Major;
            _minor.Text = Client.CurrentStudent.Minor;
            _scholarship.Text = Client.CurrentStudent.Scholarship;
            _picture.SetImageResource(Resource.Drawable.profile);
            _yearLevel.SetSelection(Client.CurrentStudent.YearLevel);
            _studentStatus.SetSelection(Client.CurrentStudent.Status);
        }
    }
}