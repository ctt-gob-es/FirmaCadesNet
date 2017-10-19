// --------------------------------------------------------------------------------------------------------------------
// SignatureDocument.cs
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

using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Cms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections;
using BcCms = Org.BouncyCastle.Asn1.Cms;
using FirmaCadesNet.Signature.Parameters;

namespace FirmaCadesNet.Signature
{
    public class SignatureDocument
    {
        #region Private variables

        private CmsSignedData _signedData;

        private IList<SignerInfoNode> _nodes;

        private IList _certs;

        private SignaturePackaging _signaturePackaging;

        #endregion

        #region Public properties

        public CmsSignedData SignedData
        {
            get
            {
                return _signedData;
            }

            set
            {
                _signedData = value;
            }
        }

        public Stream Content
        {
            get
            {
                if (_signedData.SignedContent != null)
                {
                    MemoryStream ms = new MemoryStream();
                    _signedData.SignedContent.Write(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    return ms;
                }
                else
                {
                    return null;
                }
            }
        }

        public SignaturePackaging SignaturePackaging
        {
            get
            {
                return _signaturePackaging;
            }
        }

        public IList<SignerInfoNode> SignersInfo
        {
            get
            {
                return _nodes;
            }
        }

        public IList Certificates
        {
            get
            {
                return _certs;
            }
        }

        #endregion

        #region Constructors

        public SignatureDocument(CmsSignedData signedData)
        {
            _signedData = signedData;
            _certs = CmsUtilities.GetCertificatesFromStore(_signedData.GetCertificates("Collection"));
            _signaturePackaging = _signedData.SignedContent != null ? SignaturePackaging.ATTACHED_IMPLICIT : SignaturePackaging.DETACHED_EXPLICIT;
            ReadSignersInfo();
        }

        #endregion

        #region Public methods

        public byte[] GetDocumentBytes()
        {
            return _signedData.GetEncoded();
        }

        public void Save(Stream output)
        {
            byte[] encoded = _signedData.GetEncoded();

            output.Write(encoded, 0, encoded.Length);
        }

        #endregion

        #region Private methods

        internal void ReBuildCmsSignedData()
        {
            IList<SignerInformation> list = new List<SignerInformation>();

            foreach (var node in _nodes)
            {
                list.Add(GetSignerInformation(node));
            }

            _signedData = CmsSignedData.ReplaceSigners(_signedData, new SignerInformationStore(list.ToArray()));
            ReadSignersInfo();
        }

        private void ReadSignersInfo()
        {
            _nodes = new List<SignerInfoNode>();

            foreach (var signer in _signedData.GetSignerInfos().GetSigners())
            {
                SignerInfoNode node = new SignerInfoNode((SignerInformation)signer, this);

                _nodes.Add(node);
            }
        }

        private SignerInformation GetSignerInformation(SignerInfoNode signerInfoNode)
        {
            if (signerInfoNode.CounterSignatures.Count > 0)
            {
                var nodes = GetCounterSignatures(signerInfoNode);

                BcCms.AttributeTable attributes = signerInfoNode.SignerInformation.UnsignedAttributes.Remove(CmsAttributes.CounterSignature);

                SignerInformation newSignerInformation = SignerInformation.ReplaceUnsignedAttributes(signerInfoNode.SignerInformation, attributes);

                return SignerInformation.AddCounterSigners(newSignerInformation, new SignerInformationStore(nodes.ToArray()));
            }
            else
            {
                return signerInfoNode.SignerInformation;
            }
        }

        private IList<SignerInformation> GetCounterSignatures(SignerInfoNode node)
        {
            List<SignerInformation> list = new List<SignerInformation>();

            foreach (var counterSignNode in node.CounterSignatures)
            {
                if (counterSignNode.CounterSignatures.Count > 0)
                {
                    var nodes = GetCounterSignatures(counterSignNode);

                    BcCms.AttributeTable attributes = counterSignNode.SignerInformation.UnsignedAttributes.Remove(CmsAttributes.CounterSignature);

                    SignerInformation newSignerInformation = SignerInformation.ReplaceUnsignedAttributes(counterSignNode.SignerInformation, attributes);

                    list.Add(SignerInformation.AddCounterSigners(newSignerInformation, new SignerInformationStore(nodes.ToArray())));
                }
                else
                {
                    list.Add(counterSignNode.SignerInformation);
                }
            }

            return list;
        }

        #endregion
    }
}

