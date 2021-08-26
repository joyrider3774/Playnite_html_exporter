using System;
using System.Collections.Generic;
using Playnite.SDK.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlExporterPlugin
{
    class OriginalImageDataFile
    {
        public class ImageDataFileModel
        {
            public Dictionary<string, OriginalGameImageData> OriginalImageDataDict { get; set; } = new Dictionary<string, OriginalGameImageData>();
            public string ImageOptionsString { get; set; } = String.Empty;
            public string ExportPath { get; set; } = String.Empty;
        }

        private ImageDataFileModel Data = new ImageDataFileModel();

        public bool LoadFromFile(string filename)
        {
            string ImageDataFile = Path.Combine(filename);
            if (File.Exists(filename))
            {
                Data = Serialization.FromJsonFile<ImageDataFileModel>(filename);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            Data.OriginalImageDataDict.Clear();
        }

        public OriginalGameImageData GetOriginalGameImageData(string GameId)
        {
            if (Data.OriginalImageDataDict.ContainsKey(GameId))
            {
                return Data.OriginalImageDataDict[GameId];
            }
            else
            {
                return new OriginalGameImageData();
            }
        }

        public bool SameSettingsUsed (HtmlExporterPluginSettings SettingsUsed)
        {
            return SettingsUsed.OutputFolder.Equals(Data.ExportPath) && SettingsUsed.ConvertImageOptions.GetUniqueString().Equals(Data.ImageOptionsString);
        }

        public void SetOriginalGameImageData(string GameId, OriginalGameImageData GameImageData)
        {
            Data.OriginalImageDataDict[GameId] = GameImageData;
        }

        public void SaveToFile(string filename, HtmlExporterPluginSettings SettingsUsed)
        {
            Data.ImageOptionsString = SettingsUsed.ConvertImageOptions.GetUniqueString();
            Data.ExportPath = SettingsUsed.OutputFolder;
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            FileStream fs = File.OpenWrite(filename);
            Serialization.ToJsonSteam(Data, fs, true);
        }
    }
}
