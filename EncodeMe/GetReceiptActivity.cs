using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/ic_launcher", Theme = "@style/Theme.FullScreen",
        ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class GetReceiptActivity : Activity
    {
        private EditText _receipt;
        private Button _submit;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EnterOR);
            
            _receipt = FindViewById<EditText>(Resource.Id.OrNumber);
            _submit = FindViewById<Button>(Resource.Id.SubmitButton);
            
            _submit.Click += SubmitOnClick;
        }

        private void SubmitOnClick(object sender, EventArgs eventArgs)
        {
            if (string.IsNullOrEmpty(_receipt.Text)) return;

            Client.Receipt = _receipt.Text;
            
            StartActivity(typeof(SubjectsActivity));
        }
    }
}