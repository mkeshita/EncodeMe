using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
//using Android.Support.V7.Widget;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe
{
    class SchedulesAdapter : BaseAdapter<ClassSchedule>
    {
        private List<ClassSchedule> _items;
        private Activity _context;
        
        public SchedulesAdapter(Activity context, List<ClassSchedule> items)
        {
            if(items==null) items = new List<ClassSchedule>();
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

            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.ScheduleListItem, null);

            view.FindViewById<TextView>(Resource.Id.ScheduleText).Text = item.Schedule;
            view.FindViewById<TextView>(Resource.Id.RoomText).Text = item.Room;
            view.FindViewById<TextView>(Resource.Id.InstructorText).Text = item.Instructor;
            view.FindViewById<TextView>(Resource.Id.Slots).Text = $"{item.Enrolled}/{item.Slots}";

            return view;
        }
    }
    
}