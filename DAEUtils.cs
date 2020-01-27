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

namespace Collada14
{
    internal struct DAEUrl
    {
        internal string FilePath;
        internal string Id;
    }

    internal class DAEUtils
    {
        internal static DAEUrl GetUrl(string url)
        {
            if (url.StartsWith("#"))
            {
                return new DAEUrl()
                {
                    Id = url.Substring(1)
                };
            }

            if (url.Contains("#"))
            {
                var split = url.Split('#');
                return new DAEUrl()
                {
                    FilePath = split[0],
                    Id = split[1]
                };
            }

            return new DAEUrl();
        }

        internal static int[] StringToIntArray(string values)
        {
            string[] split = values.Split(new char[] { ' ' });
            int[] result = new int[split.Length];
            for (int i = 0; i < result.Length; i++)
            {
                int.TryParse(split[i], out result[i]);
            }
            return result;
        }
    }
}
