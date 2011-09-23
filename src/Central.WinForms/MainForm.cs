﻿/*
 * Copyright 2010-2011 Bastian Eicher
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Common;
using Common.Collections;
using Common.Controls;
using Common.Storage;
using Common.Utils;
using ZeroInstall.Central.WinForms.Properties;
using ZeroInstall.Injector;
using ZeroInstall.Injector.Solver;
using ZeroInstall.Model;

namespace ZeroInstall.Central.WinForms
{
    /// <summary>
    /// The main GUI for Zero Install.
    /// </summary>
    internal partial class MainForm : Form
    {
        #region Variables
        private Policy _policy;

        /// <summary>The version number of the newest available update; <see langword="null"/> if no update is available.</summary>
        private ImplementationVersion _selfUpdateVersion;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the main GUI.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            HandleCreated += delegate
            {
                Program.ConfigureTaskbar(this, Text, null, null);

                var cacheLink = new ShellLink(buttonCacheManagement.Text.Replace("&", ""), Path.Combine(Locations.InstallBase, StoreExe + ".exe"), null);
                var configLink = new ShellLink(buttonConfiguration.Text.Replace("&", ""), Path.Combine(Locations.InstallBase, CommandsExe + ".exe"), "config");
                WindowsUtils.AddTaskLinks(Program.AppUserModelID, new[] {cacheLink, configLink});
            };

            browserNewApps.CanGoBackChanged += delegate { toolStripButtonBack.Enabled = browserNewApps.CanGoBack; };
        }
        #endregion

        #region Startup
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Locations.IsPortable) Text += " - Portable mode";
            labelVersion.Text = "v" + Application.ProductVersion;

            // ToDo: Check if the user has any MyApps entries, before showing the "new apps" page
            tabControlApps.SelectedTab = tabPageNewApps;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            try
            {
                _policy = Policy.CreateDefault(new SilentHandler());
            }
                #region Error handling
            catch (IOException ex)
            {
                Msg.Inform(this, ex.Message, MsgSeverity.Error);
                Close();
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                Msg.Inform(this, ex.Message, MsgSeverity.Error);
                Close();
                return;
            }
            catch (InvalidDataException ex)
            {
                Msg.Inform(this, ex.Message + (ex.InnerException == null ? "" : "\n" + ex.InnerException.Message), MsgSeverity.Error);
                Close();
                return;
            }
            #endregion

            browserNewApps.Navigate(_policy.Config.AppStoreHome.Replace("[LANG]", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName));

            // Don't check for updates when launched as a Zero Install implementation
            string topDir = Path.GetFileName(Locations.InstallBase) ?? Locations.InstallBase;
            if (_policy.Config.SelfUpdateEnabled && !(topDir.StartsWith("sha") && topDir.Contains("=")))
                selfUpdateWorker.RunWorkerAsync();
        }
        #endregion

        //--------------------//

        #region Self-update
        private void selfUpdateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _selfUpdateVersion = UpdateUtils.CheckSelfUpdate(_policy);
            }
                #region Error handling
            catch (UserCancelException)
            {}
            catch (IOException ex)
            {
                Log.Warn("Unable to perform self-update check:\n" + ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Warn("Unable to perform self-update check:\n" + ex.Message);
            }
            catch (SolverException ex)
            {
                Log.Warn("Unable to perform self-update check:\n" + ex.Message);
            }
            #endregion
        }

        private void selfUpdateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_selfUpdateVersion == null || !Visible) return;
            if (Msg.Ask(this, string.Format(Resources.SelfUpdateAvailable, _selfUpdateVersion), MsgSeverity.Info, Resources.SelfUpdateYes, Resources.SelfUpdateNo))
            {
                try
                {
                    ProcessUtils.LaunchHelperAssembly("0install-win", "self-update");
                    Application.Exit();
                }
                    #region Error handling
                catch (FileNotFoundException ex)
                {
                    Msg.Inform(this, string.Format(Resources.FailedToRun + "\n" + ex.Message, "0install-win"), MsgSeverity.Error);
                }
                catch (Win32Exception ex)
                {
                    Msg.Inform(this, string.Format(Resources.FailedToRun + "\n" + ex.Message, "0install-win"), MsgSeverity.Error);
                }
                #endregion
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            while (selfUpdateWorker.IsBusy)
                Application.DoEvents();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Attempts to launch a .NET helper assembly in the application's base directory. Displays friendly error messages if something goes wrong.
        /// </summary>
        /// <param name="assembly">The name of the assembly to launch (without the file extension).</param>
        /// <param name="arguments">The command-line arguments to pass to the assembly.</param>
        private void LaunchHelperAssembly(string assembly, string arguments)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(assembly)) throw new ArgumentNullException("assembly");
            #endregion

            try
            {
                ProcessUtils.LaunchHelperAssembly(assembly, arguments);
            }
                #region Error handling
            catch (FileNotFoundException ex)
            {
                Msg.Inform(this, string.Format(Resources.FailedToRun + "\n" + ex.Message, assembly), MsgSeverity.Error);
            }
            catch (Win32Exception ex)
            {
                Msg.Inform(this, string.Format(Resources.FailedToRun + "\n" + ex.Message, assembly), MsgSeverity.Error);
            }
            #endregion
        }

        /// <summary>
        /// Opens a URL in the system's default browser.
        /// </summary>
        /// <param name="url">The URL to open.</param>
        private void OpenInBrowser(string url)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("uri");
            #endregion

            try
            {
                Process.Start(url);
            }
                #region Error handling
            catch (FileNotFoundException ex)
            {
                Msg.Inform(this, ex.Message, MsgSeverity.Error);
            }
            catch (Win32Exception ex)
            {
                Msg.Inform(this, ex.Message, MsgSeverity.Error);
            }
            #endregion
        }
        #endregion

        #region Launch feed
        /// <summary>
        /// Attempts to launch a feed and closes the main window.
        /// </summary>
        /// <param name="feedUri">The URI of the feed to be launched.</param>
        private void LaunchFeed(string feedUri)
        {
            LaunchHelperAssembly("0install-win", "run --gui --no-wait " + StringUtils.EscapeArgument(feedUri));
        }
        #endregion

        //--------------------//

        #region Toolbar
        private void toolStripButtonBack_Click(object sender, EventArgs e)
        {
            browserNewApps.GoBack();
        }
        #endregion

        #region Browser
        /// <summary>A URL postfix that indicates that the URL points to an installable Zero Install feed.</summary>
        private const string UrlPostfixFeed = "#0install-feed";

        /// <summary>A URL postfix that indicates that the URL should be opened in an external browser.</summary>
        private const string UrlPostfixBrowser = "#external-browser";

        private void browserNewApps_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            switch (e.Url.Fragment)
            {
                case UrlPostfixFeed:
                    e.Cancel = true;

                    string feedUri = e.Url.AbsoluteUri.Replace(UrlPostfixFeed, "");

                    // ToDo: Display more details about the feed
                    //if (Msg.Ask(this, "Do you want to launch this application?\n" + feedUri, MsgSeverity.Info, "Yes\nLaunch the application", "No\nGo back to the list"))
                    LaunchFeed(feedUri);
                    break;

                case UrlPostfixBrowser:
                    e.Cancel = true;
                    OpenInBrowser(e.Url.AbsoluteUri.Replace(UrlPostfixBrowser, ""));
                    break;
            }
        }

        private void browserNewApps_NewWindow(object sender, CancelEventArgs e)
        {
            // Prevent any popups
            e.Cancel = true;
        }
        #endregion

        #region Tools
        /// <summary>
        /// The EXE name (without the file ending) for the Windows Commands binary.
        /// </summary>
        private const string CommandsExe = "0install-win";

        /// <summary>
        /// The EXE name (without the file ending) for the Windows Store Management binary.
        /// </summary>
        private const string StoreExe = "0store-win";

        private void buttonLaunchInterface_Click(object sender, EventArgs e)
        {
            string interfaceID = InputBox.Show(null, "Zero Install", Resources.EnterInterfaceUrl);
            if (string.IsNullOrEmpty(interfaceID)) return;

            LaunchHelperAssembly(CommandsExe, "run --gui " + StringUtils.EscapeArgument(interfaceID));
        }

        private void buttonCacheManagement_Click(object sender, EventArgs e)
        {
            LaunchHelperAssembly(StoreExe, null);
        }

        private void buttonConfiguration_Click(object sender, EventArgs e)
        {
            LaunchHelperAssembly(CommandsExe, "config");
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            OpenInBrowser("http://0install.de/help/");
        }
        #endregion

        #region Drag and drop handling
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                LaunchFeed(EnumerableUtils.GetFirst(files));
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
                LaunchFeed((string)e.Data.GetData(DataFormats.Text));
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(DataFormats.FileDrop))
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }
        #endregion
    }
}
