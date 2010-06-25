using System;
using System.Collections.Generic;
using ProjectManager.Projects.AS2;
using ProjectManager.Projects.AS3;
using PluginCore.Managers;
using System.Windows.Forms;
using ProjectManager.Actions;
using System.IO;
using PluginCore.Helpers;
using PluginCore.Localization;
using ProjectManager.Helpers;
using ProjectManager.Projects;
using FlashDevelop;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using PluginCore;

namespace AutoBuildPlugin
{
    public delegate void BuildCompleteHandler(bool runOutput);

    class BuildAction
    {
        public BuildAction()
        {
            fdProcess = new FDProcessRunner(Globals.MainForm);
        }

        private FDProcessRunner fdProcess;

        public event BuildCompleteHandler BuildComplete;
        public event BuildCompleteHandler BuildFailed;

        string ipcName;

        public bool Build(Project project, bool runOutput)
        {
            ipcName = Globals.MainForm.ProcessArgString("$(BuildIPC)");

            string compiler = null;
            if (project.NoOutput)
            {
                // get the compiler for as3 projects, or else the FDBuildCommand pre/post command in FDBuild will fail on "No Output" projects
                if (project.Language == "as3")
                    compiler = ProjectManager.Actions.BuildActions.GetCompilerPath(project);

                if (project.PreBuildEvent.Trim().Length == 0 && project.PostBuildEvent.Trim().Length == 0)
                {
                    // no output and no build commands
                    if (project is AS2Project || project is AS3Project)
                    {
                        //RunFlashIDE(runOutput);
                        //ErrorManager.ShowInfo(TextHelper.GetString("ProjectManager.Info.NoOutputAndNoBuild"));
                    }
                    else
                    {
                        //ErrorManager.ShowInfo("Info.InvalidProject");
                        ErrorManager.ShowInfo(TextHelper.GetString("ProjectManager.Info.NoOutputAndNoBuild"));
                    }
                    return false;
                }
            }
            else
            {
                // Ask the project to validate itself
                string error;
                project.ValidateBuild(out error);

                if (error != null)
                {
                    ErrorManager.ShowInfo(TextHelper.GetString(error));
                    return false;
                }

                // 出力先が空だったとき
                if (project.OutputPath.Length < 1)
                {
                    ErrorManager.ShowInfo(TextHelper.GetString("ProjectManager.Info.SpecifyValidOutputSWF"));
                    return false;
                }

                compiler = BuildActions.GetCompilerPath(project);
                if (compiler == null || (!Directory.Exists(compiler) && !File.Exists(compiler)))
                {
                    string info = TextHelper.GetString("ProjectManager.Info.InvalidCustomCompiler");
                    MessageBox.Show(info, TextHelper.GetString("ProjectManager.Title.ConfigurationRequired"), MessageBoxButtons.OK);
                    return false;
                }
            }

            return FDBuild(project, runOutput, compiler);
             
        }
        /*
        private void RunFlashIDE(bool runOutput, bool noTrace)
        {
            string cmd = (runOutput) ? "testmovie" : "buildmovie";
            //if (!PluginMain.Settings.DisableExtFlashIntegration) cmd += "-fd";

            cmd += ".jsfl";
            if (!noTrace) cmd = "debug-" + cmd;

            cmd = Path.Combine("Tools", Path.Combine("flashide", cmd));
            cmd = PathHelper.ResolvePath(cmd, null);
            if (cmd == null || !File.Exists(cmd))
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.JsflNotFound"));
            }
            else
            {
                DataEvent de = new DataEvent(EventType.Command, "ASCompletion.CallFlashIDE", cmd);
                EventManager.DispatchEvent(this, de);
            }
        }
        */
        private bool FDBuild(Project project, bool runOutput, string compiler)
        {
            string fdBuildDir = Path.Combine(PathHelper.ToolDir, "fdbuild");
            string fdBuildPath = Path.Combine(fdBuildDir, "fdbuild.exe");

            string arguments = " -ipc " + ipcName;

            if (compiler != null && compiler.Length > 0)
                arguments += " -compiler \"" + compiler + "\"";

            //arguments += " -notrace";
            arguments += " -library \"" + PathHelper.LibraryDir + "\"";

            foreach (string cp in ProjectManager.PluginMain.Settings.GlobalClasspaths)
                arguments += " -cp \"" + Environment.ExpandEnvironmentVariables(cp) + "\"";

            arguments = arguments.Replace("\\\"", "\""); // c# console args[] bugfix

            fdProcess.StartProcess(fdBuildPath, "\"" + project.ProjectPath + "\"" + arguments,
                 project.Directory, delegate(bool success)
                 {
                     //menus.DisabledForBuild = false;
                     //menus.ConfigurationSelector.Enabled = true; // !project.NoOutput;
                     if (success)
                     {
                         //TraceManager.Add("Builded!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                         //SetStatusBar(TextHelper.GetString("Info.BuildSucceeded"));
                         AddTrustFile(project);
                         OnBuildComplete(runOutput);
                     }
                     else
                     {
                         //TraceManager.Add("NON!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                         //SetStatusBar(TextHelper.GetString("Info.BuildFailed"));
                         OnBuildFailed(runOutput);
                     }
                     
                 }
             );

            return true;
        }

        void OnBuildComplete(bool runOutput)
        {
            if (BuildComplete != null) BuildComplete(runOutput);
        }

        void OnBuildFailed(bool runOutput)
        {
            if (BuildFailed != null) BuildFailed(runOutput);
        }

        void AddTrustFile(Project project)
        {
            string directory = Path.GetDirectoryName(project.OutputPathAbsolute);
            if (!Directory.Exists(directory)) return;
            string trustParams = "FlashDevelop.cfg;" + directory;
            DataEvent de = new DataEvent(EventType.Command, "ASCompletion.CreateTrustFile", trustParams);
            EventManager.DispatchEvent(this, de);
        }

        public void NotifyBuildStarted() { fdProcess.ProcessStartedEventCaught(); }
        public void NotifyBuildEnded(string result) { fdProcess.ProcessEndedEventCaught(result); }
        public void SetStatusBar(string text) { Globals.MainForm.StatusLabel.Text = " " + text; }
    }
}
