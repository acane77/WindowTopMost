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
            this.lstWindow = new System.Windows.Forms.ListBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnGetHandle = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnCancelTopmost = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstWindow
            // 
            this.lstWindow.FormattingEnabled = true;
            this.lstWindow.ItemHeight = 17;
            this.lstWindow.Location = new System.Drawing.Point(15, 14);
            this.lstWindow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstWindow.Name = "lstWindow";
            this.lstWindow.Size = new System.Drawing.Size(513, 378);
            this.lstWindow.TabIndex = 1;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(15, 404);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(109, 33);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "刷新";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnGetHandle
            // 
            this.btnGetHandle.Location = new System.Drawing.Point(414, 404);
            this.btnGetHandle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnGetHandle.Name = "btnGetHandle";
            this.btnGetHandle.Size = new System.Drawing.Size(87, 33);
            this.btnGetHandle.TabIndex = 3;
            this.btnGetHandle.Text = "get handle";
            this.btnGetHandle.UseVisualStyleBackColor = true;
            this.btnGetHandle.Visible = false;
            this.btnGetHandle.Click += new System.EventHandler(this.btnGetHandle_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(168, 404);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(117, 33);
            this.button1.TabIndex = 4;
            this.button1.Text = "窗口置顶";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancelTopmost
            // 
            this.btnCancelTopmost.Location = new System.Drawing.Point(291, 404);
            this.btnCancelTopmost.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancelTopmost.Name = "btnCancelTopmost";
            this.btnCancelTopmost.Size = new System.Drawing.Size(117, 33);
            this.btnCancelTopmost.TabIndex = 5;
            this.btnCancelTopmost.Text = "取消置顶";
            this.btnCancelTopmost.UseVisualStyleBackColor = true;
            this.btnCancelTopmost.Click += new System.EventHandler(this.btnCancelTopmost_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 451);
            this.Controls.Add(this.btnCancelTopmost);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnGetHandle);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lstWindow);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmMain";
            this.Text = "Window Top-Mostify";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListBox lstWindow;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnGetHandle;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnCancelTopmost;
    }
}

