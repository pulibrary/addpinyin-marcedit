namespace AddPinyin
{
    partial class frmMain
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
            this.labelStatus = new System.Windows.Forms.Label();
            this.availableTagsList = new System.Windows.Forms.ListBox();
            this.selectedTagsList = new System.Windows.Forms.ListBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.whichFieldsLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.convertButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.progressLabel = new System.Windows.Forms.Label();
            this.fieldOriginalRadio = new System.Windows.Forms.RadioButton();
            this.field880radio = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.swapCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(11, 9);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(256, 13);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "There are # record(s) with unconverted Chinese text.";
            // 
            // availableTagsList
            // 
            this.availableTagsList.FormattingEnabled = true;
            this.availableTagsList.Location = new System.Drawing.Point(36, 70);
            this.availableTagsList.Name = "availableTagsList";
            this.availableTagsList.Size = new System.Drawing.Size(65, 121);
            this.availableTagsList.TabIndex = 1;
            this.availableTagsList.SelectedIndexChanged += new System.EventHandler(this.availableTagsList_SelectedIndexChanged);
            // 
            // selectedTagsList
            // 
            this.selectedTagsList.FormattingEnabled = true;
            this.selectedTagsList.Location = new System.Drawing.Point(168, 70);
            this.selectedTagsList.Name = "selectedTagsList";
            this.selectedTagsList.Size = new System.Drawing.Size(65, 121);
            this.selectedTagsList.TabIndex = 2;
            this.selectedTagsList.SelectedIndexChanged += new System.EventHandler(this.selectedTagsList_SelectedIndexChanged);
            // 
            // removeButton
            // 
            this.removeButton.Enabled = false;
            this.removeButton.Location = new System.Drawing.Point(108, 93);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(54, 23);
            this.removeButton.TabIndex = 3;
            this.removeButton.Text = "<<";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // addButton
            // 
            this.addButton.Enabled = false;
            this.addButton.Location = new System.Drawing.Point(108, 137);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(54, 23);
            this.addButton.TabIndex = 4;
            this.addButton.Text = ">>";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // whichFieldsLabel
            // 
            this.whichFieldsLabel.AutoSize = true;
            this.whichFieldsLabel.Location = new System.Drawing.Point(36, 22);
            this.whichFieldsLabel.Name = "whichFieldsLabel";
            this.whichFieldsLabel.Size = new System.Drawing.Size(192, 13);
            this.whichFieldsLabel.TabIndex = 5;
            this.whichFieldsLabel.Text = "Which fields would you like to convert?";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Do Not Convert";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(166, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Convert";
            // 
            // convertButton
            // 
            this.convertButton.Location = new System.Drawing.Point(110, 267);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(75, 23);
            this.convertButton.TabIndex = 8;
            this.convertButton.Text = "Convert";
            this.convertButton.UseVisualStyleBackColor = true;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(192, 267);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(33, 244);
            this.progressLabel.MinimumSize = new System.Drawing.Size(20, 0);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(20, 13);
            this.progressLabel.TabIndex = 10;
            // 
            // fieldOriginalRadio
            // 
            this.fieldOriginalRadio.AutoSize = true;
            this.fieldOriginalRadio.Checked = true;
            this.fieldOriginalRadio.Location = new System.Drawing.Point(106, 201);
            this.fieldOriginalRadio.Name = "fieldOriginalRadio";
            this.fieldOriginalRadio.Size = new System.Drawing.Size(85, 17);
            this.fieldOriginalRadio.TabIndex = 11;
            this.fieldOriginalRadio.TabStop = true;
            this.fieldOriginalRadio.Text = "Original Field";
            this.fieldOriginalRadio.UseVisualStyleBackColor = true;
            // 
            // field880radio
            // 
            this.field880radio.AutoSize = true;
            this.field880radio.Location = new System.Drawing.Point(192, 201);
            this.field880radio.Name = "field880radio";
            this.field880radio.Size = new System.Drawing.Size(68, 17);
            this.field880radio.TabIndex = 12;
            this.field880radio.Text = "Field 880";
            this.field880radio.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 201);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Put pinyin in:";
            // 
            // swapCheckbox
            // 
            this.swapCheckbox.AutoSize = true;
            this.swapCheckbox.Checked = true;
            this.swapCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.swapCheckbox.Location = new System.Drawing.Point(36, 224);
            this.swapCheckbox.Name = "swapCheckbox";
            this.swapCheckbox.Size = new System.Drawing.Size(240, 17);
            this.swapCheckbox.TabIndex = 14;
            this.swapCheckbox.Text = "Swap order of existing parallel fields if needed";
            this.swapCheckbox.UseVisualStyleBackColor = true;
            this.swapCheckbox.CheckedChanged += new System.EventHandler(this.swapCheckbox_CheckedChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 308);
            this.Controls.Add(this.swapCheckbox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.field880radio);
            this.Controls.Add(this.fieldOriginalRadio);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.convertButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.whichFieldsLabel);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.selectedTagsList);
            this.Controls.Add(this.availableTagsList);
            this.Controls.Add(this.labelStatus);
            this.Name = "frmMain";
            this.ShowIcon = false;
            this.Text = "Add Parallel Pinyin Fields";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ListBox availableTagsList;
        private System.Windows.Forms.ListBox selectedTagsList;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Label whichFieldsLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.RadioButton fieldOriginalRadio;
        private System.Windows.Forms.RadioButton field880radio;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox swapCheckbox;
    }
}