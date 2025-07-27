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
        private IUninstaller _uninstaller;
        private List<Software> _uninstallItems;
        private List<Software> _filteredResults;
        private ConcurrentDictionary<string, List<Software>> _lookup;
        private string[] _lookupKeyNames;

        public MainWindow()
        {
            InitializeComponent();

            _uninstaller = UninstallerFactory.Create();

            _uninstallItems = _uninstaller.GetInstalledSoftware().OrderBy(x => x.Name).ToList();
            _filteredResults = new List<Software>();
            _lookup = GetDictionary(_uninstallItems);
            _lookupKeyNames = _lookup.Keys.ToArray();

            ListViewSoftware.ItemsSource = _uninstallItems;
        }

        private ConcurrentDictionary<string, List<Software>> GetDictionary(List<Software> uninstallItems)
        {
            var lookup = new ConcurrentDictionary<string, List<Software>>();

            foreach (Software program in uninstallItems)
            {
                if (program.Name == null) continue;

                if (lookup.ContainsKey(program.Name))
                {
                    lookup[program.Name].Add(program);
                }
                else
                {
                    lookup.TryAdd(program.Name, new List<Software> { program });
                }
            }

            return lookup;
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_uninstaller.IsValid())
            {
                var selected = ListViewSoftware.SelectedItems.Cast<Software>();

                _uninstaller.Uninstall(selected);

                foreach (Software program in selected)
                {
                    _uninstallItems.Remove(program);
                    _filteredResults.Remove(program);
                }

                RefreshItemSource();
            }
        }

        private async void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_filteredResults == null)
            {
                return; //constructor did not run yet, exit.
            }

            await Task.Delay(1000);//wait a second for typing...

            TextBox textBox = (TextBox)e.Source;

            SetMatching(textBox.Text?.Trim());
        }

        private void SetMatching(string filter)
        {
            _filteredResults.Clear();


            /*  
             *  if the amount of items is less than 200, simply loop through the items with a normal loop.
             *  if the amount is greater than 200, use a parallel algorithm to improve response time.
             */

            if (_lookupKeyNames.Count() < 200)
            {
                foreach (string match in _lookupKeyNames)
                {
                    if (match.IndexOf(filter, StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        _filteredResults.AddRange(_lookup[match]);
                    }
                }
            }
            else 
            {
                SetMatchingParallel(filter); //use concurrent algorithm
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