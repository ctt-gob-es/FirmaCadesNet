namespace DemoFirmaCadesNet
{
    partial class FrmPrincipal
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSeleccionarFichero = new System.Windows.Forms.Button();
            this.txtFichero = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rbDetachedExplicit = new System.Windows.Forms.RadioButton();
            this.rbAttachedImplicit = new System.Windows.Forms.RadioButton();
            this.cmbAlgoritmo = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnContraFirma = new System.Windows.Forms.Button();
            this.txtHashPolitica = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtIdentificadorPolitica = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCoFirmar = new System.Windows.Forms.Button();
            this.btnCargarFirma = new System.Windows.Forms.Button();
            this.btnGuardarFirma = new System.Windows.Forms.Button();
            this.btnCadesT = new System.Windows.Forms.Button();
            this.txtURLSellado = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnFirmar = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnValidar = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtHuellaPrecalculada = new System.Windows.Forms.TextBox();
            this.btnFirmarHuella = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSeleccionarFichero);
            this.groupBox1.Controls.Add(this.txtFichero);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.rbDetachedExplicit);
            this.groupBox1.Controls.Add(this.rbAttachedImplicit);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(606, 140);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Formato de firma";
            // 
            // btnSeleccionarFichero
            // 
            this.btnSeleccionarFichero.Location = new System.Drawing.Point(425, 99);
            this.btnSeleccionarFichero.Name = "btnSeleccionarFichero";
            this.btnSeleccionarFichero.Size = new System.Drawing.Size(28, 23);
            this.btnSeleccionarFichero.TabIndex = 5;
            this.btnSeleccionarFichero.Text = "...";
            this.btnSeleccionarFichero.UseVisualStyleBackColor = true;
            this.btnSeleccionarFichero.Click += new System.EventHandler(this.btnSeleccionarFichero_Click);
            // 
            // txtFichero
            // 
            this.txtFichero.Location = new System.Drawing.Point(13, 100);
            this.txtFichero.Name = "txtFichero";
            this.txtFichero.Size = new System.Drawing.Size(412, 20);
            this.txtFichero.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Fichero original";
            // 
            // rbDetachedExplicit
            // 
            this.rbDetachedExplicit.AutoSize = true;
            this.rbDetachedExplicit.Location = new System.Drawing.Point(13, 51);
            this.rbDetachedExplicit.Name = "rbDetachedExplicit";
            this.rbDetachedExplicit.Size = new System.Drawing.Size(107, 17);
            this.rbDetachedExplicit.TabIndex = 1;
            this.rbDetachedExplicit.Text = "Detached explicit";
            this.rbDetachedExplicit.UseVisualStyleBackColor = true;
            // 
            // rbAttachedImplicit
            // 
            this.rbAttachedImplicit.AutoSize = true;
            this.rbAttachedImplicit.Checked = true;
            this.rbAttachedImplicit.Location = new System.Drawing.Point(13, 27);
            this.rbAttachedImplicit.Name = "rbAttachedImplicit";
            this.rbAttachedImplicit.Size = new System.Drawing.Size(102, 17);
            this.rbAttachedImplicit.TabIndex = 0;
            this.rbAttachedImplicit.TabStop = true;
            this.rbAttachedImplicit.Text = "Attached implicit";
            this.rbAttachedImplicit.UseVisualStyleBackColor = true;
            // 
            // cmbAlgoritmo
            // 
            this.cmbAlgoritmo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlgoritmo.FormattingEnabled = true;
            this.cmbAlgoritmo.Items.AddRange(new object[] {
            "SHA1",
            "SHA256",
            "SHA512"});
            this.cmbAlgoritmo.Location = new System.Drawing.Point(16, 290);
            this.cmbAlgoritmo.Name = "cmbAlgoritmo";
            this.cmbAlgoritmo.Size = new System.Drawing.Size(108, 21);
            this.cmbAlgoritmo.TabIndex = 40;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 273);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 13);
            this.label7.TabIndex = 39;
            this.label7.Text = "Algoritmo de huella";
            // 
            // btnContraFirma
            // 
            this.btnContraFirma.Location = new System.Drawing.Point(178, 333);
            this.btnContraFirma.Name = "btnContraFirma";
            this.btnContraFirma.Size = new System.Drawing.Size(75, 23);
            this.btnContraFirma.TabIndex = 38;
            this.btnContraFirma.Text = "ContraFirma";
            this.btnContraFirma.UseVisualStyleBackColor = true;
            this.btnContraFirma.Click += new System.EventHandler(this.btnContraFirma_Click);
            // 
            // txtHashPolitica
            // 
            this.txtHashPolitica.Location = new System.Drawing.Point(277, 237);
            this.txtHashPolitica.Name = "txtHashPolitica";
            this.txtHashPolitica.Size = new System.Drawing.Size(339, 20);
            this.txtHashPolitica.TabIndex = 35;
            this.txtHashPolitica.Text = "G7roucf600+f03r/o0bAOQ6WAs0=";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(274, 222);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(165, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Valor hash de la política (base64)";
            // 
            // txtIdentificadorPolitica
            // 
            this.txtIdentificadorPolitica.Location = new System.Drawing.Point(16, 238);
            this.txtIdentificadorPolitica.Name = "txtIdentificadorPolitica";
            this.txtIdentificadorPolitica.Size = new System.Drawing.Size(235, 20);
            this.txtIdentificadorPolitica.TabIndex = 33;
            this.txtIdentificadorPolitica.Text = "2.16.724.1.3.1.1.2.1.9";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 222);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(158, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Identificador de política de firma";
            // 
            // btnCoFirmar
            // 
            this.btnCoFirmar.Location = new System.Drawing.Point(97, 333);
            this.btnCoFirmar.Name = "btnCoFirmar";
            this.btnCoFirmar.Size = new System.Drawing.Size(75, 23);
            this.btnCoFirmar.TabIndex = 31;
            this.btnCoFirmar.Text = "Co-Firmar";
            this.btnCoFirmar.UseVisualStyleBackColor = true;
            this.btnCoFirmar.Click += new System.EventHandler(this.btnCoFirmar_Click);
            // 
            // btnCargarFirma
            // 
            this.btnCargarFirma.Location = new System.Drawing.Point(519, 279);
            this.btnCargarFirma.Name = "btnCargarFirma";
            this.btnCargarFirma.Size = new System.Drawing.Size(97, 23);
            this.btnCargarFirma.TabIndex = 30;
            this.btnCargarFirma.Text = "Cargar firma";
            this.btnCargarFirma.UseVisualStyleBackColor = true;
            this.btnCargarFirma.Click += new System.EventHandler(this.btnCargarFirma_Click);
            // 
            // btnGuardarFirma
            // 
            this.btnGuardarFirma.Location = new System.Drawing.Point(519, 310);
            this.btnGuardarFirma.Name = "btnGuardarFirma";
            this.btnGuardarFirma.Size = new System.Drawing.Size(97, 23);
            this.btnGuardarFirma.TabIndex = 29;
            this.btnGuardarFirma.Text = "Guardar firma";
            this.btnGuardarFirma.UseVisualStyleBackColor = true;
            this.btnGuardarFirma.Click += new System.EventHandler(this.btnGuardarFirma_Click);
            // 
            // btnCadesT
            // 
            this.btnCadesT.Location = new System.Drawing.Point(277, 333);
            this.btnCadesT.Name = "btnCadesT";
            this.btnCadesT.Size = new System.Drawing.Size(144, 23);
            this.btnCadesT.TabIndex = 27;
            this.btnCadesT.Text = "Ampliar a CAdES-T";
            this.btnCadesT.UseVisualStyleBackColor = true;
            this.btnCadesT.Click += new System.EventHandler(this.btnCadesT_Click);
            // 
            // txtURLSellado
            // 
            this.txtURLSellado.Location = new System.Drawing.Point(16, 183);
            this.txtURLSellado.Name = "txtURLSellado";
            this.txtURLSellado.Size = new System.Drawing.Size(265, 20);
            this.txtURLSellado.TabIndex = 24;
            this.txtURLSellado.Text = "http://tss.accv.es:8318/tsa";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "URL servidor sellado de tiempo";
            // 
            // btnFirmar
            // 
            this.btnFirmar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFirmar.Location = new System.Drawing.Point(16, 333);
            this.btnFirmar.Name = "btnFirmar";
            this.btnFirmar.Size = new System.Drawing.Size(75, 23);
            this.btnFirmar.TabIndex = 22;
            this.btnFirmar.Text = "Firmar";
            this.btnFirmar.UseVisualStyleBackColor = true;
            this.btnFirmar.Click += new System.EventHandler(this.btnFirmar_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "CSIG|*.csig";
            // 
            // btnValidar
            // 
            this.btnValidar.Location = new System.Drawing.Point(519, 341);
            this.btnValidar.Name = "btnValidar";
            this.btnValidar.Size = new System.Drawing.Size(97, 23);
            this.btnValidar.TabIndex = 41;
            this.btnValidar.Text = "Validar firmar";
            this.btnValidar.UseVisualStyleBackColor = true;
            this.btnValidar.Click += new System.EventHandler(this.btnValidar_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 379);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(145, 13);
            this.label3.TabIndex = 42;
            this.label3.Text = "Huella precalculada (base64)";
            // 
            // txtHuellaPrecalculada
            // 
            this.txtHuellaPrecalculada.Location = new System.Drawing.Point(19, 399);
            this.txtHuellaPrecalculada.Name = "txtHuellaPrecalculada";
            this.txtHuellaPrecalculada.Size = new System.Drawing.Size(433, 20);
            this.txtHuellaPrecalculada.TabIndex = 43;
            // 
            // btnFirmarHuella
            // 
            this.btnFirmarHuella.Location = new System.Drawing.Point(455, 397);
            this.btnFirmarHuella.Name = "btnFirmarHuella";
            this.btnFirmarHuella.Size = new System.Drawing.Size(75, 23);
            this.btnFirmarHuella.TabIndex = 44;
            this.btnFirmarHuella.Text = "Firmar huella";
            this.btnFirmarHuella.UseVisualStyleBackColor = true;
            this.btnFirmarHuella.Click += new System.EventHandler(this.btnFirmarHuella_Click);
            // 
            // FrmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 433);
            this.Controls.Add(this.btnFirmarHuella);
            this.Controls.Add(this.txtHuellaPrecalculada);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnValidar);
            this.Controls.Add(this.cmbAlgoritmo);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnContraFirma);
            this.Controls.Add(this.txtHashPolitica);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtIdentificadorPolitica);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCoFirmar);
            this.Controls.Add(this.btnCargarFirma);
            this.Controls.Add(this.btnGuardarFirma);
            this.Controls.Add(this.btnCadesT);
            this.Controls.Add(this.txtURLSellado);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnFirmar);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test firma CAdES";
            this.Load += new System.EventHandler(this.FrmPrincipal_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSeleccionarFichero;
        private System.Windows.Forms.TextBox txtFichero;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbDetachedExplicit;
        private System.Windows.Forms.RadioButton rbAttachedImplicit;
        private System.Windows.Forms.ComboBox cmbAlgoritmo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnContraFirma;
        private System.Windows.Forms.TextBox txtHashPolitica;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtIdentificadorPolitica;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCoFirmar;
        private System.Windows.Forms.Button btnCargarFirma;
        private System.Windows.Forms.Button btnGuardarFirma;
        private System.Windows.Forms.Button btnCadesT;
        private System.Windows.Forms.TextBox txtURLSellado;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnFirmar;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnValidar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtHuellaPrecalculada;
        private System.Windows.Forms.Button btnFirmarHuella;
    }
}

