using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using NetworkCommsDotNet.Tools;
using NORSU.EncodeMe.Network;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using File = Java.IO.File;

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

            Directory = new File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "EncodeMe");
            if (!Directory.Exists())
            {
                Directory.Mkdirs();
            }

            _picture.Clickable = true;
            _picture.Click += PictureOnClick;
            
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

        private void PictureOnClick(object o, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);

            File = new File(Directory, $"EncodeMe_{Guid.NewGuid()}.jpg");

            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(File));

            StartActivityForResult(intent, 0);
        }
        private static File Directory { get; set; }
        private static File File { get; set; }
        public static Bitmap bitmap;
        
        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(File);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);

            // Display in ImageView. We will resize the bitmap to fit the display
            // Loading the full sized image will consume to much memory 
            // and cause the application to crash.

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = _picture.Height;
            bitmap = LoadAndResizeBitmap(File.Path, width, height);
            
            if (bitmap != null)
            {
                
                _picture.SetImageBitmap(bitmap);
                using (var mem = new MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 4, mem);
                    var res = await Client.ChangePicture(mem.ToArray());
                    if (!(res?.Success ?? false))
                        await Client.ChangePicture(mem.ToArray());
                }
                bitmap = null;
            }

            // Dispose of the Java side bitmap.
            GC.Collect();
        }

        public static Bitmap LoadAndResizeBitmap(string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options {InJustDecodeBounds = true};
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                    ? outHeight / height
                    : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
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
          //  _picture.SetImageResource(Resource.Drawable.profile);
            _yearLevel.SetSelection(Client.CurrentStudent.YearLevel);
            _studentStatus.SetSelection(Client.CurrentStudent.Status);

            if (Client.CurrentStudent.Picture?.Length > 0)
            {
                _picture.SetImageBitmap(BitmapFactory.DecodeByteArray(Client.CurrentStudent.Picture, 0,
                    Client.CurrentStudent.Picture.Length));
            }
        }
    }
}