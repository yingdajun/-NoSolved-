/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for Additional information regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
==================================================================== */

namespace NPOI.HSLF.Record
{
    using System;
    using System.IO;

    /**
     * An atom record that specifies that a shape is a header or footer placeholder shape
     *
     * @since  PowerPoint 2007
     * @author Yegor Kozlov
     */
    public class RoundTripHFPlaceholder12 : RecordAtom
    {
        /**
         * Record header.
         */
        private byte[] _header;

        /**
         * Specifies the placeholder shape ID.
         *
         * MUST be {@link OEPlaceholderAtom#MasterDate},  {@link OEPlaceholderAtom#MasterSlideNumber},
         * {@link OEPlaceholderAtom#MasterFooter}, or {@link OEPlaceholderAtom#MasterHeader}
         */
        private byte _placeholderId;

        /**
         * Constructs the comment atom record from its source data.
         *
         * @param source the source data as a byte array.
         * @param start the start offset into the byte array.
         * @param len the length of the slice in the byte array.
         */
        protected RoundTripHFPlaceholder12(byte[] source, int start, int len)
        {
            // Get the header.
            _header = new byte[8];
            Array.Copy(source, start, _header, 0, 8);

            // Get the record data.
            _placeholderId = source[start + 8];
        }

        /**
         * Gets the comment number (note - each user normally has their own count).
         * @return the comment number.
         */
        public int GetPlaceholderId()
        {
            return _placeholderId;
        }

        /**
         * Sets the comment number (note - each user normally has their own count).
         * @param number the comment number.
         */
        public void SetPlaceholderId(int number)
        {
            _placeholderId = (byte)number;
        }

        /**
         * Gets the record type.
         * @return the record type.
         */
        public override long RecordType
        {
            get { return RecordTypes.RoundTripHFPlaceholder12.typeID; }
        }

        /**
         * Write the contents of the record back, so it can be written
         * to disk
         *
         * @param out the output stream to write to.
         * @throws java.io.IOException if an error occurs.
         */
        public override void WriteOut(Stream out1)
        {
            out1.Write(_header, (int)out1.Position, _header.Length);
            out1.Write(new byte[1]{_placeholderId}, (int)out1.Position,1);
        }
    }
}

