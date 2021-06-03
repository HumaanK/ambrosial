﻿using Ambrosial.Ambrosial;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ambrosial.Ambrosial.Classes;
using Newtonsoft.Json;
using System.Net;
using Ambrosial.Ambrosial.Forms;
using System.Drawing;

namespace Ambrosial
{
    public partial class Launcher : Form
    {
        public Launcher()
        {
            this.Visible = false;
            InitializeComponent();
        }

        private void Launcher_Load(object sender, EventArgs e)
        {
            try
            {
                Version.Text = AmbrosialC.installedVersion;
                Username.Text = new StreamReader(Environment.GetEnvironmentVariable("LocalAppData") + @"\packages\microsoft.minecraftuwp_8WEKYB3D8BBWE\localstate\games\com.mojang\minecraftpe\options.txt").ReadLine().Replace("mp_username:", "");
                setupUI();
            }
            catch { }
            this.Visible = true;
            if (Utils.i >= 1)
            {
                if (AmbrosialC.SettingsJson.shouldGetVersion)
                {
                    DialogResult dialogResult = MessageBox.Show($"There seems to be more than 1 (specifically {Utils.i + 1}) versions of Minecraft: Windows 10 edition installed.\nDo you wish to disable version checking when launching clients?", "Invalid number of installations", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dialogResult == DialogResult.Yes)
                    {
                        AmbrosialC.SettingsJson.shouldGetVersion = false;
                        Directory.CreateDirectory(Utils.ambrosialPath + $@"\assets\userimport\");
                        if (File.Exists(Utils.ambrosialPath + $@"\assets\userimport\AmbrosialConfig-SettingsJson.amb"))
                            File.Delete(Utils.ambrosialPath + $@"\assets\userimport\AmbrosialConfig-SettingsJson.amb");
                        File.WriteAllText(Utils.ambrosialPath + $@"\assets\userimport\AmbrosialConfig-SettingsJson.amb", AmbrosialC.SettingsJson.getEncrypted());
                    }
                }
            }
        }

        int buttonYoffset = 2;

        public void setupUI()
        {

            // setup buttons
            foreach (GameVersion ver in AmbrosialC.versionRegistry)
            {
                Guna.UI2.WinForms.Guna2Button versionButton = new Guna.UI2.WinForms.Guna2Button();
                versionButton.Animated = true;
                versionButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                versionButton.CheckedState.Parent = versionButton;
                versionButton.CustomImages.Parent = versionButton;
                versionButton.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
                versionButton.Font = new System.Drawing.Font("Yu Gothic UI Light", 10.25F);
                versionButton.ForeColor = System.Drawing.Color.White;
                versionButton.HoverState.Parent = versionButton;
                versionButton.Location = new System.Drawing.Point(-1, buttonYoffset);
                versionButton.ShadowDecoration.Enabled = true;
                versionButton.ShadowDecoration.Parent = versionButton;
                versionButton.Size = new System.Drawing.Size(180, 45);
                versionButton.TabIndex = 5;
                versionButton.Text = ver.name;
                // startup panel
                Panel normalPan = new Panel();
                normalPan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
                normalPan.Location = new System.Drawing.Point(176, 26);
                normalPan.Size = new System.Drawing.Size(624, 433);

                // box
                PictureBox boxx = new PictureBox();
                boxx.BackgroundImage = global::Ambrosial.Properties.Resources.Ambrosial;
                boxx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                boxx.Location = new System.Drawing.Point(240, 98);
                boxx.Size = new System.Drawing.Size(151, 191);
                boxx.TabIndex = 9;
                boxx.TabStop = false;

                // label unda box
                Label boxlabel = new Label();
                boxlabel.AutoSize = true;
                boxlabel.Font = new System.Drawing.Font("Yu Gothic UI Light", 10.25F);
                boxlabel.Location = new System.Drawing.Point(282, 270);
                boxlabel.Size = new System.Drawing.Size(69, 19);
                boxlabel.TabIndex = 3;
                boxlabel.Text = "Ambrosial";

                // add
                this.Controls.Add(normalPan);
                normalPan.Controls.Add(boxx);
                normalPan.Controls.Add(boxlabel);
                boxlabel.BringToFront();
                clientspanel.Controls.Add(versionButton);
                buttonYoffset += 45;


                foreach (Client c in ver.clients)
                {
                    // side button
                    Guna.UI2.WinForms.Guna2Button clientButton = new Guna.UI2.WinForms.Guna2Button();
                    clientButton.Animated = true;
                    clientButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                    clientButton.CheckedState.Parent = clientButton;
                    clientButton.CustomImages.Parent = clientButton;
                    clientButton.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
                    clientButton.Font = new System.Drawing.Font("Yu Gothic UI Light", 10.25F);
                    clientButton.ForeColor = System.Drawing.Color.White;
                    clientButton.HoverState.Parent = clientButton;
                    clientButton.Location = new System.Drawing.Point(-1, buttonYoffset);
                    clientButton.ShadowDecoration.Enabled = true;
                    clientButton.ShadowDecoration.Parent = clientButton;
                    clientButton.Size = new System.Drawing.Size(180, 45);
                    clientButton.TabIndex = 0;
                    clientButton.Text = c.name;
                    buttonYoffset += 45;
                    if (!c.hasCachedPanel)
                    {
                        Utils.log($@"Panel for {c.name} is not cached, creating...");
                        // main panel
                        Panel pan = new Panel();
                        pan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
                        pan.Location = new System.Drawing.Point(176, 26);
                        pan.Size = new System.Drawing.Size(624, 433);
                        clientButton.MouseDown += (sender, EventArgs) => { showPanel(sender, EventArgs, pan); };

                        // panel banner
                        PictureBox banner = new PictureBox();
                        banner.BackgroundImage = Image.FromFile(c.bannerphotoPath);
                        banner.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                        banner.Location = new System.Drawing.Point(0, 1);
                        banner.Size = new System.Drawing.Size(624, 191);
                        banner.TabIndex = 4;
                        banner.TabStop = false;

                        // client lalbel
                        Label lab = new Label();
                        lab.AutoSize = true;
                        lab.Font = new System.Drawing.Font("Yu Gothic UI Light", 20.25F);
                        lab.Location = new System.Drawing.Point(3, 195);
                        lab.Size = new System.Drawing.Size(75, 37);
                        lab.Text = c.name;

                        // desc
                        Label desclabel = new Label();
                        desclabel.AutoSize = true;
                        desclabel.Font = new System.Drawing.Font("Yu Gothic UI Light", 12.25F);
                        desclabel.Location = new System.Drawing.Point(7, 229);
                        desclabel.Size = new System.Drawing.Size(194, 23);
                        desclabel.TabIndex = 5;
                        desclabel.Text = $"{c.version} : ";
                        foreach (string line in c.types)
                        {
                            desclabel.Text += line + " : ";
                        }
                        desclabel.Text = Utils.TrimEnd(desclabel.Text, " : ");

                        // latest update top text
                        Label latestUpd = new Label();
                        latestUpd.AutoSize = true;
                        latestUpd.Font = new System.Drawing.Font("Yu Gothic UI Light", 14.25F);
                        latestUpd.Location = new System.Drawing.Point(8, 252);
                        latestUpd.Size = new System.Drawing.Size(125, 25);
                        latestUpd.TabIndex = 6;
                        latestUpd.Text = "Latest update:";

                        // actual update text
                        Label updText = new Label();
                        updText.AutoSize = true;
                        updText.Font = new System.Drawing.Font("Yu Gothic UI Light", 10.25F);
                        updText.Location = new System.Drawing.Point(22, 280);
                        updText.Size = new System.Drawing.Size(401, 19);
                        updText.Text = c.latestUpdateInfo;

                        // separataor
                        Guna.UI2.WinForms.Guna2VSeparator separator = new Guna.UI2.WinForms.Guna2VSeparator();
                        separator.FillThickness = 3;
                        separator.Location = new System.Drawing.Point(11, 280);
                        separator.Size = new System.Drawing.Size(10, 19);

                        Size size = TextRenderer.MeasureText(updText.Text, updText.Font);
                        updText.Height = size.Height;
                        separator.Height = size.Height;

                        // button
                        Guna.UI2.WinForms.Guna2Button launch = new Guna.UI2.WinForms.Guna2Button();
                        launch.Animated = true;
                        launch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                        launch.CheckedState.Parent = launch;
                        launch.CustomImages.Parent = launch;
                        launch.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
                        launch.Font = new System.Drawing.Font("Yu Gothic UI Light", 10.25F);
                        launch.ForeColor = System.Drawing.Color.White;
                        launch.HoverState.Parent = launch;
                        launch.Location = new System.Drawing.Point(226, 367);
                        launch.ShadowDecoration.Enabled = true;
                        launch.ShadowDecoration.Parent = launch;
                        launch.Size = new System.Drawing.Size(180, 45);
                        launch.TabIndex = 6;
                        launch.Text = "Launch";
                        launch.MouseDown += (sender, EventArgs) => { installEvent(sender, EventArgs, c); };

                        clientspanel.Controls.Add(clientButton);
                        this.Controls.Add(pan);
                        pan.Controls.Add(banner);
                        pan.Controls.Add(lab);
                        pan.Controls.Add(launch);
                        pan.Controls.Add(latestUpd);
                        pan.Controls.Add(separator);
                        pan.Controls.Add(desclabel);
                        pan.Controls.Add(updText);
                        Utils.log($"Created {c.name}'s panel");
                        c.hasCachedPanel = true;
                    }
                }
                normalPan.BringToFront();
            }
        }

        void showPanel(object sender, MouseEventArgs e, Panel p)
        {
            if (e.Button == MouseButtons.Left)
            {
                Utils.log("showPanel event triggered for panel " + p.Name);
                p.BringToFront();
            }
        }

        void installEvent(object sender, MouseEventArgs e, Client c)
        {
            if (e.Button == MouseButtons.Left)
            {
                c.install();
            }
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void minbut_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Username_DoubleClick(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Generate JSON?", "Developer mode", MessageBoxButtons.YesNo);
            if (Microsoft.VisualBasic.Interaction.InputBox("Enter password for developer mode.", "Enter password", "") == "dev2021")
            {
                if (dialogResult == DialogResult.Yes)
                {
                    string name = Microsoft.VisualBasic.Interaction.InputBox("Name", "JSON Generator", "");
                    string[] types = Microsoft.VisualBasic.Interaction.InputBox("Types (split with ,)", "JSON Generator", "").Split(',');
                    string link = Microsoft.VisualBasic.Interaction.InputBox("Link", "JSON Generator", "");
                    string banner = Microsoft.VisualBasic.Interaction.InputBox("Banner photo", "JSON Generator", "");
                    string upd = Microsoft.VisualBasic.Interaction.InputBox("Latest update text", "JSON Generator", "");
                    string exe = Microsoft.VisualBasic.Interaction.InputBox("Exe/Dll name", "JSON Generator", "");
                    string ver = Microsoft.VisualBasic.Interaction.InputBox("Version for", "JSON Generator", "");
                    string ver2 = Microsoft.VisualBasic.Interaction.InputBox("Client's version", "JSON Generator", "");
                    bool zip;
                    DialogResult dialogResult5 = MessageBox.Show("Is it stored in a ZIP?", "JSON Generator", MessageBoxButtons.YesNo);
                    if (dialogResult5 == DialogResult.Yes)
                    {
                        zip = true;
                    }
                    else
                        zip = false;
                    DialogResult dialogResult2 = MessageBox.Show("EXE or DLL? Yes = exe, No = dll", "JSON Generator", MessageBoxButtons.YesNo);
                    Client client;
                    if (dialogResult2 == DialogResult.Yes)
                    {
                        client = new Client(name, types, link, banner, upd, exe, ClientTypes.Type.Exe, ver, ver2, zip);
                    }
                    else
                    {
                        client = new Client(name, types, link, banner, upd, exe, ClientTypes.Type.Exe, ver, ver2, zip);
                    }
                    Utils.Serialize(client);
                    Clipboard.SetText(File.ReadAllText("Ambrosial.json"));
                    DialogResult dialogResult3 = MessageBox.Show("Add it to the client registry? (For encrypting in dev form)", "JSON Generator", MessageBoxButtons.YesNo);
                    if (dialogResult3 == DialogResult.Yes)
                    {
                        AmbrosialC.clientRegistry.Add(client);
                    }
                }
            }
        }

        private void Version_DoubleClick(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Open developer form?", "Developer mode", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (Microsoft.VisualBasic.Interaction.InputBox("Enter password for developer mode.", "Enter password", "") == "dev2021")
                {
                    new DeveloperForm().Show();
                }
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Client test = new Client("Internal test", new string[] { "hi" }, "https://cdn.discordapp.com/attachments/787223049333899265/832122943076565052/horian.dll", "no", "no", "Internal test.dll", ClientTypes.Type.Dll, "1.16.220", "1.16.220", false);
            test.install();

        }

        private void panel4_DoubleClick(object sender, EventArgs e)
        {

        }

        private void addnew_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Add custom app?", "Custom Application", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                string name = Microsoft.VisualBasic.Interaction.InputBox("Name of client", "Custom Application", "");
                string[] types = Microsoft.VisualBasic.Interaction.InputBox("Types (split with ,)", "Custom Application", "").Split(',');
                string link = Microsoft.VisualBasic.Interaction.InputBox("Link to .zip/.exe/.dll", "Custom Application", "");
                string banner = Microsoft.VisualBasic.Interaction.InputBox("Banner photo link/path", "Custom Application", "");
                string upd = Microsoft.VisualBasic.Interaction.InputBox("Latest update text", "Custom Application", "");
                string exe = Microsoft.VisualBasic.Interaction.InputBox("Final .exe/.dll name", "Custom Application", "");
                string ver = Microsoft.VisualBasic.Interaction.InputBox("For version of Minecraft", "Custom Application", "");
                string ver2 = Microsoft.VisualBasic.Interaction.InputBox("Client's version", "Custom Application", "");
                bool zip;
                DialogResult dialogResult5 = MessageBox.Show("Is it stored in a ZIP?", "Custom Application", MessageBoxButtons.YesNo);
                if (dialogResult5 == DialogResult.Yes)
                {
                    zip = true;
                }
                else
                    zip = false;
                DialogResult dialogResult2 = MessageBox.Show("EXE or DLL?\nYes = EXE\nNo = DLL", "Custom Application", MessageBoxButtons.YesNo);
                Client client;
                if (dialogResult2 == DialogResult.Yes)
                {
                    client = new Client(name, types, link, banner, upd, exe, ClientTypes.Type.Exe, ver, ver2, zip);
                }
                else
                {
                    client = new Client(name, types, link, banner, upd, exe, ClientTypes.Type.Exe, ver, ver2, zip);
                }
                Directory.CreateDirectory(Utils.ambrosialPath + $@"\assets\userimport\importedclients\");
                if (!File.Exists(Utils.ambrosialPath + $@"\assets\userimport\importedclients\{name}.amb"))
                    File.Delete(Utils.ambrosialPath + $@"\assets\userimport\importedclients\{name}.amb");
                File.WriteAllText(Utils.ambrosialPath + $@"\assets\userimport\importedclients\{name}.amb", AmbrosialEncrypt.Encrypt(JsonConvert.SerializeObject(client)));
                MessageBox.Show("Restart required to load the new client!", "Info");
            }

        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            new SettingsForm().Show();
        }
    }
}
