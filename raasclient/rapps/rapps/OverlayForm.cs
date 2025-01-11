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
using System.Drawing;
using System.Windows.Forms;
using Elbitin.Applications.RAAS.Common.Helpers;

namespace Elbitin.Applications.RAAS.RAASClient.RemoteApps
{
    public class OverlayForm : Form
    {
        public IntPtr OwnerHWnd { get; set; }
        public Color Color { get; set; }
        public Color LinesColor { get; set; } = Color.Black;
        public bool DrawFrames { get; set; } = true;
        private Version win8version = new Version(6, 2, 9200, 0);
        private const int GWL_STYLE = -16;
        private const int WS_CHILD = 0x40000000;

        public OverlayForm(int width, int height, double opacity)
        {
            // Set window properties
            DoubleBuffered = true;
            Width = width + 8;
            Height = height + 8;
            Location = new System.Drawing.Point(Screen.PrimaryScreen.WorkingArea.X, Screen.PrimaryScreen.WorkingArea.Y);
            ShowInTaskbar = false;
            AllowTransparency = true;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.LimeGreen;
            TransparencyKey = Color.LimeGreen;
            Opacity = opacity;
            TopMost = true;
            TopMost = false;
            ResizeRedraw = true;

            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        public void AttachToParent(IntPtr parentHWnd, double opacity)
        {
            IntPtr hostHandle = parentHWnd;
            IntPtr guestHandle = Handle;
            Location = new System.Drawing.Point(0, 0);
            FormBorderStyle = FormBorderStyle.None;
            SetBounds(0, 0, 0, 0, BoundsSpecified.Location);
            Hide();
            Win32Helper.SetWindowLong(guestHandle, Win32Helper.GWLParameter.GWL_STYLE, Win32Helper.GetWindowLong((IntPtr)guestHandle, (int)Win32Helper.GWLParameter.GWL_STYLE) | WS_CHILD | (int)Win32Helper.WindowStyles.WS_EX_NOACTIVATE | (int)Win32Helper.WindowStyles.WS_EX_TRANSPARENT);
            Win32Helper.SetParent(guestHandle, hostHandle);
            byte opacityByte = Convert.ToByte(opacity * 255);
            Win32Helper.SetLayeredWindowAttributes(Handle, 0, opacityByte, 0x00000002);
            IntPtr initialStyleBottom = Win32Helper.GetWindowLongPtr(Handle, (int)Win32Helper.GWLParameter.GWL_EXSTYLE);
            Win32Helper.SetWindowLongPtr(Handle, Win32Helper.GWLParameter.GWL_EXSTYLE, new IntPtr((int)initialStyleBottom | (int)Win32Helper.WindowStyles.WS_EX_NOACTIVATE | (int)Win32Helper.WindowStyles.WS_EX_LAYERED | (int)Win32Helper.WindowStyles.WS_EX_TRANSPARENT));
            Opacity = opacity;
            Show();
            Win32Helper.MoveWindow(Handle, 0, 0, Width, Height, true);
            BringToFront();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Initialize variables
            IntPtr hDC2 = Win32Helper.GetDC(Handle);
            Graphics g2 = e.Graphics;
            Win32Helper.RECT rectWin;

            // Set window dimension the same as targeted window
            Win32Helper.GetWindowRect(OwnerHWnd, out rectWin);
            Width = rectWin.right - rectWin.left;
            Height = rectWin.bottom - rectWin.top;

            // Draw overlay window
            g2.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            if (DrawFrames)
            {
                // Draw frames around the targeted window
                g2.DrawLine(new Pen(new SolidBrush(LinesColor), 8), new Point(0, 0), new Point(0, DisplayRectangle.Height));
                g2.DrawLine(new Pen(new SolidBrush(LinesColor), 8), new Point(0, DisplayRectangle.Height), new Point(DisplayRectangle.Width, DisplayRectangle.Height));
                g2.DrawLine(new Pen(new SolidBrush(LinesColor), 8), new Point(DisplayRectangle.Width, DisplayRectangle.Height), new Point(DisplayRectangle.Width, 0));
                g2.DrawLine(new Pen(new SolidBrush(LinesColor), 8), new Point(DisplayRectangle.Width, 0), new Point(0, 0));
                g2.DrawLine(new Pen(new SolidBrush(Color), 6), new Point(0, 0), new Point(0, DisplayRectangle.Height));
                g2.DrawLine(new Pen(new SolidBrush(Color), 6), new Point(0, DisplayRectangle.Height), new Point(DisplayRectangle.Width, DisplayRectangle.Height));
                g2.DrawLine(new Pen(new SolidBrush(Color), 6), new Point(DisplayRectangle.Width, DisplayRectangle.Height), new Point(DisplayRectangle.Width, 0));
                g2.DrawLine(new Pen(new SolidBrush(Color), 6), new Point(DisplayRectangle.Width, 0), new Point(0, 0));
                g2.DrawLine(new Pen(new SolidBrush(LinesColor), 2), new Point(0, 0), new Point(0, DisplayRectangle.Height));
                g2.DrawLine(new Pen(new SolidBrush(LinesColor), 2), new Point(0, DisplayRectangle.Height), new Point(DisplayRectangle.Width, DisplayRectangle.Height));
                g2.DrawLine(new Pen(new SolidBrush(LinesColor), 2), new Point(DisplayRectangle.Width, DisplayRectangle.Height), new Point(DisplayRectangle.Width, 0));
                g2.DrawLine(new Pen(new SolidBrush(LinesColor), 2), new Point(DisplayRectangle.Width, 0), new Point(0, 0));
            }
        }
    }
}