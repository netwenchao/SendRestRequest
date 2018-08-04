namespace FileUpload
{
    partial class PercentEncodingTool
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tbUrl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbAppId = new System.Windows.Forms.TextBox();
            this.tbAppSecret = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.cbMethord = new System.Windows.Forms.ComboBox();
            this.tbBaseString = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbSignature = new System.Windows.Forms.TextBox();
            this.tbRequestBody = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Url";
            // 
            // tbUrl
            // 
            this.tbUrl.Location = new System.Drawing.Point(86, 22);
            this.tbUrl.Name = "tbUrl";
            this.tbUrl.Size = new System.Drawing.Size(378, 21);
            this.tbUrl.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "APPID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "APPSecret";
            // 
            // tbAppId
            // 
            this.tbAppId.Location = new System.Drawing.Point(86, 57);
            this.tbAppId.Name = "tbAppId";
            this.tbAppId.Size = new System.Drawing.Size(438, 21);
            this.tbAppId.TabIndex = 1;
            // 
            // tbAppSecret
            // 
            this.tbAppSecret.Location = new System.Drawing.Point(86, 89);
            this.tbAppSecret.Name = "tbAppSecret";
            this.tbAppSecret.Size = new System.Drawing.Size(438, 21);
            this.tbAppSecret.TabIndex = 1;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(530, 19);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 91);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "生成";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // cbMethord
            // 
            this.cbMethord.DropDownHeight = 120;
            this.cbMethord.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMethord.FormattingEnabled = true;
            this.cbMethord.IntegralHeight = false;
            this.cbMethord.ItemHeight = 12;
            this.cbMethord.Items.AddRange(new object[] {
            "POST",
            "GET"});
            this.cbMethord.Location = new System.Drawing.Point(464, 22);
            this.cbMethord.Name = "cbMethord";
            this.cbMethord.Size = new System.Drawing.Size(60, 20);
            this.cbMethord.TabIndex = 37;
            // 
            // tbBaseString
            // 
            this.tbBaseString.Location = new System.Drawing.Point(86, 202);
            this.tbBaseString.Multiline = true;
            this.tbBaseString.Name = "tbBaseString";
            this.tbBaseString.Size = new System.Drawing.Size(438, 87);
            this.tbBaseString.TabIndex = 38;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 202);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 39;
            this.label4.Text = "BaseString";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 300);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 41;
            this.label5.Text = "Signature";
            // 
            // tbSignature
            // 
            this.tbSignature.Location = new System.Drawing.Point(86, 300);
            this.tbSignature.Multiline = true;
            this.tbSignature.Name = "tbSignature";
            this.tbSignature.Size = new System.Drawing.Size(438, 87);
            this.tbSignature.TabIndex = 40;
            // 
            // tbRequestBody
            // 
            this.tbRequestBody.Location = new System.Drawing.Point(86, 126);
            this.tbRequestBody.Multiline = true;
            this.tbRequestBody.Name = "tbRequestBody";
            this.tbRequestBody.Size = new System.Drawing.Size(438, 61);
            this.tbRequestBody.TabIndex = 43;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 129);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 12);
            this.label6.TabIndex = 42;
            this.label6.Text = "RequestBody";
            // 
            // PercentEncodingTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 456);
            this.Controls.Add(this.tbRequestBody);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbSignature);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbBaseString);
            this.Controls.Add(this.cbMethord);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.tbAppSecret);
            this.Controls.Add(this.tbAppId);
            this.Controls.Add(this.tbUrl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "PercentEncodingTool";
            this.Text = "PercentEncodingTool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbUrl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbAppId;
        private System.Windows.Forms.TextBox tbAppSecret;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.ComboBox cbMethord;
        private System.Windows.Forms.TextBox tbBaseString;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbSignature;
        private System.Windows.Forms.TextBox tbRequestBody;
        private System.Windows.Forms.Label label6;
    }
}