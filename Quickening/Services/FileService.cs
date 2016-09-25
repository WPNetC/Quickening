using Quickening.Globals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickening.Services
{
    internal static class FileService
    {
        public static void SaveNewXmlFile(string directory)
        {
            var sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = Strings.LAYOUT_FILE_FILTER;
            sfd.DefaultExt = ".xml";
            sfd.InitialDirectory = directory;

            var result = sfd.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                case System.Windows.Forms.DialogResult.Yes:
                    var path = sfd.FileName;
                    if (string.IsNullOrEmpty(path))
                        return;

                    // Ensure we copy to the Xml directory.
                    if (Path.GetDirectoryName(path) != directory)
                    {
                        path = Path.Combine(directory, Path.GetFileName(sfd.FileName));
                    }

                    int num = 0;
                    while (File.Exists(path))
                    {
                        // If file already exists increment a version number to prevent exception.
                        path = Path.Combine(directory, $"{sfd.FileName.Replace(".xml", "")}_{++num}.xml");
                    }

                    // Write base tag to new file.
                    File.WriteAllText(path, Strings.BaseXmlText);

                    var dr2 = System.Windows.MessageBox.Show($"Layout file created: {path}{Environment.NewLine}Copy path to clipboard?.",
                            "Success",
                            System.Windows.MessageBoxButton.YesNo);

                    if (dr2 == System.Windows.MessageBoxResult.Yes)
                        System.Windows.Forms.Clipboard.SetText(path);
                    break;
                default:
                    return;
            }
        }

        public static void ImportExportXmlFile(string value, string directory, string fileName = null)
        {
            if (value.ToLower() == "import")
            {
                var ofd = new System.Windows.Forms.OpenFileDialog();
                ofd.Filter = Strings.LAYOUT_FILE_FILTER;
                var result = ofd.ShowDialog();
                switch (result)
                {
                    case System.Windows.Forms.DialogResult.OK:
                    case System.Windows.Forms.DialogResult.Yes:
                        var path = Path.Combine(directory, ofd.SafeFileName);
                        int num = 0;
                        while (File.Exists(path))
                        {
                            // If file already exists increment a version number to prevent exception.
                            path = Path.Combine(directory, $"{ofd.SafeFileName.Replace(".xml", "")}_{++num}.xml");
                        }
                        File.Copy(ofd.FileName, path);
                        break;
                    default:
                        return;
                }
            }
            else if (value.ToLower() == "export")
            {
                var sfd = new System.Windows.Forms.SaveFileDialog();
                sfd.Filter = Strings.LAYOUT_FILE_FILTER;
                var result = sfd.ShowDialog();
                switch (result)
                {
                    case System.Windows.Forms.DialogResult.OK:
                    case System.Windows.Forms.DialogResult.Yes:
                        var path = sfd.FileName;
                        if (string.IsNullOrEmpty(path))
                            return;

                        int num = 0;
                        while (File.Exists(path))
                        {
                            // If file already exists increment a version number to prevent exception.
                            path = Path.Combine(directory, $"{sfd.FileName.Replace(".xml", "")}_{++num}.xml");
                        }
                        var xmlFile = Path.Combine(directory, fileName);
                        if (!File.Exists(xmlFile))
                        {
                            var dr = System.Windows.MessageBox.Show(
                                $"Cannot find source file: {xmlFile}{Environment.NewLine}Copy path to clipboard?.",
                                "Error",
                                System.Windows.MessageBoxButton.YesNo,
                                System.Windows.MessageBoxImage.Error);

                            if (dr == System.Windows.MessageBoxResult.Yes)
                                System.Windows.Forms.Clipboard.SetText(xmlFile);

                            return;
                        }

                        File.Copy(xmlFile, path);
                        var dr2 = System.Windows.MessageBox.Show($"Layout file exported to: {xmlFile}{Environment.NewLine}Copy path to clipboard?.",
                                "Success",
                                System.Windows.MessageBoxButton.YesNo);

                        if (dr2 == System.Windows.MessageBoxResult.Yes)
                            System.Windows.Forms.Clipboard.SetText(xmlFile);
                        break;
                    default:
                        return;
                }
            }
            else
            {
                throw new InvalidOperationException("Could not understand parameter: " + value);
            }
        }
    }
}
