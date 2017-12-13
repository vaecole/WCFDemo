namespace ProxyTest
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tb_proxyList = new System.Windows.Forms.TextBox();
            this.bt_start = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lb_resultOutput = new System.Windows.Forms.ListBox();
            this.tb_url2conn = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.bt_exportResult = new System.Windows.Forms.Button();
            this.tb_urls_2status = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_ConnTime = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_num2Test = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tb_proxyList
            // 
            this.tb_proxyList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tb_proxyList.Location = new System.Drawing.Point(12, 122);
            this.tb_proxyList.Multiline = true;
            this.tb_proxyList.Name = "tb_proxyList";
            this.tb_proxyList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tb_proxyList.Size = new System.Drawing.Size(292, 450);
            this.tb_proxyList.TabIndex = 0;
            // 
            // bt_start
            // 
            this.bt_start.Location = new System.Drawing.Point(12, 8);
            this.bt_start.Name = "bt_start";
            this.bt_start.Size = new System.Drawing.Size(75, 43);
            this.bt_start.TabIndex = 1;
            this.bt_start.Text = "开始检查";
            this.bt_start.UseVisualStyleBackColor = true;
            this.bt_start.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 575);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // lb_resultOutput
            // 
            this.lb_resultOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lb_resultOutput.FormattingEnabled = true;
            this.lb_resultOutput.ItemHeight = 12;
            this.lb_resultOutput.Location = new System.Drawing.Point(310, 123);
            this.lb_resultOutput.Name = "lb_resultOutput";
            this.lb_resultOutput.Size = new System.Drawing.Size(499, 448);
            this.lb_resultOutput.TabIndex = 0;
            // 
            // tb_url2conn
            // 
            this.tb_url2conn.Location = new System.Drawing.Point(164, 9);
            this.tb_url2conn.Name = "tb_url2conn";
            this.tb_url2conn.Size = new System.Drawing.Size(576, 21);
            this.tb_url2conn.TabIndex = 3;
            this.tb_url2conn.Text = "http://www.baidu.com";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(93, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "连通性检查";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "代理列表(ip:port)：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(308, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "检查结果：";
            // 
            // bt_exportResult
            // 
            this.bt_exportResult.Enabled = false;
            this.bt_exportResult.Location = new System.Drawing.Point(734, 97);
            this.bt_exportResult.Name = "bt_exportResult";
            this.bt_exportResult.Size = new System.Drawing.Size(75, 23);
            this.bt_exportResult.TabIndex = 7;
            this.bt_exportResult.Text = "导出结果";
            this.bt_exportResult.UseVisualStyleBackColor = true;
            this.bt_exportResult.Click += new System.EventHandler(this.button2_Click);
            // 
            // tb_urls_2status
            // 
            this.tb_urls_2status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_urls_2status.Location = new System.Drawing.Point(164, 36);
            this.tb_urls_2status.Multiline = true;
            this.tb_urls_2status.Name = "tb_urls_2status";
            this.tb_urls_2status.Size = new System.Drawing.Size(645, 55);
            this.tb_urls_2status.TabIndex = 8;
            this.tb_urls_2status.Text = resources.GetString("tb_urls_2status.Text");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(93, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "状态码检查";
            // 
            // tb_ConnTime
            // 
            this.tb_ConnTime.Location = new System.Drawing.Point(775, 9);
            this.tb_ConnTime.Name = "tb_ConnTime";
            this.tb_ConnTime.Size = new System.Drawing.Size(34, 21);
            this.tb_ConnTime.TabIndex = 10;
            this.tb_ConnTime.Text = "33";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(743, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "时长";
            // 
            // tb_num2Test
            // 
            this.tb_num2Test.Location = new System.Drawing.Point(188, 99);
            this.tb_num2Test.Name = "tb_num2Test";
            this.tb_num2Test.Size = new System.Drawing.Size(114, 21);
            this.tb_num2Test.TabIndex = 12;
            this.tb_num2Test.Text = "1000";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(130, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "测试个数";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 595);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tb_num2Test);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tb_ConnTime);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tb_urls_2status);
            this.Controls.Add(this.bt_exportResult);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tb_url2conn);
            this.Controls.Add(this.tb_proxyList);
            this.Controls.Add(this.lb_resultOutput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bt_start);
            this.Name = "Form1";
            this.Text = "代理检查";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tb_proxyList;
        private System.Windows.Forms.Button bt_start;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lb_resultOutput;
        private System.Windows.Forms.TextBox tb_url2conn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bt_exportResult;
        private System.Windows.Forms.TextBox tb_urls_2status;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_ConnTime;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_num2Test;
        private System.Windows.Forms.Label label7;
    }
}

