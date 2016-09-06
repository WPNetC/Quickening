﻿//------------------------------------------------------------------------------
// <copyright file="AddWebDefaults.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Quickening.Services;
using System.Windows.Forms;
using System.IO;

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
        public const int cmdidTestSubCmd = 0x105;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid guidAddWebDefaultsPackageCmdSet = new Guid("dd7b9b46-7e8c-48a4-992f-905556b5ad36");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

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
                var menuCommandID = new CommandID(guidAddWebDefaultsPackageCmdSet, cmdidAddWebDefaults);
                var menuItem = new MenuCommand(AddWebDefaultsCallback, menuCommandID);
                commandService.AddCommand(menuItem);

                var subCommandID = new CommandID(guidAddWebDefaultsPackageCmdSet, cmdidTestSubCmd);
                var subItem = new MenuCommand(SubItemCallback, subCommandID);
                commandService.AddCommand(subItem);
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
                message = Directory.Exists(ProjectService.TemplatesDirectory) ? ProjectService.TemplatesDirectory : "Error";
                var ps = new ParserService();
                var list = ps.PaseXML(Path.Combine(ProjectService.XmlDirectory, "web-basic.xml"), true);

                // Show a message box to prove we were here
                var result = VsShellUtilities.ShowMessageBox(
                    this.ServiceProvider,
                    message,
                    title,
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }

        }

        private void SubItemCallback(object sender, EventArgs e)
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
