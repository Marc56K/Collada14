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

using Collada14.Schema;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
namespace Collada14
{
    public abstract class DAELoaderContext
    {
        public IDAESceneNode Load(string fileName)
        {
            var loader = new DAELoaderNode(this);
            loader.Open(fileName);
            return loader.GetSceneGraph();
        }

        #region Logging

        abstract public void LogInfo(object sender, string message);
        abstract public void LogError(object sender, string message);

        #endregion

        #region Factory

        abstract public IDAEMaterial CreateMaterial(DAEVector4 ambient, DAEVector4 diffuse, DAEVector4 specular, double shininess, bool isTransparent, string texturePath);
        abstract public IDAEGeometry CreateGeometry(DAEVertexDescription[] vertices);

        abstract public IDAEGroupNode CreateGroupNode(string name, DAEMatrix4 localTransformation, params IDAESceneNode[] childNodes);
        abstract public IDAEShapeNode CreateShapeNode(string name, IDAEMaterial material, IDAEGeometry geometry);
        abstract public IDAEAmbientLightNode CreateAmbientLightNode(string name, DAEVector4 color);
        abstract public IDAEDirectionalLightNode CreateDirectionalLightNode(string name, DAEVector4 color);
        abstract public IDAEPointLightNode CreatePointLightNode(string name, DAEVector4 color, double constantAttenuation, double linearAttenuation, double quadraticAttenuation);
        abstract public IDAESpotLightNode CreateSpotLightNode(string name, DAEVector4 color, double constantAttenuation, double linearAttenuation, double quadraticAttenuation, double spotExponent, double spotCutOff);

        #endregion
    }
}
