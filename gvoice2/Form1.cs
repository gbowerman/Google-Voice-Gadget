using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using SharpVoice;
using System.IO;
using System.Configuration;

namespace gvoice2
{
    public partial class GoogleDialler : Form
    {
        string myCell = "";
        string myWork = "";
        string myHome = "";
        String myNumber = "";
        string accountName = "";
        string accountPassword = "";
        int lastMyNumber = 1;

        // programmable preset buttons
        string presetName1 = "Preset1";
        string presetName2 = "Preset2";
        string presetName3 = "Preset3";
        string presetName4 = "Preset4";
        string presetName5 = "Preset5";
        string presetName6 = "Preset6";
        string presetName7 = "Preset7";
        string presetName8 = "Preset8";
        string presetName9 = "Preset9";
        string presetNumber1 = "";
        string presetNumber2 = "";
        string presetNumber3 = "";
        string presetNumber4 = "";
        string presetNumber5 = "";
        string presetNumber6 = "";
        string presetNumber7 = "";
        string presetNumber8 = "";
        string presetNumber9 = "";

        // globals for storing config data
        string keyStoreDir = ".";
        string configFile = "gvoice.config";

        Voice voiceConnection = null;

        public GoogleDialler()
        {
            InitializeComponent();
            try
            {
                keyStoreDir = System.Environment.GetEnvironmentVariable("LOCALAPPDATA");
                if (keyStoreDir == null)
                    keyStoreDir = System.Environment.GetEnvironmentVariable("APPDATA");
                if (keyStoreDir == null) keyStoreDir = ".";
                keyStoreDir += "\\cosmicfort\\gvoice2";
                if (!Directory.Exists(keyStoreDir))
                {
                    Directory.CreateDirectory(keyStoreDir);
                }
                Directory.SetCurrentDirectory(keyStoreDir);

                if (!File.Exists(configFile)) File.Create(configFile);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Unable to set the key store folder. Error: " +
                    Ex.Message + ". Using current working directory",
                    "App data create exception");
                return;
            }

            // load application settings
            System.Configuration.Configuration cfg = ConfigurationManager.OpenExeConfiguration(configFile);
            if (cfg.AppSettings.Settings["accountName"] != null)
            {
                accountName = cfg.AppSettings.Settings["accountName"].Value;
                accountNameText.Text = accountName;
            }
            if (cfg.AppSettings.Settings["password"] != null)
            {
                accountPassword = cfg.AppSettings.Settings["password"].Value;
            }
            if (cfg.AppSettings.Settings["myCell"] != null)
            {
                myCell = cfg.AppSettings.Settings["myCell"].Value;
                cellphoneText.Text = myCell;
                myNumber = myCell;
            }
            if (cfg.AppSettings.Settings["myWork"] != null)
            {
                myWork = cfg.AppSettings.Settings["myWork"].Value;
                workPhoneText.Text = myWork;
            }
            if (cfg.AppSettings.Settings["myHome"] != null)
            {
                myHome = cfg.AppSettings.Settings["myHome"].Value;
                homePhoneText.Text = myHome;
            }
            if (cfg.AppSettings.Settings["presetName1"] != null)
            {
                presetName1 = cfg.AppSettings.Settings["presetName1"].Value;
                Preset1Button.Text = presetName1;
            }
            if (cfg.AppSettings.Settings["presetName2"] != null)
            {
                presetName2 = cfg.AppSettings.Settings["presetName2"].Value;
                Preset2Button.Text = presetName2;
            }
            if (cfg.AppSettings.Settings["presetName3"] != null)
            {
                presetName3 = cfg.AppSettings.Settings["presetName3"].Value;
                Preset3Button.Text = presetName3;
            }
            if (cfg.AppSettings.Settings["presetName4"] != null)
            {
                presetName4 = cfg.AppSettings.Settings["presetName4"].Value;
                Preset4Button.Text = presetName4;
            }
            if (cfg.AppSettings.Settings["presetName5"] != null)
            {
                presetName5 = cfg.AppSettings.Settings["presetName5"].Value;
                Preset5Button.Text = presetName5;
            }
            if (cfg.AppSettings.Settings["presetName6"] != null)
            {
                presetName6 = cfg.AppSettings.Settings["presetName6"].Value;
                Preset6Button.Text = presetName6;
            }
            if (cfg.AppSettings.Settings["presetName7"] != null)
            {
                presetName7 = cfg.AppSettings.Settings["presetName7"].Value;
                Preset7Button.Text = presetName7;
            }
            if (cfg.AppSettings.Settings["presetName8"] != null)
            {
                presetName8 = cfg.AppSettings.Settings["presetName8"].Value;
                Preset8Button.Text = presetName8;
            }
            if (cfg.AppSettings.Settings["presetName9"] != null)
            {
                presetName9 = cfg.AppSettings.Settings["presetName9"].Value;
                Preset9Button.Text = presetName9;
            }
            if (cfg.AppSettings.Settings["presetNumber1"] != null)
            {
                presetNumber1 = cfg.AppSettings.Settings["presetNumber1"].Value;
            }
            if (cfg.AppSettings.Settings["presetNumber2"] != null)
            {
                presetNumber2 = cfg.AppSettings.Settings["presetNumber2"].Value;
            }
            if (cfg.AppSettings.Settings["presetNumber3"] != null)
            {
                presetNumber3 = cfg.AppSettings.Settings["presetNumber3"].Value;
            }
            if (cfg.AppSettings.Settings["presetNumber4"] != null)
            {
                presetNumber4 = cfg.AppSettings.Settings["presetNumber4"].Value;
            }
            if (cfg.AppSettings.Settings["presetNumber5"] != null)
            {
                presetNumber5 = cfg.AppSettings.Settings["presetNumber5"].Value;
            }
            if (cfg.AppSettings.Settings["presetNumber6"] != null)
            {
                presetNumber6 = cfg.AppSettings.Settings["presetNumber6"].Value;
            }
            if (cfg.AppSettings.Settings["presetNumber7"] != null)
            {
                presetNumber7 = cfg.AppSettings.Settings["presetNumber7"].Value;
            }
            if (cfg.AppSettings.Settings["presetNumber8"] != null)
            {
                presetNumber8 = cfg.AppSettings.Settings["presetNumber8"].Value;
            }
            if (cfg.AppSettings.Settings["presetNumber9"] != null)
            {
                presetNumber9 = cfg.AppSettings.Settings["presetNumber9"].Value;
            }
            if (cfg.AppSettings.Settings["lastMyNumber"] != null)
            {
                lastMyNumber = Convert.ToInt32(cfg.AppSettings.Settings["lastMyNumber"].Value);
            }

            // set myNumber to the last value
            switch (lastMyNumber)
            {
                case 1:
                    cellRadioButton.Checked = true;
                    break;
                case 2:
                    workRadioButton.Checked = true;
                    break;
                case 3:
                    homeRadioButton.Checked = true;
                    break;
            }
        }

        private void dialButton_Click(object sender, EventArgs e)
        {
            if (outNumberText.TextLength < 3)
            {
                MessageBox.Show("Enter a number before selecting Dial", "Invalid outgoing number");
            }
            else
            {
                doCall(outNumberText.Text);
            }
        }

        private void smsButton_Click(object sender, EventArgs e)
        {
            if (outNumberText.TextLength < 3 || outNumberText.Text.Equals("<outgoing number>"))
            {
                MessageBox.Show("Enter a number before selecting SMS", "Invalid outgoing number");
            }
            else if (smsTextBox.TextLength < 1 || smsTextBox.Text.Equals("<text message>"))
            {
                MessageBox.Show("Enter a text message before selecting SMS", "Invalid outgoing text");
            }
            else
            {
                if (voiceConnection == null)
                {
                    if (doLogin() != 0) return;
                }
                string response = voiceConnection.sendSMS(outNumberText.Text, smsTextBox.Text);
                if (response.StartsWith("{\"ok\":true"))
                {
                    MessageBox.Show("Your SMS is being sent..", "Success");
                }
                else MessageBox.Show(response, "SMS status");
            }
        }
        private void Preset1Button_Click(object sender, EventArgs e)
        {
            doCall(presetNumber1);
        }

        private void Preset2Button_Click(object sender, EventArgs e)
        {
            doCall(presetNumber2);
        }

        private void Preset3Button_Click(object sender, EventArgs e)
        {
            doCall(presetNumber3);
        }

        private void Preset4Button_Click(object sender, EventArgs e)
        {
            doCall(presetNumber4);
        }

        private void Preset5Button_Click(object sender, EventArgs e)
        {
            doCall(presetNumber5);
        }

        private void Preset6Button_Click(object sender, EventArgs e)
        {
            doCall(presetNumber6);
        }

        private void Preset7Button_Click(object sender, EventArgs e)
        {
            doCall(presetNumber7);
        }

        private void Preset8Button_Click(object sender, EventArgs e)
        {
            doCall(presetNumber8);
        }

        private void Preset9Button_Click(object sender, EventArgs e)
        {
            doCall(presetNumber9);
        }


        private void cellRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            myNumber = myCell;
            lastMyNumber = 1;
            saveLastMyNumber();
        }

        private void workRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            myNumber = myWork;
            lastMyNumber = 2;
            saveLastMyNumber();
        }

        private void homeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            myNumber = myHome;
            lastMyNumber = 3;
            saveLastMyNumber();
        }

        private void outNumberText_Click(object sender, EventArgs e)
        {
            outNumberText.SelectionStart = 0;
            outNumberText.SelectionLength = outNumberText.Text.Length;
        }

        private void smsTextBox_Click(object sender, EventArgs e)
        {     
            smsTextBox.SelectionStart = 0;
            smsTextBox.SelectionLength = smsTextBox.Text.Length;       
        }
        private void doCall(string phoneNumber)
        {
            if (myNumber.Length < 10)
            {
                MessageBox.Show("Set a value for your home/work/cell before calling", "Invalid home number");
                return;
            }
            if (voiceConnection == null)
            {
                if (doLogin() != 0) return;
            }

            string response = voiceConnection.call(phoneNumber, myNumber);
            if (response.StartsWith("{\"ok\":true"))
            {
                MessageBox.Show("Your call is being connected..", "Success");
            }
            else MessageBox.Show(response, "Call status");
        }

        private int doLogin()
        {
            if (accountName.Length < 7)
            {
                MessageBox.Show("Account name not set correctly, expecting a gmail or googlemail address", 
                    "Account Name Error");
                return 1;
            }
            if (accountPassword.Length < 3)
            {
                MessageBox.Show("Password not set correctly", "Bad password");
                return 1;
            }
            voiceConnection = new Voice(accountName, accountPassword);
            voiceConnection.login();
            //MessageBox.Show("Logged in");
            return 0;
        }

        private void saveLastMyNumber()
        {
            System.Configuration.Configuration cfg =
                ConfigurationManager.OpenExeConfiguration(configFile);
            cfg.AppSettings.Settings.Remove("lastMyNumber");
            cfg.AppSettings.Settings.Add("lastMyNumber", lastMyNumber.ToString());

            // Save the configuration file.
            cfg.Save(ConfigurationSaveMode.Full, true);
        }

        private void accountNameText_Click(object sender, EventArgs e)
        {
            accountNameText.SelectionStart = 0;
            accountNameText.SelectionLength = accountNameText.Text.Length;
        }

        private void passwordText_Click(object sender, EventArgs e)
        {
            passwordText.UseSystemPasswordChar = true;
            passwordText.SelectionStart = 0;
            passwordText.SelectionLength = passwordText.Text.Length;
        }

        private void confirmPasswordText_Click(object sender, EventArgs e)
        {
            confirmPasswordText.UseSystemPasswordChar = true;
            confirmPasswordText.SelectionStart = 0;
            confirmPasswordText.SelectionLength = confirmPasswordText.Text.Length;
        }

        private void setButton_Click(object sender, EventArgs e)
        {
            if (accountNameText.Text.Length < 7 || accountNameText.Text.StartsWith("<"))
            {
                MessageBox.Show("Account name not set correctly, expecting a gmail or googlemail address",
                    "Account Name Error");
                return;
            }
            if (passwordText.Text.Length < 3 || passwordText.Text.StartsWith("<"))
            {
                MessageBox.Show("Password not set correctly", "Bad password");
                return;
            }
            if (!passwordText.Text.Equals(confirmPasswordText.Text))
            {
                MessageBox.Show("Passwords do not match", "Password mismatch");
                return;
            }
            accountName = accountNameText.Text;
            accountPassword = passwordText.Text;

            System.Configuration.Configuration cfg =
                ConfigurationManager.OpenExeConfiguration(configFile);
            cfg.AppSettings.Settings.Remove("accountName");
            cfg.AppSettings.Settings.Remove("password");
            cfg.AppSettings.Settings.Add("accountName", accountName);
            cfg.AppSettings.Settings.Add("password", accountPassword);

            // Save the configuration file.
            cfg.Save(ConfigurationSaveMode.Full, true);
            MessageBox.Show("Account details for " + accountName + " saved.", "Account saved");
        }

        private void setHomeButton_Click(object sender, EventArgs e)
        {
            if (cellphoneText.Text.Length < 10)
            {
                MessageBox.Show("Invalid cell phone number", "Cell number Error");
                return;
            }
            if (workPhoneText.Text.Length < 10)
            {
                MessageBox.Show("Invalid work phone number", "Work number Error");
                return;
            }
            if (homePhoneText.Text.Length < 10)
            {
                MessageBox.Show("Invalid home phone number", "Cell number Error");
                return;
            }

            myCell = cellphoneText.Text;
            if (cellRadioButton.Checked == true) myNumber = myCell;
            myWork = workPhoneText.Text;
            if (workRadioButton.Checked == true) myNumber = myWork;
            myHome = homePhoneText.Text;
            if (homeRadioButton.Checked == true) myNumber = myHome;

            // save number settings to app data
            System.Configuration.Configuration cfg =
                ConfigurationManager.OpenExeConfiguration(configFile);
            cfg.AppSettings.Settings.Remove("myCell");
            cfg.AppSettings.Settings.Remove("myWork");
            cfg.AppSettings.Settings.Remove("myHome");
            cfg.AppSettings.Settings.Add("myCell", myCell);
            cfg.AppSettings.Settings.Add("myWork", myWork);
            cfg.AppSettings.Settings.Add("myHome", myHome);

            // Save the configuration file.
            cfg.Save(ConfigurationSaveMode.Full, true);
            MessageBox.Show("Numbers saved.", "My numbers saved");
        }

        private void cellphoneText_Enter(object sender, EventArgs e)
        {
            cellphoneText.SelectionStart = 0;
            cellphoneText.SelectionLength = cellphoneText.Text.Length;
        }

        private void workPhoneText_Enter(object sender, EventArgs e)
        {
            workPhoneText.SelectionStart = 0;
            workPhoneText.SelectionLength = workPhoneText.Text.Length;
        }

        private void homePhoneText_Enter(object sender, EventArgs e)
        {
            homePhoneText.SelectionStart = 0;
            homePhoneText.SelectionLength = homePhoneText.Text.Length;
        }

        private void presetNameText_Enter(object sender, EventArgs e)
        {
            presetNameText.SelectionStart = 0;
            presetNameText.SelectionLength = presetNameText.Text.Length;
        }

        private void presetNumberText_Enter(object sender, EventArgs e)
        {
            presetNumberText.SelectionStart = 0;
            presetNumberText.SelectionLength = presetNumberText.Text.Length;
        }

        private void savePresetButton_Click(object sender, EventArgs e)
        {
            if (presetListBox.SelectedItem == null)
            {
                MessageBox.Show("Select a preset number before saving.", "No preset selected");
                return;
            }
            System.Configuration.Configuration cfg = ConfigurationManager.OpenExeConfiguration(configFile);
            switch (presetListBox.SelectedItem.ToString())
            {
                case "Preset1":
                    Preset1Button.Text = presetNameText.Text;
                    cfg.AppSettings.Settings.Remove("presetName1");
                    cfg.AppSettings.Settings.Remove("presetNumber1");
                    cfg.AppSettings.Settings.Add("presetName1", presetNameText.Text);
                    cfg.AppSettings.Settings.Add("presetNumber1", presetNumberText.Text);
                    break;
                case "Preset2":
                    Preset2Button.Text = presetNameText.Text;
                    cfg.AppSettings.Settings.Remove("presetName2");
                    cfg.AppSettings.Settings.Remove("presetNumber2");
                    cfg.AppSettings.Settings.Add("presetName2", presetNameText.Text);
                    cfg.AppSettings.Settings.Add("presetNumber2", presetNumberText.Text);
                    break;
                case "Preset3":
                    Preset3Button.Text = presetNameText.Text;
                    cfg.AppSettings.Settings.Remove("presetName3");
                    cfg.AppSettings.Settings.Remove("presetNumber3");
                    cfg.AppSettings.Settings.Add("presetName3", presetNameText.Text);
                    cfg.AppSettings.Settings.Add("presetNumber3", presetNumberText.Text);
                    break;
                case "Preset4":
                    Preset4Button.Text = presetNameText.Text;
                    cfg.AppSettings.Settings.Remove("presetName4");
                    cfg.AppSettings.Settings.Remove("presetNumber4");
                    cfg.AppSettings.Settings.Add("presetName4", presetNameText.Text);
                    cfg.AppSettings.Settings.Add("presetNumber4", presetNumberText.Text);
                    break;
                case "Preset5":
                    Preset5Button.Text = presetNameText.Text;
                    cfg.AppSettings.Settings.Remove("presetName5");
                    cfg.AppSettings.Settings.Remove("presetNumber5");
                    cfg.AppSettings.Settings.Add("presetName5", presetNameText.Text);
                    cfg.AppSettings.Settings.Add("presetNumber5", presetNumberText.Text);
                    break;
                case "Preset6":
                    Preset6Button.Text = presetNameText.Text;
                    cfg.AppSettings.Settings.Remove("presetName6");
                    cfg.AppSettings.Settings.Remove("presetNumber6");
                    cfg.AppSettings.Settings.Add("presetName6", presetNameText.Text);
                    cfg.AppSettings.Settings.Add("presetNumber6", presetNumberText.Text);
                    break;
                case "Preset7":
                    Preset7Button.Text = presetNameText.Text;
                    cfg.AppSettings.Settings.Remove("presetName7");
                    cfg.AppSettings.Settings.Remove("presetNumber7");
                    cfg.AppSettings.Settings.Add("presetName7", presetNameText.Text);
                    cfg.AppSettings.Settings.Add("presetNumber7", presetNumberText.Text);
                    break;
                case "Preset8":
                    Preset8Button.Text = presetNameText.Text;
                    cfg.AppSettings.Settings.Remove("presetName8");
                    cfg.AppSettings.Settings.Remove("presetNumber8");
                    cfg.AppSettings.Settings.Add("presetName8", presetNameText.Text);
                    cfg.AppSettings.Settings.Add("presetNumber8", presetNumberText.Text);
                    break;
                case "Preset9":
                    Preset9Button.Text = presetNameText.Text;
                    cfg.AppSettings.Settings.Remove("presetName9");
                    cfg.AppSettings.Settings.Remove("presetNumber9");
                    cfg.AppSettings.Settings.Add("presetName9", presetNameText.Text);
                    cfg.AppSettings.Settings.Add("presetNumber9", presetNumberText.Text);
                    break;
            }

            // Save the configuration file.
            cfg.Save(ConfigurationSaveMode.Full, true);
            MessageBox.Show("New presets saved.", "Presets saved");
        }
    }
}
