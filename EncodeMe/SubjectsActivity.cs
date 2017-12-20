using System.Collections.Generic;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Transitions;
using Android.Views;
using Android.Widget;
using Java.Interop;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/ic_launcher", Theme = "@style/Theme")]
    class SubjectsActivity : Activity
    {
        private static SubjectsActivity _instance;
        
        private static List<ClassSchedule> _schedules = new List<ClassSchedule>();

        public static void AddSchedule(ClassSchedule schedule)
        {
            _schedules.Add(schedule);
            _instance._subjectsView.Adapter = new SubjectsAdapter(_instance, _schedules);
        }

        public SubjectsActivity()
        {
            _instance = this;
        }

        private ListView _subjectsView;
        
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Subjects);

            _subjectsView = FindViewById<ListView>(Resource.Id.SubjectsListView);
            
            var btn = FindViewById<Button>(Resource.Id.AddSubjectButton);
            btn.Click += (s, args) =>
            {
                 StartActivityForResult(typeof(ScheduleBrowserActivity),7);
            };
        }

       
    }
}