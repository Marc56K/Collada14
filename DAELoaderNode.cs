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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Collada14
{
    internal class DAELoaderNode
    {
        internal COLLADA Document { get; set; }
        internal DAELibraryVisualScenes LibVisualScenes { get; set; }
        internal DAELibraryEffects LibEffects { get; set; }
        internal DAELibraryGeometries LibGeometries { get; set; }
        internal DAELibraryImages LibImages { get; set; }
        internal DAELibraryLights LibLights { get; set; }
        internal DAELibraryMaterials LibMaterials { get; set; }
        internal DAELibraryNodes LibNodes { get; set; }
        internal DAELoaderContext Context { get; set; }
        internal string FileName { get; set; }

        internal DAELoaderNode(DAELoaderContext ctx)
        {
            Context = ctx;
        }

        internal void Open(string fileName)
        {
            Context.LogInfo(this, "Loading " + fileName);

            FileName = fileName;
            Document = COLLADA.Load(fileName);

            LibVisualScenes = new DAELibraryVisualScenes();
            LibEffects = new DAELibraryEffects();
            LibGeometries = new DAELibraryGeometries();
            LibImages = new DAELibraryImages();
            LibLights = new DAELibraryLights();
            LibMaterials = new DAELibraryMaterials();
            LibNodes = new DAELibraryNodes();

            foreach (var item in Document.Items)
            {
                if (item is library_visual_scenes)
                    LibVisualScenes.Init(item as library_visual_scenes);
                else if (item is library_effects)
                    LibEffects.Init(item as library_effects);
                else if (item is library_geometries)
                    LibGeometries.Init(item as library_geometries);
                else if (item is library_images)
                    LibImages.Init(item as library_images);
                else if (item is library_lights)
                    LibLights.Init(item as library_lights);
                else if (item is library_materials)
                    LibMaterials.Init(item as library_materials);
                else if (item is library_nodes)
                    LibNodes.Init(item as library_nodes);
            }

            Context.LogInfo(this, "Loading " + fileName + " finished");
        }

        internal IDAESceneNode GetSceneGraph()
        {
            return LibVisualScenes.GetSceneNode(this, DAEUtils.GetUrl(Document.scene.instance_visual_scene.url).Id);
        }

        private Dictionary<string, DAELoaderNode> _subLoader = new Dictionary<string, DAELoaderNode>();
        internal DAELoaderNode GetLoaderForUrl(DAEUrl url)
        {
            if (!string.IsNullOrEmpty(url.FilePath))
            {
                string path = url.FilePath;
                string filePath = Path.GetDirectoryName(this.FileName).Replace(@"\", "/");
                path = filePath + "/" + path.Replace("file:///", "");
                if (!_subLoader.ContainsKey(path))
                {
                    _subLoader[path] = new DAELoaderNode(this.Context);
                    _subLoader[path].Open(path);
                }
                //_subLoader[path].Open(path);
                return _subLoader[path];
            }

            return this;
        }
    }
}
