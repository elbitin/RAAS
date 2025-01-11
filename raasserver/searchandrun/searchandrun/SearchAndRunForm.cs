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
﻿using Elbitin.Applications.RAAS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Elbitin.Applications.RAAS.RAASServer.SearchAndRun
{
    public partial class SearchAndRunForm : Form
    {
        private String[] systemFilePaths;
        private List<String> searchList = new List<string>();
        private int seachListSelectedIndex = 0;
        private Dictionary<String, Icon> iconCache = new Dictionary<string, Icon>();
        private int panelHeight = 0;
        private int panelY = 0;
        private RAASServer.SearchAndRun.DrawPanel searchPanel;

        public SearchAndRunForm()
        {
            InitializeComponent();
            InitializeForm();
            InitializeSearchPanel();
            InitializeSearchFileTextBox();
            systemFilePaths = Directory.GetFiles(Environment.SystemDirectory).Where(item => Path.GetExtension(item) == ".exe" || Path.GetExtension(item) == ".msc").ToArray();
        }

        private void InitializeSearchFileTextBox()
        {
            const int textBoxX = 20;
            const int textBoxY = 20;
            searchFileTextBox.Width = 740;
            searchFileTextBox.BorderStyle = BorderStyle.None;
            searchFileTextBox.Location = new Point(textBoxX, textBoxY);
        }

        private void InitializeSearchPanel()
        {
            this.searchPanel = new RAASServer.SearchAndRun.DrawPanel();
            this.searchPanel.Location = new System.Drawing.Point(160, 168);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Size = new System.Drawing.Size(1504, 520);
            this.searchPanel.TabIndex = 2;
            this.searchPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.SearchPanel_Paint);
            this.searchPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SearchPanel_Click);
            this.Controls.Add(this.searchPanel);
            searchPanel.Width = 750;
            panelY = searchFileTextBox.Height + 24;
            searchPanel.Location = new Point(10, panelY);
            panelHeight = 540 - (searchFileTextBox.Height + 20);
            searchPanel.Height = panelHeight;
            searchPanel.Hide();
            searchPanel.BackColor = Color.White;
            searchPanel.AutoScroll = false;
            searchPanel.HorizontalScroll.Enabled = false;
            searchPanel.HorizontalScroll.Visible = false;
            searchPanel.HorizontalScroll.Maximum = 0;
            searchPanel.AutoScroll = true;
        }

        private void InitializeForm()
        {
            this.Width = 820;
            this.Height = 600;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            System.Drawing.Graphics formGraphics;
            formGraphics = this.CreateGraphics();
            formGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            formGraphics.FillRectangle(myBrush, new Rectangle(10, 10, searchFileTextBox.Width + 10, searchFileTextBox.Height + 20));
            Pen myPen = new Pen(Color.Black);
            formGraphics.DrawEllipse(myPen, new Rectangle(searchFileTextBox.Width + 25, 23, 30, 30));
            formGraphics.FillEllipse(new System.Drawing.SolidBrush(Color.White), new Rectangle(searchFileTextBox.Width + 25, 23, 30, 30));
            Pen pen = new Pen(Color.Black);
            System.Drawing.SolidBrush blackBrush = new System.Drawing.SolidBrush(Color.Black);
            formGraphics.DrawString("?", new Font(FontFamily.GenericSansSerif, 16), blackBrush, searchFileTextBox.Width + 30, 25);
            formGraphics.DrawLine(new Pen(Color.LightGray), 10, 10 + searchFileTextBox.Height + 20, searchFileTextBox.Width + 10, 10 + searchFileTextBox.Height + 20);
            myBrush.Dispose();
            formGraphics.Dispose();
        }

        private void SearchFileTextBox_TextChanged(object sender, EventArgs e)
        {
            if (searchFileTextBox.Text.Length >= 1)
            {
                // Search and add system files 
                List<string> result = new List<string>();
                String searchFileTextBoxString = searchFileTextBox.Text.Replace("\\", "\\\\").Replace("[", "\\[").Replace("]", "\\]").Replace(".", "[.]").Replace("*", "(.*)").Replace("?", ".") + "(.*)";
                System.Text.RegularExpressions.Regex stringMatch = new System.Text.RegularExpressions.Regex("^" + searchFileTextBoxString, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                foreach (String systemFilePath in systemFilePaths)
                {
                    if (stringMatch.IsMatch(systemFilePath.Split('\\').Last()))
                        result.Add(systemFilePath);
                }

                // Add search options to files in System Index 
                string connectionString = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";
                OleDbConnection connection = new OleDbConnection(connectionString);
                OleDbCommand command;
                string query = @"SELECT System.ItemUrl  FROM SystemIndex " +
                    @"WHERE System.FileName LIKE '" + searchFileTextBox.Text.Replace("\\", "\\\\").Replace("_", "[_]").Replace("%", "[%]").Replace('*', '%').Replace('?', '_') + "%' AND System.ItemPathDisplay LIKE '_:%'";
                command = new OleDbCommand(query, connection);

                // Add items which match search string
                connection.Open();
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(reader.GetString(0).Substring(5).Replace("/", "\\"));
                }
                connection.Close();

                // Update search list 
                searchList.Clear();
                iconCache.Clear();
                if (result.Count >= 1)
                {
                    foreach (String fileName in result)
                    {
                        if (!fileName.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase) && File.Exists(fileName))
                            searchList.Add(fileName);
                    }
                    searchPanel.Visible = true;
                    if (searchList.Count == 1)
                        seachListSelectedIndex = 0;
                    else
                        seachListSelectedIndex = -1;
                }
                else
                    searchPanel.Visible = false;
                searchPanel.VerticalScroll.Value = 0;
                searchPanel.PerformLayout();
                searchPanel.Invalidate();
                searchPanel.Refresh();
            }
            else
            {
                searchPanel.Invalidate();
                searchPanel.Refresh();
                searchPanel.Visible = false;
            }
        }

        private void SearchPanel_Paint(object sender, PaintEventArgs e)
        {
            if (searchList.Count > 0)
            {
                searchPanel.Height = Math.Min(searchList.Count * 60, panelHeight);
                searchPanel.AutoScrollMinSize = new Size(0, searchList.Count * 60);
                e.Graphics.TranslateTransform(searchPanel.AutoScrollPosition.X, searchPanel.AutoScrollPosition.Y);
                for (int i = 0; i < searchList.Count; i++)
                {
                    // Lazy loading
                    if ((i - 10) * 60 < -searchPanel.AutoScrollPosition.Y && -searchPanel.AutoScrollPosition.Y + 540 - (searchFileTextBox.Height + 20) < (i + 10) * 60)
                    {
                        int yOffset = i * 60;
                        Pen pen = new Pen(Color.LightGray);
                        e.Graphics.DrawLine(pen, 0, yOffset, searchPanel.Width, yOffset);
                        StringFormat format = new StringFormat();
                        format.Trimming = StringTrimming.EllipsisCharacter;
                        String showFileName = Path.GetFileName(searchList[i]);
                        String showDirectoryName = Path.GetDirectoryName(searchList[i]);
                        if (seachListSelectedIndex == i)
                            e.Graphics.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle(0, yOffset + 1, searchPanel.Width, yOffset + 60));
                        else
                            e.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, yOffset + 1, searchPanel.Width, yOffset + 60));
                        e.Graphics.DrawString(showFileName, new Font(FontFamily.GenericSansSerif, 12), new SolidBrush(Color.Black), new RectangleF(70, yOffset + 10, 600, 20), format);
                        e.Graphics.DrawString(showDirectoryName, new Font(FontFamily.GenericSansSerif, 8), new SolidBrush(Color.Black), new RectangleF(70, yOffset + 35, 600, 15), format);
                        e.Graphics.DrawString(">", new Font(FontFamily.GenericMonospace, 25), new SolidBrush(Color.Gray), 700, yOffset + 12);
                        if (!iconCache.Keys.Contains(searchList[i]))
                        {
                            try
                            {
                                Icon associatedIcon = Icon.ExtractAssociatedIcon(searchList[i]);
                                iconCache.Add(searchList[i], associatedIcon);
                            }
                            catch
                            {
                                if (iconCache.Keys.Contains(searchList[i]))
                                    iconCache[searchList[i]] = null;
                                else
                                    iconCache.Add(searchList[i], null);
                            }
                        }
                        if (iconCache.Keys.Contains(searchList[i]) && iconCache[searchList[i]] != null)
                            e.Graphics.DrawIcon(iconCache[searchList[i]], new Rectangle(10, yOffset + 10, 40, 40));
                    }
                }
            }
        }

        private void SearchFileTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                e.SuppressKeyPress = true;
                if (e.KeyCode == Keys.Down)
                {
                    if (seachListSelectedIndex < searchList.Count - 1)
                    {
                        seachListSelectedIndex = seachListSelectedIndex + 1;
                    }
                }
                else if (e.KeyCode == Keys.Up)
                {
                    if (seachListSelectedIndex > 0)
                    {
                        seachListSelectedIndex = seachListSelectedIndex - 1;
                    }
                }
                if (-searchPanel.AutoScrollPosition.Y + panelHeight < 60 * (seachListSelectedIndex + 1))
                {
                    searchPanel.VerticalScroll.Value = 60 * (seachListSelectedIndex + 1) - panelHeight;
                    searchPanel.PerformLayout();
                }
                if (-searchPanel.AutoScrollPosition.Y > 60 * seachListSelectedIndex)
                {
                    searchPanel.VerticalScroll.Value = 60 * seachListSelectedIndex;
                    searchPanel.PerformLayout();
                }
                searchPanel.Invalidate();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                (new Thread(() => {
                    RunSelectedProgram();
                })).Start();
            }
        }


        private void RunSelectedProgram()
        {
            if (seachListSelectedIndex >= 0 && File.Exists(searchList[seachListSelectedIndex]))
            {
                if (seachListSelectedIndex >= 0)
                {
                    try
                    {
                        // Run file
                        Process proc = new Process();
                        proc.StartInfo.FileName = searchList[seachListSelectedIndex];
                        proc.StartInfo.UseShellExecute = true;
                        proc.Start();
                    }
                    catch
                    {
                        MessageBox.Show(Properties.Resources.File_FailedToOpenMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Exit application
                    Application.Exit();
                }
            }
            else
                SystemSounds.Beep.Play();
        }

        private void SearchAndRunForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                (new Thread(() => {
                    RunSelectedProgram();
                })).Start();
            }
        }

        private void SearchPanel_Click(object sender, MouseEventArgs e)
        {
            seachListSelectedIndex = (int)((e.Y - searchPanel.AutoScrollPosition.Y) / 60);
            searchPanel.Invalidate();
            Thread.Sleep(50);
            (new Thread(() => {
                RunSelectedProgram();
            })).Start();
        }

        private void SearchAndRunForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > searchFileTextBox.Width + 25 && e.X < searchFileTextBox.Width + 25 + 30 && e.Y > 23 && e.Y < 23 + 30)
                this.Cursor = Cursors.Hand;
            else
                this.Cursor = Cursors.Arrow;
        }

        private void SearchAndRunForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X > searchFileTextBox.Width + 25 && e.X < searchFileTextBox.Width + 25 + 30 && e.Y > 23 && e.Y < 23 + 30)
                Help.ShowPopup(this, Properties.Resources.File_SearchMessage, this.PointToScreen(new Point(searchFileTextBox.Width + 25 + 30, 23 + 30)));
        }
    }
}
