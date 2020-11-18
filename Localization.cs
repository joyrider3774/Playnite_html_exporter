using Playnite.SDK;
using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace HtmlExporterPlugin
{

    //based on code from lacro59 from 
    //https://github.com/Lacro59/playnite-plugincommon/blob/master/Localization.cs
    //
    public class Localization
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        public static void SetPluginLanguage(string pluginFolder, string language, bool DefaultLoad = false)
        {
            // Load default for missing
            if (!DefaultLoad)
            {
                SetPluginLanguage(pluginFolder, "LocSource", true);
            }
            
            var dictionaries = Application.Current.Resources.MergedDictionaries;
            var langFile = Path.Combine(pluginFolder, "Localization\\" + language + ".xaml");

            // Load localization
            if (File.Exists(langFile))
            {
                ResourceDictionary res = null;
                try
                {
                    using (var stream = new StreamReader(langFile))
                    {
                        res = (ResourceDictionary)XamlReader.Load(stream.BaseStream);
                        res.Source = new Uri(langFile, UriKind.Absolute);
                    }
                    
                    foreach (var key in res.Keys)
                    {
                        if (res[key] is string locString && String.IsNullOrEmpty(locString))
                        {
                            res.Remove(key);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"Failed to parse localization file {langFile}.");
                    return;
                }

                dictionaries.Add(res);
            }
            else
            {
                logger.Warn($"File {langFile} not found.");
            }
        }
    }
}
