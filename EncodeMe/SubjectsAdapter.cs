using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe
{
    class SubjectsAdapter : BaseAdapter<ClassSchedule>
    {
        private List<ClassSchedule> _items;
        private Activity _context;

        public SubjectsAdapter(Activity context, List<ClassSchedule> items)
        {
            _items = items;
            _context = context;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override ClassSchedule this[int position] => _items[position];
        public override int Count => _items.Count;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _items[position];

            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.SubjectItem, null);

            return GetView(view, item);
        }

        public static View GetView(View view, ClassSchedule item)
        {
            view.FindViewById<TextView>(Resource.Id.SubjectCode).Text = item.SubjectCode;
            view.FindViewById<TextView>(Resource.Id.Schedule).Text = item.Schedule;
            view.FindViewById<TextView>(Resource.Id.Enrolled).Text = $"{item.Enrolled}/{item.Slots}";
            var stat = view.FindViewById<TextView>(Resource.Id.Status);
            if (item.EnrollmentStatus == ScheduleStatuses.Pending)
            {
                stat.Visibility = ViewStates.Invisible;
            }
            else
            {
                stat.Visibility = ViewStates.Visible;
                stat.Text = item.EnrollmentStatus.ToString();
            }
            return view;
        }
    }
}