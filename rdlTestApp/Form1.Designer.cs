namespace rdlTestApp
{
    partial class Form1
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnTerritorySalesDrilldown = new System.Windows.Forms.Button();
            this.btnSalesOrderDetail = new System.Windows.Forms.Button();
            this.btnProductCatalog = new System.Windows.Forms.Button();
            this.btnEmployeeSalesSummary = new System.Windows.Forms.Button();
            this.buttonCompanySales = new System.Windows.Forms.Button();
            this.reportViewer1 = new RdlViewer.ReportViewer();
            this.parametersEntry1 = new RdlViewer.ParametersEntry();
            this.panelTop.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.AutoSize = true;
            this.panelTop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelTop.Controls.Add(this.parametersEntry1);
            this.panelTop.Controls.Add(this.panelButtons);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(675, 129);
            this.panelTop.TabIndex = 2;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnTerritorySalesDrilldown);
            this.panelButtons.Controls.Add(this.btnSalesOrderDetail);
            this.panelButtons.Controls.Add(this.btnProductCatalog);
            this.panelButtons.Controls.Add(this.btnEmployeeSalesSummary);
            this.panelButtons.Controls.Add(this.buttonCompanySales);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelButtons.Location = new System.Drawing.Point(0, 0);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(675, 100);
            this.panelButtons.TabIndex = 2;
            // 
            // btnTerritorySalesDrilldown
            // 
            this.btnTerritorySalesDrilldown.Location = new System.Drawing.Point(12, 55);
            this.btnTerritorySalesDrilldown.Name = "btnTerritorySalesDrilldown";
            this.btnTerritorySalesDrilldown.Size = new System.Drawing.Size(120, 37);
            this.btnTerritorySalesDrilldown.TabIndex = 0;
            this.btnTerritorySalesDrilldown.Text = "Territory Sales Drilldown";
            this.btnTerritorySalesDrilldown.UseVisualStyleBackColor = true;
            // 
            // btnSalesOrderDetail
            // 
            this.btnSalesOrderDetail.Location = new System.Drawing.Point(388, 12);
            this.btnSalesOrderDetail.Name = "btnSalesOrderDetail";
            this.btnSalesOrderDetail.Size = new System.Drawing.Size(120, 37);
            this.btnSalesOrderDetail.TabIndex = 0;
            this.btnSalesOrderDetail.Text = "Sales Order Detail";
            this.btnSalesOrderDetail.UseVisualStyleBackColor = true;
            this.btnSalesOrderDetail.Click += new System.EventHandler(this.btnSalesOrderDetail_Click);
            // 
            // btnProductCatalog
            // 
            this.btnProductCatalog.Location = new System.Drawing.Point(262, 12);
            this.btnProductCatalog.Name = "btnProductCatalog";
            this.btnProductCatalog.Size = new System.Drawing.Size(120, 37);
            this.btnProductCatalog.TabIndex = 0;
            this.btnProductCatalog.Text = "Product Catalog";
            this.btnProductCatalog.UseVisualStyleBackColor = true;
            this.btnProductCatalog.Click += new System.EventHandler(this.btnProductCatalog_Click);
            // 
            // btnEmployeeSalesSummary
            // 
            this.btnEmployeeSalesSummary.Location = new System.Drawing.Point(136, 12);
            this.btnEmployeeSalesSummary.Name = "btnEmployeeSalesSummary";
            this.btnEmployeeSalesSummary.Size = new System.Drawing.Size(120, 37);
            this.btnEmployeeSalesSummary.TabIndex = 0;
            this.btnEmployeeSalesSummary.Text = "Employee Sales Summary";
            this.btnEmployeeSalesSummary.UseVisualStyleBackColor = true;
            this.btnEmployeeSalesSummary.Click += new System.EventHandler(this.btnEmployeeSalesSummary_Click);
            // 
            // buttonCompanySales
            // 
            this.buttonCompanySales.Location = new System.Drawing.Point(12, 12);
            this.buttonCompanySales.Name = "buttonCompanySales";
            this.buttonCompanySales.Size = new System.Drawing.Size(118, 37);
            this.buttonCompanySales.TabIndex = 0;
            this.buttonCompanySales.Text = "Company Sales";
            this.buttonCompanySales.UseVisualStyleBackColor = true;
            this.buttonCompanySales.Click += new System.EventHandler(this.buttonCompanySales_Click);
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportViewer1.Location = new System.Drawing.Point(0, 129);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(675, 282);
            this.reportViewer1.TabIndex = 0;
            // 
            // parametersEntry1
            // 
            this.parametersEntry1.AutoSize = true;
            this.parametersEntry1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.parametersEntry1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parametersEntry1.Location = new System.Drawing.Point(0, 100);
            this.parametersEntry1.Name = "parametersEntry1";
            this.parametersEntry1.Size = new System.Drawing.Size(675, 29);
            this.parametersEntry1.TabIndex = 3;
            this.parametersEntry1.ViewReportButtonText = "View";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 411);
            this.Controls.Add(this.reportViewer1);
            this.Controls.Add(this.panelTop);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RdlViewer.ReportViewer reportViewer1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnTerritorySalesDrilldown;
        private System.Windows.Forms.Button btnSalesOrderDetail;
        private System.Windows.Forms.Button btnProductCatalog;
        private System.Windows.Forms.Button btnEmployeeSalesSummary;
        private System.Windows.Forms.Button buttonCompanySales;
        private RdlViewer.ParametersEntry parametersEntry1;
    }
}