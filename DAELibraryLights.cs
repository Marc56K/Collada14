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
using System.Globalization;
using System.Runtime.InteropServices;

namespace Collada14
{
    internal class DAELibraryLights
    {
        private library_lights _lib;

        internal DAELibraryLights()
        {
        }

        internal void Init(library_lights lib)
        {
            _lib = lib;
        }

        internal IDAESceneNode GetLightNode(DAELoaderNode loader, string id)
        {
            foreach (light l in _lib.light)
            {
                if (l.id.Equals(id))
                {
                    return CreateLightNode(loader, l);
                }
            }

            return null;
        }

        private DAEVector4 GetColor(TargetableFloat3 deaColor)
        {
            return new DAEVector4(deaColor.Values[0], deaColor.Values[1], deaColor.Values[2], 1);
        }

        private IDAESceneNode CreateLightNode(DAELoaderNode loader, light l)
        {
            IDAESceneNode result = null;
            if (l.technique_common.Item is lightTechnique_commonAmbient)
            {
                var ambLight = l.technique_common.Item as lightTechnique_commonAmbient;

                if (ambLight.color != null)
                    result = loader.Context.CreateAmbientLightNode(l.name, GetColor(ambLight.color));
            }
            else if (l.technique_common.Item is lightTechnique_commonDirectional)
            {
                var dirLight = l.technique_common.Item as lightTechnique_commonDirectional;

                if (dirLight.color != null)
                    result = loader.Context.CreateDirectionalLightNode(l.name, GetColor(dirLight.color));
            }
            else if (l.technique_common.Item is lightTechnique_commonPoint)
            {
                var pointLight = l.technique_common.Item as lightTechnique_commonPoint;

                DAEVector4 color;
                if (pointLight.color != null)
                    color = GetColor(pointLight.color);
                else
                    color = new DAEVector4(1, 1, 1, 1);

                double constantAttenuation = 1;
                if (pointLight.constant_attenuation != null)
                    constantAttenuation = pointLight.constant_attenuation.Value;

                double linearAttenuation = 0;
                if (pointLight.linear_attenuation != null)
                    linearAttenuation = pointLight.linear_attenuation.Value;

                double quadraticAttenuation = 0;
                if (pointLight.quadratic_attenuation != null)
                    quadraticAttenuation = pointLight.quadratic_attenuation.Value;

                result = loader.Context.CreatePointLightNode(l.name, color, constantAttenuation, linearAttenuation, quadraticAttenuation);
            }
            else if (l.technique_common.Item is lightTechnique_commonSpot)
            {
                var spotLight = l.technique_common.Item as lightTechnique_commonSpot;

                DAEVector4 color;
                if (spotLight.color != null)
                    color = GetColor(spotLight.color);
                else
                    color = new DAEVector4(1, 1, 1, 1);

                double constantAttenuation = 1;
                if (spotLight.constant_attenuation != null)
                    constantAttenuation = spotLight.constant_attenuation.Value;

                double linearAttenuation = 0;
                if (spotLight.linear_attenuation != null)
                    linearAttenuation = spotLight.linear_attenuation.Value;

                double quadraticAttenuation = 0;
                if (spotLight.quadratic_attenuation != null)
                    quadraticAttenuation = spotLight.quadratic_attenuation.Value;

                double spotExponent = 0;
                if (spotLight.falloff_exponent != null)
                    spotExponent = spotLight.falloff_exponent.Value;

                double spotCutOff = System.Math.PI; // default is 180°
                if (spotLight.falloff_angle != null)
                {
                    spotCutOff = spotLight.falloff_angle.Value / 2;
                }
                else if (l.extra != null && l.extra.Length > 0)
                {
                    foreach (var extra in l.extra)
                    {
                        if (extra.technique != null && extra.technique.Length > 0)
                        {
                            foreach (var extraTech in extra.technique)
                            {
                                if (extraTech.Any != null && extraTech.Any.Length > 0)
                                {
                                    foreach (var ele in extraTech.Any)
                                    {
                                        if (ele.ChildNodes != null)
                                        {
                                            for (int i = 0; i < ele.ChildNodes.Count; i++)
                                            {
                                                var n = ele.ChildNodes[i];
                                                if (n.Name == "falloff")
                                                {
                                                    float angle = 0;
                                                    if (float.TryParse(n.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out angle))
                                                    {
                                                        spotCutOff = ((double)angle * System.Math.PI / 180) / 2;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                result = loader.Context.CreateSpotLightNode(l.name, color, constantAttenuation, linearAttenuation, quadraticAttenuation, spotExponent, spotCutOff);
            }

            return result;
        }
    }
}
