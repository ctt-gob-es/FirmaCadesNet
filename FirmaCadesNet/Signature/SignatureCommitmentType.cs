// --------------------------------------------------------------------------------------------------------------------
// SignatureCommitmentType.cs
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

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Esf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmaCadesNet.Signature.Parameters
{
    public class SignatureCommitmentType
    {
        #region Private variables

        private DerObjectIdentifier _oid;        

        #endregion

        #region Public properties

        public static SignatureCommitmentType ProofOfOrigin = new SignatureCommitmentType(CommitmentTypeIdentifier.ProofOfOrigin);
        public static SignatureCommitmentType ProofOfReceipt = new SignatureCommitmentType(CommitmentTypeIdentifier.ProofOfReceipt);
        public static SignatureCommitmentType ProofOfDelivery = new SignatureCommitmentType(CommitmentTypeIdentifier.ProofOfDelivery);
        public static SignatureCommitmentType ProofOfSender = new SignatureCommitmentType(CommitmentTypeIdentifier.ProofOfSender);
        public static SignatureCommitmentType ProofOfApproval = new SignatureCommitmentType(CommitmentTypeIdentifier.ProofOfApproval);
        public static SignatureCommitmentType ProofOfCreation = new SignatureCommitmentType(CommitmentTypeIdentifier.ProofOfCreation);

        public DerObjectIdentifier Oid
        {
            get
            {
                return _oid;
            }
        }

        #endregion

        #region Constructors

        public SignatureCommitmentType(DerObjectIdentifier identifier)
        {
            _oid = identifier;
        }

        #endregion
    }
}
