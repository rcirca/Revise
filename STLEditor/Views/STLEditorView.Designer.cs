namespace STLEditor
{
    partial class STLEditorView
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
            this._stlDataGrid = new System.Windows.Forms.DataGridView();
            this._loadButton = new System.Windows.Forms.Button();
            this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this._stlDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _stlDataGrid
            // 
            this._stlDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._stlDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._stlDataGrid.Location = new System.Drawing.Point(12, 50);
            this._stlDataGrid.Name = "_stlDataGrid";
            this._stlDataGrid.Size = new System.Drawing.Size(775, 388);
            this._stlDataGrid.TabIndex = 0;
            // 
            // _loadButton
            // 
            this._loadButton.Location = new System.Drawing.Point(13, 13);
            this._loadButton.Name = "_loadButton";
            this._loadButton.Size = new System.Drawing.Size(115, 31);
            this._loadButton.TabIndex = 1;
            this._loadButton.Text = "Load";
            this._loadButton.UseVisualStyleBackColor = true;
            this._loadButton.Click += new System.EventHandler(this.OnLoadFile);
            // 
            // _openFileDialog
            // 
            this._openFileDialog.FileName = "_openFileDialog";
            this._openFileDialog.Filter = "(*.STL)|*.STL|(*.stl)|*.stl|All files (*.*)|*.*";
            // 
            // STLEditorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this._loadButton);
            this.Controls.Add(this._stlDataGrid);
            this.Name = "STLEditorView";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this._stlDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _stlDataGrid;
        private System.Windows.Forms.Button _loadButton;
        private System.Windows.Forms.OpenFileDialog _openFileDialog;
    }
}

