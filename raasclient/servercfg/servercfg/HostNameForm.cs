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
﻿using System;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Elbitin.Applications.RAAS.RAASClient.ServerCfg
{
    public partial class HostNameForm : Form
    {
        public String ServerName { get; set; }
        public System.Windows.Forms.DialogResult DialogButton { get; set; }
        private const int HOST_NAME_TOOLTIP_DURATION = 6000;
        private const int HOS_NAME_MAX_LENGTH = 15;

        public HostNameForm()
        {
            InitializeComponent();
            InitializeLocalizedStrings();
        }

        private void InitializeLocalizedStrings()
        {
            this.cancelButton.Text = Properties.Resources.HostNameForm_CancelButton;
            this.okButton.Text = Properties.Resources.HostNameForm_AddButton;
            this.hostNameLabel.Text = Properties.Resources.HostNameForm_HostNameLabel;
            this.Text = Properties.Resources.HostNameForm_HostNameFormTitle;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (hostTextBox.Text.Length > 0)
            {
                ServerName = hostTextBox.Text;
                DialogButton = DialogResult.OK;
                this.Close();
            }
            else
                MessageBox.Show(Properties.Resources.HostName_EnterHostNameMessage);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            ServerName = "";
            DialogButton = DialogResult.Cancel;
            this.Close();
        }

        private const int ILLEGAL_HOSTNAME_CHARACTERS_TOOLTIP_DURATION = 4000;
        private void HostTextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            Regex disallowedCharacters = new Regex("[*\\[\\]\\(\\)\\?\\|\\<\\>\\\\\\/\\:\\\"]");
            if (!Char.IsControl(e.KeyChar) && hostTextBox.Text.Length >= HOS_NAME_MAX_LENGTH)
            {
                ToolTip messageToolTip = new ToolTip();
                messageToolTip.Show(String.Format(Properties.Resources.HostName_MaximumCharactersToolTip, HOS_NAME_MAX_LENGTH.ToString()), hostTextBox, HOST_NAME_TOOLTIP_DURATION);
                e.Handled = true;
                SystemSounds.Beep.Play();
            }
            if (disallowedCharacters.IsMatch(e.KeyChar.ToString()))
            {
                ToolTip messageToolTip = new ToolTip();
                messageToolTip.Show(Properties.Resources.HostName_AllowedCharactersToolTip, hostTextBox, ILLEGAL_HOSTNAME_CHARACTERS_TOOLTIP_DURATION);
                e.Handled = true;
                SystemSounds.Beep.Play();
            }
        }

        private void HostTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OkButton_Click(this, new EventArgs());
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
