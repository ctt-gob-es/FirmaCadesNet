// --------------------------------------------------------------------------------------------------------------------
// CadesTUpgrader.cs
//
// FirmaCadesNet - Librería para la generación de firmas CAdES
// Copyright (C) 2017 Dpto. de Nuevas Tecnologías de la Dirección General de Urbanismo del Ayto. de Cartagena
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/. 
//
// E-Mail: informatica@gemuc.es
// 
// --------------------------------------------------------------------------------------------------------------------

using FirmaCadesNet.Clients;
using FirmaCadesNet.Crypto;
using FirmaCadesNet.Signature;
using FirmaCadesNet.Upgraders.Parameters;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Cms;
using System.Collections;
using System.Collections.Generic;
using BcCms = Org.BouncyCastle.Asn1.Cms;

namespace FirmaCadesNet.Upgraders
{
    public class CadesTUpgrader : ICadesUpgrader
    {
        #region Public methods

        public void Upgrade(SignatureDocument signatureDocument, SignerInfoNode signerInfoNode, UpgradeParameters parameters)
        {
            BcCms.AttributeTable unsigned = signerInfoNode.SignerInformation.UnsignedAttributes;         
            IDictionary unsignedAttrHash = null;

            if (unsigned == null)
            {
                unsignedAttrHash = new Dictionary<DerObjectIdentifier, BcCms.Attribute>();
            }
            else
            {
                unsignedAttrHash = signerInfoNode.SignerInformation.UnsignedAttributes.ToDictionary();
            }

            BcCms.Attribute signatureTimeStamp = GetTimeStampAttribute(PkcsObjectIdentifiers.IdAASignatureTimeStampToken
                , parameters.TsaClient, parameters.DigestMethod, signerInfoNode.SignerInformation.GetSignature());
            
            unsignedAttrHash.Add(PkcsObjectIdentifiers.IdAASignatureTimeStampToken, signatureTimeStamp);

            SignerInformation newsi = SignerInformation.ReplaceUnsignedAttributes(signerInfoNode.SignerInformation, 
                new BcCms.AttributeTable(unsignedAttrHash));

            signerInfoNode.SignerInformation = newsi;
        }

        #endregion

        #region Private methods

        private BcCms.Attribute GetTimeStampAttribute(DerObjectIdentifier oid
                    , TimeStampClient tsa, DigestMethod digestMethod, byte[] messageImprint)
        {
            byte[] toTimeStamp = digestMethod.CalculateDigest(messageImprint);
            byte[] timeStampToken = tsa.GetTimeStamp(toTimeStamp, digestMethod, true);

            BcCms.Attribute signatureTimeStamp = new BcCms.Attribute(oid, new DerSet(Asn1Object.FromByteArray
                (timeStampToken)));

            return signatureTimeStamp;
        }

        #endregion
    }
}
