
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.WorksPathTextBox = new System.Windows.Forms.TextBox();
            this.MyWorksDataGridView = new System.Windows.Forms.DataGridView();
            this.WorkPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PazienteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StudioColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChoosePathButton = new System.Windows.Forms.Button();
            this.LoadWorksButton = new System.Windows.Forms.Button();
            this.LoadSelectedWorkButton = new System.Windows.Forms.Button();
            this.NomePaziente = new System.Windows.Forms.TextBox();
            this.CognomePaziente = new System.Windows.Forms.TextBox();
            this.StudioDentistico = new System.Windows.Forms.TextBox();
            this.DataLavoroPicker = new System.Windows.Forms.DateTimePicker();
            this.Denti = new System.Windows.Forms.TextBox();
            this.BackUpPath = new System.Windows.Forms.TextBox();
            this.ChooseBackUpFolderButton = new System.Windows.Forms.Button();
            this.BackUpButton = new System.Windows.Forms.Button();
            this.LoopBackUpButton = new System.Windows.Forms.Button();
            this.BackupProgressBar = new System.Windows.Forms.ProgressBar();
            this.CercaLabel = new System.Windows.Forms.Label();
            this.CercaTextBox = new System.Windows.Forms.TextBox();
            this.SoloNonSalvatiCheckBox = new System.Windows.Forms.CheckBox();
            this.DataLabel = new System.Windows.Forms.Label();
            this.StudioLabel = new System.Windows.Forms.Label();
            this.NomeLabel = new System.Windows.Forms.Label();
            this.CognomeLabel = new System.Windows.Forms.Label();
            this.DentiLabel = new System.Windows.Forms.Label();
            this.AggiornaSalvatiCheckBox = new System.Windows.Forms.CheckBox();
            this.AnnullaButton = new System.Windows.Forms.Button();
            this.BarraStato = new System.Windows.Forms.StatusStrip();
            this.StatoLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ContatoreLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.MyWorksDataGridView)).BeginInit();
            this.BarraStato.SuspendLayout();
            this.SuspendLayout();
            //
            // WorksPathTextBox
            //
            this.WorksPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WorksPathTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WorksPathTextBox.Location = new System.Drawing.Point(12, 12);
            this.WorksPathTextBox.Name = "WorksPathTextBox";
            this.WorksPathTextBox.ReadOnly = true;
            this.WorksPathTextBox.Size = new System.Drawing.Size(560, 24);
            this.WorksPathTextBox.TabIndex = 0;
            //
            // MyWorksDataGridView
            //
            this.MyWorksDataGridView.AllowUserToAddRows = false;
            this.MyWorksDataGridView.AllowUserToDeleteRows = false;
            this.MyWorksDataGridView.AllowUserToResizeRows = false;
            this.MyWorksDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MyWorksDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MyWorksDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.WorkPath,
            this.PazienteColumn,
            this.StudioColumn,
            this.DataColumn});
            this.MyWorksDataGridView.Location = new System.Drawing.Point(12, 110);
            this.MyWorksDataGridView.MultiSelect = false;
            this.MyWorksDataGridView.Name = "MyWorksDataGridView";
            this.MyWorksDataGridView.ReadOnly = true;
            this.MyWorksDataGridView.RowHeadersVisible = false;
            this.MyWorksDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.MyWorksDataGridView.Size = new System.Drawing.Size(560, 424);
            this.MyWorksDataGridView.TabIndex = 7;
            this.MyWorksDataGridView.SelectionChanged += new System.EventHandler(this.MyWorksDataGridView_SelectionChanged);
            //
            // WorkPath
            //
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.WorkPath.DefaultCellStyle = dataGridViewCellStyle1;
            this.WorkPath.HeaderText = "Lavoro";
            this.WorkPath.Name = "WorkPath";
            this.WorkPath.ReadOnly = true;
            this.WorkPath.Width = 170;
            //
            // PazienteColumn
            //
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this.PazienteColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.PazienteColumn.HeaderText = "Paziente";
            this.PazienteColumn.Name = "PazienteColumn";
            this.PazienteColumn.ReadOnly = true;
            this.PazienteColumn.Width = 150;
            //
            // StudioColumn
            //
            this.StudioColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this.StudioColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.StudioColumn.HeaderText = "Studio";
            this.StudioColumn.Name = "StudioColumn";
            this.StudioColumn.ReadOnly = true;
            //
            // DataColumn
            //
            dataGridViewCellStyle4.Format = "dd/MM/yyyy";
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            this.DataColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.DataColumn.HeaderText = "Data";
            this.DataColumn.Name = "DataColumn";
            this.DataColumn.ReadOnly = true;
            this.DataColumn.Width = 80;
            //
            // ChoosePathButton
            //
            this.ChoosePathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ChoosePathButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChoosePathButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChoosePathButton.Location = new System.Drawing.Point(580, 9);
            this.ChoosePathButton.Name = "ChoosePathButton";
            this.ChoosePathButton.Size = new System.Drawing.Size(200, 30);
            this.ChoosePathButton.TabIndex = 1;
            this.ChoosePathButton.Text = "Seleziona Cartella CAD-Data";
            this.ChoosePathButton.UseVisualStyleBackColor = true;
            this.ChoosePathButton.Click += new System.EventHandler(this.ChoosePathButton_Click);
            //
            // LoadWorksButton
            //
            this.LoadWorksButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LoadWorksButton.Location = new System.Drawing.Point(786, 9);
            this.LoadWorksButton.Name = "LoadWorksButton";
            this.LoadWorksButton.Size = new System.Drawing.Size(100, 30);
            this.LoadWorksButton.TabIndex = 2;
            this.LoadWorksButton.Text = "Ricarica Lavori";
            this.LoadWorksButton.UseVisualStyleBackColor = true;
            this.LoadWorksButton.Click += new System.EventHandler(this.LoadWorksButton_Click);
            //
            // LoadSelectedWorkButton
            //
            this.LoadSelectedWorkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LoadSelectedWorkButton.Location = new System.Drawing.Point(580, 110);
            this.LoadSelectedWorkButton.Name = "LoadSelectedWorkButton";
            this.LoadSelectedWorkButton.Size = new System.Drawing.Size(306, 30);
            this.LoadSelectedWorkButton.TabIndex = 8;
            this.LoadSelectedWorkButton.Text = "Ricarica Dati Lavoro (annulla modifiche)";
            this.LoadSelectedWorkButton.UseVisualStyleBackColor = true;
            this.LoadSelectedWorkButton.Click += new System.EventHandler(this.LoadSelectedWorkButton_Click);
            //
            // NomePaziente
            //
            this.NomePaziente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NomePaziente.Location = new System.Drawing.Point(645, 202);
            this.NomePaziente.Name = "NomePaziente";
            this.NomePaziente.Size = new System.Drawing.Size(241, 20);
            this.NomePaziente.TabIndex = 14;
            //
            // CognomePaziente
            //
            this.CognomePaziente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CognomePaziente.Location = new System.Drawing.Point(645, 228);
            this.CognomePaziente.Name = "CognomePaziente";
            this.CognomePaziente.Size = new System.Drawing.Size(241, 20);
            this.CognomePaziente.TabIndex = 16;
            //
            // StudioDentistico
            //
            this.StudioDentistico.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StudioDentistico.Location = new System.Drawing.Point(645, 176);
            this.StudioDentistico.Name = "StudioDentistico";
            this.StudioDentistico.Size = new System.Drawing.Size(241, 20);
            this.StudioDentistico.TabIndex = 12;
            //
            // DataLavoroPicker
            //
            this.DataLavoroPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DataLavoroPicker.Location = new System.Drawing.Point(645, 150);
            this.DataLavoroPicker.Name = "DataLavoroPicker";
            this.DataLavoroPicker.Size = new System.Drawing.Size(241, 20);
            this.DataLavoroPicker.TabIndex = 10;
            //
            // Denti
            //
            this.Denti.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Denti.Location = new System.Drawing.Point(645, 254);
            this.Denti.Name = "Denti";
            this.Denti.Size = new System.Drawing.Size(241, 20);
            this.Denti.TabIndex = 18;
            //
            // BackUpPath
            //
            this.BackUpPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BackUpPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackUpPath.Location = new System.Drawing.Point(12, 46);
            this.BackUpPath.Name = "BackUpPath";
            this.BackUpPath.ReadOnly = true;
            this.BackUpPath.Size = new System.Drawing.Size(560, 24);
            this.BackUpPath.TabIndex = 3;
            //
            // ChooseBackUpFolderButton
            //
            this.ChooseBackUpFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ChooseBackUpFolderButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ChooseBackUpFolderButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChooseBackUpFolderButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChooseBackUpFolderButton.Location = new System.Drawing.Point(580, 43);
            this.ChooseBackUpFolderButton.Name = "ChooseBackUpFolderButton";
            this.ChooseBackUpFolderButton.Size = new System.Drawing.Size(200, 30);
            this.ChooseBackUpFolderButton.TabIndex = 4;
            this.ChooseBackUpFolderButton.Text = "Seleziona Cartella BackUp";
            this.ChooseBackUpFolderButton.UseVisualStyleBackColor = true;
            this.ChooseBackUpFolderButton.Click += new System.EventHandler(this.ChooseBackUpFolderButton_Click);
            //
            // BackUpButton
            //
            this.BackUpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BackUpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackUpButton.Location = new System.Drawing.Point(580, 290);
            this.BackUpButton.Name = "BackUpButton";
            this.BackUpButton.Size = new System.Drawing.Size(306, 40);
            this.BackUpButton.TabIndex = 19;
            this.BackUpButton.Text = "BackUp Singolo";
            this.BackUpButton.UseVisualStyleBackColor = true;
            this.BackUpButton.Click += new System.EventHandler(this.BackUpButton_Click);
            //
            // LoopBackUpButton
            //
            this.LoopBackUpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LoopBackUpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoopBackUpButton.Location = new System.Drawing.Point(580, 360);
            this.LoopBackUpButton.Name = "LoopBackUpButton";
            this.LoopBackUpButton.Size = new System.Drawing.Size(306, 40);
            this.LoopBackUpButton.TabIndex = 21;
            this.LoopBackUpButton.Text = "BackUp Completo";
            this.LoopBackUpButton.UseVisualStyleBackColor = true;
            this.LoopBackUpButton.Click += new System.EventHandler(this.LoopBackUpButton_Click);
            //
            // BackupProgressBar
            //
            this.BackupProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BackupProgressBar.Location = new System.Drawing.Point(12, 544);
            this.BackupProgressBar.Name = "BackupProgressBar";
            this.BackupProgressBar.Size = new System.Drawing.Size(874, 24);
            this.BackupProgressBar.TabIndex = 23;
            //
            // CercaLabel
            //
            this.CercaLabel.AutoSize = true;
            this.CercaLabel.Location = new System.Drawing.Point(12, 85);
            this.CercaLabel.Name = "CercaLabel";
            this.CercaLabel.Size = new System.Drawing.Size(38, 13);
            this.CercaLabel.TabIndex = 24;
            this.CercaLabel.Text = "Cerca:";
            //
            // CercaTextBox
            //
            this.CercaTextBox.Location = new System.Drawing.Point(56, 82);
            this.CercaTextBox.Name = "CercaTextBox";
            this.CercaTextBox.Size = new System.Drawing.Size(250, 20);
            this.CercaTextBox.TabIndex = 5;
            this.CercaTextBox.TextChanged += new System.EventHandler(this.CercaTextBox_TextChanged);
            //
            // SoloNonSalvatiCheckBox
            //
            this.SoloNonSalvatiCheckBox.AutoSize = true;
            this.SoloNonSalvatiCheckBox.Location = new System.Drawing.Point(320, 84);
            this.SoloNonSalvatiCheckBox.Name = "SoloNonSalvatiCheckBox";
            this.SoloNonSalvatiCheckBox.Size = new System.Drawing.Size(150, 17);
            this.SoloNonSalvatiCheckBox.TabIndex = 6;
            this.SoloNonSalvatiCheckBox.Text = "Mostra solo i non salvati";
            this.SoloNonSalvatiCheckBox.UseVisualStyleBackColor = true;
            this.SoloNonSalvatiCheckBox.CheckedChanged += new System.EventHandler(this.SoloNonSalvatiCheckBox_CheckedChanged);
            //
            // DataLabel
            //
            this.DataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DataLabel.AutoSize = true;
            this.DataLabel.Location = new System.Drawing.Point(580, 154);
            this.DataLabel.Name = "DataLabel";
            this.DataLabel.Size = new System.Drawing.Size(33, 13);
            this.DataLabel.TabIndex = 9;
            this.DataLabel.Text = "Data:";
            //
            // StudioLabel
            //
            this.StudioLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StudioLabel.AutoSize = true;
            this.StudioLabel.Location = new System.Drawing.Point(580, 179);
            this.StudioLabel.Name = "StudioLabel";
            this.StudioLabel.Size = new System.Drawing.Size(40, 13);
            this.StudioLabel.TabIndex = 11;
            this.StudioLabel.Text = "Studio:";
            //
            // NomeLabel
            //
            this.NomeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NomeLabel.AutoSize = true;
            this.NomeLabel.Location = new System.Drawing.Point(580, 205);
            this.NomeLabel.Name = "NomeLabel";
            this.NomeLabel.Size = new System.Drawing.Size(38, 13);
            this.NomeLabel.TabIndex = 13;
            this.NomeLabel.Text = "Nome:";
            //
            // CognomeLabel
            //
            this.CognomeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CognomeLabel.AutoSize = true;
            this.CognomeLabel.Location = new System.Drawing.Point(580, 231);
            this.CognomeLabel.Name = "CognomeLabel";
            this.CognomeLabel.Size = new System.Drawing.Size(55, 13);
            this.CognomeLabel.TabIndex = 15;
            this.CognomeLabel.Text = "Cognome:";
            //
            // DentiLabel
            //
            this.DentiLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DentiLabel.AutoSize = true;
            this.DentiLabel.Location = new System.Drawing.Point(580, 257);
            this.DentiLabel.Name = "DentiLabel";
            this.DentiLabel.Size = new System.Drawing.Size(36, 13);
            this.DentiLabel.TabIndex = 17;
            this.DentiLabel.Text = "Denti:";
            //
            // AggiornaSalvatiCheckBox
            //
            this.AggiornaSalvatiCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AggiornaSalvatiCheckBox.AutoSize = true;
            this.AggiornaSalvatiCheckBox.Location = new System.Drawing.Point(580, 340);
            this.AggiornaSalvatiCheckBox.Name = "AggiornaSalvatiCheckBox";
            this.AggiornaSalvatiCheckBox.Size = new System.Drawing.Size(196, 17);
            this.AggiornaSalvatiCheckBox.TabIndex = 20;
            this.AggiornaSalvatiCheckBox.Text = "Aggiorna anche i lavori già salvati";
            this.AggiornaSalvatiCheckBox.UseVisualStyleBackColor = true;
            //
            // AnnullaButton
            //
            this.AnnullaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AnnullaButton.Enabled = false;
            this.AnnullaButton.Location = new System.Drawing.Point(580, 406);
            this.AnnullaButton.Name = "AnnullaButton";
            this.AnnullaButton.Size = new System.Drawing.Size(306, 30);
            this.AnnullaButton.TabIndex = 22;
            this.AnnullaButton.Text = "Annulla";
            this.AnnullaButton.UseVisualStyleBackColor = true;
            this.AnnullaButton.Click += new System.EventHandler(this.AnnullaButton_Click);
            //
            // BarraStato
            //
            this.BarraStato.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatoLabel,
            this.ContatoreLabel});
            this.BarraStato.Location = new System.Drawing.Point(0, 578);
            this.BarraStato.Name = "BarraStato";
            this.BarraStato.Size = new System.Drawing.Size(898, 22);
            this.BarraStato.TabIndex = 25;
            //
            // StatoLabel
            //
            this.StatoLabel.Name = "StatoLabel";
            this.StatoLabel.Size = new System.Drawing.Size(883, 17);
            this.StatoLabel.Spring = true;
            this.StatoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // ContatoreLabel
            //
            this.ContatoreLabel.Name = "ContatoreLabel";
            this.ContatoreLabel.Size = new System.Drawing.Size(0, 17);
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(898, 600);
            this.Controls.Add(this.BarraStato);
            this.Controls.Add(this.AnnullaButton);
            this.Controls.Add(this.AggiornaSalvatiCheckBox);
            this.Controls.Add(this.DentiLabel);
            this.Controls.Add(this.CognomeLabel);
            this.Controls.Add(this.NomeLabel);
            this.Controls.Add(this.StudioLabel);
            this.Controls.Add(this.DataLabel);
            this.Controls.Add(this.SoloNonSalvatiCheckBox);
            this.Controls.Add(this.CercaTextBox);
            this.Controls.Add(this.CercaLabel);
            this.Controls.Add(this.BackupProgressBar);
            this.Controls.Add(this.LoopBackUpButton);
            this.Controls.Add(this.BackUpButton);
            this.Controls.Add(this.Denti);
            this.Controls.Add(this.DataLavoroPicker);
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
            this.MinimumSize = new System.Drawing.Size(914, 639);
            this.Name = "Form1";
            this.Text = "ExoCAD Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MyWorksDataGridView)).EndInit();
            this.BarraStato.ResumeLayout(false);
            this.BarraStato.PerformLayout();
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
        private System.Windows.Forms.DateTimePicker DataLavoroPicker;
        private System.Windows.Forms.TextBox Denti;
        private System.Windows.Forms.TextBox BackUpPath;
        private System.Windows.Forms.Button ChooseBackUpFolderButton;
        private System.Windows.Forms.Button BackUpButton;
        private System.Windows.Forms.Button LoopBackUpButton;
        private System.Windows.Forms.ProgressBar BackupProgressBar;
        private System.Windows.Forms.DataGridViewTextBoxColumn WorkPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn PazienteColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StudioColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataColumn;
        private System.Windows.Forms.Label CercaLabel;
        private System.Windows.Forms.TextBox CercaTextBox;
        private System.Windows.Forms.CheckBox SoloNonSalvatiCheckBox;
        private System.Windows.Forms.Label DataLabel;
        private System.Windows.Forms.Label StudioLabel;
        private System.Windows.Forms.Label NomeLabel;
        private System.Windows.Forms.Label CognomeLabel;
        private System.Windows.Forms.Label DentiLabel;
        private System.Windows.Forms.CheckBox AggiornaSalvatiCheckBox;
        private System.Windows.Forms.Button AnnullaButton;
        private System.Windows.Forms.StatusStrip BarraStato;
        private System.Windows.Forms.ToolStripStatusLabel StatoLabel;
        private System.Windows.Forms.ToolStripStatusLabel ContatoreLabel;
    }
}

