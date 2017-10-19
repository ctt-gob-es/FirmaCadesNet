// --------------------------------------------------------------------------------------------------------------------
// CadesService.cs
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

using crypto.src.crypto.signers;
using FirmaCadesNet.Crypto;
using FirmaCadesNet.Signature;
using FirmaCadesNet.Signature.Parameters;
using FirmaCadesNet.Util;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Asn1.Ess;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BcCms = Org.BouncyCastle.Asn1.Cms;

namespace FirmaCadesNet
{
    public class CadesService
    {
        #region Public methods

        /// <summary>
        /// Realiza la firma del contenido de entrada. También acepta el valor de la huella del contenido de entrada
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SignatureDocument Sign(Stream input, SignatureParameters parameters)
        {
            if (input == null && parameters.PreCalculatedDigest == null)
            {
                throw new Exception("Se necesita especificar el contenido a firmar");
            }

            CheckParameters(parameters);

            return ComputeSignature(input, parameters, null);
        }

        /// <summary>
        /// Aplica una cofirma a una firma CAdES ya existente
        /// </summary>
        /// <param name="sigDocument"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SignatureDocument CoSign(SignatureDocument sigDocument, SignatureParameters parameters)
        {
            if (sigDocument == null)
            {
                throw new Exception("Se necesita una firma previa para poder realizar la cofirma");
            }

            CheckParameters(parameters);

            return ComputeSignature(sigDocument.Content, parameters, sigDocument.SignedData);
        }

        /// <summary>
        /// Realiza la contrafirma de una firma CAdES existente
        /// </summary>
        /// <param name="sigDocument"></param>
        /// <param name="signerInfoNode"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SignatureDocument CounterSign(SignatureDocument sigDocument, SignerInfoNode signerInfoNode, SignatureParameters parameters)
        {
            if (sigDocument == null)
            {
                throw new Exception("Se necesita una firma previa para poder realizar la cofirma");
            }

            if (signerInfoNode == null)
            {
                throw new Exception("Se necesita especificar el nodo de firma para aplicar la contrafirma");
            }

            CheckParameters(parameters);

            byte[] signature = null;

            using (MemoryStream ms = new MemoryStream(signerInfoNode.SignerInformation.GetSignature()))
            {
                byte[] toBeSigned = ToBeSigned(new CmsProcessableInputStream(ms), parameters, null, true);
                signature = parameters.Signer.SignData(toBeSigned, parameters.DigestMethod);
            }

            CustomCMSSignedDataGenerator generator = CreateSignedGenerator(new PreComputedSigner(signature), parameters, null);

            var result = generator.GenerateCounterSigners(signerInfoNode.SignerInformation);

            SignerInformation updatedSI = SignerInformation.AddCounterSigners(signerInfoNode.SignerInformation, result);

            List<X509Certificate> certs = new List<X509Certificate>();
            IX509Store originalCertStore = sigDocument.SignedData.GetCertificates("Collection");

            signerInfoNode.SignerInformation = updatedSI;

            CollectionUtilities.AddRange(certs, GetCertificatesFromStore(originalCertStore));

            X509CertificateParser parser = new X509CertificateParser();
            var signerCertificate = parser.ReadCertificate(parameters.Certificate.GetRawCertData());

            if (!CheckCertExists(signerCertificate, originalCertStore))
            {
                certs.Add(signerCertificate);
            }

            IX509Store certStore = X509StoreFactory.Create("Certificate/Collection", new X509CollectionStoreParameters(certs));

            CmsSignedData newSignedData = CmsSignedData.ReplaceCertificatesAndCrls(sigDocument.SignedData, certStore, sigDocument.SignedData.GetCrls("Collection"), null);

            return new SignatureDocument(newSignedData);
        }

        public SignatureDocument Load(Stream input)
        {
            return new SignatureDocument(new CmsSignedData(input));
        }

        public SignatureDocument Load(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                return Load(fs);
            }
        }

        #endregion

        #region Private methods

        private void CheckParameters(SignatureParameters parameters)
        {
            if (parameters == null)
            {
                throw new Exception("Los parámetros para generar la firma son obligatorios");
            }

            if (parameters.Signer == null)
            {
                throw new Exception("No se ha especificado ningún proveedor para generar la firma");
            }

            if (parameters.Certificate == null)
            {
                throw new Exception("No se ha especificado el certificado empleado para la firma");
            }
        }

        /// <summary>
        /// Método para crear el generador de firmas
        /// </summary>
        /// <param name="signerProvider"></param>
        /// <param name="parameters"></param>
        /// <param name="originalSignedData"></param>
        /// <returns></returns>
        private CustomCMSSignedDataGenerator CreateSignedGenerator(ISigner signerProvider,
            SignatureParameters parameters, CmsSignedData originalSignedData)
        {
            X509CertificateParser parser = new X509CertificateParser();
            var signerCertificate = parser.ReadCertificate(parameters.Certificate.GetRawCertData());

            CustomCMSSignedDataGenerator generator = new CustomCMSSignedDataGenerator();

            Dictionary<DerObjectIdentifier, Asn1Encodable> signedAttrDic = GetSignedAttributes(parameters);

            if (!signedAttrDic.ContainsKey(PkcsObjectIdentifiers.IdAAContentHint) &&
                originalSignedData != null)
            {
                var attrContentHint = GetContentHintAttribute(originalSignedData.GetSignerInfos());

                if (attrContentHint != null)
                {
                    signedAttrDic.Add(PkcsObjectIdentifiers.IdAAContentHint, attrContentHint);
                }
            }

            CmsAttributeTableGenerator signedAttrGen = new DefaultSignedAttributeTableGenerator
                    (new Org.BouncyCastle.Asn1.Cms.AttributeTable(signedAttrDic));

            generator.SignerProvider = signerProvider;
            generator.AddSigner(new NullPrivateKey(), signerCertificate,
                PkcsObjectIdentifiers.RsaEncryption.Id, parameters.DigestMethod.Oid, signedAttrGen, null);

            if (originalSignedData != null)
            {
                generator.AddSigners(originalSignedData.GetSignerInfos());
            }

            bool addSignerCert = true;

            if (originalSignedData != null)
            {
                IX509Store originalCertStore = originalSignedData.GetCertificates("Collection");

                generator.AddCertificates(originalCertStore);

                addSignerCert = !CheckCertExists(signerCertificate, originalCertStore);
            }

            if (addSignerCert)
            {
                List<X509Certificate> certs = new List<X509Certificate>();
                certs.Add(signerCertificate);

                IX509Store certStore = X509StoreFactory.Create("Certificate/Collection", new X509CollectionStoreParameters(certs));
                generator.AddCertificates(certStore);
            }

            return generator;
        }

        /// <summary>
        /// Devuelve una lista con los certificados contenidos en un almacén de certificados
        /// </summary>
        /// <param name="certStore"></param>
        /// <returns></returns>
        private IList GetCertificatesFromStore(IX509Store certStore)
        {
            try
            {
                IList certs = new List<object>();

                if (certStore != null)
                {
                    foreach (X509Certificate c in certStore.GetMatches(null))
                    {
                        certs.Add(c);
                    }
                }

                return certs;
            }
            catch (CertificateEncodingException e)
            {
                throw new CmsException("error encoding certs", e);
            }
            catch (Exception e)
            {
                throw new CmsException("error processing certs", e);
            }
        }

        /// <summary>
        /// Comprueba si un certificado ya existe en un almacén dado
        /// </summary>
        /// <param name="cert"></param>
        /// <param name="certStore"></param>
        /// <returns></returns>
        private bool CheckCertExists(X509Certificate cert, IX509Store certStore)
        {
            X509CertStoreSelector selector = new X509CertStoreSelector();
            selector.Certificate = cert;
            ICollection result = certStore.GetMatches(selector);

            if (result == null)
            {
                return false;
            }
            else
            {
                return result.Count > 0;
            }
        }

        /// <summary>
        /// Método para crear el atributo que contiene la información del certificado empleado para la firma
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private BcCms.Attribute MakeSigningCertificateAttribute(SignatureParameters parameters)
        {
            X509Certificate certificate = new X509CertificateParser().ReadCertificate(parameters.Certificate.GetRawCertData());
            TbsCertificateStructure tbs = TbsCertificateStructure.GetInstance(
                                Asn1Object.FromByteArray(
                                certificate.GetTbsCertificate()));
            GeneralName gn = new GeneralName(tbs.Issuer);
            GeneralNames gns = new GeneralNames(gn);
            IssuerSerial issuerSerial = new IssuerSerial(gns, tbs.SerialNumber);

            byte[] certHash = DigestUtilities.CalculateDigest(parameters.DigestMethod.Name, certificate.GetEncoded());

            var policies = GetPolicyInformation(certificate);

            if (parameters.DigestMethod == DigestMethod.SHA1)
            {
                SigningCertificate sc = null;

                if (policies != null)
                {
                    Asn1EncodableVector v = new Asn1EncodableVector();
                    v.Add(new DerSequence(new EssCertID(certHash, issuerSerial)));
                    v.Add(new DerSequence(policies));
                    sc = SigningCertificate.GetInstance(new DerSequence(v));
                }
                else
                {
                    sc = new SigningCertificate(new EssCertID(certHash, issuerSerial));
                }

                return new BcCms.Attribute(PkcsObjectIdentifiers.IdAASigningCertificate, new DerSet(sc));
            }
            else
            {
                EssCertIDv2 essCert = new EssCertIDv2(new AlgorithmIdentifier(parameters.DigestMethod
                    .Oid), certHash, issuerSerial);

                SigningCertificateV2 scv2 = new SigningCertificateV2(new EssCertIDv2[] { essCert }, policies);

                return new BcCms.Attribute(PkcsObjectIdentifiers.IdAASigningCertificateV2, new DerSet
                    (scv2));
            }
        }

        /// <summary>
        /// Devuelve la información de las políticas de un certificado
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        private PolicyInformation[] GetPolicyInformation(X509Certificate cert)
        {
            byte[] certPolicies = cert.GetExtensionValue("2.5.29.32").GetOctets();

            return CertificatePolicies.GetInstance(certPolicies).GetPolicyInformation();
        }


        /// <summary>
        /// Método para crear el atributo que contiene la fecha de firma
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private BcCms.Attribute MakeSigningTimeAttribute(SignatureParameters parameters)
        {
            return new BcCms.Attribute(PkcsObjectIdentifiers.Pkcs9AtSigningTime, new DerSet(new
                DerUtcTime(parameters.SigningDate.ToUniversalTime())));
        }

        /// <summary>
        /// Método para crear el atributo que contiene el rol del firmante
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private BcCms.Attribute MakeSignerAttrAttribute(SignatureParameters parameters)
        {
            DerUtf8String[] roles = new DerUtf8String[1];
            roles[0] = new DerUtf8String(parameters.SignerRole);

            BcCms.Attribute claimedRolesAttr = new BcCms.Attribute(X509ObjectIdentifiers.id_at_name, new DerSet(roles));

            return new BcCms.Attribute(PkcsObjectIdentifiers.IdAAEtsSignerAttr, new DerSet(new SignerAttribute
                (new DerSequence(claimedRolesAttr))));
        }

        /// <summary>
        /// Método para crear el atributo que contiene la información sobre la localización
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private BcCms.Attribute MakeSignerLocationAttribute(SignatureParameters parameters)
        {
            List<Asn1Encodable> postalAddressList = null;

            if (!string.IsNullOrEmpty(parameters.SignatureProductionPlace.PostalCode))
            {
                postalAddressList = new List<Asn1Encodable>();
                postalAddressList.Add(new DerUtf8String(parameters.SignatureProductionPlace.PostalCode));
            }

            SignerLocation sigLocation = new SignerLocation(
                !string.IsNullOrEmpty(parameters.SignatureProductionPlace.CountryName) ? new DerUtf8String(parameters.SignatureProductionPlace.CountryName) : null,
                !string.IsNullOrEmpty(parameters.SignatureProductionPlace.City) ? new DerUtf8String(parameters.SignatureProductionPlace.City) : null,
                postalAddressList != null ? new DerSequence(postalAddressList.ToArray()) : null
                );

            return new BcCms.Attribute(PkcsObjectIdentifiers.IdAAEtsSignerLocation, new DerSet(sigLocation));
        }

        /// <summary>
        /// Método para crear el atributo que contiene la información sobre la acción del firmante sobre el documento firmado
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private IEnumerable<BcCms.Attribute> MakeCommitmentTypeIndicationAttributes(SignatureParameters parameters)
        {
            List<Asn1Encodable> commitments = new List<Asn1Encodable>();

            foreach (var commitmentType in parameters.SignatureCommitments)
            {
                List<Asn1Encodable> qualifiers = new List<Asn1Encodable>();

                foreach (var qualifier in commitmentType.CommitmentTypeQualifiers)
                {
                    qualifiers.Add(new DerObjectIdentifier(qualifier));
                }

                commitments.Add(new CommitmentTypeIndication(commitmentType.CommitmentType.Oid, new DerSequence(qualifiers.ToArray())));
            }

            List<BcCms.Attribute> attributes = new List<BcCms.Attribute>();
            foreach (var commitmentType in commitments)
            {
                attributes.Add(new BcCms.Attribute(PkcsObjectIdentifiers.IdAAEtsCommitmentType, new DerSet(commitmentType)));
            }

            return attributes;
        }

        /// <summary>
        /// Método para crear el atributo que contiene la información sobre la politica de firma
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private BcCms.Attribute MakeSignaturePolicyAttribute(SignatureParameters parameters)
        {
            SignaturePolicyIdentifier sigPolicy = new SignaturePolicyIdentifier(new SignaturePolicyId(new DerObjectIdentifier
(parameters.SignaturePolicyInfo.PolicyIdentifier), new OtherHashAlgAndValue(new AlgorithmIdentifier(parameters.SignaturePolicyInfo.PolicyDigestAlgorithm.Oid),
   new DerOctetString(System.Convert.FromBase64String(parameters.SignaturePolicyInfo.PolicyHash)))));
            return new BcCms.Attribute(PkcsObjectIdentifiers.IdAAEtsSigPolicyID, new DerSet(sigPolicy));
        }

        /// <summary>
        /// Método que devuelve los atributos que deberán ser firmados
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private Dictionary<DerObjectIdentifier, Asn1Encodable> GetSignedAttributes(SignatureParameters parameters)
        {
            Dictionary<DerObjectIdentifier, Asn1Encodable> signedAttrs = new Dictionary<DerObjectIdentifier, Asn1Encodable>();

            BcCms.Attribute signingCertificateReference = MakeSigningCertificateAttribute(parameters);

            signedAttrs.Add((DerObjectIdentifier)signingCertificateReference.AttrType,
                signingCertificateReference);

            signedAttrs.Add(PkcsObjectIdentifiers.Pkcs9AtSigningTime, MakeSigningTimeAttribute
                (parameters));

            if (parameters.SignaturePolicyInfo != null)
            {
                signedAttrs.Add(PkcsObjectIdentifiers.IdAAEtsSigPolicyID, MakeSignaturePolicyAttribute(parameters));
            }

            if (!string.IsNullOrEmpty(parameters.SignerRole))
            {
                signedAttrs.Add(PkcsObjectIdentifiers.IdAAEtsSignerAttr, MakeSignerAttrAttribute(parameters));
            }

            if (parameters.SignatureProductionPlace != null)
            {
                signedAttrs.Add(PkcsObjectIdentifiers.IdAAEtsSignerLocation, MakeSignerLocationAttribute(parameters));
            }

            if (parameters.SignatureCommitments.Count > 0)
            {
                var commitments = MakeCommitmentTypeIndicationAttributes(parameters);

                foreach (var item in commitments)
                {
                    signedAttrs.Add(PkcsObjectIdentifiers.IdAAEtsCommitmentType, item);
                }
            }

            if (!string.IsNullOrEmpty(parameters.MimeType))
            {
                ContentHints contentHints = new ContentHints(new DerObjectIdentifier(MimeTypeHelper.GetMimeTypeOid(parameters.MimeType)));

                BcCms.Attribute contentAttr = new BcCms.Attribute(PkcsObjectIdentifiers.IdAAContentHint, new DerSet(contentHints));
                signedAttrs.Add(PkcsObjectIdentifiers.IdAAContentHint, contentAttr);
            }

            return signedAttrs;
        }

        /// <summary>
        /// Devuelve los datos finales que deberán ser firmados
        /// </summary>
        /// <param name="content"></param>
        /// <param name="parameters"></param>
        /// <param name="signedData"></param>
        /// <param name="isCounterSignature"></param>
        /// <returns></returns>
        private byte[] ToBeSigned(CmsProcessable content, SignatureParameters parameters, CmsSignedData signedData, bool isCounterSignature)
        {
            PreComputedSigner preComputedSigner = new PreComputedSigner();
            CustomCMSSignedDataGenerator generator = CreateSignedGenerator(preComputedSigner, parameters, signedData);

            if (parameters.PreCalculatedDigest != null)
            {
                generator.PreCalculatedDigest = parameters.PreCalculatedDigest;
            }
            else if (content == null)
            {
                // Si el contenido es nulo se intenta buscar el valor de la huella del contenido en las otras firmas
                generator.PreCalculatedDigest = GetDigestValue(signedData.GetSignerInfos(), parameters.DigestMethod);

                if (generator.PreCalculatedDigest == null)
                {
                    throw new Exception("No se ha podido obtener la huella del contenido");
                }
            }

            generator.PreGenerate(!isCounterSignature ? CmsObjectIdentifiers.Data.Id : null, content);

            return preComputedSigner.CurrentSignature();
        }

        /// <summary>
        /// Método que busca en las demás firmas el message-digest que coincida con el algoritmo de huella dado 
        /// </summary>
        /// <param name="siStore"></param>
        /// <param name="digestMethod"></param>
        /// <returns></returns>
        private byte[] GetDigestValue(SignerInformationStore siStore, DigestMethod digestMethod)
        {
            var signers = siStore.GetSigners();

            foreach (SignerInformation signerInfo in signers)
            {
                if (signerInfo.DigestAlgOid == digestMethod.Oid)
                {
                    BcCms.Attribute digest = signerInfo.SignedAttributes[PkcsObjectIdentifiers.Pkcs9AtMessageDigest];
                    DerOctetString derHash = (DerOctetString)digest.AttrValues[0];

                    return derHash.GetOctets();
                }
            }

            return null;
        }

        /// <summary>
        /// Método que busca en las demás firmas el tipo de contenido firmado
        /// </summary>
        /// <param name="siStore"></param>
        /// <returns></returns>
        private BcCms.Attribute GetContentHintAttribute(SignerInformationStore siStore)
        {
            var signers = siStore.GetSigners();

            foreach (SignerInformation signerInfo in signers)
            {
                BcCms.Attribute contentHint = signerInfo.SignedAttributes[PkcsObjectIdentifiers.IdAAContentHint];

                if (contentHint != null)
                {
                    return contentHint;
                }
            }

            return null;
        }

        /// <summary>
        /// Método que realiza el proceso de firmado
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parameters"></param>
        /// <param name="signedData"></param>
        /// <returns></returns>
        private SignatureDocument ComputeSignature(Stream input, SignatureParameters parameters, CmsSignedData signedData)
        {
            CmsProcessableByteArray content = null;

            if (input != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    input.CopyTo(ms);
                    content = new CmsProcessableByteArray(ms.ToArray());
                }
            }

            byte[] toBeSigned = ToBeSigned(content, parameters, signedData, false);
            byte[] signature = parameters.Signer.SignData(toBeSigned, parameters.DigestMethod);

            PreComputedSigner preComputedSigner = new PreComputedSigner(signature);
            CustomCMSSignedDataGenerator generator = CreateSignedGenerator(preComputedSigner, parameters, signedData);
            CmsSignedData newSignedData = null;

            if (parameters.SignaturePackaging == SignaturePackaging.ATTACHED_IMPLICIT && parameters.PreCalculatedDigest == null)
            {
                newSignedData = generator.Generate(content, true);
            }
            else
            {
                if (parameters.PreCalculatedDigest != null)
                {
                    generator.PreCalculatedDigest = parameters.PreCalculatedDigest;

                    newSignedData = generator.Generate(null, false);
                }
                else if (content != null)
                {
                    newSignedData = generator.Generate(content, false);
                }
                else
                {
                    generator.PreCalculatedDigest = GetDigestValue(signedData.GetSignerInfos(), parameters.DigestMethod);

                    newSignedData = generator.Generate(null, false);
                }
            }

            return new SignatureDocument(new CmsSignedData(newSignedData.GetEncoded()));
        }

        #endregion
    }
}
