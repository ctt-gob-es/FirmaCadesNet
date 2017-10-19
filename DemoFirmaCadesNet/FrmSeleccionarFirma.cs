using FirmaCadesNet.Signature;
using FirmaCadesNet.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoFirmaCadesNet
{
    public partial class FrmSeleccionarFirma : Form
    {
        private SignatureDocument _signatureDocument;

        public SignerInfoNode SignerInfo
        {
            get
            {
                return (SignerInfoNode)treeView1.SelectedNode.Tag;
            }
        }

        public FrmSeleccionarFirma(SignatureDocument sigDocument)
        {
            InitializeComponent();

            _signatureDocument = sigDocument;

            foreach (var infoNode in sigDocument.SignersInfo)
            {
                AddInfoNode(infoNode, treeView1.Nodes[0]);
            }
        }

        private void AddInfoNode(SignerInfoNode infoNode, TreeNode parentNode)
        {
            TreeNode newNode = new TreeNode(infoNode.Certificate.SubjectDN.ToString());
            newNode.Tag = infoNode;

            foreach (var counterInfoNode in infoNode.CounterSignatures)
            {
                AddInfoNode(counterInfoNode, newNode);
            }

            parentNode.Nodes.Add(newNode);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null ||
                treeView1.SelectedNode.Tag == null)
            {
                MessageBox.Show("Por favor, seleccione una firma");
                DialogResult = System.Windows.Forms.DialogResult.None;
            }
        }
    }
}
