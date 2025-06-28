/* Copyright (c) 2025 Elbitin
 *
 * This file is part of RAAS Client.
 *
 * RAAS Client is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * RAAS Client is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along with RAAS Client. If not, see <https://www.gnu.org/licenses/>.
 */
﻿// Copyright (c) Elbitin
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Timers;
using Elbitin.Applications.RAAS.RAASClient.Models;
using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.RAASClient.Helpers;
using System.Text;
using static Elbitin.Applications.RAAS.Common.Helpers.Win32Helper;

namespace Elbitin.Applications.RAAS.RAASClient.RemoteApps
{
    public partial class VisualizationsForm : Form
    {
        private bool running = true;
        private System.Timers.Timer updateTimer = new System.Timers.Timer();
        private Version win8version = new Version(6, 2, 9200, 0);
        private List<IntPtr> noOverlayHWnds = new List<IntPtr>();
        private bool visualizationsActive = false;
        private List<IntPtr> overlayWindows = new List<IntPtr>();
        private Dictionary<IntPtr, OverlayForm> windowOverlays = new Dictionary<IntPtr, OverlayForm>();
        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private bool visualizationsEnabled = true;
        private bool connectionBarEnabled = true;
        private Color connectionBarColor = Color.Black;
        private String connectionBarText = "";
        private double connectionBarOpacity = 0;
        private Color connectionBarTextColor = Color.Black;
        private Color overlaysColor = Color.Black;
        private bool frames = false;
        private Color linesColor = Color.Black;
        private FileSystemWatcher configWatcherVisualizations;
        private FileSystemWatcher configWatcherServers;
        private System.Threading.ReaderWriterLockSlim connectionBarDictionaryLock = new System.Threading.ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private System.Threading.ReaderWriterLockSlim connectionBarFormLock = new System.Threading.ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private object windowsLock = new object();
        private object hWndsLock = new object();
        private String serverName;
        private String serverDisplayName;
        private Dictionary<int, WindowsHook> windowsHooks = new Dictionary<int, WindowsHook>();
        private List<IntPtr> hWnds = new List<IntPtr>();
        private static bool connectionbarActive = false;
        private bool connectionBarInitialized = false;
        private Dictionary<int,CONNECTIONBAR> connectionBars = new Dictionary<int, CONNECTIONBAR>();
        private const int WINDOWRECT_SURROUNDSPACE_X = 4;
        private const int WINDOWRECT_SURROUNDSPACE_Y = 4;
        private const int UPDATETIMER_INTERVAL_MS = 100;
        private const int SUBSEQUENT_IN_FOCUS_COUNT = 30;
        private const int SUBSEQUENT_OUT_OF_FOCUS_COUNT = 1;
        private const int SUBSEQUENT_SCAN_OVERLAYS_COUNT = 50;
        private static int subsequentInFocusCount = 0;
        private static int subsequentOutOfFocusCount = 0;
        private static int subsequentScanOverlaysCount = 0;
        private static SpinLock showConnectionsBarLock = new SpinLock();
        private static SpinLock hideConnectionBarsLock = new SpinLock();
        private hideConnectionBarsEventCallbackHandler callbackHandlerHideConnectionBars;
        private showConnectionBarsEventCallbackHandler callbackHandlerShowConnectionBars;
        private setInvisibleWindowEventCallbackHandler callbackHandlerSetInvisibleWindow;
        private lostFocusEventCallbackHandler callbackHandlerlostFocus;
        private gotFocusEventCallbackHandler callbackHandlergotFocus;
        private windowPosChangedEventCallbackHandler callbackHandlerWindowPosChanged;
        private updateOverlayEventCallbackHandler callbackHandlerNewOverlay;
        private updateSettingsEventCallbackHandler callbackHandlerUpdateSettings;
        private delegate void activateInvokedDelegate();
        private addHookDelegate hookDel = null;
        private delegate void deactivateInvokedDelegate();
        private addHookEventCallbackHandler callbackHandlerAddHook;
        private delegate void addHookEventCallbackHandler(int threadId);
        private event addHookEventCallbackHandler addHookEvent;
        private delegate void addHookDelegate(int threadId);
        private delegate void setInvisibleWindowEventCallbackHandler(IntPtr hWnd);
        private event setInvisibleWindowEventCallbackHandler setInvisibleWindowEvent;
        private delegate void showConnectionBarsEventCallbackHandler(bool visible, bool force);
        private event showConnectionBarsEventCallbackHandler showConnectionBarsEvent;
        private forgetHwndEventCallbackHandler callbackHandlerForgetHwnd;
        private delegate void forgetHwndEventCallbackHandler(IntPtr hWnd);
        private event forgetHwndEventCallbackHandler forgetHwndEvent;
        private ignoreHwndEventCallbackHandler callbackHandlerIgnoreHwnd;
        private delegate void ignoreHwndEventCallbackHandler(IntPtr hWnd);
        private event ignoreHwndEventCallbackHandler ignoreHwndEvent;
        private delegate void hideConnectionBarsEventCallbackHandler(bool force);
        private event hideConnectionBarsEventCallbackHandler hideConnectionBarsEvent;
        private delegate void updateSettingsEventCallbackHandler();
        private event updateSettingsEventCallbackHandler updateSettingsEvent;
        private delegate void updateOverlayEventCallbackHandler(IntPtr hWnd);
        private event updateOverlayEventCallbackHandler updateOverlayEvent;
        private delegate void windowPosChangedEventCallbackHandler(IntPtr hWnd, uint flags);
        private event windowPosChangedEventCallbackHandler windowPosChangedEvent;
        private delegate void gotFocusEventCallbackHandler();
        private event gotFocusEventCallbackHandler gotFocusEvent;
        private delegate void lostFocusEventCallbackHandler();
        private event lostFocusEventCallbackHandler lostFocusEvent;


        struct CONNECTIONBAR
        {
            public ConnectionBarForm top;
            public ConnectionBarForm bottom;
        }

        public VisualizationsForm(String serverName)
        {
            this.serverName = serverName;
            visualizationsActive = false;
            SetFormProperties();
            RAASClientPathHelper.CreateMissingAppDataRAASClientDirectories();
            RegisterEventHandlers();
            SaveCurrentProcessID();
            InitializeGUI();
            UpdateConfigWatchers();
            InitializeComponent();
            SetNoOverlayForMainForm();
            HookCurrentThread();
            Hide();
            InitializeUpdateTimer();
            Show();
        }

        private void InitializeGUI()
        {
            UpdateSettings();
        }

        private void InitializeUpdateTimer()
        {
            updateTimer.Interval = UPDATETIMER_INTERVAL_MS;
            updateTimer.Elapsed += UpdateTimer_Elapsed;
            updateTimer.Enabled = true;
            updateTimer.AutoReset = false;
            updateTimer.Start();
        }

        private void HookCurrentThread()
        {
            hookDel = new addHookDelegate(AddHook);
            addHookEvent.Invoke(Thread.CurrentThread.ManagedThreadId);
        }

        private void SetNoOverlayForMainForm()
        {
            ignoreHwndEvent.Invoke(Handle);
        }

        private static void SaveCurrentProcessID()
        {
            List<int> CurrentProcessIds = new List<int>();
            CurrentProcessIds.Add(Process.GetCurrentProcess().Id);
        }

        private void RegisterEventHandlers()
        {
            callbackHandlerHideConnectionBars = new hideConnectionBarsEventCallbackHandler(HideConnectionBars);
            hideConnectionBarsEvent += callbackHandlerHideConnectionBars;
            callbackHandlerAddHook = new addHookEventCallbackHandler(AddHook);
            addHookEvent += callbackHandlerAddHook;
            callbackHandlerForgetHwnd = new forgetHwndEventCallbackHandler(ForgetHWnd);
            forgetHwndEvent += callbackHandlerForgetHwnd;
            callbackHandlerIgnoreHwnd = new ignoreHwndEventCallbackHandler(IgnoreHWnd);
            ignoreHwndEvent += callbackHandlerIgnoreHwnd;
            callbackHandlerShowConnectionBars = new showConnectionBarsEventCallbackHandler(ShowConnectionBars);
            showConnectionBarsEvent += callbackHandlerShowConnectionBars;
            callbackHandlerSetInvisibleWindow = new setInvisibleWindowEventCallbackHandler(SetInvisibleWindow);
            setInvisibleWindowEvent += callbackHandlerSetInvisibleWindow;
            callbackHandlerlostFocus = new lostFocusEventCallbackHandler(AppsLostFocus); ;
            lostFocusEvent += callbackHandlerlostFocus;
            callbackHandlergotFocus = new gotFocusEventCallbackHandler(AppsGotFocus); ;
            gotFocusEvent += callbackHandlergotFocus;
            callbackHandlerWindowPosChanged = new windowPosChangedEventCallbackHandler(WindowPosChanged); ;
            windowPosChangedEvent += callbackHandlerWindowPosChanged;
            callbackHandlerNewOverlay = new updateOverlayEventCallbackHandler(UpdateOverlay); ;
            updateOverlayEvent += callbackHandlerNewOverlay;
            callbackHandlerUpdateSettings = new updateSettingsEventCallbackHandler(UpdateSettings);
            updateSettingsEvent += callbackHandlerUpdateSettings;
        }

        private void SetFormProperties()
        {
            Visible = false;
            WindowState = FormWindowState.Minimized;
            Opacity = 0;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            CheckForIllegalCrossThreadCalls = false;
        }

        // Prevent window from being activated
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams Params = base.CreateParams;
                Params.ExStyle |= (int)Win32Helper.WindowStyles.WS_EX_TOOLWINDOW;
                return Params;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            running = false;
            updateTimer.Stop();
            RemoveHooks();
            base.OnFormClosing(e);
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        private static IntPtr SetWindowLongPtr(IntPtr hWnd, Win32Helper.GWLParameter nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return Win32Helper.SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(Win32Helper.SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            var handles = new List<IntPtr>();
            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                Win32Helper.EnumThreadWindows(thread.Id, (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);
            return handles;
        }

        private void RemoveOverlayForHWnd(IntPtr hWnd)
        {
            if (hWnds.Contains(hWnd))
                hWnds.Remove(hWnd);
            try
            {
                // Find overlays for targeted window and close them
                foreach (IntPtr hWndOverlay in windowOverlays.Keys)
                {
                    if (hWndOverlay != hWnd)
                        continue;
                    OverlayForm overlayForm = windowOverlays[hWndOverlay];

                    // Overlays found, now close them and remove reference
                    overlayForm.Opacity = 0;
                    overlayForm.Close();
                    windowOverlays.Remove(hWndOverlay);
                }
            }
            catch { }
            if (overlayWindows.Contains(hWnd))
                overlayWindows.Remove(hWnd);
        }

        private void ForgetHWnd(IntPtr hWnd)
        {
            if (noOverlayHWnds.Contains(hWnd))
                noOverlayHWnds.Remove(hWnd);
            RemoveOverlayForHWnd(hWnd);
        }

        public void IgnoreHWnd(IntPtr hWnd)
        {
            noOverlayHWnds.Add(hWnd);
            RemoveOverlayForHWnd(hWnd);
        }

        public void ActivateVisualizations()
        {
            Invoke(new activateInvokedDelegate(ActivateInvoked));
        }

        public void DeactivateVisualizations()
        {
            Invoke(new deactivateInvokedDelegate(DeactivateInvoked));
        }

        private void RemoveHooks()
        {
            foreach (KeyValuePair<int, WindowsHook> hook in windowsHooks.ToArray())
            {
                hook.Value.Dispose();
                windowsHooks.Remove(hook.Key);
            }
        }

        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            updateTimer.Stop();
            try
            {
                if (running)
                {
                    // Update focus subsequently to prevent flickering
                    UpdateFocusSubsequently();

                    // Add overlays subsequently for windows that exist but that dont have overlays
                    AddNewOverlaysForWindowsSubsequently();
                }
            }
            catch { };
            if (running)
                updateTimer.Start();
        }

        private void UpdateFocusSubsequently()
        {
            // Get foreground window handle
            IntPtr foregroundWindow = Win32Helper.GetForegroundWindow();

            // Update connection bars if needed
            if (hWnds.Contains(foregroundWindow) || (foregroundWindow == IntPtr.Zero && connectionbarActive))
            {
                subsequentOutOfFocusCount = 0;
                if (!connectionbarActive)
                {
                    subsequentInFocusCount++;
                    if (subsequentOutOfFocusCount >= SUBSEQUENT_OUT_OF_FOCUS_COUNT)
                    {
                        showConnectionBarsEvent.Invoke(true, false);
                        subsequentInFocusCount = 0;
                    }
                }
            }
            else
            {
                subsequentInFocusCount = 0;
                if (connectionbarActive)
                {
                    subsequentOutOfFocusCount++;
                    if (subsequentOutOfFocusCount >= SUBSEQUENT_IN_FOCUS_COUNT)
                    {
                        hideConnectionBarsEvent.Invoke(false);
                        subsequentOutOfFocusCount = 0;
                    }
                }
            }
        }

        private void AddNewOverlaysForWindowsSubsequently()
        {
            subsequentScanOverlaysCount++;
            if (subsequentScanOverlaysCount >= SUBSEQUENT_SCAN_OVERLAYS_COUNT)
            {
                subsequentScanOverlaysCount = 0;
                foreach (var hWnd in EnumerateProcessWindowHandles(Process.GetCurrentProcess().Id))
                {
                    if (!noOverlayHWnds.Contains(hWnd) && !overlayWindows.Contains(hWnd))
                    {
                        System.Int64 windowStyle = Win32Helper.GetWindowLong((IntPtr)hWnd, (int)Win32Helper.GWLParameter.GWL_STYLE);
                        System.Int64 windowStyleEx = Win32Helper.GetWindowLong((IntPtr)hWnd, (int)Win32Helper.GWLParameter.GWL_EXSTYLE);
                        long lWindowStyle = windowStyle;
                        if ((windowStyle & (int)Win32Helper.WindowStyles.WS_VISIBLE) != 0 && (windowStyleEx & (int)Win32Helper.WindowStyles.WS_EX_TRANSPARENT) == 0 && !noOverlayHWnds.Contains((IntPtr)hWnd))
                        {
                            if (!hWnds.Contains((IntPtr)hWnd))
                            {
                                lock (hWnds)
                                    hWnds.Add((IntPtr)hWnd);
                            }
                            updateOverlayEvent.Invoke((IntPtr)hWnd);
                        }
                    }
                }
            }
        }

        public void UpdateThreads()
        {
            Process currentProcess = Process.GetCurrentProcess();
            if (hookDel != null)
            {
                ProcessThreadCollection currentThreads = currentProcess.Threads;

                // Remove missing threads
                foreach (int threadId in windowsHooks.Keys.ToList())
                {
                    bool threadFound = false;
                    foreach (ProcessThread th in currentThreads)
                        if (th.Id == threadId)
                        {
                            threadFound = true;
                        }
                    if (!threadFound)
                        RemoveHook(threadId);
                }

                // Add new threads
                foreach (ProcessThread th in currentThreads)
                    if (!windowsHooks.Keys.Contains(th.Id))
                        Hook(th.Id);
            }
        }

        private void FileSystemWatcher_OnChange(object sender, FileSystemEventArgs e)
        {
            updateSettingsEvent.Invoke();
        }

        private void UpdateConfigWatchers()
        {
            // Create path for RAAS Server settings if needed
            String raasClientPath = RAASClientPathHelper.GetAppDataRAASClientPath();
            if (!Directory.Exists(raasClientPath))
                Directory.CreateDirectory(raasClientPath);
            String serverPath = RAASClientPathHelper.GetServerAppDataRAASClientPath(serverName);
            if (!Directory.Exists(serverPath))
                Directory.CreateDirectory(serverPath);
            String visualizationsPath = RAASClientPathHelper.GetVisualizationsConfigFilePath(serverName);

            // If Visual Aids settings file have not been created, use defaults
            if (!File.Exists(visualizationsPath))
            {
                File.Copy(RAASClientPathHelper.GetDefaultVisualizationsPath(), visualizationsPath);
            }

            // Create watcher for Visual Aids settings file
            configWatcherVisualizations = new FileSystemWatcher(Path.GetDirectoryName(visualizationsPath));
            configWatcherVisualizations.IncludeSubdirectories = false;
            configWatcherVisualizations.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
            configWatcherVisualizations.Created += new FileSystemEventHandler(FileSystemWatcher_OnChange);
            configWatcherVisualizations.Deleted += new FileSystemEventHandler(FileSystemWatcher_OnChange);
            configWatcherVisualizations.Changed += new FileSystemEventHandler(FileSystemWatcher_OnChange);
            configWatcherVisualizations.EnableRaisingEvents = true;

            // Create watcher for RAAS Server settings file
            configWatcherServers = new FileSystemWatcher(serverPath, Path.GetFileName(RAASClientPathHelper.GetServersConfigFilePath()));
            configWatcherServers.IncludeSubdirectories = false;
            configWatcherServers.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
            configWatcherServers.Created += new FileSystemEventHandler(FileSystemWatcher_OnChange);
            configWatcherServers.Deleted += new FileSystemEventHandler(FileSystemWatcher_OnChange);
            configWatcherServers.Changed += new FileSystemEventHandler(FileSystemWatcher_OnChange);
            configWatcherServers.EnableRaisingEvents = true;
        }

        private void Hook(int id)
        {
            try
            {
                if (hookDel != null)
                {
                    if (IsHandleCreated)
                        Invoke(hookDel, id);
                    else
                        hookDel.Invoke(id);
                }
            }
            catch { }
        }

        private void RemoveHook(int threadId)
        {
            windowsHooks.Remove(threadId);
        }

        private void ActivateInvoked()
        {
            visualizationsActive = true;
            foreach (CONNECTIONBAR fullConnectionBar in connectionBars.Values)
            {
                fullConnectionBar.top.Visible = true;
                fullConnectionBar.bottom.Visible = true;
            }
            UpdateSettings();
        }

        private void DeactivateInvoked()
        {
            visualizationsActive = false;
            foreach (CONNECTIONBAR fullConnectionBar in connectionBars.Values)
            {
                fullConnectionBar.top.Visible = false;
                fullConnectionBar.bottom.Visible = false;
            }
            hideConnectionBarsEvent.Invoke(true);
        }

        private void AddHook(int threadId)
        {
            WindowsHook wndHook = new WindowsHook(this, threadId, serverName);
            windowsHooks.Add(threadId, wndHook);
        }

        private void SetInvisibleWindow(IntPtr hWnd)
        {
            // Find overlays for the window and hide them
            foreach (IntPtr hWndOverlay in windowOverlays.Keys)
            {
                if (hWndOverlay != hWnd)
                    continue;
                OverlayForm overlayForm = windowOverlays[hWndOverlay];

                // Overlays found, now hide them
                overlayForm.Opacity = 0;
            }
        }

        private void HideConnectionBars(bool force = false)
        {
            if (!connectionbarActive && !force)
                return;
            if (hideConnectionBarsLock.IsHeldByCurrentThread)
                return;
            bool gotLock = false;
            try
            {
                hideConnectionBarsLock.TryEnter(ref gotLock);
                if (gotLock)
                {
                    if (!connectionbarActive && !force)
                        return;
                    try
                    {
                        connectionBarFormLock.EnterWriteLock();

                        // Hide connection bar on each display
                        for (int i = 0; i < connectionBars.Count(); i++)
                        {
                            connectionBars[i].bottom.Opacity = 0;
                            connectionBars[i].bottom.Invalidate();
                            connectionBars[i].top.Opacity = 0;
                            connectionBars[i].top.Invalidate();
                        };
                        connectionbarActive = false;
                        subsequentInFocusCount = 0;
                        subsequentOutOfFocusCount = 0;
                    }
                    catch { }
                    finally
                    {
                        connectionBarFormLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                if (gotLock)
                    hideConnectionBarsLock.Exit(true);
            }
        }

        static void ShowInactiveTop(Form frm, IntPtr parent)
        {
            Win32Helper.SetWindowPos(frm.Handle.ToInt32(), parent.ToInt32(),
            frm.Left, frm.Top, frm.Width, frm.Height,
            Win32Helper.SWP.NOACTIVATE | Win32Helper.SWP.SHOWWINDOW | Win32Helper.SWP.NOSIZE | Win32Helper.SWP.NOMOVE | Win32Helper.SWP.NOSENDCHANGING);
        }

        private void UpdateSettings()
        {
            // Get current settings
            ServerSettings serverSettings;
            VisualizationsSettings visualizationsSettings;
            try
            {
                Dictionary<String, ServerSettings> allServerSettings = ServerSettingsHelper.GetServerSettingsFromConfig();
                if (allServerSettings.Keys.Contains(serverName))
                    serverSettings = allServerSettings[serverName];
                else
                    return;
            }
            catch
            {
                MessageBox.Show(new Form() { TopMost = true }, String.Format(Properties.Resources.VisualizationsSettings_LoadFailedMessage, serverName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            try
            {
                visualizationsSettings = new VisualizationsSettings(serverName);
            }
            catch
            {
                MessageBox.Show(new Form() { TopMost = true }, String.Format(Properties.Resources.VisualizationsSettings_LoadFailedMessage, serverName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Initialize connection bar
            showConnectionBarsEvent.Invoke(false, true);

            // Implement visual aids settings
            if (!visualizationsSettings.VisualizationsActive)
            {
                connectionBarOpacity = 0;
                visualizationsEnabled = false;
                foreach (IntPtr hWndOverlay in windowOverlays.Keys)
                {
                    OverlayForm overlayForms = windowOverlays[hWndOverlay];
                    overlayForms.Opacity = 0;
                }
            }
            else
            {
                // Update visual aids variables
                visualizationsEnabled = true;
                overlaysColor = visualizationsSettings.MainColor;
                linesColor = visualizationsSettings.LineColor;
                if (visualizationsSettings.ConnectionBar)
                {
                    serverDisplayName = serverSettings.Alias;
                    connectionBarText = serverDisplayName;
                    connectionBarOpacity = visualizationsSettings.ConnectionBarOpacity;
                    connectionBarColor = visualizationsSettings.MainColor;
                    connectionBarTextColor = visualizationsSettings.TextColor;
                    connectionBarEnabled = true;
                    foreach (CONNECTIONBAR fullConnectionBar in connectionBars.Values)
                    {
                        fullConnectionBar.top.LinesColor = linesColor;
                        fullConnectionBar.top.CenterText = connectionBarText;
                        fullConnectionBar.top.CenterTextColor = connectionBarTextColor;
                        fullConnectionBar.bottom.LinesColor = linesColor;
                        fullConnectionBar.bottom.CenterText = connectionBarText;
                        fullConnectionBar.bottom.CenterTextColor = connectionBarTextColor;
                    }
                    connectionBarInitialized = true;
                }
                else
                {
                    connectionBarEnabled = false;
                }
                frames = visualizationsSettings.Frames;
            }
            if (visualizationsActive && visualizationsEnabled)
            {
                // Apply overlays
                try
                {
                    foreach (IntPtr hWndOverlay in windowOverlays.Keys)
                    {
                        OverlayForm overlayForms = windowOverlays[hWndOverlay];
                        overlayForms.DrawFrames = frames;
                        overlayForms.LinesColor = linesColor;
                        overlayForms.Color = overlaysColor;
                        overlayForms.Invalidate();
                        long lWindowStyle = Win32Helper.GetWindowLong(hWndOverlay, (int)Win32Helper.GWLParameter.GWL_STYLE);
                        if ((lWindowStyle & (long)Win32Helper.WindowStyles.WS_TILEDWINDOW) == 0 || (lWindowStyle & (int)Win32Helper.WindowStyles.WS_VISIBLE) == 0)
                            overlayForms.Opacity = 0;
                        else if (visualizationsEnabled)
                            overlayForms.Opacity = 1;
                        else
                            overlayForms.Opacity = 0;
                    }
                }
                catch { }

                // Configure ConnectionBar
                if (!connectionbarActive && connectionBarEnabled)
                    showConnectionBarsEvent.Invoke(false, true);
                else if (connectionbarActive)
                    hideConnectionBarsEvent.Invoke(true);
            }
        }

        private  ConnectionBarForm CreateConnectionBar(int screenNumber, bool isTop)
        {
            // Get screen dimensions
            int screenWorkingWidth = Screen.AllScreens.ElementAt(screenNumber).WorkingArea.Width;
            int screenWorkingHeight = Screen.AllScreens.ElementAt(screenNumber).WorkingArea.Height;

            // Create connection bar
            ConnectionBarForm connectionBarForm = new ConnectionBarForm(screenWorkingWidth, false);
            try
            {
                connectionBarForm.Width = (int)(screenWorkingWidth * 0.6) + 4;
                connectionBarForm.ScreenWorkingWidth = screenWorkingWidth;
                connectionBarForm.CenterTextColor = connectionBarTextColor;
                connectionBarForm.CenterText = connectionBarText;
                connectionBarForm.LinesColor = linesColor;
                connectionBarForm.Opacity = 0;
                connectionBarForm.Visible = false;
                connectionBarForm.IsTop = isTop;
                IgnoreHWnd(connectionBarForm.Handle);
                SetWindowLongPtr(connectionBarForm.Handle, Win32Helper.GWLParameter.GWL_HWNDPARENT, Win32Helper.GetDesktopWindow());
            }
            catch { }
            return connectionBarForm;
        }

        private void ShowConnectionBarForm(ConnectionBarForm connectionBarForm, int screenNumber, bool isTop)
        {
            try
            {
                // Get screen dimensions
                int screenWorkingWidth = Screen.AllScreens.ElementAt(screenNumber).WorkingArea.Width;
                int screenWorkingHeight = Screen.AllScreens.ElementAt(screenNumber).WorkingArea.Height;

                // Initialize connectionbar properties
                connectionBarForm.IsTop = isTop;
                connectionBarForm.Width = (int)(screenWorkingWidth * 0.6) + 4;
                connectionBarForm.ScreenWorkingWidth = screenWorkingWidth;
                connectionBarForm.CenterTextColor = connectionBarTextColor;
                connectionBarForm.CenterText = connectionBarText;

                ignoreHwndEvent.Invoke(connectionBarForm.Handle);
                connectionBarForm.MoveWindowToScreen(Screen.AllScreens.ElementAt(screenNumber));

                // Show window
                ShowInactiveTop(connectionBarForm, (IntPtr)Win32Helper.HWND_TOPMOST);
                connectionBarForm.Invalidate();

                if (!(visualizationsEnabled && visualizationsActive))
                {
                    connectionBarForm.Opacity = 0;
                }
                else if (isTop)
                {
                    connectionBarForm.Color = System.Drawing.Color.FromArgb(0, 0, 0, 0);
                    connectionBarForm.Opacity = 1;
                }
                else
                {
                    connectionBarForm.Color = connectionBarColor;
                    connectionBarForm.Opacity = connectionBarOpacity;
                }
                connectionBarForm.Visible = true;
            }
            catch { }
        }

        private void ShowConnectionBars(bool visible = true, bool force = false)
        {
            if (!connectionBarEnabled)
                return;
            if (connectionbarActive && !force)
                return;
            if (!connectionBarInitialized && visible && !force)
                return;
            if (showConnectionsBarLock.IsHeldByCurrentThread)
                return;
            bool gotLock = false;
            try
            {
                showConnectionsBarLock.TryEnter(ref gotLock);
                if (gotLock)
                {
                    try
                    {
                        if (connectionbarActive && !force)
                            return;
                        int allscreenCount = Screen.AllScreens.Count();

                        // Create new connection bars if needed
                        for (int i = connectionBars.Count(); i < allscreenCount; i++)
                        {
                            CONNECTIONBAR connectionBar;
                            connectionBar.top = CreateConnectionBar(i, true);
                            connectionBar.bottom = CreateConnectionBar(i, false);
                            connectionBars[i] = connectionBar;
                        }

                        // Remove unused connection bars if needed
                        for (int i = connectionBars.Count() - 1; i > allscreenCount - 1; i--)
                        {
                            connectionBars[i].bottom.Close();
                            connectionBars[i].top.Close();
                            connectionBars.Remove(i);
                        }

                        if (visible)
                        {
                            try
                            {
                                connectionBarFormLock.EnterWriteLock();

                                // Show existing connection bars
                                for (int i = 0; i < connectionBars.Count(); i++)
                                {
                                    ShowConnectionBarForm(connectionBars[i].bottom, i, false);
                                    ShowConnectionBarForm(connectionBars[i].top, i, true);
                                }
                                connectionbarActive = true;
                                subsequentInFocusCount = 0;
                                subsequentOutOfFocusCount = 0;
                            }
                            catch { }
                            finally
                            {
                                connectionBarFormLock.ExitWriteLock();
                            }
                        }
                    }
                    catch { }
                }
            }
            finally
            {
                if (gotLock)
                    showConnectionsBarLock.Exit(true);
            }
        }

        private OverlayForm CreateOverlay(IntPtr hWnd, bool isTop)
        {
            // Get Z-order of remote application
            IntPtr hDC = Win32Helper.GetDC(hWnd);
            Win32Helper.RECT rectWin;
            Win32Helper.GetWindowRect(hWnd, out rectWin);
            System.Drawing.Point newPoint = new System.Drawing.Point(0, 0);

            // Create overlay form
            OverlayForm overlayForm = new OverlayForm(rectWin.right - rectWin.left, rectWin.bottom - rectWin.top, 1);

            noOverlayHWnds.Add(overlayForm.Handle);

            // Set properties
            overlayForm.OwnerHWnd = hWnd;
            overlayForm.LinesColor = linesColor;
            overlayForm.Color = overlaysColor;
            overlayForm.DrawFrames = frames;
            overlayForm.ShowInTaskbar = false;
            overlayForm.Visible = false;

            // Move over remote application
            Win32Helper.MoveWindow(overlayForm.Handle, rectWin.left - WINDOWRECT_SURROUNDSPACE_X, rectWin.top - WINDOWRECT_SURROUNDSPACE_Y, 2 * WINDOWRECT_SURROUNDSPACE_X + rectWin.right - rectWin.left, 2 * WINDOWRECT_SURROUNDSPACE_Y + rectWin.bottom - rectWin.top, false);

            // Click through
            IntPtr initialStyleTop = Win32Helper.GetWindowLongPtr(overlayForm.Handle, (int)Win32Helper.GWLParameter.GWL_EXSTYLE);
            SetWindowLongPtr(overlayForm.Handle, Win32Helper.GWLParameter.GWL_EXSTYLE, new IntPtr((int)initialStyleTop | (int)Win32Helper.WindowStyles.WS_EX_NOACTIVATE | (int)Win32Helper.WindowStyles.WS_EX_TRANSPARENT));

            if (isTop)
            {
                UpdateOverlayFrames(hWnd, overlayForm);
            }

            overlayForm.Visible = true;

            // Return overlayform
            return overlayForm;
        }

        private void UpdateOverlayFrames(IntPtr hWnd, OverlayForm overlayForm)
        {
            long lWindowStyle = Win32Helper.GetWindowLong(hWnd, (int)Win32Helper.GWLParameter.GWL_STYLE);
            if (!visualizationsActive)
            {
                overlayForm.Opacity = 0;
            }
            else if ((lWindowStyle & (long)Win32Helper.WindowStyles.WS_TILEDWINDOW) == 0 || (lWindowStyle & (int)Win32Helper.WindowStyles.WS_VISIBLE) == 0)
            {
                overlayForm.Opacity = 0;
            }
            else
            {
                overlayForm.Opacity = 1;
            }
            overlayForm.Visible = true;
        }

        private void PositionOverlay(IntPtr hWnd, OverlayForm wo)
        {
            ignoreHwndEvent.Invoke(wo.Handle);
            wo.AttachToParent(hWnd, wo.Opacity);

            // Refresh overlayform
            wo.Invalidate();
            wo.Refresh();
        }

        private void UpdateOverlay(IntPtr hWnd)
        {
            // Determine if this application already has an overlay
            if (overlayWindows.Contains(hWnd))
            {
                try
                {
                    if (windowOverlays.Keys.Contains(hWnd) && (windowOverlays[hWnd].IsDisposed))
                    {
                        if (!windowOverlays[hWnd].IsDisposed)
                        {
                            try
                            {
                                windowOverlays[hWnd].Dispose();
                            }
                            catch { }
                        }
                        try
                        {
                            windowOverlays.Remove(hWnd);
                        }
                        catch { }
                    }
                    else
                    {
                        UpdateOverlayFrames(hWnd, windowOverlays[hWnd]);
                        return;
                    }
                }
                catch { }
            }
            else
            {
                // Indicate that this remote application has an overlay
                overlayWindows.Add(hWnd);
            }

            // Create overlay forms
            OverlayForm wo;
            wo = CreateOverlay(hWnd, true);

            PositionOverlay(hWnd, wo);

            // Save reference to overlays
            try
            {
                windowOverlays.Add(hWnd, wo);
            }
            catch { }
        }

        private void WindowPosChanged(IntPtr hWnd, uint flags)
        {
            if (noOverlayHWnds.Contains(hWnd))
                return;
            Win32Helper.RECT rectWin;
            Win32Helper.GetWindowRect(hWnd, out rectWin);
            Rectangle clientRect = new Rectangle(rectWin.left, rectWin.top, rectWin.right - rectWin.left, rectWin.bottom - rectWin.top);
            try
            {
                // Find correct overlays and position them
                foreach (IntPtr hWndOverlay in windowOverlays.Keys)
                {
                    if (hWndOverlay != hWnd)
                        continue;
                    OverlayForm overlayForm = windowOverlays[hWndOverlay];
                    if (visualizationsActive && visualizationsEnabled)
                    {
                        long lWindowStyle = Win32Helper.GetWindowLong(hWnd, (int)Win32Helper.GWLParameter.GWL_STYLE);
                        if ((lWindowStyle & (long)Win32Helper.WindowStyles.WS_TILEDWINDOW) == 0 || (lWindowStyle & (int)Win32Helper.WindowStyles.WS_VISIBLE) == 0)
                            overlayForm.Opacity = 0;
                        else
                            overlayForm.Opacity = 1;        
                    }

                    // Redraw overlays
                    overlayForm.Invalidate();
                    overlayForm.Update();
                    overlayForm.Refresh();
                }
            }
            catch { }
        }

        private void AppsGotFocus()
        {
            // Show connection bar if inactive
            if (!connectionbarActive)
                ShowConnectionBars(true, true);
        }

        private void AppsLostFocus()
        {
            // Hide connection bar if active
            if (connectionbarActive)
                HideConnectionBars(true);
        }
    }
}