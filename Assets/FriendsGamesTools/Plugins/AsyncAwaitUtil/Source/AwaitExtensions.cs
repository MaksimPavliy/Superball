using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class AwaitExtensions
{
    static int count = 0;
    public static List<string> names = new List<string>();
    public static List<float> times = new List<float>();
    public static TaskAwaiter<int> GetAwaiter(this Process process)
    {
        var tcs = new TaskCompletionSource<int>();
        process.EnableRaisingEvents = true;

        process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode);

        if (process.HasExited)
        {
            tcs.TrySetResult(process.ExitCode);
        }

        return tcs.Task.GetAwaiter();
    }

    // Any time you call an async method from sync code, you can either use this wrapper
    // method or you can define your own `async void` method that performs the await
    // on the given Task
    public static async void WrapErrors(this Task task, string name)
    {
        count++;
        names.Add(name);
        times.Add(Time.time);
        //UnityEngine.Debug.Log($"inc count to {count}");
        await task;
        count--;
        var ind = names.IndexOf(name);
        names.RemoveAt(ind);
        times.RemoveAt(ind);
        //UnityEngine.Debug.Log($"dec count to {count}");
    }

    // Any time you call an async method from sync code, you can either use this wrapper
    // method or you can define your own `async void` method that performs the await
    // on the given Task
    public static async void WrapErrors(this Task task) => await task;
    public static async void StartAsync(this Task task) => await task;

    public static string debugText
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < names.Count; i++)
                sb.AppendLine($"{(int)(Time.time - times[i])}{names[i]}");
            return sb.ToString();
        }
    }
}
