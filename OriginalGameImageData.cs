using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlExporterPlugin
{
    class OriginalGameImageData
    {

        public string CoverImageFile { get; set; } = String.Empty;
        public string BackgroundImageFile { get; set; } = String.Empty;
        public string IconImageFile { get; set; } = String.Empty;
        public long CoverImageSize { get; set; } = -1;
        public long BackgroundImageSize { get; set; } = -1;
        public long IconImageSize { get; set; } = 0;
        public DateTime IconImageLastWriteTime { get; set; } = DateTime.FromFileTimeUtc(0);
        public DateTime CoverImageLastWriteTime { get; set; } = DateTime.FromFileTimeUtc(0);
        public DateTime BackgroundImageLastWriteTime { get; set; } = DateTime.FromFileTimeUtc(0);


        public OriginalGameImageData()
        {
        }

        public void SetCoverImageData(string CoverImageFilename)
        {
            CoverImageFile = Path.GetFileName(CoverImageFilename);
            if (File.Exists(CoverImageFilename))
            {
                FileInfo info = new FileInfo(CoverImageFilename);
                CoverImageSize = info.Length;
                CoverImageLastWriteTime = info.LastWriteTime;                
            }
            else
            {
                CoverImageSize = -1;
                CoverImageLastWriteTime = DateTime.FromFileTimeUtc(0);
            }
        }

        public void SetBackgroundImageData(string BackgroundImageFilename)
        {
            BackgroundImageFile = Path.GetFileName(BackgroundImageFilename);
            if (File.Exists(BackgroundImageFilename))
            {
                FileInfo info = new FileInfo(BackgroundImageFilename);
                BackgroundImageSize = info.Length;
                BackgroundImageLastWriteTime = info.LastWriteTime;
            }
            else
            {
                BackgroundImageSize = -1;
                BackgroundImageLastWriteTime = DateTime.FromFileTimeUtc(0);
            }
        }
        
        public void SetIconImageData (string IconImageFilename)
        {
            IconImageFile = Path.GetFileName(IconImageFilename);
            if (File.Exists(IconImageFilename))
            {
                FileInfo info = new FileInfo(IconImageFilename);
                IconImageSize = info.Length;
                IconImageLastWriteTime = info.LastWriteTime;
            }
            else
            {
                IconImageSize = -1;
                IconImageLastWriteTime = DateTime.FromFileTimeUtc(0);
            }
        }

        public void SetAllData(string CoverImageFilename, string BackgroundImageFilename, string IconImageFilename)
        {
            SetIconImageData(IconImageFilename);
            SetBackgroundImageData(BackgroundImageFilename);
            SetCoverImageData(CoverImageFilename);           
        }

        public bool CoverImageSame(string filename)
        {
            DateTime LastWriteTime = File.Exists(filename) ? new FileInfo(filename).LastWriteTime : DateTime.FromFileTimeUtc(0);
            return LastWriteTime.Equals(CoverImageLastWriteTime);
        }

        public bool BackgroundImageSame(string filename)
        {
            DateTime LastWriteTime = File.Exists(filename) ? new FileInfo(filename).LastWriteTime : DateTime.FromFileTimeUtc(0);
            return LastWriteTime.Equals(BackgroundImageLastWriteTime);
        }

        public bool IconImageSame(string filename)
        {
            DateTime LastWriteTime = File.Exists(filename) ? new FileInfo(filename).LastWriteTime : DateTime.FromFileTimeUtc(0);
            return LastWriteTime.Equals(IconImageLastWriteTime);
        }

    }


}
