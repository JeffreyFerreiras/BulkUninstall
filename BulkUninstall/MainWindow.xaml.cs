using BulkUninstall.Core;
using BulkUninstall.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tools.Extensions.Validation;

namespace BulkUninstall
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IUninstaller _unistaller;
        private List<Software> _uninstallItems;
        private List<Software> _filteredResults;

        private ConcurrentDictionary<string, List<Software>> _lookup;

        private string[] _lookupKeyNames;

        public MainWindow()
        {
            InitializeComponent();

            _unistaller = UninstallerFactory.Create();

            _uninstallItems = _unistaller.GetInstalledSoftware().OrderBy(x => x.Name).ToList();
            _filteredResults = new List<Software>();
            _lookup = GetDictionary(_uninstallItems);
            _lookupKeyNames = _lookup.Keys.ToArray();

            ListViewSoftware.ItemsSource = _uninstallItems;
        }

        private ConcurrentDictionary<string, List<Software>> GetDictionary(List<Software> uninstallItems)
        {
            var lookUp = new ConcurrentDictionary<string, List<Software>>();

            foreach (Software program in uninstallItems)
            {
                if (program.Name == null) continue;

                if (lookUp.ContainsKey(program.Name))
                {
                    lookUp[program.Name].Add(program);
                }
                else
                {
                    lookUp.TryAdd(program.Name, new List<Software> { program });
                }
            }

            return lookUp;
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_unistaller.IsValid())
            {
                var selected = ListViewSoftware.SelectedItems.Cast<Software>();

                _unistaller.Uninstall(selected);

                foreach (Software program in selected)
                {
                    _uninstallItems.Remove(program);
                    _filteredResults.Remove(program);
                }

                RefreshItemSource();
            }
        }

        private async void FilterTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_filteredResults == null)
            {
                return; //constructor not run yet, exit.
            }

            await Task.Delay(1000);//wait a second for them to finish typing...

            TextBox changed = (TextBox)e.Source;

            string filter = changed.Text?.Trim();

            _filteredResults.Clear();

            if (_lookupKeyNames.Count() > 100)
            {
                SetMatchingParallel(filter);
            }
            else
            {
                SetMatching(filter);
            }

            RefreshItemSource();
        }

        private void SetMatching(string filter)
        {
            foreach (string match in _lookupKeyNames)
            {
                if (match.IndexOf(filter, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    _filteredResults.AddRange(_lookup[match]);
                }
            }
        }

        private void SetMatchingParallel(string filter)
        {
            var filteredResultsConcurrent = new ConcurrentBag<Software>();

            Parallel.ForEach(_lookupKeyNames, (x) =>
            {
                if (x.IndexOf(filter, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    foreach (var prog in _lookup[x])
                    {
                        filteredResultsConcurrent.Add(prog);
                    }
                }
            });

            _filteredResults = filteredResultsConcurrent.OrderBy(x => x.Name).ToList();
        }

        private void RefreshItemSource()
        {
            ListViewSoftware.ItemsSource = null; //Item source won't refresh unless the value changes.
            ListViewSoftware.ItemsSource = _filteredResults;
        }
    }
}