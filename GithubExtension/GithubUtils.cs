using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GithubExtension
{
    public static class GithubUtils
    {
        /// <summary>
        /// Gets the path's git repository root directory
        /// </summary>
        /// <param name="path">File path or directory</param>
        /// <returns>git repository root directory, or string.Empty</returns>
        public static string GetGitRootDirectory(string path)
        {
            Debug.WriteLine("Getting git root of " + path);
            var ret = path;
            while (ret != string.Empty)
            {
                var parent = Directory.GetParent(ret);
                if (parent == null)
                    return string.Empty;

                ret = parent.FullName;
                Debug.WriteLine("Parent is " + ret);
                if (Directory.GetDirectories(ret, ".git", SearchOption.TopDirectoryOnly).Any())
                {
                    Debug.WriteLine("This is the git repository root.");
                    return ret;
                }
            }
            Debug.WriteLine(path + " does not appear to be part of a git repository!");
            return ret;
        }

        /// <summary>
        /// Gets the remote origin url for the repository
        /// </summary>
        /// <param name="gitRootPath">Path to the git repository</param>
        /// <returns>Remote origin url, or string.Empty</returns>
        public static string GetRepoUrl(string gitRootPath)
        {
            var searchRegex = @"url = (\S+)";

            if (!File.Exists(gitRootPath + "\\.git\\config"))
            {
                Debug.WriteLine("No config file found at " + gitRootPath + "\\.git");
                return string.Empty;
            }

            var gitConfigContents = File.ReadAllText(gitRootPath + "\\.git\\config");
            var originNodeHeader = "[remote \"origin\"]";
            var originNodeLocation = gitConfigContents.IndexOf(originNodeHeader);
            var originNodeEnding = gitConfigContents.IndexOf("[", originNodeLocation+1);
            originNodeEnding = (originNodeEnding == -1)? gitConfigContents.Length: originNodeEnding;
            var originNode = gitConfigContents.Substring(originNodeLocation, originNodeEnding - originNodeLocation);

            var regexMatch = Regex.Match(originNode, searchRegex, RegexOptions.IgnoreCase);
            if (!regexMatch.Success)
            {
                Debug.WriteLine("Repository config file does not specify an origin URL");
                return string.Empty;
            }

            return regexMatch.Groups[1].Value;
        }

        /// <summary>
        /// Get the current branch name
        /// </summary>
        /// <param name="gitBinLocation">Location of git executable</param>
        /// <param name="gitPath">File path in the git repository</param>
        /// <returns>Current branch name, or string.Empty</returns>
        public static string GetCurrentBranch(string gitBinLocation, string gitPath)
        {
            // TODO ensure git bin exists

            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WorkingDirectory = gitPath;
            p.StartInfo.FileName = gitBinLocation;
            p.StartInfo.Arguments = "rev-parse --abbrev-ref HEAD";
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output.StartsWith("fatal:") ? string.Empty : output.Trim();
        }

        /// <summary>
        /// Gets this file's representation on github
        /// </summary>
        /// <param name="localFileLocation"></param>
        /// <returns></returns>
        public static string GetGithubFileLocation(string localFileLocation, string gitBinLocation)
        {
            var gitRootPath = GetGitRootDirectory(localFileLocation);

            if (gitRootPath == string.Empty)
            {
                MessageBox.Show("This file is not part of a git repository!"); // TODO turn these into custom exceptions
                return string.Empty;
            }

            var githubRepoUrl = GetRepoUrl(gitRootPath);

            if (githubRepoUrl == string.Empty)
            {
                MessageBox.Show("This repository has no origin configured!");
                return string.Empty;
            }

            if (!githubRepoUrl.EndsWith(".git"))
            {
                MessageBox.Show("This repository has an unsupported origin configuration!");
                return string.Empty;
            }

            //var gitBin = @"C:\Users\Dallas\AppData\Local\GitHub\PortableGit_054f2e797ebafd44a30203088cd3d58663c627ef\bin\git.exe";
            var branchName = GetCurrentBranch(gitBinLocation, gitRootPath);
            // TODO figure out what to do in event that branchName isn't specified

            var fileSubPath = localFileLocation.Substring(localFileLocation.IndexOfDivergence(gitRootPath));

            var fileUrl = githubRepoUrl.Substring(0, githubRepoUrl.LastIndexOf(".git"));
            fileUrl += "/blob/" + branchName + fileSubPath.Replace("\\", "/");

            return fileUrl;
        }
    }
}
