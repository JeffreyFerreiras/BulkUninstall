using BulkUninstall.Core;
using BulkUninstall.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tools.Extensions.Validation;
using Tools.DataStructures;

namespace BulkUninstall
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Software> _uninstallItems;
        private List<Software> _filteredResults;
        private Dictionary<string, Software> _lookup;
        private PrefixTree _prefixTree;
        

        public List<Software> UninstallItems { get => _uninstallItems; set => _uninstallItems = value; }

        public MainWindow()
        {
            InitializeComponent();

            var unistaller = UninstallerFactory.Create();

            UninstallItems = unistaller.GetInstalledSoftware().OrderBy(x => x.Name).ToList();

            _filteredResults = new List<Software>();
            _prefixTree = new PrefixTree(UninstallItems.Select(x => x.Name));
            _lookup = GetDictionary(UninstallItems);

            ListViewSoftware.ItemsSource = UninstallItems;
        }

        private Dictionary<string, Software> GetDictionary(List<Software> uninstallItems)
        {
            throw new NotImplementedException();
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void FilterTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var changed = (TextBox)e.Source;

            string filter = changed.Text?.Trim();

            if(_prefixTree.IsValid())
            {
                var filtered = _prefixTree.Search(filter);

                if (_filteredResults.Any())
                {
                    _filteredResults.Clear();
                }

                foreach(var found in filtered)
                {
                    _filteredResults.Add(_lookup[found]);
                }

                _filteredResults.Sort();

                ListViewSoftware.ItemsSource = _filteredResults;
            }
        }
    }
}