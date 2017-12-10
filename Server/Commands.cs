using System.Windows.Input;
using NORSU.EncodeMe;
using NORSU.EncodeMe.Models;

namespace Server
{
    class Commands
    {
        private Commands() { }

        private static ICommand _delete;

        public static ICommand Delete => _delete ?? (_delete = new DelegateCommand(d =>
        {
            ((ModelBase)d).Delete();
        }, d =>
        {
            return ((ModelBase)d)?.CanDelete() ?? false;
        }));


    }
}
