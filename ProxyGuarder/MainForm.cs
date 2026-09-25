using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ProxyGuarder
{
    internal class MainForm : Form
    {
        private AppConfig _config;
        private List<string> _networks;
        private bool _blocked;
        private int _countdown;

        private Label _titleLabel;
        private Label _networkLabel;
        private Label _statusLabel;
        private Button _btnEnter;
        private Button _btnSettings;
        private Button _btnCancel;
        private Timer _timer;

        public MainForm()
        {
            _config = AppConfig.Load();

            Text = "Proxy Guarder";
            SetWindowIcon();
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(500, 280);
            Font = new Font("Aptos", 10F);

            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += OnTimerTick;

            BuildControls();
            EvaluateAndRender();
        }

        private void SetWindowIcon()
        {
            Stream iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ProxyGuarder.ico");
            if (iconStream != null)
            {
                Icon = new Icon(iconStream);
            }
        }
        private void BuildControls()
        {
            _titleLabel = new Label();
            _titleLabel.Text = "Proxy Guarder 网络拦截";
            _titleLabel.Font = new Font("Aptos", 16F, FontStyle.Bold);
            _titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            _titleLabel.SetBounds(0, 18, 500, 40);

            _networkLabel = new Label();
            _networkLabel.Font = new Font("Aptos", 12F);
            _networkLabel.TextAlign = ContentAlignment.MiddleCenter;
            _networkLabel.SetBounds(0, 70, 500, 36);

            _statusLabel = new Label();
            _statusLabel.Font = new Font("Aptos", 11F);
            _statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            _statusLabel.SetBounds(0, 116, 500, 40);

            _btnEnter = new Button();
            _btnEnter.Text = "启动代理工具";
            _btnEnter.SetBounds(75, 190, 130, 40);
            _btnEnter.Click += (s, e) => LaunchProxyTool();

            _btnSettings = new Button();
            _btnSettings.Text = "设置";
            _btnSettings.SetBounds(225, 190, 90, 40);
            _btnSettings.Click += (s, e) => OpenSettings();

            _btnCancel = new Button();
            _btnCancel.Text = "取消";
            _btnCancel.SetBounds(335, 190, 90, 40);
            _btnCancel.Click += (s, e) => Close();

            Controls.Add(_titleLabel);
            Controls.Add(_networkLabel);
            Controls.Add(_statusLabel);
            Controls.Add(_btnEnter);
            Controls.Add(_btnSettings);
            Controls.Add(_btnCancel);            StyleButton(_btnEnter, true);
            StyleButton(_btnSettings, false);
            StyleButton(_btnCancel, false);
        }


        private static void StyleButton(Button button, bool primary)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = primary ? 0 : 1;
            button.FlatAppearance.BorderColor = Color.FromArgb(208, 215, 222);
            button.BackColor = primary ? Color.FromArgb(35, 134, 54) : Color.FromArgb(246, 248, 250);
            button.ForeColor = primary ? Color.White : Color.FromArgb(31, 35, 40);
            button.Cursor = Cursors.Hand;
        }        private void EvaluateAndRender()
        {
            _networks = NetworkManager.GetConnectedNetworkNames();
            _blocked = IsAnyBlocked();

            string display = _networks.Count == 0
                ? "（未连接任何网络）"
                : string.Join("  /  ", _networks.ToArray());

            _networkLabel.Text = "当前网络：" + display;
            _networkLabel.ForeColor = _blocked ? Color.Red : Color.Black;

            if (_blocked)
            {
                _countdown = 5;
                _statusLabel.Text = "当前网络在黑名单中，" + _countdown + " 秒后自动退出";
                _statusLabel.ForeColor = Color.Red;
                SetButtonsEnabled(false);
                _timer.Start();
            }
            else
            {
                _statusLabel.Text = "请确认是否启动代理工具";
                _statusLabel.ForeColor = Color.Black;
                SetButtonsEnabled(true);
                _timer.Stop();
            }
        }

        private bool IsAnyBlocked()
        {
            foreach (string name in _networks)
            {
                if (_config.IsBlacklisted(name))
                {
                    return true;
                }
            }
            return false;
        }

        private void SetButtonsEnabled(bool enabled)
        {
            _btnEnter.Enabled = enabled;
            _btnSettings.Enabled = enabled;
            _btnCancel.Enabled = enabled;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _countdown--;
            if (_countdown <= 0)
            {
                _timer.Stop();
                Close();
            }
            else
            {
                _statusLabel.Text = "当前网络在黑名单中，" + _countdown + " 秒后自动退出";
            }
        }

        private void LaunchProxyTool()
        {
            if (!File.Exists(_config.TargetExe))
            {
                MessageBox.Show(
                    this,
                    "未找到代理工具：\n" + _config.TargetExe + "\n\n请在“设置”中配置正确的程序路径。",
                    "提示",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(_config.TargetExe) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "代理工具启动失败：\n" + ex.Message, "启动错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Close();
        }

        private void OpenSettings()
        {
            using (var form = new SettingsForm(_config))
            {
                form.ShowDialog(this);
            }
            EvaluateAndRender();
        }
    }
}




