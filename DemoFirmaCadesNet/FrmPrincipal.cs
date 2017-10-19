using FirmaCadesNet.Clients;
using FirmaCadesNet;
using FirmaCadesNet.Crypto;
using FirmaCadesNet.Signature;
using FirmaCadesNet.Signature.Parameters;
using FirmaCadesNet.Upgraders;
using FirmaCadesNet.Upgraders.Parameters;
using FirmaCadesNet.Utils;
using FirmaCadesNet.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoFirmaCadesNet
{
    public partial class FrmPrincipal : Form
    {
        SignatureDocument _signatureDocument;

        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            cmbAlgoritmo.SelectedIndex = 0;
        }

        private void btnSeleccionarFichero_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtFichero.Text = openFileDialog1.FileName;
            }
        }

        private SignaturePolicyInfo ObtenerPolitica()
        {
            if (string.IsNullOrEmpty(txtIdentificadorPolitica.Text))
            {
                return null;
            }
            
            SignaturePolicyInfo spi = new SignaturePolicyInfo();

            spi.PolicyIdentifier = txtIdentificadorPolitica.Text;
            spi.PolicyHash = txtHashPolitica.Text;

            return spi;
        }

        private DigestMethod ObtenerAlgoritmo()
        {
            if (cmbAlgoritmo.SelectedIndex == 0)
            {
                return DigestMethod.SHA1;
            }
            else if (cmbAlgoritmo.SelectedIndex == 1)
            {
                return DigestMethod.SHA256;
            }
            else
            {
                return DigestMethod.SHA512;
            }
        }

        private SignatureParameters ObtenerParametrosFirma()
        {
            SignatureParameters parametros = new SignatureParameters();
            parametros.DigestMethod = ObtenerAlgoritmo();
            parametros.SigningDate = DateTime.Now;
            parametros.SignaturePolicyInfo = ObtenerPolitica();

            return parametros;
        }

        private void btnFirmar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFichero.Text))
            {
                MessageBox.Show("Debe seleccionar un fichero para firmar.");
                return;
            }

            CadesService cs = new CadesService();
            SignatureParameters parametros = ObtenerParametrosFirma();
            parametros.Certificate = CertUtil.SelectCertificate();
            parametros.MimeType = MimeTypeInfo.GetMimeType(txtFichero.Text);

            if (rbAttachedImplicit.Checked)
            {
                parametros.SignaturePackaging = SignaturePackaging.ATTACHED_IMPLICIT;
            }
            else
            {
                parametros.SignaturePackaging = SignaturePackaging.DETACHED_EXPLICIT;
            }

            using (parametros.Signer = new Signer((X509Certificate2)parametros.Certificate))
            {
                using (FileStream fs = new FileStream(txtFichero.Text, FileMode.Open))
                {
                    _signatureDocument = cs.Sign(fs, parametros);
                }
            }

            MessageBox.Show("Firma completada, ahora puede Guardar la firma o ampliarla a CAdES-T.", "Test firma CAdES",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GuardarFirma()
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                {
                    _signatureDocument.Save(fs);
                }

                MessageBox.Show("Firma guardada correctamente.");
            }
        }

        private void btnGuardarFirma_Click(object sender, EventArgs e)
        {
            if (_signatureDocument != null)
            {
                GuardarFirma();
            }
        }

        private void btnCargarFirma_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open))
                {
                    CadesService cs = new CadesService();

                    _signatureDocument = cs.Load(fs);
                }
            }
        }

        private void btnCoFirmar_Click(object sender, EventArgs e)
        {
            if (_signatureDocument == null)
                return;

            SignatureParameters parametros = ObtenerParametrosFirma();
            parametros.Certificate = CertUtil.SelectCertificate();
            parametros.SignaturePackaging = _signatureDocument.SignaturePackaging;

            using (parametros.Signer = new Signer((X509Certificate2)parametros.Certificate))
            {
                CadesService cs = new CadesService();

                _signatureDocument = cs.CoSign(_signatureDocument, parametros);
            }

            MessageBox.Show("Firma completada, ahora puede Guardar la firma o ampliarla a CAdES-T.", "Test firma CAdES",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnContraFirma_Click(object sender, EventArgs e)
        {
            if (_signatureDocument == null)
                return;

            FrmSeleccionarFirma frm = new FrmSeleccionarFirma(_signatureDocument);
            if (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {

                MessageBox.Show("Debe seleccionar una firma");
                return;
            }

            SignatureParameters parametros = ObtenerParametrosFirma();
            parametros.SignaturePolicyInfo = null;
            parametros.Certificate = CertUtil.SelectCertificate();

            using (parametros.Signer = new Signer((X509Certificate2)parametros.Certificate))
            {
                CadesService cs = new CadesService();

                _signatureDocument = cs.CounterSign(_signatureDocument, frm.SignerInfo, parametros);
            }

            MessageBox.Show("Firma completada, ahora puede Guardar la firma o ampliarla a CAdES-T.", "Test firma CAdES",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnValidar_Click(object sender, EventArgs e)
        {
            if (_signatureDocument == null)
                return;

            FrmSeleccionarFirma frm = new FrmSeleccionarFirma(_signatureDocument);
            if (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("Debe seleccionar una firma");
                return;
            }

            CadesValidator cv = new CadesValidator();

            var result = cv.Validate(_signatureDocument, frm.SignerInfo);

            if (result.IsValid)
            {
                MessageBox.Show("Firma válida");
            }
            else
            {
                MessageBox.Show("La verificación no ha sido satisfactoria: " + result.Message);
            }
        }

        private void btnCadesT_Click(object sender, EventArgs e)
        {
            if (_signatureDocument == null)
                return;

            FrmSeleccionarFirma frm = new FrmSeleccionarFirma(_signatureDocument);
            if (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("Debe seleccionar una firma");
                return;
            }

            UpgradeParameters up = new UpgradeParameters();
            up.TsaClient = new TimeStampClient(txtURLSellado.Text);
            up.DigestMethod = DigestMethod.SHA256;

            CadesTUpgrader upgrader = new CadesTUpgrader();
            upgrader.Upgrade(_signatureDocument, frm.SignerInfo, up);

            MessageBox.Show("Firma ampliada correctamente");
        }

        private void btnFirmarHuella_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtHuellaPrecalculada.Text))
            {
                MessageBox.Show("Debe especificar el valor de huella a firmar");
                return;
            }

            SignatureParameters parametros = ObtenerParametrosFirma();

            byte[] digestValue = Convert.FromBase64String(txtHuellaPrecalculada.Text);

            if ((parametros.DigestMethod == DigestMethod.SHA1 &&
                digestValue.Length != 20) ||
                (parametros.DigestMethod == DigestMethod.SHA256 &&
                digestValue.Length != 32) ||
                (parametros.DigestMethod == DigestMethod.SHA512 &&
                digestValue.Length != 64))
            {
                MessageBox.Show("La longitud del valor de la huella no coincide con el algoritmo de huella seleccionado.");
                return;
            }

            parametros.Certificate = CertUtil.SelectCertificate();
            parametros.SignaturePolicyInfo = null;
            parametros.PreCalculatedDigest = digestValue;

            CadesService cs = new CadesService();

            using (parametros.Signer = new Signer((X509Certificate2)parametros.Certificate))
            {
                _signatureDocument = cs.Sign(null, parametros);
            }

            MessageBox.Show("Firma completada, ahora puede Guardar la firma o ampliarla a CAdES-T.", "Test firma CAdES",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
