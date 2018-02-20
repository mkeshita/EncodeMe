using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using NORSU.EncodeMe.Models;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe.ViewModels
{
    sealed class Terminals : Screen
    {
        private Terminals() : base("Encoding Stations", PackIconKind.MonitorMultiple)
        {
            ShortName = "Stations";
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            
            Messenger.Default.AddListener<Client>(Messages.ModelDeleted, m =>
            {
                MainViewModel.EnqueueMessage($"Encoding Station [{m.Hostname}:{m.IP}] deleted","UNDO",
                    d=>d.Undelete(),m);
            });
        }

        private ICommand _clearLogsCommand;

        public ICommand ClearLogCommand => _clearLogsCommand ?? (_clearLogsCommand = new DelegateCommand<Client>(d =>
        {
            TerminalLog.DeleteWhere(nameof(TerminalLog.TerminalId),d.Id,true);
        }));

        private ICommand _enableCommand;

        public ICommand EnableCommand => _enableCommand ?? (_enableCommand = new DelegateCommand<Client>(d =>
        {
            d.Update(nameof(Client.IsEnabled),true);
        }));

        private ICommand _disableCommand;

        public ICommand DisableCommand => _disableCommand ?? (_disableCommand = new DelegateCommand<Client>(d =>
        {
            d.Update(nameof(Client.IsEnabled), false);
        }));

        private ICommand _shutdownCommand;

        public ICommand ShutdownCommand => _shutdownCommand ?? (_shutdownCommand = new DelegateCommand<Client>(d =>
        {
            Server.SendCommand(d, Network.Commands.Shutdown);
        },d=>d?.IsOnline??false));

        private ICommand _CloseCommand;

        public ICommand CloseCommand => _CloseCommand ?? (_CloseCommand = new DelegateCommand<Client>(d =>
        {
            Server.SendCommand(d, Network.Commands.CloseEncoder);
        }));

        private ICommand _RestartCommand;

        public ICommand RestartCommand => _RestartCommand ?? (_RestartCommand = new DelegateCommand<Client>(d =>
        {
            Server.SendCommand(d, Network.Commands.Restart);
        }, d => d?.IsOnline ?? false));

        private static Terminals _instance;
        public static Terminals Instance => _instance ?? (_instance = new Terminals());

        private ListCollectionView _items;

        public ListCollectionView Items
        {
            get
            {
                if (_items != null) return _items;
                _items = (ListCollectionView)CollectionViewSource.GetDefaultView(Client.Cache);
                return _items;
            }
        }

        private ListCollectionView _logs;

        public ListCollectionView Logs
        {
            get
            {
                if (_logs != null) return _logs;
                _logs = (ListCollectionView) CollectionViewSource.GetDefaultView(TerminalLog.Cache);
                _logs.SortDescriptions.Add(new SortDescription("Time", ListSortDirection.Descending));
                _logs.Filter = FilterLog;
                TerminalLog.Cache.CollectionChanged += (sender, args) =>
                {
                    _logs.Filter = FilterLog;
                };
                Items.CurrentChanged += (sender, args) =>
                {
                    if (Items.IsAddingNew) return;
                    _logs.Filter = FilterLog;
                };
                return _logs;
            }
        }

        private bool FilterLog(object o)
        {
            var log = o as TerminalLog;
            if (log == null) return false;
            return log.TerminalId == ((Client) Items.CurrentItem)?.Id;
        }
    }
}
