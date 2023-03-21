using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FriendsGamesTools
{
    // Launching process and getting its string output and errors.
    public class ProcessLauncher
    {
        public bool logging;
        
        Process process;
        public async Task<(bool success, string output, string error)> 
            Execute(string fileName, string arguments, Action<string> onOutputLineReceived = null, Action<string> onErrorLineReceived = null)
        {
            // Log input.
            string commandLineInput;
            if (Application.platform == RuntimePlatform.WindowsEditor)
                commandLineInput = $"&'{fileName}' --% {arguments}";
            else
                commandLineInput = $"'{fileName}' {arguments}";
            UnityEngine.Debug.Log($">{commandLineInput}");

            var sbOutput = new StringBuilder();
            var sbError = new StringBuilder();

            // Create process.
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fileName;
            startInfo.Arguments = arguments;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            if (process != null && !process.HasExited)
                process.Kill();
            process = new Process();
            process.StartInfo = startInfo;

            // Start process with events.
            process.EnableRaisingEvents = true;
            var outputReceivedHandler = new DataReceivedEventHandler((s, e) =>
            {
                var outputLine = e.Data;
                sbOutput.AppendLine(outputLine);
                onOutputLineReceived?.Invoke(outputLine);
                if (logging)
                    UnityEngine.Debug.Log($"<{outputLine}");
            });
            var errorReceivedHandler = new DataReceivedEventHandler((s, e) =>
            {
                var outputLine = e.Data;
                sbError.AppendLine(outputLine);
                onErrorLineReceived?.Invoke(outputLine);
                if (logging)
                    UnityEngine.Debug.LogError($"<{outputLine}");
            });
            process.OutputDataReceived += outputReceivedHandler;
            process.ErrorDataReceived += errorReceivedHandler;
            try
            {
                process.Start();
            } catch (Exception e)
            {
                if (logging)
                    UnityEngine.Debug.LogError(e.Message);
                return (false, string.Empty, e.Message);
            }
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            while (!process.HasExited)
                await Awaiters.EndOfFrame;

            process.OutputDataReceived -= outputReceivedHandler;
            process.ErrorDataReceived -= errorReceivedHandler;
            return (true, sbOutput.ToString(), sbError.ToString());
        }        
    }
}