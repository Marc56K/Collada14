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

using System;
using System.Runtime.InteropServices;

namespace Collada14
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DAEVector2
    {
        public double X;
        public double Y;

        public DAEVector2(double[] xy)
        {
            X = xy[0];
            Y = xy[1];
        }

        public DAEVector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    default:
                        throw new IndexOutOfRangeException("index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("index");
                }
            }
        }

        public double Length
        {
            get
            {
                return (double)System.Math.Sqrt(LengthSquared);
            }
        }

        public double LengthSquared
        {
            get
            {
                return X * X + Y * Y;
            }
        }

        public DAEVector2 Normalized
        {
            get
            {
                double len = Length;
                DAEVector2 result = new DAEVector2();
                if (len == 0)
                    return result;

                result.X = X / len;
                result.Y = Y / len;

                return result;
            }
        }

        public static DAEVector2 operator *(DAEVector2 v, double n)
        {
            return new DAEVector2(v.X * n, v.Y * n);
        }

        public static DAEVector2 operator *(double n, DAEVector2 v)
        {
            return v * n;
        }

        public static DAEVector2 operator /(DAEVector2 v, double n)
        {
            return new DAEVector2(v.X / n, v.Y / n);
        }

        public static DAEVector2 operator +(DAEVector2 v0, DAEVector2 v1)
        {
            return new DAEVector2(v0.X + v1.X, v0.Y + v1.Y);
        }

        public static DAEVector2 operator -(DAEVector2 v0, DAEVector2 v1)
        {
            return new DAEVector2(v0.X - v1.X, v0.Y - v1.Y);
        }

        public static bool operator ==(DAEVector2 left, DAEVector2 right)
        {
            return (left.X == right.X) && (left.Y == right.Y);
        }

        public static bool operator !=(DAEVector2 left, DAEVector2 right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is DAEVector2)
            {
                return this == (DAEVector2)obj;
            }

            return false;
        }

        public override string ToString()
        {
            return String.Format("[{0} {1}]", X, Y);
        }
    }
}
