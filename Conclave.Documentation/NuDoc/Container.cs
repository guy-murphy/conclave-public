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
    using System.Linq;

    /// <summary>
    /// Base class for elements that can contain other elements.
    /// </summary>
    /// <remarks>
    /// Implements the composite pattern for the visitable model.
    /// </remarks>
    public abstract class Container : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Container"/> class.
        /// </summary>
        /// <param name="elements">The contained elements within this instance.</param>
        public Container(IEnumerable<Element> elements)
        {
            this.Elements = elements.Cached();
        }

        /// <summary>
        /// Gets the elements contained in this instance.
        /// </summary>
        public IEnumerable<Element> Elements { get; private set; }
    }
}