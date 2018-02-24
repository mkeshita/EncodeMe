using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe
{
    class ReceiptsAdapter : BaseAdapter<Receipt>
    {
        private List<Receipt> _items;
        private Activity _context;

        public ReceiptsAdapter(Activity context, List<Receipt> items)
        {
            _items = items;
            _context = context;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Receipt this[int position] => _items[position];
        public override int Count => _items.Count;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _items[position];

            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.ReceiptRow, null);

            return GetView(view, item);
        }

        public static View GetView(View view, Receipt item)
        {
            view.FindViewById<TextView>(Resource.Id.ReceiptNumber).Text = item.Number;
            view.FindViewById<TextView>(Resource.Id.DatePaid).Text = item.DatePaid.ToString("MM/dd/yyyy");
            view.FindViewById<TextView>(Resource.Id.AmountPaid).Text = item.Amount.ToString("#,##0.00");
            
            var add = view.FindViewById<RelativeLayout>(Resource.Id.AddReceipt);
            var details = view.FindViewById<LinearLayout>(Resource.Id.ReceiptDetails);

            if (item.IsPlaceHolder)
            {
                add.Visibility = ViewStates.Visible;
                details.Visibility = ViewStates.Invisible;
            }
            else
            {
                add.Visibility = ViewStates.Invisible;
                details.Visibility = ViewStates.Visible;
            }
            
            return view;
        }
    }
}