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
using System.Drawing.Drawing2D;
using Elbitin.Applications.RAAS.Common.Helpers;

namespace Elbitin.Applications.RAAS.RAASClient.RemoteApps
{
    public class ConnectionBarForm : Form
    {
        public String CenterText { get; set; } = "";
        public Color CenterTextColor { get; set; } = Color.Black;
        public Color Color { get; set; }
        public Color LinesColor { get; set; } = Color.Black;
        public int ScreenWorkingWidth { get; set; }
        public bool IsTop { get; set; } = false;

        public ConnectionBarForm(int monitorWidth, bool visible = true)
        {
            SetFormProperties(monitorWidth, visible);
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        private void SetFormProperties(int monitorWidth, bool visible)
        {
            DoubleBuffered = true;
            if (!visible)
                Opacity = 0;
            Visible = visible;
            ScreenWorkingWidth = monitorWidth;
            Location = new System.Drawing.Point(0, 0);
            ShowInTaskbar = false;
            AllowTransparency = true;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.LimeGreen;
            ShowInTaskbar = false;
            TransparencyKey = Color.LimeGreen;
            TopMost = true;
        }

        private Point[] GetPolygonPoints()
        {
            // Calculate the connection bar polygon to be displayed
            Point[] polygonPoints = new Point[8];
            polygonPoints[0].X = -2;
            polygonPoints[0].Y = -2;
            polygonPoints[1].X = 10;
            polygonPoints[1].Y = 10;
            polygonPoints[2].X = 22 + 15;
            polygonPoints[2].Y = 20;
            polygonPoints[3].X = 22 + 15 + 160;
            polygonPoints[3].Y = 20;
            polygonPoints[4].X = (int)(ScreenWorkingWidth * 0.6) - 22 - 15 - 160;
            polygonPoints[4].Y = 20;
            polygonPoints[5].X = (int)(ScreenWorkingWidth * 0.6) - 22 - 15;
            polygonPoints[5].Y = 20;
            polygonPoints[6].X = (int)(ScreenWorkingWidth * 0.6) - 10;
            polygonPoints[6].Y = 10;
            polygonPoints[7].X = (int)(ScreenWorkingWidth * 0.6) + 2;
            polygonPoints[7].Y = -2;
            return polygonPoints;
        }

        private void DrawString(Graphics g)
        {
            // Display text in center of the connection bar
            System.Drawing.Graphics formGraphics = g;
            formGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            string drawString = CenterText;
            System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 13, FontStyle.Bold);
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(CenterTextColor);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            formGraphics.DrawString(drawString, drawFont, drawBrush, DisplayRectangle, sf);
            drawFont.Dispose();
            drawBrush.Dispose();
        }

        public void MoveWindowToScreen(Screen screen)
        {
            int screenWorkingWidth = screen.WorkingArea.Width;

            // Set Parent
            Win32Helper.SetWindowLongPtr(this.Handle, Win32Helper.GWLParameter.GWL_HWNDPARENT, Win32Helper.GetDesktopWindow());

            // Move window to position
            Win32Helper.MoveWindow(this.Handle, screen.WorkingArea.X + (int)(screenWorkingWidth * 0.2) - 2, screen.WorkingArea.Y, (int)(screenWorkingWidth * 0.6) + 4, 22 + 2, false);
            Win32Helper.SetWindowPos(this.Handle, (IntPtr)Win32Helper.HWND_TOPMOST, 0, 0, 0, 0, Win32Helper.SWP.NOACTIVATE | Win32Helper.SWP.NOSIZE | Win32Helper.SWP.NOMOVE | Win32Helper.SWP.NOSENDCHANGING | Win32Helper.SWP.NOSENDCHANGING);

            // Click through
            IntPtr initialStyleTop = Win32Helper.GetWindowLongPtr(this.Handle, (int)Win32Helper.GWLParameter.GWL_EXSTYLE);
            Win32Helper.SetWindowLongPtr(this.Handle, Win32Helper.GWLParameter.GWL_EXSTYLE, new IntPtr((int)initialStyleTop | (int)Win32Helper.WindowStyles.WS_EX_LAYERED | (int)Win32Helper.WindowStyles.WS_EX_TRANSPARENT | (int)Win32Helper.WindowStyles.WS_EX_TOOLWINDOW));
        }

        protected override void OnGotFocus(EventArgs e)
        {
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ExStyle |= (int)Win32Helper.WindowStyles.WS_EX_TOPMOST; // make the form topmost
                param.ExStyle |= (int)Win32Helper.WindowStyles.WS_EX_NOACTIVATE; // prevent the form from being activated
                return param;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Visible == true)
            {
                // Draw polygon
                Point[] polygonPoints = GetPolygonPoints();
                IntPtr hDC2 = Win32Helper.GetDC(Handle);
                Graphics g2 = e.Graphics;
                g2.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                GraphicsPath path = new GraphicsPath();
                path.AddLine(polygonPoints[0], polygonPoints[1]);
                path.AddCurve(polygonPoints, 1, 1, (float)0.3);
                path.AddLine(polygonPoints[2], polygonPoints[3]);
                path.AddLine(polygonPoints[3], polygonPoints[4]);
                path.AddLine(polygonPoints[4], polygonPoints[5]);
                path.AddCurve(polygonPoints, 5, 1, (float)0.3);
                path.AddLine(polygonPoints[6], polygonPoints[7]);
                path.AddLine(polygonPoints[7], polygonPoints[0]);
                if (!IsTop)
                {
                    g2.FillPath(new SolidBrush(Color), path);
                }
                g2.DrawPath(new Pen(LinesColor), path);

                // Draw text
                DrawString(g2);
            }
        }
    }
}