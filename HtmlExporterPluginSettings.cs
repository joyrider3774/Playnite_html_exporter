using Newtonsoft.Json;
using Playnite.SDK;
using Playnite.SDK.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.IO;
using System;

namespace HtmlExporterPlugin
{
    public class HtmlExporterPluginSettings : ISettings
    {
        private readonly HtmlExporterPlugin plugin;
        private HtmlExporterPluginSettings EditDataSettings;

        [JsonIgnore]
        public List<string> AvailableTemplateFolders => plugin.TemplateFolders;
        [JsonIgnore]
        public List<string> AvailableSortFields { get; set; } = Constants.AvailableSortFields.AsQueryable().OrderBy(o => Constants.GetNameFromField(o, false)).ToList();
        [JsonIgnore]
        public List<string> AvailableGroupFields { get; set; } = Constants.AvailableGroupFields.AsQueryable().OrderBy(o => Constants.GetNameFromField(o, false)).ToList();

        public string OutputFolder { get; set; } = string.Empty;
        public UniqueList<string> ExcludeSources { get; set; } = new UniqueList<string>();
        public UniqueList<string> ExcludePlatforms { get; set; } = new UniqueList<string>();
        public List<PageObject> Pages { get; set; } = new List<PageObject>();
        public bool CopyImages { get; set; } = false;
        public bool ExcludeHiddenGames { get; set; } = true;
        public ImageOptions ConvertImageOptions { get; set; } = new ImageOptions();

        // Playnite serializes settings object to a JSON object and saves it as text file.
        // If you want to exclude some property from being saved then use `JsonIgnore` ignore attribute.
        //  [JsonIgnore]
        // public bool OptionThatWontBeSaved { get; set; } = false;

        // Parameterless constructor must exist if you want to use LoadPluginSettings method.
        public HtmlExporterPluginSettings()
        {
        }

        public HtmlExporterPluginSettings(HtmlExporterPlugin plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<HtmlExporterPluginSettings>();

            // LoadPluginSettings returns null if not saved data is available.
            if (savedSettings != null)
            {
                //new option might not exits
                if (savedSettings.ConvertImageOptions is null)
                {
                    savedSettings.ConvertImageOptions = new ImageOptions();
                } 

                RestoreSettings(savedSettings);
            }
            else
            {
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
                    Pages.Add(plugin.CreatePageObject("default list text combobox quicklinks", groupfield, true, Constants.NameField, true, true));
                }
                else
                {
                    if (Constants.DefaultDescGroupFields.Contains(groupfield))
                    {
                        Pages.Add(plugin.CreatePageObject("default list text combobox quicklinks", groupfield, false, Constants.NameField, true, true));
                    }
                    else
                    {
                        Pages.Add(plugin.CreatePageObject("default list text", groupfield, true, Constants.NameField, true, true));
                    }
                }
            }
        }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
            EditDataSettings = new HtmlExporterPluginSettings(plugin);

            plugin.SettingsView.ConvertImageOptions = ConvertImageOptions;

            foreach (PageObject page in Pages)
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
                    if (ExcludeSources.Contains(sourceName))
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
                    if (!ExcludeSources.Contains(sourceName))
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
                    if (ExcludeSources.Contains(platformName))
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
                    if (!ExcludeSources.Contains(platformName))
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
            RestoreSettings(EditDataSettings);
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.

            ConvertImageOptions = plugin.SettingsView.ConvertImageOptions;

            //Credit to felixkmh https://github.com/felixkmh/DuplicateHider/
            plugin.SettingsView.SourceComboBox.Items.Dispatcher.Invoke(() =>
            {
                foreach (CheckBox cb in plugin.SettingsView.SourceComboBox.Items)
                {
                    string name = cb.Content as string;
                    if (cb.IsChecked ?? false)
                    {
                        ExcludeSources.AddMissing(name);
                    }
                    else
                    {
                        ExcludeSources.Remove(name);
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
                        ExcludePlatforms.AddMissing(name);
                    }
                    else
                    {
                        ExcludePlatforms.Remove(name);
                    }

                }
            });

            Pages.Clear();
            foreach (PageObject page in plugin.SettingsView.PagesDataGrid.Items)
            {

                Pages.Add(page);
            }

            plugin.SavePluginSettings(this);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.

            bool returnvalue = true;
            errors = new List<string>();

            if ((String.IsNullOrEmpty(OutputFolder)) || (!Directory.Exists(OutputFolder)))
            {
                returnvalue = false;
                errors.Add(Constants.ErrorHTMLExpoterNoOutputFolder);
            }

            return returnvalue;
        }

        private void RestoreSettings(HtmlExporterPluginSettings source)
        {
            OutputFolder = source.OutputFolder;
            ExcludeSources = source.ExcludeSources;
            ExcludePlatforms = source.ExcludePlatforms;
            CopyImages = source.CopyImages;
            Pages = source.Pages;
            ExcludeHiddenGames = source.ExcludeHiddenGames;
            ConvertImageOptions = source.ConvertImageOptions;
        }
    }
}