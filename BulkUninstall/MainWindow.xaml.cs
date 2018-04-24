using BulkUninstall.Core;
using BulkUninstall.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private Dictionary<string, List<Software>> _lookup;

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

        private Dictionary<string, List<Software>> GetDictionary(List<Software> uninstallItems)
        {
            var lookUp = new Dictionary<string, List<Software>>();

            foreach (Software program in uninstallItems)
            {
                if (lookUp.ContainsKey(program.Name))
                {
                    lookUp[program.Name].Add(program);
                }
                else
                {
                    lookUp.Add(program.Name, new List<Software> { program});
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

            TextBox changed = (TextBox)e.Source;

            string filter = changed.Text?.Trim();
            string[] filtered = FindMatching(filter);
            
            _filteredResults.Clear();

            foreach(string found in filtered)
            {
                _filteredResults.AddRange(_lookup[found]);
            }

            RefreshItemSource();

            await Task.Delay(1000); //wait a second for them to finish typing...
        }

        private string[] FindMatching(string filter)
        {
            var results = new List<string>();

            foreach(var set in _lookupKeyNames)
            {
                if (set.Contains(filter))
                {
                    results.Add(set);
                }
            }

            return results.ToArray();
        }

        private void RefreshItemSource()
        {
            ListViewSoftware.ItemsSource = null; //Item source won't refresh unless the value changes.
            ListViewSoftware.ItemsSource = _filteredResults;
        }
    }
}