namespace WindowTopMost
{
    partial class frmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                // 释放 WindowList 中的资源
                if (WindowList != null)
                {
                    foreach (ProcessHnd item in WindowList)
                    {
                        item?.Dispose();
                    }
                    WindowList.Clear();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnGetHandle = new System.Windows.Forms.Button();
            this.btnTopMost = new System.Windows.Forms.Button();
            this.btnCancelTopmost = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button_selected = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lstWindow = new WindowTopMost.MiyukiListBox();
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.menuItemWindowName = new System.Windows.Forms.MenuItem();
            this.menuItemProcessName = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItemSwitchTo = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItemWindowTopMost = new System.Windows.Forms.MenuItem();
            this.menuItemCancelTopmost = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItemSetOpacity = new System.Windows.Forms.MenuItem();
            this.menuItemSetOpacityTo100 = new System.Windows.Forms.MenuItem();
            this.menuItemSetOpacityTo90 = new System.Windows.Forms.MenuItem();
            this.menuItemSetOpacityTo75 = new System.Windows.Forms.MenuItem();
            this.menuItemSetOpacityTo50 = new System.Windows.Forms.MenuItem();
            this.menuItemSetOpacityTo25 = new System.Windows.Forms.MenuItem();
            this.menuItemSetOpacityTo0 = new System.Windows.Forms.MenuItem();
            this.menuItem12 = new System.Windows.Forms.MenuItem();
            this.menuItemSetOpacityToCustomValue = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuGetInfo = new System.Windows.Forms.MenuItem();
            this.menuItemOpenLocation = new System.Windows.Forms.MenuItem();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRefresh
            // 
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRefresh.Location = new System.Drawing.Point(12, 10);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(102, 30);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "刷新";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnGetHandle
            // 
            this.btnGetHandle.Location = new System.Drawing.Point(436, 45);
            this.btnGetHandle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnGetHandle.Name = "btnGetHandle";
            this.btnGetHandle.Size = new System.Drawing.Size(87, 33);
            this.btnGetHandle.TabIndex = 3;
            this.btnGetHandle.Text = "get handle";
            this.btnGetHandle.UseVisualStyleBackColor = true;
            this.btnGetHandle.Visible = false;
            this.btnGetHandle.Click += new System.EventHandler(this.btnGetHandle_Click);
            // 
            // btnTopMost
            // 
            this.btnTopMost.Enabled = false;
            this.btnTopMost.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnTopMost.Location = new System.Drawing.Point(13, 10);
            this.btnTopMost.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnTopMost.Name = "btnTopMost";
            this.btnTopMost.Size = new System.Drawing.Size(93, 30);
            this.btnTopMost.TabIndex = 4;
            this.btnTopMost.Text = "窗口置顶";
            this.btnTopMost.UseVisualStyleBackColor = true;
            this.btnTopMost.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancelTopmost
            // 
            this.btnCancelTopmost.Enabled = false;
            this.btnCancelTopmost.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancelTopmost.Location = new System.Drawing.Point(112, 10);
            this.btnCancelTopmost.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancelTopmost.Name = "btnCancelTopmost";
            this.btnCancelTopmost.Size = new System.Drawing.Size(93, 30);
            this.btnCancelTopmost.TabIndex = 5;
            this.btnCancelTopmost.Text = "取消置顶";
            this.btnCancelTopmost.UseVisualStyleBackColor = true;
            this.btnCancelTopmost.Click += new System.EventHandler(this.btnCancelTopmost_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.button_selected);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 526);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(552, 49);
            this.panel1.TabIndex = 6;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // button_selected
            // 
            this.button_selected.Location = new System.Drawing.Point(153, 10);
            this.button_selected.Name = "button_selected";
            this.button_selected.Size = new System.Drawing.Size(79, 30);
            this.button_selected.TabIndex = 6;
            this.button_selected.Text = "selected";
            this.button_selected.UseVisualStyleBackColor = true;
            this.button_selected.Visible = false;
            this.button_selected.Click += new System.EventHandler(this.button_selected_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnCancelTopmost);
            this.panel3.Controls.Add(this.btnTopMost);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(337, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(215, 49);
            this.panel3.TabIndex = 7;
            this.panel3.Paint += new System.Windows.Forms.PaintEventHandler(this.panel3_Paint);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Window;
            this.panel2.Controls.Add(this.lstWindow);
            this.panel2.Controls.Add(this.btnGetHandle);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(552, 526);
            this.panel2.TabIndex = 7;
            // 
            // lstWindow
            // 
            this.lstWindow.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstWindow.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lstWindow.FormattingEnabled = true;
            this.lstWindow.HoverIndex = -1;
            this.lstWindow.ItemHeight = 17;
            this.lstWindow.Location = new System.Drawing.Point(0, 0);
            this.lstWindow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstWindow.Name = "lstWindow";
            this.lstWindow.Size = new System.Drawing.Size(552, 526);
            this.lstWindow.TabIndex = 1;
            this.lstWindow.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstWindow_MouseClick);
            this.lstWindow.SelectedIndexChanged += new System.EventHandler(this.lstWindow_SelectedIndexChanged);
            this.lstWindow.DoubleClick += new System.EventHandler(this.lstWindow_DoubleClick);
            this.lstWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstWindow_MouseUp);
            // 
            // contextMenu
            // 
            this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemWindowName,
            this.menuItemProcessName,
            this.menuItem3,
            this.menuItemSwitchTo,
            this.menuItem5,
            this.menuItemWindowTopMost,
            this.menuItemCancelTopmost,
            this.menuItem1,
            this.menuItemSetOpacity,
            this.menuItem8,
            this.menuGetInfo,
            this.menuItemOpenLocation});
            // 
            // menuItemWindowName
            // 
            this.menuItemWindowName.Enabled = false;
            this.menuItemWindowName.Index = 0;
            this.menuItemWindowName.Text = "窗口句柄";
            this.menuItemWindowName.Visible = false;
            // 
            // menuItemProcessName
            // 
            this.menuItemProcessName.Enabled = false;
            this.menuItemProcessName.Index = 1;
            this.menuItemProcessName.Text = "进程名";
            this.menuItemProcessName.Visible = false;
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 2;
            this.menuItem3.Text = "-";
            this.menuItem3.Visible = false;
            // 
            // menuItemSwitchTo
            // 
            this.menuItemSwitchTo.DefaultItem = true;
            this.menuItemSwitchTo.Index = 3;
            this.menuItemSwitchTo.Text = "切换至该窗口(&S)";
            this.menuItemSwitchTo.Click += new System.EventHandler(this.menuItemSwitchTo_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 4;
            this.menuItem5.Text = "-";
            // 
            // menuItemWindowTopMost
            // 
            this.menuItemWindowTopMost.Index = 5;
            this.menuItemWindowTopMost.Text = "窗口置顶(&T)";
            this.menuItemWindowTopMost.Click += new System.EventHandler(this.menuItemWindowTopMost_Click);
            // 
            // menuItemCancelTopmost
            // 
            this.menuItemCancelTopmost.Index = 6;
            this.menuItemCancelTopmost.Text = "取消置顶(&C)";
            this.menuItemCancelTopmost.Click += new System.EventHandler(this.menuItemCancelTopmost_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 7;
            this.menuItem1.Text = "-";
            // 
            // menuItemSetOpacity
            // 
            this.menuItemSetOpacity.Index = 8;
            this.menuItemSetOpacity.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemSetOpacityTo100,
            this.menuItemSetOpacityTo90,
            this.menuItemSetOpacityTo75,
            this.menuItemSetOpacityTo50,
            this.menuItemSetOpacityTo25,
            this.menuItemSetOpacityTo0,
            this.menuItem12,
            this.menuItemSetOpacityToCustomValue});
            this.menuItemSetOpacity.Text = "设置窗口透明度(&O)";
            // 
            // menuItemSetOpacityTo100
            // 
            this.menuItemSetOpacityTo100.Index = 0;
            this.menuItemSetOpacityTo100.Text = "100%";
            // 
            // menuItemSetOpacityTo90
            // 
            this.menuItemSetOpacityTo90.Index = 1;
            this.menuItemSetOpacityTo90.Text = "90%";
            // 
            // menuItemSetOpacityTo75
            // 
            this.menuItemSetOpacityTo75.Index = 2;
            this.menuItemSetOpacityTo75.Text = "75%";
            // 
            // menuItemSetOpacityTo50
            // 
            this.menuItemSetOpacityTo50.Index = 3;
            this.menuItemSetOpacityTo50.Text = "50%";
            // 
            // menuItemSetOpacityTo25
            // 
            this.menuItemSetOpacityTo25.Index = 4;
            this.menuItemSetOpacityTo25.Text = "25%";
            // 
            // menuItemSetOpacityTo0
            // 
            this.menuItemSetOpacityTo0.Index = 5;
            this.menuItemSetOpacityTo0.Text = "0%（隐藏窗口）";
            // 
            // menuItem12
            // 
            this.menuItem12.Index = 6;
            this.menuItem12.Text = "-";
            // 
            // menuItemSetOpacityToCustomValue
            // 
            this.menuItemSetOpacityToCustomValue.Index = 7;
            this.menuItemSetOpacityToCustomValue.Text = "自定义(&C)...";
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 9;
            this.menuItem8.Text = "-";
            // 
            // menuGetInfo
            // 
            this.menuGetInfo.Index = 10;
            this.menuGetInfo.Text = "显示信息(&I)";
            this.menuGetInfo.Click += new System.EventHandler(this.menuGetInfo_Click);
            // 
            // menuItemOpenLocation
            // 
            this.menuItemOpenLocation.Index = 11;
            this.menuItemOpenLocation.Text = "打开进程位置(&O)";
            this.menuItemOpenLocation.Click += new System.EventHandler(this.menuItemOpenLocation_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 575);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(350, 350);
            this.Name = "frmMain";
            this.Text = "Window Top Most";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnGetHandle;
        private System.Windows.Forms.Button btnTopMost;
        private System.Windows.Forms.Button btnCancelTopmost;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button_selected;
        private MiyukiListBox lstWindow;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.Windows.Forms.MenuItem menuItemWindowName;
        private System.Windows.Forms.MenuItem menuItemProcessName;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItemSwitchTo;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem menuItemWindowTopMost;
        private System.Windows.Forms.MenuItem menuItemCancelTopmost;
        private System.Windows.Forms.MenuItem menuItem8;
        private System.Windows.Forms.MenuItem menuItemOpenLocation;
        private System.Windows.Forms.MenuItem menuGetInfo;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItemSetOpacity;
        private System.Windows.Forms.MenuItem menuItemSetOpacityTo100;
        private System.Windows.Forms.MenuItem menuItemSetOpacityTo90;
        private System.Windows.Forms.MenuItem menuItemSetOpacityTo75;
        private System.Windows.Forms.MenuItem menuItemSetOpacityTo50;
        private System.Windows.Forms.MenuItem menuItemSetOpacityTo25;
        private System.Windows.Forms.MenuItem menuItemSetOpacityTo0;
        private System.Windows.Forms.MenuItem menuItem12;
        private System.Windows.Forms.MenuItem menuItemSetOpacityToCustomValue;
    }
}

