/* Copyright (c) 2025 Elbitin
 *
 * This file is part of RAAS Server.
 *
 * RAAS Server is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * RAAS Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along with RAAS Server. If not, see <https://www.gnu.org/licenses/>.
 */
﻿using Elbitin.Applications.RAAS.RAASServer.Helpers;
using System.Drawing;
using System.Windows.Forms;

namespace Elbitin.Applications.RAAS.RAASServer.Autostart
{
    public partial class AutostartForm : Form
    {
        private const int OFFSCREEN = 100000;

        public AutostartForm()
        {
            SetFormProperties();
            InitializeComponent();
            Hide();
            try
            {
                RAASServerProgramHelper.StartStartupPrograms();
            }
            catch { }
        }

        private void SetFormProperties()
        {
            Opacity = 0;
            Visible = false;
            ShowInTaskbar = false;
            Location = new Point(OFFSCREEN, OFFSCREEN);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            WindowState = FormWindowState.Minimized;
        }
    }
}
