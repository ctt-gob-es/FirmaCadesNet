using FirmaCadesNet.Signature;
using FirmaCadesNet.Upgraders.Parameters;
using Org.BouncyCastle.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmaCadesNet.Upgraders
{
    interface ICadesUpgrader
    {
        void Upgrade(SignatureDocument signatureDocument, SignerInfoNode signerInfoNode, UpgradeParameters parameters);
    }
}
