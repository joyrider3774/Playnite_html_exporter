using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlExporterPlugin
{
    public class ImageOptions
    {
        public string ImageMagickLocation { get; set; } = String.Empty;
        public bool ConvertToJpg { get; set; } = false;
        public bool ConvertToPng { get; set; } = false;
        public string JpgQuality { get; set; } = "85";

        public bool ResizeCoverImage { get; set; } = false;
        public string CoverImageWidth { get; set; } = "180";
        public string CoverImageHeight { get; set; } = "270";

        public bool ResizeBackgroundImage { get; set; } = false;
        public string BackgroundImageWidth { get; set; } = "1280";
        public string BackgroundImageHeight { get; set; } = "720";

        public bool ResizeIconImage { get; set; } = false;
        public string IconImageWidth { get; set; } = "48";
        public string IconImageHeight { get; set; } = "48";

        public bool ForceConversion { get; set; } = false;

        public string MaxTasks { get; set; } = "1";

        public bool AlwaysProcess { get; set; } = false;

        public bool DetectDuplicates { get; set; } = false;

        private bool ImageMagickFound { get; set; } = false;

        public string GetUniqueString(bool includeMaxTasks = false, bool includeAlwaysProcess = false)
        {
            return ConvertToJpg.ToString() + '_' + ConvertToPng.ToString() + '_' + JpgQuality.ToString() + '_' +
                ResizeCoverImage.ToString() + '_' + CoverImageWidth + '_' + CoverImageHeight + '_' +
                ResizeBackgroundImage.ToString() + '_' + BackgroundImageWidth + '_' + BackgroundImageHeight + '_' +
                ResizeIconImage.ToString() + '_' + IconImageWidth + '_' + IconImageHeight + '_' +
                ForceConversion.ToString() + '_' + DetectDuplicates.ToString() + (includeMaxTasks ? '_' + MaxTasks.ToString() : String.Empty) +
                (includeAlwaysProcess ? '_' + AlwaysProcess.ToString() : String.Empty);
        }

        public ImageOptions()
        {
            CheckForImageMagick();
        }

        public bool CheckForImageMagick()
        {
            ImageMagickFound = !String.IsNullOrEmpty(ImageMagickLocation) && File.Exists(ImageMagickLocation);
            return ImageMagickFound;
        }

        public bool BackgroundNeedsConversion(string filename)
        {
            return ImageMagickFound && (ConvertToJpg && (!Path.GetExtension(filename).ToLower().Equals(".jpg") || ForceConversion) || ResizeBackgroundImage);
        }

        public bool IconNeedsConversion(string filename)
        {            
            return ImageMagickFound && (ConvertToPng && (!Path.GetExtension(filename).ToLower().Equals(".png") || ForceConversion) || ResizeIconImage);
        }

        public bool CoverNeedsConversion(string filename)
        {
            return ImageMagickFound && (ConvertToJpg && (!Path.GetExtension(filename).ToLower().Equals(".jpg") || ForceConversion) || ResizeCoverImage);
        }

        public string CoverDestFilename(string filename)
        {
            if (!ConvertToJpg || !ImageMagickFound)
            {
                return filename;
            }
            else
            {
                return Path.ChangeExtension(filename, ".jpg");
            }
        }

        public string BackgroundDestFilename(string filename)
        {
            if (!ConvertToJpg || !ImageMagickFound)
            {
                return filename;
            }
            else
            {
                return Path.ChangeExtension(filename, ".jpg");
            }
        }

        public string IconDestFilename(string filename)
        {
            if (!ConvertToPng || !ImageMagickFound)
            {
                return filename;
            }
            else
            {
                return Path.ChangeExtension(filename, ".png");               
            }
        }

        private string MakeOptions(string SourceFileExt, string DestFileExt, bool NeedsResize, string Width, string Height, bool ConvertToJpg, bool ConvertToPng, string JpegQuality)
        {            
            string Result = string.Empty;
            string SourceExt = SourceFileExt.ToLower();
            //only icon converts to png but it seems gif files have same problem and playnite also allows ico and gif to be set for background and cover.
            //This can still fail if the files got no extensions like some files do in playnite
            if (ConvertToPng || SourceExt.Equals(".gif") || SourceExt.Equals(".ico") || SourceExt.Equals(".tiff"))
            {
                //converts multiple images inside an ico to a single image otherwise imagemagick generates multiple files
                //credits https://legacy.imagemagick.org/discourse-server/viewtopic.php?p=164113#p164113
                Result = "( -clone 0--1 -layers Merge ) -channel A -evaluate Multiply \" %[fx: w == u[-1].w ? 1 : 0] % \"  +channel +delete -background None -layers merge ";
            }

            if (DestFileExt.ToLower().Equals(".jpg"))
            {
                if (!string.IsNullOrEmpty(Result))
                {
                    Result += " ";
                }
                Result +=  "-interlace plane -quality " + JpegQuality;
            }
          
            //> only larger images to resize
            if (NeedsResize)
            {
                if (!string.IsNullOrEmpty(Result))
                {
                    Result += " ";
                }
                Result += "-resize " + Width + "x" + Height + ">";
            }
            return Result;
        }

        public string BackgroundOptions(string SourceFileExt, string DestFileExt)
        {
            return MakeOptions(SourceFileExt, DestFileExt, ResizeBackgroundImage, BackgroundImageWidth, BackgroundImageHeight, ConvertToJpg, false, JpgQuality);
        }

        public string CoverOptions(string SourceFileExt, string DestFileExt)
        {
            return MakeOptions(SourceFileExt, DestFileExt, ResizeCoverImage, CoverImageWidth, CoverImageHeight, ConvertToJpg, false, JpgQuality);
        }

        public string IconOptions(string SourceFileExt, string DestFileExt)
        {
            return MakeOptions(SourceFileExt, DestFileExt, ResizeIconImage, IconImageWidth, IconImageHeight, false, ConvertToPng, JpgQuality);
        }
    }
}
