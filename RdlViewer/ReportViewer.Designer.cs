namespace RdlViewer
{
    partial class ReportViewer
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportViewer));
            this.panelReport = new System.Windows.Forms.Panel();
            this.textCurrentPage = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textPages = new System.Windows.Forms.TextBox();
            this.buttonPageUp = new System.Windows.Forms.Button();
            this.buttonPageDown = new System.Windows.Forms.Button();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.comboExport = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.buttonPrintPreview = new System.Windows.Forms.Button();
            this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
            this.buttonPageSetup = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panelReport
            // 
            this.panelReport.AutoScroll = true;
            this.panelReport.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelReport.Location = new System.Drawing.Point(0, 31);
            this.panelReport.Name = "panelReport";
            this.panelReport.Size = new System.Drawing.Size(767, 370);
            this.panelReport.TabIndex = 0;
            this.panelReport.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelReport_MouseClick);
            this.panelReport.Paint += new System.Windows.Forms.PaintEventHandler(this.panelReport_Paint);
            // 
            // textCurrentPage
            // 
            this.textCurrentPage.Location = new System.Drawing.Point(41, 4);
            this.textCurrentPage.Name = "textCurrentPage";
            this.textCurrentPage.Size = new System.Drawing.Size(37, 20);
            this.textCurrentPage.TabIndex = 1;
            this.textCurrentPage.Validating += new System.ComponentModel.CancelEventHandler(this.textCurrentPage_Validating);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Page";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(84, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "of";
            // 
            // textPages
            // 
            this.textPages.Location = new System.Drawing.Point(106, 4);
            this.textPages.Name = "textPages";
            this.textPages.ReadOnly = true;
            this.textPages.Size = new System.Drawing.Size(37, 20);
            this.textPages.TabIndex = 1;
            // 
            // buttonPageUp
            // 
            this.buttonPageUp.Image = global::RdlViewer.Properties.Resources.ARW03UP;
            this.buttonPageUp.Location = new System.Drawing.Point(149, 2);
            this.buttonPageUp.Name = "buttonPageUp";
            this.buttonPageUp.Size = new System.Drawing.Size(29, 23);
            this.buttonPageUp.TabIndex = 3;
            this.buttonPageUp.UseVisualStyleBackColor = true;
            this.buttonPageUp.Click += new System.EventHandler(this.buttonPageUp_Click);
            // 
            // buttonPageDown
            // 
            this.buttonPageDown.Image = global::RdlViewer.Properties.Resources.ARW03DN;
            this.buttonPageDown.Location = new System.Drawing.Point(184, 2);
            this.buttonPageDown.Name = "buttonPageDown";
            this.buttonPageDown.Size = new System.Drawing.Size(29, 23);
            this.buttonPageDown.TabIndex = 3;
            this.buttonPageDown.UseVisualStyleBackColor = true;
            this.buttonPageDown.Click += new System.EventHandler(this.buttonPageDown_Click);
            // 
            // buttonPrint
            // 
            this.buttonPrint.Location = new System.Drawing.Point(240, 1);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(75, 23);
            this.buttonPrint.TabIndex = 4;
            this.buttonPrint.Text = "Print...";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // comboExport
            // 
            this.comboExport.FormattingEnabled = true;
            this.comboExport.Items.AddRange(new object[] {
            "---",
            "Txt",
            "Xls",
            "Pdf",
            "Csv"});
            this.comboExport.Location = new System.Drawing.Point(557, 4);
            this.comboExport.Name = "comboExport";
            this.comboExport.Size = new System.Drawing.Size(121, 21);
            this.comboExport.TabIndex = 5;
            this.comboExport.SelectedIndexChanged += new System.EventHandler(this.comboExport_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(495, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Export To:";
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            // 
            // buttonPrintPreview
            // 
            this.buttonPrintPreview.Location = new System.Drawing.Point(321, 1);
            this.buttonPrintPreview.Name = "buttonPrintPreview";
            this.buttonPrintPreview.Size = new System.Drawing.Size(75, 23);
            this.buttonPrintPreview.TabIndex = 4;
            this.buttonPrintPreview.Text = "Preview...";
            this.buttonPrintPreview.UseVisualStyleBackColor = true;
            this.buttonPrintPreview.Click += new System.EventHandler(this.buttonPrintPreview_Click);
            // 
            // buttonPageSetup
            // 
            this.buttonPageSetup.Location = new System.Drawing.Point(402, 1);
            this.buttonPageSetup.Name = "buttonPageSetup";
            this.buttonPageSetup.Size = new System.Drawing.Size(75, 23);
            this.buttonPageSetup.TabIndex = 4;
            this.buttonPageSetup.Text = "Setup...";
            this.buttonPageSetup.UseVisualStyleBackColor = true;
            this.buttonPageSetup.Click += new System.EventHandler(this.buttonPageSetup_Click);
            // 
            // ReportViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboExport);
            this.Controls.Add(this.buttonPageSetup);
            this.Controls.Add(this.buttonPrintPreview);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.buttonPageUp);
            this.Controls.Add(this.buttonPageDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textPages);
            this.Controls.Add(this.textCurrentPage);
            this.Controls.Add(this.panelReport);
            this.Name = "ReportViewer";
            this.Size = new System.Drawing.Size(767, 401);
            this.Resize += new System.EventHandler(this.ReportViewer_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelReport;
        private System.Windows.Forms.TextBox textCurrentPage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textPages;
        private System.Windows.Forms.Button buttonPageDown;
        private System.Windows.Forms.Button buttonPageUp;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.ComboBox comboExport;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
        private System.Windows.Forms.Button buttonPrintPreview;
        private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
        private System.Windows.Forms.Button buttonPageSetup;
    }
}
