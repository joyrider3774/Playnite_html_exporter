using Newtonsoft.Json;
using Playnite.SDK;
using Playnite.SDK.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.IO;
using System;
using Playnite.SDK.Data;

namespace HtmlExporterPlugin
{
    public class HtmlExporterPluginSettings
    {
        public string OutputFolder { get; set; } = string.Empty;
        public UniqueList<string> ExcludeSources { get; set; } = new UniqueList<string>();
        public UniqueList<string> ExcludePlatforms { get; set; } = new UniqueList<string>();
        public List<PageObject> Pages { get; set; } = new List<PageObject>();
        public bool CopyImages { get; set; } = false;
        public bool ExcludeHiddenGames { get; set; } = true;
        public ImageOptions ConvertImageOptions { get; set; } = new ImageOptions();
    }

    public class HtmlExporterPluginSettingsViewModel : ObservableObject, ISettings
    { 
        private readonly HtmlExporterPlugin plugin;
        private HtmlExporterPluginSettings editingClone { get; set; }

        private HtmlExporterPluginSettings settings;
        public HtmlExporterPluginSettings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }

        public List<string> AvailableTemplateFolders => plugin.TemplateFolders;
        public List<string> AvailableSortFields { get; set; } = Constants.AvailableSortFields.AsQueryable().OrderBy(o => Constants.GetNameFromField(o, false)).ToList();
        public List<string> AvailableGroupFields { get; set; } = Constants.AvailableGroupFields.AsQueryable().OrderBy(o => Constants.GetNameFromField(o, false)).ToList();

        

        // Playnite serializes settings object to a JSON object and saves it as text file.
        // If you want to exclude some property from being saved then use `JsonIgnore` ignore attribute.
        //  [JsonIgnore]
        // public bool OptionThatWontBeSaved { get; set; } = false;

        public HtmlExporterPluginSettingsViewModel(HtmlExporterPlugin plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<HtmlExporterPluginSettings>();

            // LoadPluginSettings returns null if not saved data is available.
            if (savedSettings != null)
            {
                Settings = savedSettings;
                //new option might not exits
                if (Settings.ConvertImageOptions is null)
                {
                    Settings.ConvertImageOptions = new ImageOptions();
                }
            }
            else
            {
                Settings = new HtmlExporterPluginSettings();
                DoResetPages();
            }
        }

        private void DoResetPages()
        {
            foreach (string groupfield in Constants.AvailableGroupFields)
            {
                if (groupfield == Constants.NotGroupedField)
                {
                    continue;
                }

                if (Constants.FakeGameFields.Contains(groupfield))
                {
                    Settings.Pages.Add(plugin.CreatePageObject("default list text combobox quicklinks", groupfield, true, Constants.NameField, true, true));
                }
                else
                {
                    if (Constants.DefaultDescGroupFields.Contains(groupfield))
                    {
                        Settings.Pages.Add(plugin.CreatePageObject("default list text combobox quicklinks", groupfield, false, Constants.NameField, true, true));
                    }
                    else
                    {
                        Settings.Pages.Add(plugin.CreatePageObject("default list text", groupfield, true, Constants.NameField, true, true));
                    }
                }
            }
        }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
            editingClone = Serialization.GetClone(Settings);

            plugin.SettingsView.ConvertImageOptions = Settings.ConvertImageOptions;

            foreach (PageObject page in Settings.Pages)
            {
                plugin.SettingsView.PagesDataGrid.Items.Add(page);
            }

            //Credit to felixkmh https://github.com/felixkmh/DuplicateHider/
            plugin.SettingsView.SourceComboBox.Items.Dispatcher.Invoke(() =>
            {
                List<CheckBox> checkBoxes = new List<CheckBox>();
                foreach (var source in plugin.PlayniteApi.Database.Sources.AsQueryable().OrderBy(o => (o != null) ? o.Name : null).Concat(new List<GameSource> { null }))
                {
                    string sourceName = source != null ? source.Name : Constants.UndefinedString;
                    if (Settings.ExcludeSources.Contains(sourceName))
                    {
                        var cb = new CheckBox { Content = sourceName, Tag = source };
                        cb.IsChecked = true;
                        checkBoxes.Add(cb);
                        plugin.SettingsView.SourceComboBox.Items.Add(cb);
                    }
                }
                foreach (var source in plugin.PlayniteApi.Database.Sources.AsQueryable().OrderBy(o => (o != null) ? o.Name : null).Concat(new List<GameSource> { null }))
                {
                    string sourceName = source != null ? source.Name : Constants.UndefinedString;
                    if (!Settings.ExcludeSources.Contains(sourceName))
                    {
                        var cb = new CheckBox { Content = sourceName, Tag = source };
                        cb.IsChecked = false;
                        checkBoxes.Add(cb);
                        plugin.SettingsView.SourceComboBox.Items.Add(cb);
                    }
                }
                plugin.SettingsView.Sources = checkBoxes;
            });

            plugin.SettingsView.PlatformsComboBox.Items.Dispatcher.Invoke(() =>
            {
                List<CheckBox> checkBoxes = new List<CheckBox>();
                foreach (var platform in plugin.PlayniteApi.Database.Platforms.AsQueryable().OrderBy(o => (o != null) ? o.Name : null).Concat(new List<Platform> { null }))
                {
                    string platformName = platform != null ? platform.Name : Constants.UndefinedString;
                    if (Settings.ExcludeSources.Contains(platformName))
                    {
                        CheckBox cb = new CheckBox { Content = platformName, Tag = platform };
                        cb.IsChecked = true;
                        checkBoxes.Add(cb);
                        plugin.SettingsView.PlatformsComboBox.Items.Add(cb);
                    }
                }
                foreach (var platform in plugin.PlayniteApi.Database.Platforms.AsQueryable().OrderBy(o => (o != null) ? o.Name : null).Concat(new List<Platform> { null }))
                {
                    string platformName = platform != null ? platform.Name : Constants.UndefinedString;
                    if (!Settings.ExcludeSources.Contains(platformName))
                    {
                        var cb = new CheckBox { Content = platformName, Tag = platform };
                        cb.IsChecked = false;
                        checkBoxes.Add(cb);
                        plugin.SettingsView.PlatformsComboBox.Items.Add(cb);
                    }
                }
                plugin.SettingsView.Platforms = checkBoxes;
            });
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
            Settings = editingClone;
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.

            Settings.ConvertImageOptions = plugin.SettingsView.ConvertImageOptions;

            //Credit to felixkmh https://github.com/felixkmh/DuplicateHider/
            plugin.SettingsView.SourceComboBox.Items.Dispatcher.Invoke(() =>
            {
                foreach (CheckBox cb in plugin.SettingsView.SourceComboBox.Items)
                {
                    string name = cb.Content as string;
                    if (cb.IsChecked ?? false)
                    {
                        Settings.ExcludeSources.AddMissing(name);
                    }
                    else
                    {
                        Settings.ExcludeSources.Remove(name);
                    }

                }
            });

            plugin.SettingsView.PlatformsComboBox.Items.Dispatcher.Invoke(() =>
            {
                foreach (CheckBox cb in plugin.SettingsView.PlatformsComboBox.Items)
                {
                    string name = cb.Content as string;
                    if (cb.IsChecked ?? false)
                    {
                        Settings.ExcludePlatforms.AddMissing(name);
                    }
                    else
                    {
                        Settings.ExcludePlatforms.Remove(name);
                    }

                }
            });

            Settings.Pages.Clear();
            foreach (PageObject page in plugin.SettingsView.PagesDataGrid.Items)
            {
                Settings.Pages.Add(page);
            }

            plugin.SavePluginSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.

            bool returnvalue = true;
            errors = new List<string>();

            if (String.IsNullOrEmpty(Settings.OutputFolder) || (!Directory.Exists(Settings.OutputFolder)))
            {
                returnvalue = false;
                errors.Add(Constants.ErrorHTMLExpoterNoOutputFolder);
            }

            return returnvalue;
        }        
    }
}