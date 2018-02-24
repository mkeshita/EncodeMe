using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using NORSU.EncodeMe.Models;
using NORSU.EncodeMe.Network;
using Xceed.Words.NET;
using VerticalAlignment = System.Windows.VerticalAlignment;

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
                MainViewModel.EnqueueMessage($"Encoding Station [{m.Hostname}:{m.IpAddress}] deleted","UNDO",
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
            TerminalLog.Add(d.Id, $"Terminal enabled by {MainViewModel.Instance.CurrentUser?.Username}");
        }));

        private ICommand _disableCommand;

        public ICommand DisableCommand => _disableCommand ?? (_disableCommand = new DelegateCommand<Client>(d =>
        {
            d.Update(nameof(Client.IsEnabled), false);
            Server.SendCommand(d, Network.Commands.CloseEncoder);
            d.IsOnline = false;
            d.Encoder = null;
            d.CancelRequest();
            TerminalLog.Add(d.Id, $"Terminal disabled by {MainViewModel.Instance.CurrentUser?.Username}");
        }));

        private ICommand _shutdownCommand;

        public ICommand ShutdownCommand => _shutdownCommand ?? (_shutdownCommand = new DelegateCommand<Client>(d =>
        {
            Server.SendCommand(d, Network.Commands.Shutdown);
            TerminalLog.Add(d.Id, $"Remotely shutdown by {MainViewModel.Instance.CurrentUser?.Username}");
        },d=>d?.IsOnline??false));

        private ICommand _CloseCommand;

        public ICommand CloseCommand => _CloseCommand ?? (_CloseCommand = new DelegateCommand<Client>(d =>
        {
            Server.SendCommand(d, Network.Commands.CloseEncoder);
            d.IsOnline = false;
            d.Encoder = null;
            d.CancelRequest();
            TerminalLog.Add(d.Id, $"Remotely closed by {MainViewModel.Instance.CurrentUser?.Username}");
        }, d => d?.IsOnline ?? false));

        private ICommand _RestartCommand;

        public ICommand RestartCommand => _RestartCommand ?? (_RestartCommand = new DelegateCommand<Client>(d =>
        {
            Server.SendCommand(d, Network.Commands.Restart);
            TerminalLog.Add(d.Id, $"Remotely restarted by {MainViewModel.Instance.CurrentUser?.Username}");
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
        
        internal static void Print(string path)
        {
            var info = new ProcessStartInfo(path);
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = true;
            info.Verb = "PrintTo";
            try
            {
                Process.Start(info);
            } catch(Exception e)
            {
                //
            }
        }

        private bool _IsPrinting;

        public bool IsPrinting
        {
            get => _IsPrinting;
            set
            {
                if(value == _IsPrinting)
                    return;
                _IsPrinting = value;
                OnPropertyChanged(nameof(IsPrinting));
            }
        }
        
        private ICommand _printSuggestionsCommand;

        public ICommand PrintLogCommand =>
            _printSuggestionsCommand ?? (_printSuggestionsCommand = new DelegateCommand(
            d =>
            {
                IsPrinting = true;
                Task.Factory.StartNew(() =>
                {

                
                if(!(Items.CurrentItem is Client client)) return;

                if(!Directory.Exists("Temp"))
                    Directory.CreateDirectory("Temp");

                var temp = Path.Combine("Temp", $"TerminalLog [{DateTime.Now:yy-MMM-dd}].docx");
                var template = $@"Templates\TerminalLog.docx";

                var fontSize = 0d;
                var borders = new List<double>();
                using(var doc = File.Exists(template) ? DocX.Load(template) : DocX.Create(temp))
                {
                    var tbl = doc.Tables.FirstOrDefault();
                    if(tbl == null)
                    {
                        tbl = doc.AddTable(1, 3);

                        var r = tbl.Rows[0];
                        r.Cells[0].Paragraphs.First().Append("TYPE").Alignment = Alignment.center;
                        r.Cells[1].Paragraphs.First().Append("TIME").Alignment = Alignment.center;
                        r.Cells[2].Paragraphs.First().Append("EVENT").Alignment = Alignment.center;
                    }


                    for(int i = 0; i < tbl.ColumnCount; i++)
                    {
                        borders.Add(tbl.Rows[0].Cells[i].Width);
                    }
                    
                    foreach(Models.TerminalLog item in Logs)
                    {
                        var r = tbl.InsertRow();
                    
                        var p = r.Cells[0].Paragraphs.First().Append(item.Type.ToString());
                        p.LineSpacingAfter = 0;
                        p.Alignment = Alignment.center;
                        
                        p = r.Cells[1].Paragraphs.First().Append(item.Time.ToString("MM-dd-yyyy hh:mm tt"));
                        p.LineSpacingAfter = 0;
                        p.Alignment = Alignment.center;

                        p = r.Cells[2].Paragraphs.First().Append(item.Message);
                        p.LineSpacingAfter = 0;
                        p.Alignment = Alignment.left;
                        
                        for(var i = 0; i < borders.Count; i++)
                        {
                            r.Cells[i].Width = borders[i];
                            //r.Cells[i].Paragraphs.First().FontSize(10);
                        }
                    }
                    
                    if(doc.Tables.Count==0)
                        doc.InsertTable(tbl);

                    doc.ReplaceText("[NAME]", client.Hostname);
                    doc.ReplaceText("[IP]", client.IpAddress);

                    var border = new Xceed.Words.NET.Border(BorderStyle.Tcbs_single, BorderSize.one, 0,
                        System.Drawing.Color.Black);
                    tbl.SetBorder(TableBorderType.Bottom, border);
                    tbl.SetBorder(TableBorderType.Left, border);
                    tbl.SetBorder(TableBorderType.Right, border);
                    tbl.SetBorder(TableBorderType.Top, border);
                    tbl.SetBorder(TableBorderType.InsideV, border);
                    tbl.SetBorder(TableBorderType.InsideH, border);

                    try
                    {
                        File.Delete(temp);
                    } catch(Exception e)
                    {
                        //
                    }

                    doc.SaveAs(temp);
                }

                Print(temp);
                    IsPrinting = false;
                });
            }));

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
