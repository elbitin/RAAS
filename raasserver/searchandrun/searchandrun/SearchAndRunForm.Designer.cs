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
﻿using System.Windows.Forms;

namespace Elbitin.Applications.RAAS.RAASServer.SearchAndRun
{
    partial class SearchAndRunForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchAndRunForm));
            this.searchFileTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // searchFileTextBox
            // 
            this.searchFileTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchFileTextBox.Font = new System.Drawing.Font("Microsoft Yi Baiti", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchFileTextBox.Location = new System.Drawing.Point(20, 20);
            this.searchFileTextBox.Name = "searchFileTextBox";
            this.searchFileTextBox.Size = new System.Drawing.Size(740, 81);
            this.searchFileTextBox.TabIndex = 1;
            this.searchFileTextBox.TextChanged += new System.EventHandler(this.SearchFileTextBox_TextChanged);
            this.searchFileTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchFileTextBox_KeyDown);
            // 
            // SearchAndRunForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1972, 1421);
            this.Controls.Add(this.searchFileTextBox);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SearchAndRunForm";
            this.Text = "Search & Run";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchAndRunForm_KeyDown);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SearchAndRunForm_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SearchAndRunForm_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox searchFileTextBox;
    }
}

