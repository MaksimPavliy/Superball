using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    public static partial class Utils
    {
        public static Color AlphaTo(this Color col, float a) {
            col.a = a;
            return col;
        }
        public static bool IsVertical() => Screen.height > Screen.width;
        public static bool IsHorizontal() => !IsVertical();
        private static List<string> suffixes = new List<string> { "K", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ai", "ak", "MTTT", "BTTT", "TTTT" };
        public static string ToShownMoney(this int amount, bool dollarSign = true) => ((double)amount).ToShownMoney(dollarSign);
        public static string ToShownMoney(this double amount, bool dollarSign = true)
        {
            var str = amount.ToStringWithSuffixes();
            if (dollarSign)
                str = "$" + str;
            return str;
        }
        public static string ToStringWithSuffixes(this double value)
        {
            if (!value.IsSane())
                return value.ToString();
            if (value < 10000d)
                return $"{Mathf.RoundToInt((float)value)}";
            int decimals = (int)Math.Log10(value);
            int suffixInd = (decimals / 3) - 1;
            double divider = Math.Pow(10d, decimals - (decimals % 3));
            double shown = value / divider;
            string res = shown.ToString(3 - (decimals % 3)) + suffixes[suffixInd];
            return res;
            // 1
            // 12
            // 123
            // 1.234K
            // 12.34K
            // 123.4K
            // 1.234M
            // 12.34M
            // 123.4M
            // 1.234G
        }

        public static (int years, int monthes, int days, int hours, int minutes, int sec) SplitTime(float seconds) {
            const float SecondsPerMinute = 60;
            const float SecondsPerHour = SecondsPerMinute * 60;
            const float SecondsPerDay = SecondsPerHour * 24;
            const float SecondsPerYear = SecondsPerDay * 365;
            const float SecondsPerMonth = SecondsPerDay * 30;
            var years = (int)(seconds / SecondsPerYear);
            seconds -= years * SecondsPerYear;
            var monthes = (int)(seconds / SecondsPerMonth);
            seconds -= monthes * SecondsPerMonth;
            var days = (int)(seconds / SecondsPerDay);
            seconds -= days * SecondsPerDay;
            var hours = (int)(seconds / SecondsPerHour);
            seconds -= hours * SecondsPerHour;
            var minutes = (int)(seconds / SecondsPerMinute);
            seconds -= minutes * SecondsPerMinute;
            var sec = (int)seconds;
            return (years, monthes, days, hours, minutes, sec);
        }
        public static string ToShownTimeShort(this float seconds) {
            var (years, monthes, days, hours, minutes, sec) = SplitTime(seconds);
            if (years != 0)
                return $"{years}:{monthes}:{days}:{hours}:{minutes}:{sec}";
            else if (monthes != 0)
                return $"{monthes}:{days}:{hours}:{minutes}:{sec}";
            else if (days != 0)
                return $"{days}:{hours}:{minutes}:{sec}";
            else if (hours != 0)
                return $"{hours}:{minutes}:{sec}";
            else
                return $"{minutes}:{sec}";
        }
        public static string ToShownTime(this float seconds)
        {
            var (years, monthes, days, hours, minutes, sec) = SplitTime(seconds);
            if (years != 0)
            {
                if (monthes != 0)
                {
                    if (days != 0)
                        return $"{years} y {monthes} m {days} d";
                    else
                        return $"{years} y {monthes} m";
                }
                else
                    return $"{years} y";
            }
            if (monthes != 0)
            {
                if (days != 0)
                    return $"{monthes} m {days} d";
                else
                    return $"{monthes} m";
            }
            if (days != 0)
            {
                if (hours != 0)
                    return $"{days} d {hours} h";
                else
                    return $"{days} d";
            }
            if (hours != 0)
            {
                if (minutes != 0)
                {
                    if (sec != 0)
                        return $"{hours} h {minutes} m {sec} s";
                    else
                        return $"{hours} h {minutes} m";
                }
                else
                    return $"{hours} h";
            }
            else if (minutes != 0)
            {
                if (sec != 0)
                    return $"{minutes} m {sec} s";
                else
                    return $"{minutes} m";
            }
            else
                return $"{sec} s";
        }
        public static string ToShownPercents(this float coef)
            => $"{(int)(coef * 100)}%";
        public static string ToShownPercents(this double coef)
            => $"{(int)(coef * 100)}%";

        public static void ShowProgressbar(int curr, int max, TextMeshProUGUI label, Image progressFill)
        {
            if (label != null)
                label.text = $"{curr}/{max}";
            if (progressFill != null)
                progressFill.fillAmount = curr / (float)max;
        }
        public static void UpdateShownAnimation(string animation, float progress, Animator animator) {
            if (!animator.gameObject.activeInHierarchy) return;
            animator.Play(animation, 0, progress);
            animator.Update(0f);
        }
        public static Dictionary<string, AnimationClip> GetClipsDict(Animator animator) {
            var clips = new Dictionary<string, AnimationClip>();
            animator.runtimeAnimatorController.animationClips.ForEach(
                clip => clips.Add(clip.name, clip));
            return clips;
        }
    }
}