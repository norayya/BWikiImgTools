namespace Editor;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
        richTextBox1 = new System.Windows.Forms.RichTextBox();
        SuspendLayout();
        // 
        // richTextBox1
        // 
        richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
        richTextBox1.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
        richTextBox1.Location = new System.Drawing.Point(4, 5);
        richTextBox1.Name = "richTextBox1";
        richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
        richTextBox1.Size = new System.Drawing.Size(536, 743);
        richTextBox1.TabIndex = 0;
        richTextBox1.Text = "";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(544, 761);
        Controls.Add(richTextBox1);
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Text = "MainForm";
        Shown += MainForm_Shown;
        ResumeLayout(false);
    }

    private System.Windows.Forms.RichTextBox richTextBox1;

    #endregion
}