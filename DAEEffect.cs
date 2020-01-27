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
using System.Runtime.InteropServices;

namespace Collada14
{
    internal class DAEEffect
    {
        private string _imageId;
        private DAEVector4 _ambient = new DAEVector4(1.0f, 1.0f, 1.0f, 1.0f);
        private DAEVector4 _diffuse = new DAEVector4(1.0f, 1.0f, 1.0f, 1.0f);
        private DAEVector4 _specular = new DAEVector4(1.0f, 1.0f, 1.0f, 1.0f);
        private float _shininess = 5.0f;
        private bool _isTransparent = false;

        internal DAEEffect(effect efx)
        {
            foreach (effectFx_profile_abstractProfile_COMMON profile in efx.Items)
            {
                if (profile.Items != null)
                {
                    foreach (var item in profile.Items)
                    {
                        if (item is common_newparam_type)
                        {
                            common_newparam_type newparam = item as common_newparam_type;
                            if (newparam.Item is fx_surface_common)
                            {
                                foreach (fx_surface_init_from_common initFrom in (newparam.Item as fx_surface_common).init_from)
                                {
                                    _imageId = initFrom.Value;
                                    break;
                                }
                            }
                        }
                    }
                }

                effectFx_profile_abstractProfile_COMMONTechnique technique = profile.technique;

                DAEVector4 transpColor = new DAEVector4(1, 1, 1, 1);
                double transparency = 1;
                fx_opaque_enum opaqueMode = fx_opaque_enum.RGB_ZERO;

                if (technique.Item is effectFx_profile_abstractProfile_COMMONTechniquePhong) // phong
                {
                    effectFx_profile_abstractProfile_COMMONTechniquePhong p = technique.Item as effectFx_profile_abstractProfile_COMMONTechniquePhong;
                    if (p.transparent != null)
                    {
                        if (p.transparency != null)
                        {
                            if (p.transparency.Item is common_float_or_param_typeFloat)
                            {
                                opaqueMode = p.transparent.opaque;
                                transpColor = GetRgbaColor(p.transparent.Item);
                                transparency = (p.transparency.Item as common_float_or_param_typeFloat).Value;
                            }
                        }
                    }

                    if (p.ambient != null)
                        _ambient = GetColor(opaqueMode, GetRgbaColor(p.ambient.Item), transpColor, transparency);

                    if (p.diffuse != null)
                        _diffuse = GetColor(opaqueMode, GetRgbaColor(p.diffuse.Item), transpColor, transparency);

                    if (p.specular != null)
                        _specular = GetColor(opaqueMode, GetRgbaColor(p.specular.Item), transpColor, transparency);

                    if (p.shininess != null && p.shininess.Item is common_float_or_param_typeFloat)
                        _shininess = (float)(p.shininess.Item as common_float_or_param_typeFloat).Value;
                }
                else if (technique.Item is effectFx_profile_abstractProfile_COMMONTechniqueBlinn) // blinn
                {
                    effectFx_profile_abstractProfile_COMMONTechniqueBlinn p = technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueBlinn;
                    if (p.transparent != null)
                    {
                        if (p.transparency != null)
                        {
                            if (p.transparency.Item is common_float_or_param_typeFloat)
                            {
                                opaqueMode = p.transparent.opaque;
                                transpColor = GetRgbaColor(p.transparent.Item);
                                transparency = (p.transparency.Item as common_float_or_param_typeFloat).Value;
                            }
                        }
                    }

                    if (p.ambient != null)
                        _ambient = GetColor(opaqueMode, GetRgbaColor(p.ambient.Item), transpColor, transparency);

                    if (p.diffuse != null)
                        _diffuse = GetColor(opaqueMode, GetRgbaColor(p.diffuse.Item), transpColor, transparency);

                    if (p.specular != null)
                        _specular = GetColor(opaqueMode, GetRgbaColor(p.specular.Item), transpColor, transparency);

                    if (p.shininess != null && p.shininess.Item is common_float_or_param_typeFloat)
                        _shininess = (float)(p.shininess.Item as common_float_or_param_typeFloat).Value;
                }
                else if (technique.Item is effectFx_profile_abstractProfile_COMMONTechniqueLambert) // lambert
                {
                    effectFx_profile_abstractProfile_COMMONTechniqueLambert p = technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueLambert;
                    if (p.transparent != null)
                    {
                        if (p.transparency != null)
                        {
                            if (p.transparency.Item is common_float_or_param_typeFloat)
                            {
                                opaqueMode = p.transparent.opaque;
                                transpColor = GetRgbaColor(p.transparent.Item);
                                transparency = (p.transparency.Item as common_float_or_param_typeFloat).Value;
                            }
                        }
                    }

                    if (p.ambient != null)
                        _ambient = GetColor(opaqueMode, GetRgbaColor(p.ambient.Item), transpColor, transparency);

                    if (p.diffuse != null)
                        _diffuse = GetColor(opaqueMode, GetRgbaColor(p.diffuse.Item), transpColor, transparency);

                    _specular = new DAEVector4(0.0f, 0.0f, 0.0f, 0.0f);
                }
                else if (technique.Item is effectFx_profile_abstractProfile_COMMONTechniqueConstant) // constant
                {
                    effectFx_profile_abstractProfile_COMMONTechniqueConstant p = technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueConstant;
                    if (p.transparent != null)
                    {
                        if (p.transparency != null)
                        {
                            if (p.transparency.Item is common_float_or_param_typeFloat)
                            {
                                opaqueMode = p.transparent.opaque;
                                transpColor = GetRgbaColor(p.transparent.Item);
                                transparency = (p.transparency.Item as common_float_or_param_typeFloat).Value;
                            }
                        }
                    }

                    if (p.emission != null)
                    {
                        _ambient = GetColor(opaqueMode, GetRgbaColor(p.emission.Item), transpColor, transparency);
                        _diffuse = GetColor(opaqueMode, GetRgbaColor(p.emission.Item), transpColor, transparency);
                        _specular = GetColor(opaqueMode, GetRgbaColor(p.emission.Item), transpColor, transparency);
                        _shininess = 0;
                    }
                }

                _isTransparent = _diffuse.W < 1.0;
            }
        }

        private DAEVector4 GetRgbaColor(object commonColor)
        {
            if (commonColor is common_color_or_texture_typeColor)
            {
                common_color_or_texture_typeColor color = commonColor as common_color_or_texture_typeColor;
                DAEVector4 result = new DAEVector4();
                result.X = color.Values[0];
                result.Y = color.Values[1];
                result.Z = color.Values[2];
                result.W = color.Values[3];
                return result;
            }

            return new DAEVector4(1, 1, 1, 1);
        }

        private DAEVector4 GetColor(fx_opaque_enum opaqueMode, DAEVector4 color, DAEVector4 transparentColor, double transparency)
        {
            switch (opaqueMode)
            {
                case fx_opaque_enum.A_ONE:
                    return GetAOneColor(color, transparentColor, transparency);
                default:
                    return GetRgbZeroColor(color, transparentColor, transparency);
            }
        }

        private DAEVector4 GetAOneColor(DAEVector4 color, DAEVector4 transparentColor, double transparency)
        {
            DAEVector4 result = new DAEVector4();
            result.X = color.X * transparentColor.W * transparency;
            result.Y = color.Y * transparentColor.W * transparency;
            result.Z = color.Z * transparentColor.W * transparency;
            result.W = color.W * transparentColor.W * transparency;
            return result;
        }
        private DAEVector4 GetRgbZeroColor(DAEVector4 color, DAEVector4 transparentColor, double transparency)
        {
            DAEVector4 result = new DAEVector4();
            result.X = color.X * (1.0 - transparentColor.X * transparency);
            result.Y = color.Y * (1.0 - transparentColor.Y * transparency);
            result.Z = color.Z * (1.0 - transparentColor.Z * transparency);
            result.W = 1.0 - GetLuminance(transparentColor.XYZ) * transparency;
            return result;
        }


        private double GetLuminance(DAEVector3 color)
        {
            return color.X * 0.212671 + color.Y * 0.715160 + color.Z * 0.072169;
        }

        internal IDAEMaterial CreateMaterial(DAELoaderNode loader)
        {
            return loader.Context.CreateMaterial(_ambient, _diffuse, _specular, _shininess, _isTransparent, loader.LibImages.GetTexturePath(loader, _imageId));
        }

        internal static IDAEMaterial GetDefaultMaterial(DAELoaderNode loader)
        {
            return loader.Context.CreateMaterial(new DAEVector4(1, 1, 1, 1), new DAEVector4(1, 1, 1, 1), new DAEVector4(0.5f, 0.5f, 0.5f, 1), 50, false, null);
        }
    }
}
