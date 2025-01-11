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
﻿// Copyright (c) Elbitin
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using Elbitin.Applications.RAAS.Common.Helpers;

namespace Elbitin.Applications.RAAS.RAASServer.KeepAlive
{
    public partial class KeepAliveForm : Form
    {
        const int OFFSCREEN = 100000;

        public KeepAliveForm()
        {
            InitializeComponent();
            SetFormProperties();
            this.Hide();
        }

        private void SetFormProperties()
        {
            this.Location = new Point(OFFSCREEN, OFFSCREEN);
            this.Opacity = 0;
            this.Visible = false;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }
    }
}
