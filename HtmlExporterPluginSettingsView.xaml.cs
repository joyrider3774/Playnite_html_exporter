using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Playnite.SDK;
using Playnite.SDK.Plugins;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Globalization;
using System;
using System.Windows;

namespace HtmlExporterPlugin
{
    public class RowNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataGridRow row = (DataGridRow)value;
            if (row == null)
                throw new InvalidOperationException("This converter class can only be used with DataGridRow elements.");

            return row.GetIndex() + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public partial class HtmlExporterPluginSettingsView : UserControl
    {

        private readonly HtmlExporterPlugin plugin;

        public HtmlExporterPluginSettingsView()
        {
            InitializeComponent();
        }

        public HtmlExporterPluginSettingsView(HtmlExporterPlugin plugin)
        {
            this.plugin = plugin;

            InitializeComponent();
        }

        private void ButSelectFolder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TbOutputFolder.Text = plugin.PlayniteApi.Dialogs.SelectFolder();
        }
        
        //Credit to felixkmh https://github.com/felixkmh/DuplicateHider/
        public IEnumerable<CheckBox> Sources { get; set; }
        private void SourceComboBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                comboBox.Items.Clear();
                foreach (var checkbox in Sources.OrderByDescending(o => o.IsChecked).ThenBy(o => o.Content))
                {
                    if (string.IsNullOrEmpty(comboBox.Text) || ((string)checkbox.Content).ToLower().Contains(comboBox.Text.ToLower()))
                        comboBox.Items.Add(checkbox);
                }
                comboBox.IsDropDownOpen = true;
            }
        }


        public IEnumerable<CheckBox> Platforms { get; set; }
        //limit possibilities in combobox when typing
        private void PlatformsComboBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                comboBox.Items.Clear();
                foreach (var checkbox in Platforms.OrderByDescending(o => o.IsChecked).ThenBy(o => o.Content))
                {
                    if (string.IsNullOrEmpty(comboBox.Text) || ((string)checkbox.Content).ToLower().Contains(comboBox.Text.ToLower()))
                        comboBox.Items.Add(checkbox);
                }
                comboBox.IsDropDownOpen = true;
            }
        }

        //sort the combobox
        private void PlatformsComboBox_DropDownClosed(object sender, System.EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                comboBox.Items.Clear();
                foreach (var checkbox in Platforms.OrderByDescending(o => o.IsChecked).ThenBy(o=> o.Content))
                {
                    comboBox.Items.Add(checkbox);
                }                
            }
        }

        private void SourceComboBox_DropDownClosed(object sender, System.EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                comboBox.Items.Clear();
                foreach (var checkbox in Sources.OrderByDescending(o => o.IsChecked).ThenBy(o => o.Content))
                {
                    comboBox.Items.Add(checkbox);
                }
            }
        }

 
        private void BtnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int addedindex = PagesDataGrid.Items.Add(plugin.CreatePageObject(String.Empty, Constants.NameField, true, Constants.NameField, true, true));
            PagesDataGrid.SelectedIndex = addedindex;
           // PagesDataGrid.ScrollIntoView(PagesDataGrid.SelectedItem);
            //to fix row nr
            PagesDataGrid.Items.Refresh();
            PagesDataGrid.ScrollIntoView(PagesDataGrid.SelectedItem);
        }

        private void BtnDel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PagesDataGrid.SelectedItem != null)
            {
                int indexOfSelectedItem = PagesDataGrid.Items.IndexOf(PagesDataGrid.SelectedItem);
                PagesDataGrid.Items.Remove(PagesDataGrid.SelectedItem);
                if (PagesDataGrid.Items.Count > 0)
                {
                    if (PagesDataGrid.Items.Count > indexOfSelectedItem)
                    {
                        PagesDataGrid.SelectedItem = PagesDataGrid.Items[indexOfSelectedItem];
                    }
                    else
                    {
                        if (indexOfSelectedItem - 1 > -1)
                        {
                            PagesDataGrid.SelectedItem = PagesDataGrid.Items[indexOfSelectedItem - 1];
                        }
                    }

                }
            }
            //to fix row nr
            PagesDataGrid.Items.Refresh();
        }

        public void DoReset(string context)
        {
            if (plugin.PlayniteApi.Dialogs.ShowMessage(Constants.RevertPagesQuestion1 + " " + context + " " + Constants.RevertPagesQuestion2,
                Constants.AppName, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                PagesDataGrid.Items.Clear();

                foreach (string groupfield in Constants.AvailableGroupFields)
                {
                    if(groupfield == Constants.NotGroupedField)
                    {
                        continue;
                    }
                    if (Constants.FakeGameFields.Contains(groupfield))
                    {
                        PagesDataGrid.Items.Add(plugin.CreatePageObject("default " + context + (String.IsNullOrEmpty(context) ? String.Empty : " ") + "combobox quicklinks", groupfield, true, Constants.NameField, true, true));
                    }
                    else
                    {
                        if (Constants.DefaultDescGroupFields.Contains (groupfield))
                        {
                            PagesDataGrid.Items.Add(plugin.CreatePageObject("default " + context + (String.IsNullOrEmpty(context) ? String.Empty : " ") + "combobox quicklinks", groupfield, false, Constants.NameField, true, true));
                        }
                        else
                        {
                            PagesDataGrid.Items.Add(plugin.CreatePageObject("default" + (String.IsNullOrEmpty(context) ? String.Empty : " ") + context, groupfield, true, Constants.NameField, true, true));
                        }
                    }
                }
                PagesDataGrid.SelectedItem = PagesDataGrid.Items[0];
                //to fix row nr
                PagesDataGrid.Items.Refresh();
                PagesDataGrid.ScrollIntoView(PagesDataGrid.SelectedItem);
            }
        }
        private void BtnReset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmRevert") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;
        }

        private void PagesDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void BtnMoveUp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PagesDataGrid.SelectedItem != null)
            {
                int indexOfSelectedItem = PagesDataGrid.Items.IndexOf(PagesDataGrid.SelectedItem);
                if (indexOfSelectedItem > 0)
                {
                    Object tmp = PagesDataGrid.Items[indexOfSelectedItem];
                    PagesDataGrid.Items[indexOfSelectedItem] = PagesDataGrid.Items[indexOfSelectedItem - 1];
                    PagesDataGrid.Items[indexOfSelectedItem - 1] = tmp;
                    PagesDataGrid.SelectedItem = PagesDataGrid.Items[indexOfSelectedItem - 1];
                    PagesDataGrid.ScrollIntoView(PagesDataGrid.SelectedItem);
                }
            }
        }

        private void BtnMoveDown_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PagesDataGrid.SelectedItem != null)
            {
                int indexOfSelectedItem = PagesDataGrid.Items.IndexOf(PagesDataGrid.SelectedItem);
                if (indexOfSelectedItem < PagesDataGrid.Items.Count -1)
                {
                    Object tmp = PagesDataGrid.Items[indexOfSelectedItem];
                    PagesDataGrid.Items[indexOfSelectedItem] = PagesDataGrid.Items[indexOfSelectedItem + 1];
                    PagesDataGrid.Items[indexOfSelectedItem + 1] = tmp;
                    PagesDataGrid.SelectedItem = PagesDataGrid.Items[indexOfSelectedItem + 1];
                    PagesDataGrid.ScrollIntoView(PagesDataGrid.SelectedItem);
                }
            }
        }

        private void PagesDataGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PagesDataGrid.Items.Count > 0)
            {
                if (PagesDataGrid.SelectedItem == null)
                {
                    PagesDataGrid.SelectedItem = PagesDataGrid.Items[0];
                }
            }
        }

        private void MnuDefaultGrid_Click(object sender, RoutedEventArgs e)
        {
            DoReset(String.Empty);
        }

        private void MnuDefaultList_Click(object sender, RoutedEventArgs e)
        {
            DoReset("list");
        }

        private void MnuDefaultListText_Click(object sender, RoutedEventArgs e)
        {
            DoReset("list text");
        }
    }
}