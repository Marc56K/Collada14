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
    internal class DAEGeometry
    {
        private class MeshSource
        {
            public double[] Values { get; private set; }
            public int Stride { get; private set; }

            public MeshSource(double[] values, int stride)
            {
                Values = values;
                Stride = stride;
            }

            public DAEVector3 GetVector3(int index)
            {
                DAEVector3 result = new DAEVector3(0, 0, 0);

                if (Stride > 0)
                    result.X = Values[index * Stride];

                if (Stride > 1)
                    result.Y = Values[index * Stride + 1];

                if (Stride > 2)
                    result.Z = Values[index * Stride + 2];

                return result;
            }
        }

        private List<DAETriangles> _triangles = new List<DAETriangles>();

        internal DAEGeometry(DAELoaderNode loader, geometry geo)
        {
            if (geo.Item == null || !(geo.Item is mesh))
            {
                // empty or not supported geometry
                return;
            }

            mesh m = geo.Item as mesh;

            // load sources
            Dictionary<string, MeshSource> sources = new Dictionary<string, MeshSource>();
            foreach (source src in m.source)
            {
                if (src.Item is float_array)
                {
                    float_array values = src.Item as float_array;
                    int stride = (int)src.technique_common.accessor.stride;
                    sources.Add(src.id, new MeshSource(values.Values, stride));
                }
            }

            // load positions
            foreach (InputLocal input in m.vertices.input)
            {
                if (input.semantic.Equals("POSITION"))
                {
                    sources.Add(m.vertices.id, sources[DAEUtils.GetUrl(input.source).Id]);
                    break;
                }
            }

            // load primitives
            foreach (var item in m.Items)
            {
                // load triangles
                if (item is triangles)
                {
                    triangles tris = item as triangles;

                    DAEVertexDescription[] vertices = new DAEVertexDescription[tris.count * 3];
                    int[] p = DAEUtils.StringToIntArray(tris.p);
                    int stepSize = p.Length / ((int)tris.count * 3);
                    foreach (InputLocalOffset input in tris.input)
                    {
                        SetVertices(input, p, stepSize, sources, vertices);
                    }

                    if (!ContainsTangents(tris.input))
                    {
                        GenerateMeshTangents(vertices);
                    }

                    _triangles.Add(new DAETriangles(loader, vertices, tris.material));
                }
                else if (item is polylist)
                {
                    polylist polys = item as polylist;

                    int[] polyVertexCounts = DAEUtils.StringToIntArray(polys.vcount);
                    int vertexCount = 0;
                    int triCount = 0;
                    for (int i = 0; i < polyVertexCounts.Length; i++)
                    {
                        vertexCount += polyVertexCounts[i];
                        triCount += polyVertexCounts[i] - 2;
                    }

                    DAEVertexDescription[] polyVertices = new DAEVertexDescription[vertexCount];
                    int[] p = DAEUtils.StringToIntArray(polys.p);
                    int stepSize = p.Length / vertexCount;

                    foreach (InputLocalOffset input in polys.input)
                    {
                        SetVertices(input, p, stepSize, sources, polyVertices);
                    }

                    // triangulation
                    DAEVertexDescription[] triVertices = new DAEVertexDescription[triCount * 3];
                    int triVertexIdx = 0;
                    int polyVertexIdx = 0;
                    for (int i = 0; i < polyVertexCounts.Length; i++)
                    {
                        int triVertex = 0;
                        for (int k = 0; k < polyVertexCounts[i]; k++)
                        {
                            if (triVertex == 3)
                            {
                                triVertex = 1;
                                k--;

                                // repeat first vertex
                                triVertices[triVertexIdx++] = polyVertices[polyVertexIdx];
                            }

                            triVertices[triVertexIdx++] = polyVertices[polyVertexIdx + k];
                            triVertex++;
                        }

                        // skip to next polygon
                        polyVertexIdx += polyVertexCounts[i];
                    }

                    if (!ContainsTangents(polys.input))
                    {
                        GenerateMeshTangents(triVertices);
                    }

                    _triangles.Add(new DAETriangles(loader, triVertices, polys.material));
                }
            }
        }

        private bool ContainsTangents(InputLocalOffset[] inputs)
        {
            bool foundTangents = false;
            bool foundBinormals = false;
            foreach (InputLocalOffset input in inputs)
            {
                if (input.semantic.Equals("TEXTANGENT"))
                {
                    foundTangents = true;
                }
                else if (input.semantic.Equals("TEXBINORMAL"))
                {
                    foundBinormals = true;
                }
            }

            return foundTangents && foundBinormals;
        }

        private void SetVertices(InputLocalOffset input, int[] p, int stepSize, Dictionary<string, MeshSource> sources, DAEVertexDescription[] outputVertices)
        {
            if (input.semantic.Equals("VERTEX"))
            {
                MeshSource meshSource = sources[DAEUtils.GetUrl(input.source).Id];
                for (int i = 0; i < outputVertices.Length; i++)
                {
                    int idx = p[i * stepSize + (int)input.offset];
                    outputVertices[i].Vertex = meshSource.GetVector3(idx);
                }
            }
            else if (input.semantic.Equals("NORMAL"))
            {
                MeshSource meshSource = sources[DAEUtils.GetUrl(input.source).Id];
                for (int i = 0; i < outputVertices.Length; i++)
                {
                    int idx = p[i * stepSize + (int)input.offset];
                    outputVertices[i].Normal = meshSource.GetVector3(idx);
                }
            }
            else if (input.semantic.Equals("TEXCOORD"))
            {
                MeshSource meshSource = sources[DAEUtils.GetUrl(input.source).Id];
                for (int i = 0; i < outputVertices.Length; i++)
                {
                    int idx = p[i * stepSize + (int)input.offset];
                    outputVertices[i].TexCoord = meshSource.GetVector3(idx);
                    outputVertices[i].TexCoord = new DAEVector3(outputVertices[i].TexCoord.X, 1 - outputVertices[i].TexCoord.Y, outputVertices[i].TexCoord.Z);
                }
            }
            else if (input.semantic.Equals("TEXTANGENT"))
            {
                MeshSource meshSource = sources[DAEUtils.GetUrl(input.source).Id];
                for (int i = 0; i < outputVertices.Length; i++)
                {
                    int idx = p[i * stepSize + (int)input.offset];
                    outputVertices[i].TexTangent = meshSource.GetVector3(idx);
                }
            }
            else if (input.semantic.Equals("TEXBINORMAL"))
            {
                MeshSource meshSource = sources[DAEUtils.GetUrl(input.source).Id];
                for (int i = 0; i < outputVertices.Length; i++)
                {
                    int idx = p[i * stepSize + (int)input.offset];
                    outputVertices[i].TexBinormal = meshSource.GetVector3(idx);
                }
            }
        }

        internal static void GenerateMeshTangents(DAEVertexDescription[] vertices)
        {
            DAEVector3[] v = new DAEVector3[3];
            DAEVector3[] n = new DAEVector3[3];
            DAEVector3[] t = new DAEVector3[3];
            for (int i = 0; i < vertices.Length; i += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    v[j] = vertices[i + j].Vertex;
                    n[j] = vertices[i + j].Normal;
                    t[j] = vertices[i + j].TexCoord;
                }

                DAEVector3[] tangents = GenerateTriangleTangents(v, n, t);
                for (int j = 0; j < 3; j++)
                {
                    vertices[i + j].TexTangent = tangents[j];
                    vertices[i + j].TexBinormal = tangents[j].Cross(n[j]).Normalized;
                }
            }
        }

        /// <summary>
        /// Generates tangents for a triangle. Source:
        /// http://download.autodesk.com/us/maya/2009help/index.html?url=Appendix_A_Tangent_and_binormal_vectors.htm,topicNumber=d0e216694
        /// </summary>
        /// <param name="v">Vertices of the triangle</param>
        /// <param name="n">Normals of the triangle</param>
        /// <param name="t">Texture coords of the triangle</param>
        /// <returns>the tangents</returns>
        internal static DAEVector3[] GenerateTriangleTangents(DAEVector3[] v, DAEVector3[] n, DAEVector3[] t)
        {
            if (v.Length == 3 && n.Length == 3 && t.Length == 3)
            {
                DAEVector3[] finalTangentArray = new DAEVector3[3];
                double[,] tangentArray = new double[3, 3];
                double[] edge1 = new double[3];
                double[] edge2 = new double[3];
                DAEVector3 crossP;
                // ==============================================
                // x, s, t
                // S & T vectors get used several times in this vector,
                // but are only computed once.
                // ==============================================
                edge1[0] = v[1].X - v[0].X;
                edge1[1] = t[1].X - t[0].X; // s-vector - don't need to compute
                // this multiple times
                edge1[2] = t[1].Y - t[0].Y; // t-vector
                edge2[0] = v[2].X - v[0].X;
                edge2[1] = t[2].X - t[0].X; // another s-vector
                edge2[2] = t[2].Y - t[0].Y; // another t-vector
                crossP = new DAEVector3(edge1).Cross(new DAEVector3(edge2)).Normalized;
                bool degnerateUVTangentPlane = crossP.X == 0.0f;
                if (degnerateUVTangentPlane)
                    crossP = new DAEVector3(1, crossP.Y, crossP.Z);
                double tanX = -crossP.Y / crossP.X;
                tangentArray[0, 0] = tanX;
                tangentArray[1, 0] = tanX;
                tangentArray[2, 0] = tanX;
                // --------------------------------------------------------
                // y, s, t
                // --------------------------------------------------------
                edge1[0] = v[1].Y - v[0].Y;
                edge2[0] = v[2].Y - v[0].Y;
                crossP = new DAEVector3(edge1).Cross(new DAEVector3(edge2)).Normalized;
                degnerateUVTangentPlane = crossP.X == 0.0f;
                if (degnerateUVTangentPlane)
                    crossP = new DAEVector3(1, crossP.Y, crossP.Z);
                double tanY = -crossP.Y / crossP.X;
                tangentArray[0, 1] = tanY;
                tangentArray[1, 1] = tanY;
                tangentArray[2, 1] = tanY;
                // ------------------------------------------------------
                // z, s, t
                // ------------------------------------------------------
                edge1[0] = v[1].Z - v[0].Z;
                edge2[0] = v[2].Z - v[0].Z;
                crossP = new DAEVector3(edge1).Cross(new DAEVector3(edge2)).Normalized;
                degnerateUVTangentPlane = crossP.X == 0.0f;
                if (degnerateUVTangentPlane)
                    crossP = new DAEVector3(1, crossP.Y, crossP.Z);
                double tanZ = -crossP.Y / crossP.X;
                tangentArray[0, 2] = tanZ;
                tangentArray[1, 2] = tanZ;
                tangentArray[2, 2] = tanZ;

                // Orthnonormalize to normal
                for (int i = 0; i < 3; i++)
                {
                    DAEVector3 tangent = new DAEVector3(tangentArray[i, 0], tangentArray[i, 1], tangentArray[i, 2]);
                    finalTangentArray[i] = (tangent - (n[i] * (tangent.Dot(n[i])))).Normalized;
                }

                return finalTangentArray;
            }

            return null;
        }

        internal List<IDAEShapeNode> GetShapeNodes(DAELoaderNode loader, Dictionary<string, string> instanceMaterials, string parentNodeName)
        {
            List<IDAEShapeNode> result = new List<IDAEShapeNode>();

            foreach (DAETriangles tris in _triangles)
            {
                IDAEMaterial mat;
                if (tris.MaterialSymbol != null)
                {
                    mat = loader.LibMaterials.GetMaterial(loader, DAEUtils.GetUrl(instanceMaterials[tris.MaterialSymbol]).Id);
                }
                else
                {
                    mat = loader.LibMaterials.GetDefaultMaterial(loader);
                }
                result.Add(loader.Context.CreateShapeNode(parentNodeName, mat, tris.Geometry));
            }

            return result;
        }
    }
}
