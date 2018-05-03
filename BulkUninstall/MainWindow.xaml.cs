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

        void SetGridView()
        {
            ListViewSoftware.View.GetValue();
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
                return; //constructor did not run yet, exit.
            }

            await Task.Delay(1000);//wait a second for typing...

            TextBox changed = (TextBox)e.Source;

            SetMatching(changed.Text?.Trim());
        }

        private void SetMatching(string filter)
        {
            _filteredResults.Clear();

            if (_lookupKeyNames.Count() > 100)
            {
                foreach (string match in _lookupKeyNames)
                {
                    if (match.IndexOf(filter, StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        _filteredResults.AddRange(_lookup[match]);
                    }
                }
            }
            else //use concurrent algorithm
            {
                SetMatchingParallel(filter);
            }

            RefreshItemSource();
        }

        private void SetMatchingParallel(string filter)
        {
            var filteredResultsConcurrent = new ConcurrentBag<Software>();

            Parallel.ForEach(_lookupKeyNames, (x) =>
            {
                if (x.IndexOf(filter, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    foreach (Software program in _lookup[x])
                    {
                        filteredResultsConcurrent.Add(program);
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