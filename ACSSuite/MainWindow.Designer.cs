namespace ACS4Ingest
{
  partial class MainWindow
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
            System.Windows.Forms.Button btnPackageRequest;
            this.TestWebCallButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            btnPackageRequest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPackageRequest
            // 
            btnPackageRequest.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            btnPackageRequest.Location = new System.Drawing.Point(12, 97);
            btnPackageRequest.Name = "btnPackageRequest";
            btnPackageRequest.Size = new System.Drawing.Size(188, 50);
            btnPackageRequest.TabIndex = 3;
            btnPackageRequest.Text = "Package Request";
            btnPackageRequest.UseVisualStyleBackColor = true;
            btnPackageRequest.Click += new System.EventHandler(this.btnPackageRequest_Click);
            // 
            // TestWebCallButton
            // 
            this.TestWebCallButton.Location = new System.Drawing.Point(11, 11);
            this.TestWebCallButton.Margin = new System.Windows.Forms.Padding(2);
            this.TestWebCallButton.Name = "TestWebCallButton";
            this.TestWebCallButton.Size = new System.Drawing.Size(189, 48);
            this.TestWebCallButton.TabIndex = 2;
            this.TestWebCallButton.Text = "Get Catalog";
            this.TestWebCallButton.UseVisualStyleBackColor = true;
            this.TestWebCallButton.Click += new System.EventHandler(this.TestWebCallButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // button1
            // 
            this.button1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.button1.Location = new System.Drawing.Point(12, 181);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(188, 50);
            this.button1.TabIndex = 4;
            this.button1.Text = "Delete All Books";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnDeleteAllBooks_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(222, 254);
            this.Controls.Add(this.button1);
            this.Controls.Add(btnPackageRequest);
            this.Controls.Add(this.TestWebCallButton);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainWindow";
            this.Text = "ACS Suites";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button TestWebCallButton;
    private System.Windows.Forms.OpenFileDialog openFileDialog1;
    private System.Windows.Forms.Button button1;
  }
}