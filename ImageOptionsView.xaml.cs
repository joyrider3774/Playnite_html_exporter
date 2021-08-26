using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.IO;

namespace HtmlExporterPlugin
{
    public partial class ImageOptionsView : UserControl
    {
        private readonly HtmlExporterPlugin plugin;
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public ImageOptionsView()
        {
            InitializeComponent();
        }

        public ImageOptionsView(HtmlExporterPlugin plugin)
        {
            this.plugin = plugin;
            InitializeComponent();
        }

        private bool ValidWidthHeight(string Text)
        {
            int userVal;
            if (int.TryParse(Text, out userVal))
            {
                return (userVal >= 1) && (userVal <= 10000);
            }
            else
            {
                return false;
            }
        }

        private bool ValidNumber(string Text, int minval, int maxval)
        {
            int userVal;
            if (int.TryParse(Text, out userVal))
            {
                return (userVal >= minval) && (userVal <= maxval);
            }
            else
            {
                return false;
            }
        }

        public bool ValidateInput()
        {
            return ValidNumber(tbMaxTasks.Text, 1, 16) && ValidNumber(tbJpgQuality.Text,1, 100) && ValidWidthHeight(tbBackgroundImageHeight.Text) && ValidWidthHeight(tbBackgroundImageWidth.Text) &&
                ValidWidthHeight(tbCoverImageHeight.Text) && ValidWidthHeight(tbCoverImageWidth.Text) && ValidWidthHeight(tbIconImageHeight.Text) &&
                ValidWidthHeight(tbIconImageWidth.Text);
        }

        string MagickInstallPath(RegistryView view)
        {
            string Result = String.Empty;
            RegistryKey Key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
            if (Key != null)
            {
                RegistryKey SubKey = Key.OpenSubKey("Software\\ImageMagick\\Current");
                if (SubKey != null)
                {
                    object RegInstallDir = SubKey.GetValue("BinPath");
                    if (RegInstallDir != null)
                    {
                        Result = RegInstallDir.ToString();
                        if (Directory.Exists(Result))
                        {
                            Result = System.IO.Path.Combine(Result, "magick.exe");
                        }
                    }
                }
            }

            return !File.Exists(Result) ? String.Empty : Result;
        }

        private void ButDetect_Click(object sender, RoutedEventArgs e)
        {
            string tmp = MagickInstallPath(RegistryView.Registry64);
            if (String.IsNullOrEmpty(tmp))
            {
                tmp = MagickInstallPath(RegistryView.Registry32);
            }

            if (!String.IsNullOrEmpty(tmp))
            {
                TbImageMagickLocation.Text = tmp;
            }
        }

        private void ButSelectExe_Click(object sender, RoutedEventArgs e)
        {
            string tmp = plugin.PlayniteApi.Dialogs.SelectFile("Exe Files|*.exe");
            if (File.Exists(tmp))
            {
                TbImageMagickLocation.Text = tmp;
            }
        }
    }
}