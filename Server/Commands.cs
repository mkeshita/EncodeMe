using System.Windows.Input;
using NORSU.EncodeMe.Models;

namespace Server
{
    static class Commands
    {
        private static ICommand _SaveCommand;

        public static ICommand Save => _SaveCommand ??
                                       (_SaveCommand =
                                           new DelegateCommand<ModelBase>(m => m.Save(), m => m.CanSave()));

        private static ICommand _deleteCommand;

        public static ICommand Delete => _deleteCommand ??
                                         (_deleteCommand =
                                             new DelegateCommand<ModelBase>(m => m.Delete(),
                                                 m => m.CanDelete()));
    }
}