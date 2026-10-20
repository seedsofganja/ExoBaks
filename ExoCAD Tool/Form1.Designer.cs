
namespace ExoCAD_Tool
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.WorksPathTextBox = new System.Windows.Forms.TextBox();
            this.MyWorksDataGridView = new System.Windows.Forms.DataGridView();
            this.WorkPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChoosePathButton = new System.Windows.Forms.Button();
            this.LoadWorksButton = new System.Windows.Forms.Button();
            this.LoadSelectedWorkButton = new System.Windows.Forms.Button();
            this.NomePaziente = new System.Windows.Forms.TextBox();
            this.CognomePaziente = new System.Windows.Forms.TextBox();
            this.StudioDentistico = new System.Windows.Forms.TextBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.Denti = new System.Windows.Forms.TextBox();
            this.BackUpPath = new System.Windows.Forms.TextBox();
            this.ChooseBackUpFolderButton = new System.Windows.Forms.Button();
            this.BackUpButton = new System.Windows.Forms.Button();
            this.LoopBackUpButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.MyWorksDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // WorksPathTextBox
            // 
            this.WorksPathTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WorksPathTextBox.Location = new System.Drawing.Point(17, 20);
            this.WorksPathTextBox.Name = "WorksPathTextBox";
            this.WorksPathTextBox.ReadOnly = true;
            this.WorksPathTextBox.Size = new System.Drawing.Size(263, 24);
            this.WorksPathTextBox.TabIndex = 0;
            // 
            // MyWorksDataGridView
            // 
            this.MyWorksDataGridView.AllowUserToAddRows = false;
            this.MyWorksDataGridView.AllowUserToDeleteRows = false;
            this.MyWorksDataGridView.AllowUserToResizeRows = false;
            this.MyWorksDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MyWorksDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.WorkPath});
            this.MyWorksDataGridView.Location = new System.Drawing.Point(17, 58);
            this.MyWorksDataGridView.MultiSelect = false;
            this.MyWorksDataGridView.Name = "MyWorksDataGridView";
            this.MyWorksDataGridView.ReadOnly = true;
            this.MyWorksDataGridView.RowHeadersVisible = false;
            this.MyWorksDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.MyWorksDataGridView.Size = new System.Drawing.Size(263, 471);
            this.MyWorksDataGridView.TabIndex = 1;
            this.MyWorksDataGridView.SelectionChanged += new System.EventHandler(this.MyWorksDataGridView_SelectionChanged);
            // 
            // WorkPath
            // 
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.WorkPath.DefaultCellStyle = dataGridViewCellStyle1;
            this.WorkPath.HeaderText = "Lavori";
            this.WorkPath.Name = "WorkPath";
            this.WorkPath.ReadOnly = true;
            this.WorkPath.Width = 250;
            // 
            // ChoosePathButton
            // 
            this.ChoosePathButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChoosePathButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChoosePathButton.Location = new System.Drawing.Point(286, 18);
            this.ChoosePathButton.Name = "ChoosePathButton";
            this.ChoosePathButton.Size = new System.Drawing.Size(221, 30);
            this.ChoosePathButton.TabIndex = 2;
            this.ChoosePathButton.Text = "Seleziona Cartella CAD-Data";
            this.ChoosePathButton.UseVisualStyleBackColor = true;
            this.ChoosePathButton.Click += new System.EventHandler(this.ChoosePathButton_Click);
            // 
            // LoadWorksButton
            // 
            this.LoadWorksButton.Location = new System.Drawing.Point(516, 15);
            this.LoadWorksButton.Name = "LoadWorksButton";
            this.LoadWorksButton.Size = new System.Drawing.Size(77, 33);
            this.LoadWorksButton.TabIndex = 3;
            this.LoadWorksButton.Text = "Carica Lavori";
            this.LoadWorksButton.UseVisualStyleBackColor = true;
            this.LoadWorksButton.Click += new System.EventHandler(this.LoadWorksButton_Click);
            // 
            // LoadSelectedWorkButton
            // 
            this.LoadSelectedWorkButton.Location = new System.Drawing.Point(365, 58);
            this.LoadSelectedWorkButton.Name = "LoadSelectedWorkButton";
            this.LoadSelectedWorkButton.Size = new System.Drawing.Size(103, 32);
            this.LoadSelectedWorkButton.TabIndex = 4;
            this.LoadSelectedWorkButton.Text = "Carica Lavoro";
            this.LoadSelectedWorkButton.UseVisualStyleBackColor = true;
            this.LoadSelectedWorkButton.Click += new System.EventHandler(this.LoadSelectedWorkButton_Click);
            // 
            // NomePaziente
            // 
            this.NomePaziente.Location = new System.Drawing.Point(286, 148);
            this.NomePaziente.Name = "NomePaziente";
            this.NomePaziente.ReadOnly = true;
            this.NomePaziente.Size = new System.Drawing.Size(289, 20);
            this.NomePaziente.TabIndex = 5;
            // 
            // CognomePaziente
            // 
            this.CognomePaziente.Location = new System.Drawing.Point(286, 174);
            this.CognomePaziente.Name = "CognomePaziente";
            this.CognomePaziente.ReadOnly = true;
            this.CognomePaziente.Size = new System.Drawing.Size(289, 20);
            this.CognomePaziente.TabIndex = 5;
            // 
            // StudioDentistico
            // 
            this.StudioDentistico.Location = new System.Drawing.Point(286, 122);
            this.StudioDentistico.Name = "StudioDentistico";
            this.StudioDentistico.ReadOnly = true;
            this.StudioDentistico.Size = new System.Drawing.Size(289, 20);
            this.StudioDentistico.TabIndex = 5;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Enabled = false;
            this.dateTimePicker1.Location = new System.Drawing.Point(325, 96);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(182, 20);
            this.dateTimePicker1.TabIndex = 6;
            // 
            // Denti
            // 
            this.Denti.Location = new System.Drawing.Point(286, 200);
            this.Denti.Name = "Denti";
            this.Denti.ReadOnly = true;
            this.Denti.Size = new System.Drawing.Size(289, 20);
            this.Denti.TabIndex = 7;
            // 
            // BackUpPath
            // 
            this.BackUpPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackUpPath.Location = new System.Drawing.Point(286, 405);
            this.BackUpPath.Name = "BackUpPath";
            this.BackUpPath.ReadOnly = true;
            this.BackUpPath.Size = new System.Drawing.Size(289, 24);
            this.BackUpPath.TabIndex = 0;
            // 
            // ChooseBackUpFolderButton
            // 
            this.ChooseBackUpFolderButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ChooseBackUpFolderButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChooseBackUpFolderButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChooseBackUpFolderButton.Location = new System.Drawing.Point(286, 353);
            this.ChooseBackUpFolderButton.Name = "ChooseBackUpFolderButton";
            this.ChooseBackUpFolderButton.Size = new System.Drawing.Size(289, 46);
            this.ChooseBackUpFolderButton.TabIndex = 2;
            this.ChooseBackUpFolderButton.Text = "Seleziona Cartella BackUp";
            this.ChooseBackUpFolderButton.UseVisualStyleBackColor = true;
            this.ChooseBackUpFolderButton.Click += new System.EventHandler(this.ChooseBackUpFolderButton_Click);
            // 
            // BackUpButton
            // 
            this.BackUpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackUpButton.Location = new System.Drawing.Point(365, 435);
            this.BackUpButton.Name = "BackUpButton";
            this.BackUpButton.Size = new System.Drawing.Size(103, 41);
            this.BackUpButton.TabIndex = 8;
            this.BackUpButton.Text = "BackUp Singolo";
            this.BackUpButton.UseVisualStyleBackColor = true;
            this.BackUpButton.Click += new System.EventHandler(this.BackUpButton_Click);
            // 
            // LoopBackUpButton
            // 
            this.LoopBackUpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoopBackUpButton.Location = new System.Drawing.Point(365, 482);
            this.LoopBackUpButton.Name = "LoopBackUpButton";
            this.LoopBackUpButton.Size = new System.Drawing.Size(103, 48);
            this.LoopBackUpButton.TabIndex = 9;
            this.LoopBackUpButton.Text = "BackUp Completo";
            this.LoopBackUpButton.UseVisualStyleBackColor = true;
            this.LoopBackUpButton.Click += new System.EventHandler(this.LoopBackUpButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(17, 535);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(558, 39);
            this.progressBar1.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(286, 238);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(286, 589);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "label2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(605, 627);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.LoopBackUpButton);
            this.Controls.Add(this.BackUpButton);
            this.Controls.Add(this.Denti);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.CognomePaziente);
            this.Controls.Add(this.StudioDentistico);
            this.Controls.Add(this.NomePaziente);
            this.Controls.Add(this.LoadSelectedWorkButton);
            this.Controls.Add(this.LoadWorksButton);
            this.Controls.Add(this.ChooseBackUpFolderButton);
            this.Controls.Add(this.ChoosePathButton);
            this.Controls.Add(this.BackUpPath);
            this.Controls.Add(this.MyWorksDataGridView);
            this.Controls.Add(this.WorksPathTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "ExoCAD Tool";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MyWorksDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox WorksPathTextBox;
        private System.Windows.Forms.DataGridView MyWorksDataGridView;
        private System.Windows.Forms.Button ChoosePathButton;
        private System.Windows.Forms.Button LoadWorksButton;
        private System.Windows.Forms.Button LoadSelectedWorkButton;
        private System.Windows.Forms.TextBox NomePaziente;
        private System.Windows.Forms.TextBox CognomePaziente;
        private System.Windows.Forms.TextBox StudioDentistico;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.TextBox Denti;
        private System.Windows.Forms.TextBox BackUpPath;
        private System.Windows.Forms.Button ChooseBackUpFolderButton;
        private System.Windows.Forms.Button BackUpButton;
        private System.Windows.Forms.Button LoopBackUpButton;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridViewTextBoxColumn WorkPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

