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
    using System;

    /// <summary>
    /// Exposes the visitor pattern on a visitable model that 
    /// can receive visitors.
    /// </summary>
    public interface IVisitable
    {
        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <typeparam name="TVisitor">The type of the visitor, inferred from the passed-in <paramref name="visitor"/>.</typeparam>
        /// <param name="visitor">The visitor instance to accept.</param>
        /// <returns>The received visitor. Allows for easy collecting of the results from the visitor.</returns>
        TVisitor Accept<TVisitor>(TVisitor visitor) where TVisitor : Visitor;
    }
}
