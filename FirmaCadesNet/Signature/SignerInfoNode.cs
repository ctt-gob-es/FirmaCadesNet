// --------------------------------------------------------------------------------------------------------------------
// SignerInfoNode.cs
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

using FirmaCadesNet.Crypto;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Asn1.Ess;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using BcCms = Org.BouncyCastle.Asn1.Cms;

namespace FirmaCadesNet.Signature
{
    public class SignerInfoNode
    {
        #region Private variables

        private SignatureDocument _sigDocument;

        private SignerInformation _signerInformation;

        private IList<SignerInfoNode> _counterSignatures;

        private DateTime? _signingDate;

        private IEnumerable<string> _signerRoles;

        private TimeStampToken _timeStamp;

        private X509Certificate _certificate;

        #endregion

        #region Public properties

        public SignerInformation SignerInformation
        {
            get
            {
                return _signerInformation;
            }

            set
            {
                _signerInformation = value;
                ReadInformation();
                _sigDocument.ReBuildCmsSignedData();
            }
        }

        public IList<SignerInfoNode> CounterSignatures
        {
            get
            {
                return _counterSignatures;
            }
        }

        public DateTime? SigningDate
        {
            get
            {
                return _signingDate;
            }
        }

        public IEnumerable<string> SignerRoles
        {
            get
            {
                return _signerRoles;
            }
        }

        public TimeStampToken TimeStamp
        {
            get
            {
                return _timeStamp;
            }
        }

        public X509Certificate Certificate
        {
            get
            {
                return _certificate;
            }
        }

        #endregion

        #region Constructors

        internal SignerInfoNode(SignerInformation signerInformation, SignatureDocument sigDocument)
        {
            _signerInformation = signerInformation;
            _sigDocument = sigDocument;
            ReadInformation();
        }

        #endregion

        #region Private methods

        private void ReadInformation()
        {
            if (_signerInformation.SignedAttributes[PkcsObjectIdentifiers.Pkcs9AtSigningTime] != null)
            {
                _signingDate = DerUtcTime.GetInstance(_signerInformation.SignedAttributes[PkcsObjectIdentifiers.Pkcs9AtSigningTime].AttrValues[0]).ToDateTime().ToLocalTime();
            }

            if (_signerInformation.SignedAttributes[PkcsObjectIdentifiers.IdAAEtsSignerAttr] != null)
            {
                var signerAttr = SignerAttribute.GetInstance(_signerInformation.SignedAttributes[PkcsObjectIdentifiers.IdAAEtsSignerAttr].AttrValues[0]);

                List<string> claimedRoles = new List<string>();

                foreach (BcCms.Attribute claimedAttr in signerAttr.ClaimedAttributes)
                {
                    foreach (var value in claimedAttr.AttrValues)
                    {
                        claimedRoles.Add(DerUtf8String.GetInstance(value).GetString());
                    }
                }

                _signerRoles = claimedRoles;
            }

            if (_signerInformation.UnsignedAttributes != null &&
                _signerInformation.UnsignedAttributes[PkcsObjectIdentifiers.IdAASignatureTimeStampToken] != null)
            {
                _timeStamp = new TimeStampToken(new CmsSignedData(_signerInformation.UnsignedAttributes[PkcsObjectIdentifiers.IdAASignatureTimeStampToken].AttrValues[0].GetEncoded()));
            }

            // Se leen las contrafirmas
            var signers = _signerInformation.GetCounterSignatures().GetSigners();

           _counterSignatures = new List<SignerInfoNode>();

            foreach (var signer in signers)
            {
                SignerInfoNode node = new SignerInfoNode((SignerInformation)signer, _sigDocument);

                _counterSignatures.Add(node);
            }

            // Se intenta identificar el certificado empleado para la firma, esto quizás se pueda mejorar
            byte[] certHash = null;
            IssuerSerial issuerSerial = null;

            if (_signerInformation.DigestAlgOid == DigestMethod.SHA1.Oid)
            {
                BcCms.Attribute attr = _signerInformation.SignedAttributes[PkcsObjectIdentifiers.IdAASigningCertificate];
                SigningCertificate sc = SigningCertificate.GetInstance(attr.AttrValues[0]);                
                EssCertID ecid = sc.GetCerts()[0];
                issuerSerial = ecid.IssuerSerial;
                certHash = ecid.GetCertHash();
            }
            else
            {
                BcCms.Attribute attr = _signerInformation.SignedAttributes[PkcsObjectIdentifiers.IdAASigningCertificateV2];
                SigningCertificateV2 sc2 = SigningCertificateV2.GetInstance(attr.AttrValues[0]);
                EssCertIDv2 ecid = sc2.GetCerts()[0];
                issuerSerial = ecid.IssuerSerial;
                certHash = ecid.GetCertHash();
            }

            DigestMethod digestMethod = DigestMethod.GetByOid(_signerInformation.DigestAlgOid);            

            foreach (X509CertificateStructure cs in _sigDocument.Certificates)
            {
                if (issuerSerial == null || cs.TbsCertificate.SerialNumber.Equals(issuerSerial.Serial))
                {
                    byte[] currentCertHash = digestMethod.CalculateDigest(cs.GetEncoded());

                    if (certHash.SequenceEqual(currentCertHash))
                    {
                        _certificate = new X509Certificate(cs);
                        break;
                    }
                }
            }
        }

        #endregion
    }
}
