using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe.ViewModels
{
    class WorkViewModel : ViewModelBase
    {
        public static async Task ShowDialog(GetWorkResult work)
        {
            var view = new WorkView();
            view.ShowDialog();
        }
    }
}
