using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HtmlExporterPlugin
{
    class ImageProcessRunner
    {
        private class ImageProcessDef
        {
            public string path { get; set; }
            public string arguments { get; set; }
            public string workDir { get; set; }
            public bool noWindow { get; set; }
            public bool copyonly { get; set; }
            public string source { get; set; }
            public string dest { get; set; }
            public string destconverted { get; set; }

            public ImageProcessDef()
            {
            }
            
            public ImageProcessDef(ImageProcessDef Source)
            {
                path = Source.path;
                arguments = Source.arguments;
                workDir = Source.workDir;
                noWindow = Source.noWindow;
                copyonly = Source.copyonly;
                source = Source.source;
                dest = Source.dest;
                destconverted = Source.destconverted;
            }
        }

        public delegate void DelCallback(int current, int max);

        public int SuccesFullConverts = 0;
        public int FailedConverts = 0;
        public List<string> FailedFiles = new List<string>();
        
        private readonly List<ImageProcessDef> ImagesProcessDefList = new List<ImageProcessDef>();
        private CancellationTokenSource cancellationTokenSource = null;  

        private static int StartProcessWait(string path, string arguments, string workDir, bool noWindow = false)
        {
            var startupPath = path;
            if (path.Contains(".."))
            {
                startupPath = Path.GetFullPath(path);
            }

            var info = new ProcessStartInfo(startupPath)
            {
                Arguments = arguments,
                WorkingDirectory = string.IsNullOrEmpty(workDir) ? (new FileInfo(startupPath)).Directory.FullName : workDir
            };

            if (noWindow)
            {
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
            }

            using (var proc = Process.Start(info))
            {
                proc.WaitForExit();
                return proc.ExitCode;
            }
        }

        public void addImageProcess(bool copyonly, string source, string dest, string destconverted, string exepath, string arguments, string workDir, bool noWindow = false)
        {
            ImageProcessDef ImageProcess = new ImageProcessDef();
            ImageProcess.path = exepath;
            ImageProcess.arguments = arguments;
            ImageProcess.workDir = workDir;
            ImageProcess.noWindow = noWindow;
            ImageProcess.copyonly = copyonly;
            ImageProcess.source = source;
            ImageProcess.dest = dest;
            ImageProcess.destconverted = destconverted;
            ImagesProcessDefList.Add(ImageProcess);

        }

        public void Clear()
        {
            ImagesProcessDefList.Clear();
        }
        
        public void Stop()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }

        public int Count()
        {
            return ImagesProcessDefList.Count;
        }

        public void Start(int maxConcurrency, DelCallback CallBack)
        {
            using (SemaphoreSlim concurrencySemaphore = new SemaphoreSlim(maxConcurrency))
            {
                if (ImagesProcessDefList.Count == 0)
                {
                    return;
                }
                cancellationTokenSource = new CancellationTokenSource();
                int count = 0;
                FailedConverts = 0;
                SuccesFullConverts = 0;
                FailedFiles.Clear();
                int max = ImagesProcessDefList.Count();
                List<Task<bool>> tasks = new List<Task<bool>>();
                foreach (ImageProcessDef ImageProcess in ImagesProcessDefList)
                {
                    concurrencySemaphore.Wait();
                    Task<bool> t = Task.Factory.StartNew((object o) =>
                    {
                        try
                        {
                            ImageProcessDef ImageProcessDefTask = (ImageProcessDef)o;
                            
                            if (File.Exists(ImageProcessDefTask.dest))
                            {
                                File.Delete(ImageProcessDefTask.dest);
                            }
                            
                            Directory.CreateDirectory(Path.GetDirectoryName(ImageProcessDefTask.dest));
                            if (ImageProcessDefTask.copyonly)
                            {
                                bool needcopy = true;
                                if (File.Exists(ImageProcessDefTask.dest))
                                {
                                    needcopy = File.GetLastWriteTime(ImageProcessDefTask.source).CompareTo(
                                        File.GetLastWriteTime(ImageProcessDefTask.dest)) != 0;
                                }
                                if (needcopy)
                                {
                                    File.Copy(ImageProcessDefTask.source, ImageProcessDefTask.dest, true);
                                }

                                return File.Exists(ImageProcessDefTask.dest);
                            }
                            else
                            {
                                int exitCode = StartProcessWait(ImageProcessDefTask.path, ImageProcessDefTask.arguments, ImageProcessDefTask.workDir, ImageProcessDefTask.noWindow);

                                if (exitCode != 0)
                                {
                                    if (File.Exists(ImageProcessDefTask.destconverted))
                                    {
                                        File.Delete(ImageProcessDefTask.destconverted);
                                    }
                                    return false;
                                }
                                //copy file from original source if same filetype and original was smaller or copy anyways if conversion failed
                                if (Path.GetExtension(ImageProcessDefTask.source.ToLower()).Equals(Path.GetExtension(ImageProcessDefTask.destconverted.ToLower())))
                                {
                                    if (File.Exists(ImageProcessDefTask.destconverted))
                                    {
                                        long destsize = new System.IO.FileInfo(ImageProcessDefTask.destconverted).Length;
                                        long sourcesize = new System.IO.FileInfo(ImageProcessDefTask.source).Length;
                                        if (sourcesize < destsize)
                                        {
                                            File.Copy(ImageProcessDefTask.source, ImageProcessDefTask.destconverted, true);
                                        }
                                    }
                                    else
                                    {
                                        File.Copy(ImageProcessDefTask.source, ImageProcessDefTask.destconverted, true);
                                    }
                                }

                                return File.Exists(ImageProcessDefTask.destconverted);
                            }
                        }
                        finally
                        {
                            _ = concurrencySemaphore.Release();
                        }
                    }, new ImageProcessDef(ImageProcess), cancellationTokenSource.Token);
                    
                    tasks.Add(t);
                    count += 1;
                    CallBack(count, max);
                    
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }
                }
                try
                {
                    Task.WaitAll(tasks.ToArray());
                    
                    foreach (Task<bool> task in tasks)
                    {
                        if (task.Result)
                        {
                            SuccesFullConverts += 1;
                        }
                        else
                        {
                            FailedConverts += 1;
                            ImageProcessDef tmp = (ImageProcessDef)task.AsyncState;
                            if (tmp != null)
                            {
                                FailedFiles.Add(tmp.source);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {                 
                }
                finally
                {
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
                }
            }
        }
    }
}
