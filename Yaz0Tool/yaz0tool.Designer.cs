namespace TestToo
{
    partial class Yaz0
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Yaz0));
            this.go_button = new System.Windows.Forms.Button();
            this.browse_input = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.browse_output = new System.Windows.Forms.Button();
            this.input_path = new System.Windows.Forms.RichTextBox();
            this.output_path = new System.Windows.Forms.RichTextBox();
            this.input = new System.Windows.Forms.OpenFileDialog();
            this.output = new System.Windows.Forms.FolderBrowserDialog();
            this.enc = new System.Windows.Forms.RadioButton();
            this.dec = new System.Windows.Forms.RadioButton();
            this.status = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.clear_output = new System.Windows.Forms.Button();
            this.prog = new System.Windows.Forms.ProgressBar();
            this.bgw = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // go_button
            // 
            this.go_button.Location = new System.Drawing.Point(216, 104);
            this.go_button.Name = "go_button";
            this.go_button.Size = new System.Drawing.Size(75, 23);
            this.go_button.TabIndex = 0;
            this.go_button.Text = "Go";
            this.go_button.UseVisualStyleBackColor = true;
            this.go_button.Click += new System.EventHandler(this.go_button_Click);
            // 
            // browse_input
            // 
            this.browse_input.Location = new System.Drawing.Point(99, 104);
            this.browse_input.Name = "browse_input";
            this.browse_input.Size = new System.Drawing.Size(75, 23);
            this.browse_input.TabIndex = 1;
            this.browse_input.Text = "Browse";
            this.browse_input.UseVisualStyleBackColor = true;
            this.browse_input.Click += new System.EventHandler(this.browse_input_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(347, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Output";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(120, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Input";
            // 
            // browse_output
            // 
            this.browse_output.Location = new System.Drawing.Point(328, 104);
            this.browse_output.Name = "browse_output";
            this.browse_output.Size = new System.Drawing.Size(75, 23);
            this.browse_output.TabIndex = 10;
            this.browse_output.Text = "Browse";
            this.browse_output.UseVisualStyleBackColor = true;
            this.browse_output.Click += new System.EventHandler(this.browse_output_Click);
            // 
            // input_path
            // 
            this.input_path.BackColor = System.Drawing.Color.Gainsboro;
            this.input_path.Location = new System.Drawing.Point(23, 61);
            this.input_path.Multiline = false;
            this.input_path.Name = "input_path";
            this.input_path.ReadOnly = true;
            this.input_path.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.input_path.Size = new System.Drawing.Size(222, 22);
            this.input_path.TabIndex = 11;
            this.input_path.Text = "";
            // 
            // output_path
            // 
            this.output_path.BackColor = System.Drawing.Color.Gainsboro;
            this.output_path.Location = new System.Drawing.Point(264, 61);
            this.output_path.MaxLength = 40;
            this.output_path.Multiline = false;
            this.output_path.Name = "output_path";
            this.output_path.ReadOnly = true;
            this.output_path.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.output_path.Size = new System.Drawing.Size(222, 22);
            this.output_path.TabIndex = 12;
            this.output_path.Text = "        Default is input file\'s parent folder";
            this.output_path.WordWrap = false;
            // 
            // input
            // 
            this.input.FileName = "input";
            this.input.Title = "Input File";
            this.input.FileOk += new System.ComponentModel.CancelEventHandler(this.input_FileOk);
            // 
            // output
            // 
            this.output.Description = "Choose Output Location";
            // 
            // enc
            // 
            this.enc.AutoCheck = false;
            this.enc.AutoSize = true;
            this.enc.Checked = true;
            this.enc.Location = new System.Drawing.Point(143, 163);
            this.enc.Name = "enc";
            this.enc.Size = new System.Drawing.Size(62, 17);
            this.enc.TabIndex = 13;
            this.enc.TabStop = true;
            this.enc.Text = "Encode";
            this.enc.UseVisualStyleBackColor = true;
            this.enc.Click += new System.EventHandler(this.enc_Click);
            // 
            // dec
            // 
            this.dec.AutoCheck = false;
            this.dec.AutoSize = true;
            this.dec.Location = new System.Drawing.Point(303, 163);
            this.dec.Name = "dec";
            this.dec.Size = new System.Drawing.Size(63, 17);
            this.dec.TabIndex = 14;
            this.dec.TabStop = true;
            this.dec.Text = "Decode";
            this.dec.UseVisualStyleBackColor = true;
            this.dec.Click += new System.EventHandler(this.dec_Click);
            // 
            // status
            // 
            this.status.AcceptsReturn = true;
            this.status.AcceptsTab = true;
            this.status.BackColor = System.Drawing.Color.Gainsboro;
            this.status.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.status.Location = new System.Drawing.Point(23, 220);
            this.status.Multiline = true;
            this.status.Name = "status";
            this.status.ReadOnly = true;
            this.status.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.status.Size = new System.Drawing.Size(474, 138);
            this.status.TabIndex = 16;
            this.status.TabStop = false;
            this.status.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(231, 192);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Output";
            // 
            // clear_output
            // 
            this.clear_output.Location = new System.Drawing.Point(216, 364);
            this.clear_output.Name = "clear_output";
            this.clear_output.Size = new System.Drawing.Size(75, 23);
            this.clear_output.TabIndex = 18;
            this.clear_output.Text = "Clear Output";
            this.clear_output.UseVisualStyleBackColor = true;
            this.clear_output.Click += new System.EventHandler(this.clear_output_Click);
            // 
            // prog
            // 
            this.prog.ForeColor = System.Drawing.Color.Black;
            this.prog.Location = new System.Drawing.Point(23, 393);
            this.prog.Name = "prog";
            this.prog.Size = new System.Drawing.Size(463, 59);
            this.prog.TabIndex = 19;
            // 
            // Yaz0
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(509, 463);
            this.Controls.Add(this.prog);
            this.Controls.Add(this.clear_output);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.status);
            this.Controls.Add(this.dec);
            this.Controls.Add(this.enc);
            this.Controls.Add(this.output_path);
            this.Controls.Add(this.input_path);
            this.Controls.Add(this.browse_output);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.browse_input);
            this.Controls.Add(this.go_button);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(525, 502);
            this.MinimumSize = new System.Drawing.Size(525, 502);
            this.Name = "Yaz0";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Yaz0 Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button go_button;
        private System.Windows.Forms.Button browse_input;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button browse_output;
        private System.Windows.Forms.RichTextBox input_path;
        private System.Windows.Forms.RichTextBox output_path;
        private System.Windows.Forms.OpenFileDialog input;
        private System.Windows.Forms.FolderBrowserDialog output;
        private System.Windows.Forms.RadioButton enc;
        private System.Windows.Forms.RadioButton dec;
        private System.Windows.Forms.TextBox status;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button clear_output;
        private System.Windows.Forms.ProgressBar prog;
        private System.ComponentModel.BackgroundWorker bgw;
    }
}

