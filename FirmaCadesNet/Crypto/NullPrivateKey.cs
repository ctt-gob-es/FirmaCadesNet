// --------------------------------------------------------------------------------------------------------------------
// NullPrivateKey.cs
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

using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmaCadesNet.Crypto
{
    public class NullPrivateKey : AsymmetricKeyParameter
    {
        public NullPrivateKey()
            : base(true)
        {

        }

        public override string ToString()
        {
            return "NULL";
        }
    }
}
