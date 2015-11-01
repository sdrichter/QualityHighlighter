/*
  QualityHighlighter plugin for KeePass 2.x.
  Copyright (C) 2015 by Scott Richter <scott.d.richter@gmail.com>

  This program is free software; you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation; either version 2 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using KeePass.Ecas;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib;
using KeePassLib.Cryptography;
using KeePassLib.Security;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace QualityHighlighter
{
    /// <summary>
    /// A simple plugin to highlight rows in the KeePass database based on password quality.
    /// Allows for easily spotting weak passwords.
    /// </summary>
    public class QualityHighlighterExt : Plugin
    {
        /// <summary>
        /// Name of the custom toolbar button to toggle the highlights.
        /// </summary>
        private const string ToggleBtnCommand = "QualityHighlighterToggle";

        private IPluginHost _host = null;
        private bool _highlightsOn = true;

        //Quality classification cutoffs, populated per KeePass website.
        //In the future, might make these configurable.
        private const uint VeryWeakQualityMax = 64;
        private const uint WeakQualityMax = 80;
        private const uint ModerateQualityMax = 112;
        private const uint StrongQualityMax = 128;

        //Predefined colors for each quality level.
        //In the future, might make these configurable.
        private Color VeryWeakColor = Color.Red;
        private Color WeakColor = Color.Orange;
        private Color ModerateColor = Color.Yellow;
        private Color StrongColor = Color.YellowGreen;
        private Color VeryStrongColor = Color.Green;

        public override bool Initialize(IPluginHost host)
        {
            _highlightsOn = true;

            _host = host;

            _host.MainWindow.UIStateUpdated += MainWindow_UIStateUpdated;

            _host.MainWindow.AddCustomToolBarButton(ToggleBtnCommand, "Toggle Highlights", "Toggle quality level highlights on or off.");
            _host.TriggerSystem.RaisingEvent += TriggerSystem_RaisingEvent;

            return true;
        }

        public override void Terminate()
        {
            _host.MainWindow.UIStateUpdated -= MainWindow_UIStateUpdated;
            _host.TriggerSystem.RaisingEvent -= TriggerSystem_RaisingEvent;

            _host.MainWindow.RemoveCustomToolBarButton(ToggleBtnCommand);
        }

        public override Image SmallIcon
        {
            get
            {
                //TODO: Implement.
                return base.SmallIcon;
            }
        }

        public override string UpdateUrl
        {
            get
            {
                return "https://cdn.rawgit.com/sdrichter/QualityHighlighter/master/VERSION";
            }
        }
        
        private void TriggerSystem_RaisingEvent(object sender, EcasRaisingEventArgs e)
        {
            //Check if the event is our toggle button and toggle the highlights on or off accordingly.
            EcasPropertyDictionary dict = e.Properties;
            if(dict == null) { Debug.Assert(false); return; }

            string command = e.Properties.Get<string>(EcasProperty.CommandID);

            if (command != null && command.Equals(ToggleBtnCommand))
            {
                _highlightsOn = !_highlightsOn;
                _host.MainWindow.UpdateUI(false, null, false, null, true, null, false);
            }
        }

        private void MainWindow_UIStateUpdated(object sender, EventArgs e)
        {
            //This method iterates through all the entries and either highlights
            //them if highlighting is on, or unhighlights if it is off.
            ListView lv = (_host.MainWindow.Controls.Find(
                "m_lvEntries", true)[0] as ListView);
            if (lv == null) { Debug.Assert(false); return; }

            lv.BeginUpdate();

            foreach (ListViewItem lvi in lv.Items)
            {
                if (_highlightsOn)
                {
                    PwListItem li = (lvi.Tag as PwListItem);
                    if (li == null) { Debug.Assert(false); continue; }

                    PwEntry pe = li.Entry;
                    if (pe == null) { Debug.Assert(false); continue; }

                    ProtectedString pStr = pe.Strings.Get(PwDefs.PasswordField);
                    if (pStr == null) { Debug.Assert(false); continue; }

                    string pw = pStr.ReadString();
                    uint bits = QualityEstimation.EstimatePasswordBits(pw.ToCharArray());
                    if (bits <= VeryWeakQualityMax)
                        lvi.BackColor = VeryWeakColor;
                    else if (bits <= WeakQualityMax)
                        lvi.BackColor = WeakColor;
                    else if (bits <= ModerateQualityMax)
                        lvi.BackColor = ModerateColor;
                    else if (bits <= StrongQualityMax)
                        lvi.BackColor = StrongColor;
                    else
                        lvi.BackColor = VeryStrongColor;
                }
                else
                {
                    lvi.BackColor = Color.Transparent;
                }
            }

            lv.EndUpdate();
        }
    }
}