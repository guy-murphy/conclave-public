﻿#region Apache Licensed
/*
 Copyright 2013 Clarius Consulting, Daniel Cazzulino

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
#endregion

namespace ClariusLabs.NuDoc
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the <c>see</c> documentation tag.
    /// </summary>
    /// <remarks>
    /// See http://msdn.microsoft.com/en-US/library/acd0tfbe(v=vs.80).aspx.
    /// </remarks>
    public class See : Container
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="See" /> class.
        /// </summary>
        /// <param name="cref">The member id of the referenced member, if any.</param>
        /// <param name="langword">The element langword, if any.</param>
        /// <param name="elements">The child elements.</param>
        public See(string cref, string langword, IEnumerable<Element> elements)
            : base(elements)
        {
            this.Cref = cref;
            this.Langword = langword;
        }

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        public override TVisitor Accept<TVisitor>(TVisitor visitor)
        {
            visitor.VisitSee(this);
            return visitor;
        }

        /// <summary>
        /// Gets the member id of the referenced member.
        /// </summary>
        public string Cref { get; private set; }

        /// <summary>
        /// Gets the original langword attribute.
        /// </summary>
        public string Langword { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return "<see>" + base.ToString();
        }
    }
}