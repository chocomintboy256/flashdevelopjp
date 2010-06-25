using System;
using System.Collections.Generic;
using System.IO;
using PluginCore;
using PluginCore.Managers;
using System.Windows.Forms;

namespace AutoBuildPlugin
{
    public delegate void SingleFileWathcerChangedHandler();

    public class SingleFileWatcher
    {
        private FileSystemWatcher watcher;

        SingleFileWathcerChangedHandler singleFileWathcerChangedHandler;

        private DelayExecuter delex;

        public SingleFileWatcher(ToolStrip ui, string path, SingleFileWathcerChangedHandler callback)
        {
            singleFileWathcerChangedHandler = callback;

            delex = new DelayExecuter(1000, delegate() { singleFileWathcerChangedHandler(); });
           
            watcher = new System.IO.FileSystemWatcher();
            //監視するディレクトリを指定
            watcher.Path = Path.GetDirectoryName(path);
            //監視するファイルを指定
            watcher.Filter = Path.GetFileName(path);
            //最終更新日時、ファイルサイズの変更を監視する
            watcher.NotifyFilter =
                (System.IO.NotifyFilters.Size
                |System.IO.NotifyFilters.LastWrite);
            //UIのスレッドにマーシャリングする
            watcher.SynchronizingObject = ui;

            //イベントハンドラの追加
            watcher.Changed += new System.IO.FileSystemEventHandler(watcherChanged);
            watcher.Created += new System.IO.FileSystemEventHandler(watcherChanged);

            //監視を開始する
            watcher.EnableRaisingEvents = true;
        }

        private void watcherChanged(System.Object source, System.IO.FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case System.IO.WatcherChangeTypes.Changed:
                case System.IO.WatcherChangeTypes.Created:
                    delex.TimerStart();
                    break;
            }
        }

        public void Dispose(){
            //監視を終了
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            watcher = null;

            delex.Dispose();
        }
    }
}
