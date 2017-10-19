﻿// --------------------------------------------------------------------------------------------------------------------
// Signer.cs
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

using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace FirmaCadesNet.Crypto
{
    public class Signer : IDisposable
    {
        #region Private variables

        private bool _disposeCryptoProvider;
        private RSACryptoServiceProvider _cryptoProvider;

        #endregion

        #region Constructors

        public Signer(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }
            
            if (!certificate.HasPrivateKey)
            {
                throw new Exception("El certificado no contiene ninguna clave privada");
            }

            SetSigningKey(certificate);
        }

        #endregion

        #region Public methods

        public byte[] SignData(byte[] data, DigestMethod digestMethod)
        {
            using (var hashAlg = digestMethod.GetHashAlgorithm())
            {
                return _cryptoProvider.SignData(data, hashAlg);
            }            
        }
        
        public void Dispose()
        {
            if (_disposeCryptoProvider && _cryptoProvider != null)
            {
                _cryptoProvider.Dispose();
            }
        }

        #endregion

        #region Private methods

        private void SetSigningKey(X509Certificate2 certificate)
        {
            var key = (RSACryptoServiceProvider)certificate.PrivateKey;

            if (key.CspKeyContainerInfo.ProviderName == CryptoConst.MS_STRONG_PROV ||
                key.CspKeyContainerInfo.ProviderName == CryptoConst.MS_ENHANCED_PROV ||
                key.CspKeyContainerInfo.ProviderName == CryptoConst.MS_DEF_PROV || 
                key.CspKeyContainerInfo.ProviderName == CryptoConst.MS_DEF_RSA_SCHANNEL_PROV)
            {
                Type CspKeyContainerInfo_Type = typeof(CspKeyContainerInfo);

                FieldInfo CspKeyContainerInfo_m_parameters = CspKeyContainerInfo_Type.GetField("m_parameters", BindingFlags.NonPublic | BindingFlags.Instance);
                CspParameters parameters = (CspParameters)CspKeyContainerInfo_m_parameters.GetValue(key.CspKeyContainerInfo);

                var cspparams = new CspParameters(CryptoConst.PROV_RSA_AES, CryptoConst.MS_ENH_RSA_AES_PROV, key.CspKeyContainerInfo.KeyContainerName);
                cspparams.KeyNumber = parameters.KeyNumber;
                cspparams.Flags = parameters.Flags;
                _cryptoProvider = new RSACryptoServiceProvider(cspparams);

                _disposeCryptoProvider = true;
            }
            else
            {
                _cryptoProvider = key;
                _disposeCryptoProvider = false;
            }
        }

        #endregion
    }
}
