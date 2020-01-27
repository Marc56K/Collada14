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

namespace Collada14
{
    internal class DAELibraryImages
    {
        private library_images _lib;
        private Dictionary<string, string> _textures = new Dictionary<string, string>();

        internal DAELibraryImages()
        {
        }

        internal void Init(library_images lib)
        {
            _lib = lib;
        }

        internal string GetTexturePath(DAELoaderNode loader, string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            if (_textures.ContainsKey(id))
            {
                return _textures[id];
            }
            else if (!string.IsNullOrEmpty(id))
            {
                foreach (image img in _lib.image)
                {
                    if (img.id.Equals(id))
                    {
                        return CreateTexturePath(loader, img);
                    }
                }
            }

            return null;
        }

        private string CreateTexturePath(DAELoaderNode loader, image img)
        {
            string tex = null;

            if (img.Item is string)
            {
                string path = img.Item as string;
                string filePath = Path.GetDirectoryName(loader.FileName).Replace(@"\", "/");
                path = filePath + "/" + path.Replace("file:///", "");
                if (File.Exists(path))
                {
                    tex = path;
                }
                else
                {
                    loader.Context.LogError(this, "Image not found: " + path);
                }
            }

            _textures.Add(img.id, tex);
            return tex;
        }
    }
}
