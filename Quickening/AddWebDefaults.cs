//------------------------------------------------------------------------------
// <copyright file="AddWebDefaults.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Quickening.Globals;
using Quickening.Services;
using Quickening.Views;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Quickening
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AddWebDefaults
    {
        /// <summary>
        /// Command IDs.
        /// </summary>
        public const int cmdidAddWebDefaults = 0x0100;

        public const int cmdidAddAngularModule = 0x105;
        public const int cmdidAddAngularController = 0x106;
        public const int cmdidAddAngularDirective = 0x107;
        public const int cmdidAddAngularService = 0x108;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid guidAddWebDefaultsPackageCmdSet = new Guid("dd7b9b46-7e8c-48a4-992f-905556b5ad36");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        private CommandID webDefaultsCommandID;
        private CommandID angularModuleCommandID;
        private CommandID angularControllerCommandID;
        private CommandID angularServiceCommandID;
        private CommandID angularDirectiveCommandID;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddWebDefaults"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private AddWebDefaults(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                webDefaultsCommandID = new CommandID(guidAddWebDefaultsPackageCmdSet, cmdidAddWebDefaults);
                var awdMenuItem = new MenuCommand(AddWebDefaultsCallback, webDefaultsCommandID);
                commandService.AddCommand(awdMenuItem);

                angularModuleCommandID = new CommandID(guidAddWebDefaultsPackageCmdSet, cmdidAddAngularModule);
                var aamMenuItem = new MenuCommand(AddAngularModuleCallback, angularModuleCommandID);
                commandService.AddCommand(aamMenuItem);

                angularControllerCommandID = new CommandID(guidAddWebDefaultsPackageCmdSet, cmdidAddAngularController);
                var aacMenuItem = new MenuCommand(AddAngularControllerCallback, angularControllerCommandID);
                commandService.AddCommand(aacMenuItem);

                angularServiceCommandID = new CommandID(guidAddWebDefaultsPackageCmdSet, cmdidAddAngularService);
                var aasMenuItem = new MenuCommand(AddAngularServiceCallback, angularServiceCommandID);
                commandService.AddCommand(aasMenuItem);

                angularDirectiveCommandID = new CommandID(guidAddWebDefaultsPackageCmdSet, cmdidAddAngularDirective);
                var aadMenuItem = new MenuCommand(AddAngularDirectiveCallback, angularDirectiveCommandID);
                commandService.AddCommand(aadMenuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AddWebDefaults Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new AddWebDefaults(package);
        }

        private void AddWebDefaultsCallback(object sender, EventArgs e)
        {
            var title = "Add Web Defaults";
            var message = "Add default web structure?";

            var dr = MessageBox.Show(message, title, MessageBoxButtons.YesNo);

            if (dr == DialogResult.Yes)
            {
                string resultTitle = "";
                string resultMessage = "";
                OLEMSGICON icon;
                var defaultFile = Strings.DefaultXmlFile;
                var path = Path.Combine(Strings.LayoutsDirectory, defaultFile);

                // Check we have a default XML file.
                if (string.IsNullOrEmpty(defaultFile))
                {
                    resultTitle = "Warning";
                    resultMessage = "No default layout file selected. Open configurator and select one.";
                    icon = OLEMSGICON.OLEMSGICON_WARNING;
                }
                // Check the file exists.
                else if (!File.Exists(path))
                {
                    resultTitle = "Error";
                    resultMessage = "Default layout file not found on disk: " + path;
                    icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                }
                else
                {
                    // Try and create structure.
                    try
                    {
                        var ps = new ParserService();
                        var list = ps.PaseXML(path, true);

                        resultTitle = "Success";
                        resultMessage = "File structure created.";
                        icon = OLEMSGICON.OLEMSGICON_INFO;
                    }
                    catch(Exception ex)
                    {
                        resultTitle = "Exception";
                        resultMessage = ex.Message;
                        icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                    }
                }

                VsShellUtilities.ShowMessageBox(
                    this.ServiceProvider,
                    resultMessage,
                    resultTitle,
                    icon,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }

        private void AddAngularModuleCallback(object sender, EventArgs e)
        {
            IVsUIShell uiShell = (IVsUIShell)this.ServiceProvider.GetService(
                typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            uiShell.ShowMessageBox(
                0,
                ref clsid,
                "TestCommand",
                string.Format(CultureInfo.CurrentCulture,
                "Inside TestCommand.SubItemCallback()",
                this.ToString()),
                string.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_INFO,
                0,
                out result);
        }

        private void AddAngularControllerCallback(object sender, EventArgs e)
        {
            var layout = "NgController.xml";
            var path = Path.Combine(Strings.AngularDirectory, layout);

            using (var tbp = new TextInputPopUp("Filename", "Module", "Controller Name", "Function Name"))
            {
                var dr = tbp.ShowDialog();

                if (dr == true)
                {
                    var fileName = tbp.Values[0][1];
                    var modName = tbp.Values[1][1];
                    var ctrlName = tbp.Values[2][1];
                    var fnName = tbp.Values[3][1];
                }
            } 
        }

        private void AddAngularServiceCallback(object sender, EventArgs e)
        {
            IVsUIShell uiShell = (IVsUIShell)this.ServiceProvider.GetService(
                typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            uiShell.ShowMessageBox(
                0,
                ref clsid,
                "TestCommand",
                string.Format(CultureInfo.CurrentCulture,
                "Inside TestCommand.SubItemCallback()",
                this.ToString()),
                string.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_INFO,
                0,
                out result);
        }

        private void AddAngularDirectiveCallback(object sender, EventArgs e)
        {
            IVsUIShell uiShell = (IVsUIShell)this.ServiceProvider.GetService(
                typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            uiShell.ShowMessageBox(
                0,
                ref clsid,
                "TestCommand",
                string.Format(CultureInfo.CurrentCulture,
                "Inside TestCommand.SubItemCallback()",
                this.ToString()),
                string.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_INFO,
                0,
                out result);
        }
    }
}