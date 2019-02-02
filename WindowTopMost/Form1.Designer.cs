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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnGetHandle = new System.Windows.Forms.Button();
            this.btnTopMost = new System.Windows.Forms.Button();
            this.btnCancelTopmost = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button_selected = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuWindowInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuProcessName = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSwitchTo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuTopMost = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUntopMost = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuOpenProcLocation = new System.Windows.Forms.ToolStripMenuItem();
            this.lstWindow = new WindowTopMost.MiyukiListBox();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.rightClickMenu.SuspendLayout();
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
            this.panel1.Location = new System.Drawing.Point(0, 489);
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
            this.panel2.Size = new System.Drawing.Size(552, 489);
            this.panel2.TabIndex = 7;
            // 
            // rightClickMenu
            // 
            this.rightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuWindowInfo,
            this.menuProcessName,
            this.toolStripMenuItem3,
            this.menuSwitchTo,
            this.toolStripMenuItem1,
            this.menuTopMost,
            this.menuUntopMost,
            this.toolStripMenuItem2,
            this.menuOpenProcLocation});
            this.rightClickMenu.Name = "rightClickMenu";
            this.rightClickMenu.Size = new System.Drawing.Size(165, 176);
            // 
            // menuWindowInfo
            // 
            this.menuWindowInfo.Enabled = false;
            this.menuWindowInfo.Name = "menuWindowInfo";
            this.menuWindowInfo.Size = new System.Drawing.Size(164, 22);
            this.menuWindowInfo.Text = "Window Handle";
            // 
            // menuProcessName
            // 
            this.menuProcessName.Enabled = false;
            this.menuProcessName.Name = "menuProcessName";
            this.menuProcessName.Size = new System.Drawing.Size(164, 22);
            this.menuProcessName.Text = "Process Name";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(161, 6);
            // 
            // menuSwitchTo
            // 
            this.menuSwitchTo.Name = "menuSwitchTo";
            this.menuSwitchTo.Size = new System.Drawing.Size(164, 22);
            this.menuSwitchTo.Text = "切换至该窗口";
            this.menuSwitchTo.Click += new System.EventHandler(this.menuSwitchTo_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(161, 6);
            // 
            // menuTopMost
            // 
            this.menuTopMost.Name = "menuTopMost";
            this.menuTopMost.Size = new System.Drawing.Size(164, 22);
            this.menuTopMost.Text = "窗口置顶";
            this.menuTopMost.Click += new System.EventHandler(this.menuTopMost_Click);
            // 
            // menuUntopMost
            // 
            this.menuUntopMost.Name = "menuUntopMost";
            this.menuUntopMost.Size = new System.Drawing.Size(164, 22);
            this.menuUntopMost.Text = "取消置顶";
            this.menuUntopMost.Click += new System.EventHandler(this.menuUntopMost_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(161, 6);
            // 
            // menuOpenProcLocation
            // 
            this.menuOpenProcLocation.Name = "menuOpenProcLocation";
            this.menuOpenProcLocation.Size = new System.Drawing.Size(164, 22);
            this.menuOpenProcLocation.Text = "打开进程位置";
            this.menuOpenProcLocation.Click += new System.EventHandler(this.menuOpenProcLocation_Click);
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
            this.lstWindow.Size = new System.Drawing.Size(552, 489);
            this.lstWindow.TabIndex = 1;
            this.lstWindow.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstWindow_MouseClick);
            this.lstWindow.SelectedIndexChanged += new System.EventHandler(this.lstWindow_SelectedIndexChanged);
            this.lstWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstWindow_MouseUp);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 538);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(350, 350);
            this.Name = "frmMain";
            this.Text = "Window Top-Mostify";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.rightClickMenu.ResumeLayout(false);
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
        private System.Windows.Forms.ContextMenuStrip rightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem menuWindowInfo;
        private System.Windows.Forms.ToolStripMenuItem menuProcessName;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuTopMost;
        private System.Windows.Forms.ToolStripMenuItem menuUntopMost;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem menuOpenProcLocation;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem menuSwitchTo;
    }
}

