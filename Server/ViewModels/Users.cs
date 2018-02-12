using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MaterialDesignThemes.Wpf;

namespace NORSU.EncodeMe.ViewModels
{
    class Users : Screen
    {
        private Users() : base("USERS", PackIconKind.AccountMultipleOutline)
        {
        }

        private static Users _instance;
        public static Users Instance => _instance ?? (_instance = new Users());
    }
}
