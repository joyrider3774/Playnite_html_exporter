using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Windows;
using Playnite.SDK;
using System.ComponentModel;

namespace HtmlExporterPlugin
{

    public partial class HtmlExporterPluginSettingsView : UserControl
    {

        private readonly HtmlExporterPlugin plugin;
        private ImageOptionsView ConvertImageOptionsView;
        public ImageOptions ConvertImageOptions;


        public HtmlExporterPluginSettingsView()
        {
            InitializeComponent();
        }

        public HtmlExporterPluginSettingsView(HtmlExporterPlugin plugin)
        {
            this.plugin = plugin;
            ConvertImageOptionsView = new ImageOptionsView(this.plugin);
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

        public void DoReset(string context, bool forcesingle = false)
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
                    if (Constants.FakeGameFields.Contains(groupfield) && (groupfield != Constants.PlatformField) &&
                       (groupfield != Constants.AgeRatingField) && (groupfield != Constants.RegionField) &&
                       (groupfield != Constants.LibraryField) && !forcesingle)
                    {
                        PagesDataGrid.Items.Add(plugin.CreatePageObject("default " + context + (String.IsNullOrEmpty(context) ? String.Empty : " ") + "combobox quicklinks", groupfield, true, Constants.NameField, true, true));
                    }
                    else
                    {
                        if (Constants.DefaultDescGroupFields.Contains (groupfield) && !forcesingle)
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

        private void MnuDefaultTableText_Click(object sender, RoutedEventArgs e)
        {
            DoReset("table text", true);
        }

        private void MnuDefaultTable_Click(object sender, RoutedEventArgs e)
        {
            DoReset("table", true);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = !ConvertImageOptionsView.ValidateInput();
            if (e.Cancel)
            {
                plugin.PlayniteApi.Dialogs.ShowMessage(Constants.EnterValidValuesText);
            }
        }

        private void But_CloseClick(object sender, RoutedEventArgs e)
        {
            var myWindow = (Window)ConvertImageOptionsView.Parent;
            myWindow.Close();
        }

       

        private void ButImageOptions_Click(object sender, RoutedEventArgs e)
        {
            var window = plugin.PlayniteApi.Dialogs.CreateWindow(new WindowCreationOptions
            {
                ShowMinimizeButton = false,
                ShowMaximizeButton = false
            });
            

            window.Height = 475;
            window.Width = 800;
            window.Title = Constants.ImageOptionsText;

            // Set content of a window. Can be loaded from xaml, loaded from UserControl or created from code behind

            ConvertImageOptionsView.ButClose.Click += But_CloseClick;

            window.Content = ConvertImageOptionsView;

            window.DataContext = ConvertImageOptions;
            
            // Set owner if you need to create modal dialog window
            window.Owner = plugin.PlayniteApi.Dialogs.GetCurrentAppWindow();
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Closing += Window_Closing;
            // Use Show or ShowDialog to show the window
            window.ShowDialog();

           
        }

    }
}