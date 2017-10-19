// --------------------------------------------------------------------------------------------------------------------
// SignatureParameters.cs
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
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace FirmaCadesNet.Signature.Parameters
{
    public enum SignaturePackaging
    {
        DETACHED_EXPLICIT,
        ATTACHED_IMPLICIT
    }
   
    public class SignatureParameters
    {
        #region Private variables

        private DigestMethod _defaultDigestMethod = DigestMethod.SHA1;

        #endregion

        #region Public properties

        public Signer Signer { get; set; }

        public X509Certificate Certificate { get; set; }

        public DigestMethod DigestMethod { get; set; }

        public byte[] PreCalculatedDigest { get; set; }

        public DateTime SigningDate { get; set; }

        public string SignerRole { get; set; }

        public List<SignatureCommitment> SignatureCommitments { get; private set; }

        public SignatureProductionPlace SignatureProductionPlace { get; set; }

        public SignaturePolicyInfo SignaturePolicyInfo { get; set; }

        public SignaturePackaging SignaturePackaging { get; set; }

        public string MimeType { get; set; }
        
        #endregion

        #region Constructors

        public SignatureParameters()
        {
            this.DigestMethod = _defaultDigestMethod;
            this.SignatureCommitments = new List<SignatureCommitment>();
            this.SigningDate = DateTime.Now;
        }

        #endregion
    }
}
