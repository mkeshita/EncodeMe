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
        private ListView _receipts;
        private Button _submit;
        private ProgressBar _progressBar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Client.RequestStatus?.IsSubmitted ?? false)
            {
                try
                {
                    StartActivity(typeof(StatusActivity));
                }
                catch (Exception e)
                {
                    //
                }
                Finish();
                return;
            }
            
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EnterOR);
            
            _receipts = FindViewById<ListView>(Resource.Id.ReceiptsList);
            Receipts.Add(NewReceipt);
            _receipts.Adapter = new ReceiptsAdapter(this, Receipts);


            _submit = FindViewById<Button>(Resource.Id.SubmitButton);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.Progress);

            _submit.Click += SubmitOnClick;
            _receipts.ItemClick += ReceiptsOnItemClick;
        }

        private void ReceiptsOnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (!(_receipts.Adapter is ReceiptsAdapter adapter)) return;
            var receipt = adapter[e.Position];
            if (!receipt.IsPlaceHolder)
            {
                Toast.MakeText(this,"Long press to delete",ToastLength.Short).Show();
                return;
            }

            var dlg = new AlertDialog.Builder(this).Create();

            var dlgView = LayoutInflater.Inflate(Resource.Layout.OrDialog, null);
            var cancel = dlgView.FindViewById<Button>(Resource.Id.Cancel);
            var accept = dlgView.FindViewById<Button>(Resource.Id.Accept);
            var number = dlgView.FindViewById<EditText>(Resource.Id.ReceiptNumber);
            var date = dlgView.FindViewById<EditText>(Resource.Id.DatePaid);
            var amount = dlgView.FindViewById<EditText>(Resource.Id.AmountPaid);
            accept.Enabled = false;

            cancel.Click += (o, args) =>
            {
                dlg.Dismiss();
            };

            accept.Click += (o, args) =>
            {
                var or = ParseReceipt(number.Text, date.Text, amount.Text);
                if (or == null) return;
                AddReceipt(or);
                dlg.Dismiss();
            };
            
            number.AfterTextChanged += (o, args) =>
            {
                var or = ParseReceipt(number.Text, date.Text, amount.Text);
                accept.Enabled = or != null;
            };
            date.AfterTextChanged += (o, args) =>
            {
                var or = ParseReceipt(number.Text, date.Text, amount.Text);
                accept.Enabled = or != null;
            };
            amount.AfterTextChanged += (o, args) =>
            {
                var or = ParseReceipt(number.Text, date.Text, amount.Text);
                accept.Enabled = or != null;
            };
            
            dlg.SetView(dlgView);
            dlg.Show();
        }

        private Receipt ParseReceipt(string number, string date, string amount)
        {
            if (string.IsNullOrWhiteSpace(number))
                return null;
            var dDate = DateTime.Now;
            if (!DateTime.TryParse(date, out dDate))
                return null;
            var dAmount = 0d;
            if (!double.TryParse(amount, out dAmount))
                return null;
            return new Receipt()
            {
                Amount = dAmount,
                DatePaid = dDate,
                Number = number
            };
        }

        private Receipt NewReceipt = new Receipt() {IsPlaceHolder = true};
        
        private void AddReceipt(Receipt receipt)
        {
            Receipts.Remove(NewReceipt);
            Receipts.Add(receipt);
            if(!(Client.Server.MaxReceipts>0 && Receipts.Count >= Client.Server.MaxReceipts))
                Receipts.Add(NewReceipt);
            
            _receipts.Adapter = new ReceiptsAdapter(this,Receipts);
        }

        protected override void OnResume()
        {
            if (Client.RequestStatus?.IsSubmitted ?? false)
            {
                StartActivity(typeof(StatusActivity));
                Finish();
                return;
            }
            base.OnResume();
        }

        private List<Receipt> Receipts = new List<Receipt>();
        
        private async void SubmitOnClick(object sender, EventArgs eventArgs)
        {
            if (Receipts == null || Receipts.Count == 0)
                return;
            
            _progressBar.Indeterminate = true;

            Receipts.Remove(NewReceipt);
            var res = await Client.StartEnrollment(Receipts);
            Receipts.Add(NewReceipt);
            _progressBar.Indeterminate = false;
            
            if (res?.Success ?? false)
            {
                StartActivity(typeof(SubjectsActivity));
                Finish();
            }
            else
            {
                var dlg = new AlertDialog.Builder(this);
                if (res == null)
                {
                    dlg.SetMessage("Disconnected from server");
                    dlg.SetMessage("Please make sure you are connected to the server and try again.");
                    dlg.SetPositiveButton("EXIT", (o, args) =>
                    {
                        FinishAffinity();
                    });
                }
                else
                {
                    dlg.SetMessage("Unable to process request");
                    dlg.SetMessage(res.ErrorMessage);
                    dlg.SetPositiveButton("OKAY", (o, args) =>
                    {
                    
                    });
                }
                
                
                dlg.Show();
            }
            
        }
    }
}