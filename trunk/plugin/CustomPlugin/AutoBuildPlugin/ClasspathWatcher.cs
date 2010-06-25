using System;
using System.Collections.Generic;
using System.IO;
using PluginCore;
using PluginCore.Managers;
using ASCompletion.Model;
using ASCompletion.Context;
using System.Windows.Forms;

namespace AutoBuildPlugin
{
    public delegate void ClasspathWathcerChangedHandler();

    public class ClasspathWatcher
    {
        private FileSystemWatcher[] watchers;

        ClasspathWathcerChangedHandler classpathWathcerChangedHandler;
        private DelayExecuter delex;

        public ClasspathWatcher(ToolStrip ui, IProject project, ClasspathWathcerChangedHandler callback)
        {
            classpathWathcerChangedHandler = callback;

            delex = new DelayExecuter(1000, delegate() { classpathWathcerChangedHandler(); });

            int g = 0;
            foreach (PathModel path in ASContext.Context.Classpath)
            {
                if (path.Path.ToString().Length > 2 && !path.Path.ToString().Contains(".swc"))
                {
                    g++;
                }
            }

            String[] _ary = new String[g];

            int n = 0;
            foreach (PathModel path in ASContext.Context.Classpath)
            {
                if (path.Path.ToString().Length > 2 && !path.Path.ToString().Contains(".swc"))
                {
                    _ary[n] = path.Path.ToString();
                    //Console.WriteLine(_ary[n]);
                    n++;
                }
            }

            //TraceManager.Add(n.ToString());

            watchers = new FileSystemWatcher[_ary.Length];

            for (int i = 0, ix = _ary.Length; i < ix; i++)
            {
                watchers[i] = new System.IO.FileSystemWatcher();
                //監視するディレクトリを指定
                watchers[i].Path = _ary[i];
                //最終アクセス日時、最終更新日時、ファイル、フォルダ名の変更を監視する
                watchers[i].NotifyFilter =
                    (System.IO.NotifyFilters.LastWrite
                    | System.IO.NotifyFilters.FileName
                    | System.IO.NotifyFilters.DirectoryName);
                //拡張子
                watchers[i].Filter = "";

                watchers[i].IncludeSubdirectories = true;

                //UIのスレッドにマーシャリングする
                watchers[i].SynchronizingObject = ui;

                //イベントハンドラの追加
                watchers[i].Changed += new System.IO.FileSystemEventHandler(watcher_Changed);
                watchers[i].Created += new System.IO.FileSystemEventHandler(watcher_Changed);
                watchers[i].Deleted += new System.IO.FileSystemEventHandler(watcher_Changed);
                watchers[i].Renamed += new System.IO.RenamedEventHandler(watcher_Renamed);

                //監視を開始する
                watchers[i].EnableRaisingEvents = true;
            }


            

        }

        private void watcher_Changed(System.Object source, System.IO.FileSystemEventArgs e)
        {
            //Console.WriteLine("watcher_Changed");
            switch (e.ChangeType)
            {
                case System.IO.WatcherChangeTypes.Changed:
                    delex.TimerStart();
                    break;
                    /*
                case System.IO.WatcherChangeTypes.Created:
                    //TraceManager.Add("ファイル 「" + e.FullPath + "」が作成されました。");
                    break;
                case System.IO.WatcherChangeTypes.Deleted:
                    //TraceManager.Add("ファイル 「" + e.FullPath + "」が削除されました。");
                    break;
                     */
            }
        }

        private void watcher_Renamed(System.Object source, System.IO.RenamedEventArgs e)
        {
            //Console.WriteLine("watcher_Renamed");
            delex.TimerStart();
            //classpathWathcerChangedHandler();
        }

        public void Dispose()
        {
            //Console.WriteLine("Dispose");
            if (watchers == null) return;

            for (int i = watchers.GetLowerBound(0); i < watchers.GetUpperBound(0); i++)
            {
                //監視を終了
                watchers[i].EnableRaisingEvents = false;
                watchers[i].Dispose();
                watchers[i] = null;
            }
            watchers = null;

            delex.Dispose();
        }
    }
}
