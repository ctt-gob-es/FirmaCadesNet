// --------------------------------------------------------------------------------------------------------------------
// DigestMethod.cs
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
using System.Security.Cryptography;

namespace FirmaCadesNet.Crypto
{
    public class DigestMethod
    {
        #region Private variables

        private string _name;
        private string _uri;
        private string _oid;

        #endregion

        #region Public properties

        public static DigestMethod SHA1 = new DigestMethod("SHA-1", "http://www.w3.org/2000/09/xmldsig#sha1", "1.3.14.3.2.26");
        public static DigestMethod SHA256 = new DigestMethod("SHA-256", "http://www.w3.org/2001/04/xmlenc#sha256", "2.16.840.1.101.3.4.2.1");
        public static DigestMethod SHA512 = new DigestMethod("SHA-512", "http://www.w3.org/2001/04/xmlenc#sha512", "2.16.840.1.101.3.4.2.3");


        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string URI
        {
            get
            {
                return _uri;
            }
        }

        public string Oid
        {
            get
            {
                return _oid;
            }
        }

        #endregion

        #region Constructors

        private DigestMethod(string name, string uri, string oid)
        {
            _name = name;
            _uri = uri;
            _oid = oid;
        }

        #endregion

        #region Public methods

        public static DigestMethod GetByOid(string oid)
        {
            if (oid == SHA1.Oid)
            {
                return SHA1;
            }
            else if (oid == SHA256.Oid)
            {
                return SHA256;
            }
            else if (oid == SHA512.Oid)
            {
                return SHA512;
            }
            else
            {
                throw new Exception("Unsupported digest method");
            }
        }

        public static DigestMethod GetByName(string algoName)
        {
            if ("SHA-1".Equals(algoName) || "SHA1".Equals(algoName))
            {
                return SHA1;
            }
            if ("SHA-256".Equals(algoName) || "SHA256".Equals(algoName))
            {
                return SHA256;
            }
            if ("SHA-512".Equals(algoName) || "SHA512".Equals(algoName))
            {
                return SHA512;
            }

            throw new Exception("unsupported algo: " + algoName);
        }

        public HashAlgorithm GetHashAlgorithm()
        {
            if (_name == "SHA-1")
            {
                return System.Security.Cryptography.SHA1.Create();
            }
            else if (_name == "SHA-256")
            {
                return System.Security.Cryptography.SHA256.Create();
            }
            else if (_name == "SHA-512")
            {
                return System.Security.Cryptography.SHA512.Create();
            }
            else
            {
                throw new Exception("Algoritmo no soportado");
            }
        }

        public byte[] CalculateDigest(byte[] data)
        {
            using (var hashAlgorithm = GetHashAlgorithm())
            {
                return hashAlgorithm.ComputeHash(data);
            }
        }

        #endregion
    }
}
