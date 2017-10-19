// --------------------------------------------------------------------------------------------------------------------
// MimeTypeHelper.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmaCadesNet.Util
{
    class MimeTypeHelper
    {
        private static string _defaultOid = "1.2.840.113549.1.7.1"; // Data

        private static Dictionary<string, string> _mimeTypes = new Dictionary<string, string>()
        {
            {"application/pdf", "1.2.840.10003.5.109.1"},
            {"application/postscript", "1.2.840.10003.5.109.2"},
            {"text/html", "1.2.840.10003.5.109.3"},
            {"image/tiff", "1.2.840.10003.5.109.4"},
            {"image/gif", "1.2.840.10003.5.109.5"},
            {"image/jpeg", "1.2.840.10003.5.109.6"},
            {"image/png", "1.2.840.10003.5.109.7"},
            {"video/mpeg", "1.2.840.10003.5.109.8"},
            {"text/sgml", "1.2.840.10003.5.109.9"},
            {"text/xml", "1.2.840.10003.5.109.10"},
            {"application/msword", "1.2.840.113556.4.2"},
            {"application/vnd.ms-excel", "1.2.840.113556.4.3"},
            {"application/vnd.ms-project", "1.2.840.113556.4.4"},
            {"application/vnd.ms-powerpoint", "1.2.840.113556.4.5"},
            {"application/vnd.ms-works", "1.2.840.113556.4.6"}
        };

        public static string GetMimeTypeOid(string mimeType)
        {
            if (_mimeTypes.ContainsKey(mimeType))
            {
                return _mimeTypes[mimeType];
            }
            else
            {
                return _defaultOid;
            }
        }
    }
}
