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
﻿namespace Elbitin.Applications.RAAS.RAASClient.RemoteApps
{
    partial class RemoteAppsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteAppsForm));
            this.button1 = new System.Windows.Forms.Button();
            this.axMsRdpClient8NotSafeForScripting1 = new AxMSTSCLib.AxMsRdpClient8NotSafeForScripting();
            ((System.ComponentModel.ISupportInitialize)(this.axMsRdpClient8NotSafeForScripting1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            // 
            // axMsRdpClient8NotSafeForScripting1
            // 
            this.axMsRdpClient8NotSafeForScripting1.Enabled = true;
            this.axMsRdpClient8NotSafeForScripting1.Location = new System.Drawing.Point(481, 264);
            this.axMsRdpClient8NotSafeForScripting1.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.axMsRdpClient8NotSafeForScripting1.Name = "axMsRdpClient8NotSafeForScripting1";
            this.axMsRdpClient8NotSafeForScripting1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMsRdpClient8NotSafeForScripting1.OcxState")));
            this.axMsRdpClient8NotSafeForScripting1.Size = new System.Drawing.Size(480, 480);
            this.axMsRdpClient8NotSafeForScripting1.TabIndex = 1;
            // 
            // RemoteAppsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 367);
            this.Controls.Add(this.axMsRdpClient8NotSafeForScripting1);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(100000, 100000);
            this.Name = "RemoteAppsForm";
            this.ShowInTaskbar = false;
            this.Text = "RemoteAppForm";
            ((System.ComponentModel.ISupportInitialize)(this.axMsRdpClient8NotSafeForScripting1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private AxMSTSCLib.AxMsRdpClient8NotSafeForScripting axMsRdpClient8NotSafeForScripting1;
    }
}