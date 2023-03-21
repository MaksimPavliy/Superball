using System;
using System.Collections.Generic;
using UnityEngine;
namespace FriendsGamesTools
{
    public static partial class Utils
    {
        public static float Sqr(this float f) => f * f;
        public static Vector3 GetPerpendicular(Vector3 dir) {
            var option1 = Vector3.Cross(dir, Vector3.up);
            var option2 = Vector3.Cross(dir, Vector3.forward);
            if (option1.sqrMagnitude > option2.sqrMagnitude)
                return option1;
            else
                return option2;
        }
        public static float Multiply(this IEnumerable<float> items) {
            var mul = 1f;
            foreach (var v in items)
                mul *= v;
            return mul;
        }
        public static float Sigmoid(float x) => 1f / (1f + Mathf.Exp(-x));
        public static float SigmoidInverse(float s) => -Mathf.Log(1f / s - 1);
        public static int Pow(int basE, int power) => Mathf.RoundToInt(Mathf.Pow(basE, power));
        public static float Frac(this float v) => v - Mathf.Floor(v);
        public static double Frac(this double v) => v - Math.Floor(v);
        public static bool Between(this int val, int min, int max, bool including = true)
        {
            if (min < val && val < max)
                return true;
            if (including && (val == min || val == max))
                return true;
            return false;
        }
        public static int RoundToInt(this double val) => (int)Math.Round(val);
        public static bool InRange(this float v, float min, float max, bool including = true)
        {
            if (including)
                return min <= v && v <= max;
            else
                return min < v && v < max;
        }
        static float ZeroIfSmaller(float v, float eps) => Mathf.Abs(v) < eps ? 0 : v;
        public static Vector3 RemoveSmallComponents(this Vector3 v, float eps)
            => new Vector3(ZeroIfSmaller(v.x, eps), ZeroIfSmaller(v.y, eps), ZeroIfSmaller(v.z, eps));
        public static int Sign(int val) => val > 0 ? 1 : (val < 0 ? -1 : 0);
        public static int Sign(this bool val) => val ? 1 : -1;
        public static float Sign(this float val) => val > 0 ? 1f : val == 0 ? 0 : -1f;
        public static Vector3 Rotate(this Vector3 v, Quaternion rotation) => rotation * v;
        public static Vector2 Rotate(this Vector2 v, float degrees) {
            var rad = degrees * Mathf.Deg2Rad;
            var cos = Mathf.Cos(rad);
            var sin = Mathf.Sin(rad);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }
        public static Vector2 XZ(this Vector3 v) => new Vector2(v.x, v.z);
        public static Vector2 XY(this Vector3 v) => new Vector2(v.x, v.y);
        public static Vector2 YZ(this Vector3 v) => new Vector2(v.y, v.z);
        public static Vector2 Rotate90(this Vector2 v) => new Vector2(- v.y, v.x);
        public static Vector2 XTo(this Vector2 v, float x) => new Vector2(x, v.y);
        public static Vector2 YTo(this Vector2 v, float y) => new Vector2(v.x, y);
        public static Vector3 ToXZ(this Vector2 v) => new Vector3(v.x, 0, v.y);
        public static Vector3 ZTo(this Vector2 v, float z) => new Vector3(v.x, v.y, z);
        public static Vector3 XTo0(this Vector3 v) => new Vector3(0, v.y, v.z);
        public static Vector3 YTo0(this Vector3 v) => new Vector3(v.x, 0, v.z);
        public static Vector3 ZTo0(this Vector3 v) => new Vector3(v.x, v.y, 0);
        public static Vector3 XTo(this Vector3 v, float x) => new Vector3(x, v.y, v.z);
        public static Vector3 YTo(this Vector3 v, float y) => new Vector3(v.x, y, v.z);
        public static Vector3 ZTo(this Vector3 v, float z) => new Vector3(v.x, v.y, z);
        public static bool IsSane(this Quaternion v) => IsSane(v.x) && IsSane(v.y) && IsSane(v.z) && IsSane(v.w);
        public static bool IsSane(this Vector3 v) => IsSane(v.x) && IsSane(v.y) && IsSane(v.z);
        public static bool IsSane(this Vector2 v) => IsSane(v.x) && IsSane(v.y);
        public static bool IsSane(this float v) => !float.IsInfinity(v) && !float.IsNaN(v);
        public static bool IsSane(this double v) => !double.IsInfinity(v) && !double.IsNaN(v);
        public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max)
            => new Vector2(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y));
        public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max)
           => new Vector3(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y), Mathf.Clamp(v.z, min.z, max.z));
        public static long ToHash(this long val1, long val2, params long[] vals)
        {
            var hash = val1.ToHash();
            hash += val2;
            hash += hash << 11; hash ^= hash >> 7;
            if (vals != null)
            {
                for (int i = 0; i < vals.Length; i++)
                {
                    hash += vals[i];
                    hash += hash << 11; hash ^= hash >> 7;
                }
            }
            return hash;
        }
        const long startHashSalt = 925549877;
        public static long ToHash(this long val)
        {
            long hash = startHashSalt;
            hash += val;
            hash += hash << 11; hash ^= hash >> 7;
            return hash;
        }
        public static long ToHash(this int val) => ToHash((long)val);
        public static long ToHash(this bool val) => ToHash(val ? 1 : 0);
        public static long ToHash(this string val) => val?.GetHashCode() ?? startHashSalt;
        public static long ToHash<T>(this IEnumerable<T> items, Func<T, long> itemToHash)
        {
            long hash = startHashSalt;
            foreach (var item in items)
                hash = hash.ToHash(itemToHash(item));
            return hash;
        }
        public static double MoveTowards(double curr, double tgt, double maxDelta)
        {
            if (Math.Abs(curr - tgt) <= maxDelta)
                return tgt;
            else if (curr < tgt)
                return curr + maxDelta;
            else
                return curr - maxDelta;
        }
        public static double Clamp(double val, double min, double max) => Math.Min(Math.Max(val, min), max);
        public static double Lerp(double from, double to, double t)
        {
            t = Clamp(t, 0, 1);
            return from * (1 - t) + to * t;
        }
        public static float PowerInt(float v, int power)
        {
            var res = 1f;
            for (int i = 0; i < power; i++)
                res *= v;
            return res;
        }
        public static double PowerInt(double v, int power)
        {
            var res = 1d;
            for (int i = 0; i < power; i++)
                res *= v;
            return res;
        }
    }
}