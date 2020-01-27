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

namespace Collada14
{
    internal class DAENode
    {
        private node _node;

        internal DAENode(node n)
        {
            _node = n;
        }

        internal IDAESceneNode GetSceneNode(DAELoaderNode loader)
        {
            List<IDAESceneNode> children = new List<IDAESceneNode>();

            // load geometry
            if (_node.instance_geometry != null && _node.instance_geometry.Length > 0)
            {
                foreach (var instGeo in _node.instance_geometry)
                {
                    Dictionary<string, string> instanceMaterials = new Dictionary<string, string>();
                    if (instGeo.bind_material != null)
                    {
                        foreach (instance_material instMat in instGeo.bind_material.technique_common)
                        {
                            instanceMaterials.Add(instMat.symbol, instMat.target);
                        }
                    }

                    DAEGeometry geo = loader.LibGeometries.GetGeometry(loader, DAEUtils.GetUrl(instGeo.url).Id);
                    List<IDAEShapeNode> shapes = geo.GetShapeNodes(loader, instanceMaterials, _node.name);

                    foreach (IDAEShapeNode shape in shapes)
                    {
                        children.Add(shape);
                    }
                }
            }

            // load lights
            if (_node.instance_light != null && _node.instance_light.Length > 0)
            {
                foreach (var instLight in _node.instance_light)
                {
                    IDAESceneNode lightNode = loader.LibLights.GetLightNode(loader, DAEUtils.GetUrl(instLight.url).Id);
                    if (lightNode != null)
                    {
                        children.Add(lightNode);
                    }
                }
            }

            // load local children
            if (_node.node1 != null && _node.node1.Length > 0)
            {
                foreach (node child in _node.node1)
                {
                    DAENode n = new DAENode(child);
                    IDAESceneNode childNode = n.GetSceneNode(loader);
                    if (childNode != null)
                    {
                        children.Add(childNode);
                    }
                }
            }

            // load remote children
            if (_node.instance_node != null)
            {
                foreach (InstanceWithExtra child in _node.instance_node)
                {
                    IDAESceneNode childNode = null;
                    var url = DAEUtils.GetUrl(child.url);
                    if (string.IsNullOrEmpty(url.FilePath))
                    {
                        childNode = loader.LibNodes.GetSceneNode(loader, url.Id);
                    }
                    else
                    {
                        var extLoader = loader.GetLoaderForUrl(url);
                        childNode = extLoader.GetSceneGraph(); //TODO only load node with url.Id
                    }
                    if (childNode != null)
                    {
                        children.Add(childNode);
                    }
                }
            }

            // load transformation
            DAEMatrix4 finalTrans = DAEMatrix4.Identity;
            if (_node.Items != null)
            {
                for (int i = 0; i < _node.Items.Length; i++)
                {
                    object trans = _node.Items[i];
                    ItemsChoiceType2 transType = _node.ItemsElementName[i];

                    if (transType == ItemsChoiceType2.matrix)
                    {
                        matrix m = trans as matrix;
                        DAEMatrix4 k = DAEMatrix4.Identity;
                        k.M11 = m.Values[0];
                        k.M12 = m.Values[1];
                        k.M13 = m.Values[2];
                        k.M14 = m.Values[3];
                        k.M21 = m.Values[4];
                        k.M22 = m.Values[5];
                        k.M23 = m.Values[6];
                        k.M24 = m.Values[7];
                        k.M31 = m.Values[8];
                        k.M32 = m.Values[9];
                        k.M33 = m.Values[10];
                        k.M34 = m.Values[11];
                        k.M41 = m.Values[12];
                        k.M42 = m.Values[13];
                        k.M43 = m.Values[14];
                        k.M44 = m.Values[15];
                        finalTrans *= k;
                    }
                    else if (transType == ItemsChoiceType2.rotate)
                    {
                        rotate r = trans as rotate;
                        finalTrans *= DAEMatrix4.Rotation(r.Values[0], r.Values[1], r.Values[2], (r.Values[3] * System.Math.PI / 180));
                    }
                    else if (transType == ItemsChoiceType2.lookat)
                    {
                        //lookat l = trans as lookat;
                        //finalTrans *= SharpDX.Matrix.LookAtLH(
                        //    new SharpDX.Vector3((float)l.Values[0], (float)l.Values[1], (float)l.Values[2]),
                        //    new SharpDX.Vector3((float)l.Values[3], (float)l.Values[4], (float)l.Values[5]),
                        //    new SharpDX.Vector3((float)l.Values[6], (float)l.Values[7], (float)l.Values[8]));

                        // not implemented
                    }
                    else if (transType == ItemsChoiceType2.scale)
                    {
                        TargetableFloat3 s = trans as TargetableFloat3;
                        finalTrans *= DAEMatrix4.Scaling(s.Values[0], s.Values[1], s.Values[2]);
                    }
                    else if (transType == ItemsChoiceType2.translate)
                    {
                        TargetableFloat3 t = trans as TargetableFloat3;
                        finalTrans *= DAEMatrix4.Translation(t.Values[0], t.Values[1], t.Values[2]);
                    }
                    else if (transType == ItemsChoiceType2.skew)
                    {
                        // not implemented
                    }
                }
            }

            return loader.Context.CreateGroupNode(_node.name, finalTrans, children.ToArray());
        }
    }
}
