/* MIT License (MIT)
 *
 * Copyright (c) 2013 Marc Roßbach
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Runtime.InteropServices;

namespace Collada14
{
    public struct DAEVertexDescription
    {
        public DAEVector3 Vertex;
        public DAEVector3 TexCoord;
        public DAEVector3 Normal;
        public DAEVector3 TexTangent;
        public DAEVector3 TexBinormal;
    }

    internal class DAETriangles
    {
        internal IDAEGeometry Geometry { get; private set; }
        internal string MaterialSymbol { get; private set; }

        internal DAETriangles(DAELoaderNode loader, DAEVertexDescription[] vertices, string materialSymbol)
        {
            this.Geometry = loader.Context.CreateGeometry(vertices);
            this.MaterialSymbol = materialSymbol;
        }
    }
}
