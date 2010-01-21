namespace RdlViewer
{
    partial class ParametersDialog
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
            this.parametersEntry1 = new RdlViewer.ParametersEntry();
            this.SuspendLayout();
            // 
            // parametersEntry1
            // 
            this.parametersEntry1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parametersEntry1.Location = new System.Drawing.Point(0, 0);
            this.parametersEntry1.Name = "parametersEntry1";
            this.parametersEntry1.Size = new System.Drawing.Size(292, 266);
            this.parametersEntry1.TabIndex = 0;
            this.parametersEntry1.ViewReportButtonText = "OK";
            // 
            // ParametersDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.parametersEntry1);
            this.Name = "ParametersDialog";
            this.Text = "ParametersDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private ParametersEntry parametersEntry1;
    }
}