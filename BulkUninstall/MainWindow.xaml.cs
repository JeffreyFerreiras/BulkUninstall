using BulkUninstall.Core;
using BulkUninstall.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BulkUninstall
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IUninstaller _uninstaller;
        private List<Software> _installedSoftware;
        private List<Software> _filteredSoftware;
        private ConcurrentDictionary<string, List<Software>> _softwareLookup;
        private string[] _softwareNames;

        public MainWindow()
        {
            InitializeComponent();

            _uninstaller = UninstallerFactory.Create();

            _installedSoftware = _uninstaller.GetInstalledSoftware().OrderBy(s => s.Name).ToList();
            _filteredSoftware = new List<Software>();
            _softwareLookup = BuildSoftwareLookup(_installedSoftware);
            _softwareNames = _softwareLookup.Keys.ToArray();

            ListViewSoftware.ItemsSource = _installedSoftware;
        }

        private ConcurrentDictionary<string, List<Software>> BuildSoftwareLookup(List<Software> installedSoftware)
        {
            var softwareLookup = new ConcurrentDictionary<string, List<Software>>();

            foreach (var software in installedSoftware)
            {
                if (software.Name == null) continue;

                if (softwareLookup.ContainsKey(software.Name))
                {
                    softwareLookup[software.Name].Add(software);
                }
                else
                {
                    softwareLookup.TryAdd(software.Name, new List<Software> { software });
                }
            }

            return softwareLookup;
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_uninstaller.IsValid())
            {
                var selectedSoftware = ListViewSoftware.SelectedItems.Cast<Software>().ToList();

                _uninstaller.Uninstall(selectedSoftware);

                foreach (var software in selectedSoftware)
                {
                    _installedSoftware.Remove(software);
                    _filteredSoftware.Remove(software);
                }

                RefreshSoftwareListView();
            }
        }

        private async void FilterTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_filteredSoftware == null)
                return;

            await Task.Delay(1000); // Wait for typing...

            var textBox = (TextBox)e.Source;
            ApplyFilter(textBox.Text?.Trim());
        }

        private void ApplyFilter(string filter)
        {
            _filteredSoftware.Clear();

            if (_softwareNames.Length < 200)
            {
                foreach (var name in _softwareNames)
                {
                    if (name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        _filteredSoftware.AddRange(_softwareLookup[name]);
                    }
                }
            }
            else
            {
                ApplyFilterParallel(filter);
            }

            RefreshSoftwareListView();
        }

        private void ApplyFilterParallel(string filter)
        {
            var concurrentFilteredSoftware = new ConcurrentBag<Software>();

            Parallel.ForEach(_softwareNames, name =>
            {
                if (name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    foreach (var software in _softwareLookup[name])
                    {
                        concurrentFilteredSoftware.Add(software);
                    }
                }
            });

            _filteredSoftware = concurrentFilteredSoftware.OrderBy(s => s.Name).ToList();
        }

        private void RefreshSoftwareListView()
        {
            ListViewSoftware.ItemsSource = null; // Force refresh
            ListViewSoftware.ItemsSource = _filteredSoftware;
        }
    }
}