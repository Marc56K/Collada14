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
    internal class DAELibraryVisualScenes
    {
        private library_visual_scenes _lib;

        internal DAELibraryVisualScenes()
        {
        }

        internal void Init(library_visual_scenes lib)
        {
            _lib = lib;
        }

        internal IDAESceneNode GetSceneNode(DAELoaderNode loader, string id)
        {
            foreach (visual_scene scene in _lib.visual_scene)
            {
                if (id.Equals(scene.id))
                {
                    return CreateSceneNode(loader, scene);
                }
            }

            return null;
        }

        private IDAESceneNode CreateSceneNode(DAELoaderNode loader, visual_scene scene)
        {
            List<IDAESceneNode> childs = new List<IDAESceneNode>();
            foreach (node n in scene.node)
            {
                DAENode node = new DAENode(n);
                IDAESceneNode sceneNode = node.GetSceneNode(loader);
                if (sceneNode != null)
                {
                    childs.Add(sceneNode);
                }
            }

            return loader.Context.CreateGroupNode(scene.id, DAEMatrix4.Identity, childs.ToArray());
        }
    }
}
