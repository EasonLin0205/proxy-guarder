using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ProxyGuarder
{
    internal class SettingsForm : Form
    {
        private readonly AppConfig _config;
        private readonly List<string> _workingBlacklist;

        private TextBox _targetTextBox;
        private TextBox _addTextBox;
        private ListBox _listBox;

        public SettingsForm(AppConfig config)
        {
            _config = config;
            _workingBlacklist = new List<string>(config.Blacklist);

            Text = "Proxy Guarder 设置";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(540, 460);
            Font = new Font("Aptos", 10F);

            BuildControls();
            LoadToUi();
        }

        private void BuildControls()
        {
            Label targetLabel = new Label();
            targetLabel.Text = "代理工具路径：";
            targetLabel.SetBounds(20, 20, 120, 22);

            _targetTextBox = new TextBox();
            _targetTextBox.SetBounds(20, 46, 400, 28);

            Button browseButton = new Button();
            browseButton.Text = "浏览…";
            browseButton.SetBounds(430, 45, 80, 30);
            browseButton.Click += (s, e) => BrowseTarget();

            Label blacklistLabel = new Label();
            blacklistLabel.Text = "网络拦截名单";
            blacklistLabel.SetBounds(20, 90, 300, 22);

            _addTextBox = new TextBox();
            _addTextBox.SetBounds(20, 116, 320, 28);

            Button addButton = new Button();
            addButton.Text = "添加";
            addButton.SetBounds(350, 115, 80, 30);
            addButton.Click += (s, e) => AddItem();

            _listBox = new ListBox();
            _listBox.SetBounds(20, 155, 490, 215);
            _listBox.SelectionMode = SelectionMode.One;

            Button removeButton = new Button();
            removeButton.Text = "删除选中";
            removeButton.SetBounds(20, 385, 105, 34);
            removeButton.Click += (s, e) => RemoveSelected();

            Button clearButton = new Button();
            clearButton.Text = "清空";
            clearButton.SetBounds(135, 385, 85, 34);
            clearButton.Click += (s, e) => ClearAll();
Button saveButton = new Button();
            saveButton.Text = "保存";
            saveButton.SetBounds(325, 385, 85, 34);
            saveButton.Click += (s, e) => SaveAndClose();

            Button cancelButton = new Button();
            cancelButton.Text = "取消";
            cancelButton.SetBounds(420, 385, 90, 34);
            cancelButton.Click += (s, e) => Close();

            Controls.Add(targetLabel);
            Controls.Add(_targetTextBox);
            Controls.Add(browseButton);
            Controls.Add(blacklistLabel);
            Controls.Add(_addTextBox);
            Controls.Add(addButton);
            Controls.Add(_listBox);
            Controls.Add(removeButton);
            Controls.Add(clearButton);
            Controls.Add(saveButton);
            Controls.Add(cancelButton);            StyleButton(addButton, false);
            StyleButton(removeButton, false);
            StyleButton(clearButton, false);
            StyleButton(saveButton, true);
            StyleButton(cancelButton, false);
        }


        private static void StyleButton(Button button, bool primary)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = primary ? 0 : 1;
            button.FlatAppearance.BorderColor = Color.FromArgb(208, 215, 222);
            button.BackColor = primary ? Color.FromArgb(35, 134, 54) : Color.FromArgb(246, 248, 250);
            button.ForeColor = primary ? Color.White : Color.FromArgb(31, 35, 40);
            button.Cursor = Cursors.Hand;
        }        private void LoadToUi()
        {
            _targetTextBox.Text = _config.TargetExe;
            RefreshList();
        }

        private void RefreshList()
        {
            _listBox.Items.Clear();
            foreach (string item in _workingBlacklist)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    _listBox.Items.Add(item.Trim());
                }
            }
        }

        private void BrowseTarget()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "选择代理工具程序";
                dialog.Filter = "可执行文件 (*.exe)|*.exe|所有文件 (*.*)|*.*";
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _targetTextBox.Text = dialog.FileName;
                }
            }
        }

        private void AddItem()
        {
            string name = _addTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show(this, "请输入网络名称。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_workingBlacklist.Exists(x => string.Equals(x, name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show(this, "该网络名称已在黑名单中。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _workingBlacklist.Add(name);
            RefreshList();
            _addTextBox.Clear();
            _addTextBox.Focus();
        }

        private void RemoveSelected()
        {
            if (_listBox.SelectedIndex >= 0)
            {
                _workingBlacklist.RemoveAt(_listBox.SelectedIndex);
                RefreshList();
            }
        }

        private void ClearAll()
        {
            if (_workingBlacklist.Count == 0)
            {
                return;
            }

            if (MessageBox.Show(
                    this,
                    "确定清空全部黑名单吗？",
                    "确认",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _workingBlacklist.Clear();
                RefreshList();
            }
        }

        private void SaveAndClose()
        {
            string target = _targetTextBox.Text.Trim();
            _config.TargetExe = target;
            _config.Blacklist = new List<string>(_workingBlacklist);
            _config.Save();

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}





