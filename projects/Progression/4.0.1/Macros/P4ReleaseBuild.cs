using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;
using FlashDevelop;
using PluginCore;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore.Localization;
using ProjectManager.Helpers;
using ProjectManager.Projects;
using ProjectManager.Projects.AS2;
using ProjectManager.Projects.AS3;

public class P4ReleaseBuild
{
	private static string VERSION = "1.0.1";
	
	public static void Execute()
	{
		Project project = (Project) PluginBase.CurrentProject;
		if (project == null) return;
		
		TraceManager.Add("P4ReleaseBuild : version " + VERSION);
		
		mainForm = Globals.MainForm;

		// setup FDProcess helper class
		fdProcess = new FDProcessRunner(mainForm);
		
		Build(project, true);
	}
	
	static IMainForm mainForm;
	static FDProcessRunner fdProcess;
	static bool usingProjectDefinedCompiler;
	
	private static bool Build(Project project, bool runOutput)
	{
		// save modified files
		mainForm.CallCommand("SaveAllModified", null);
		
		string compiler = null;
		if (project.NoOutput) {
			// get the compiler for as3 projects, or else the FDBuildCommand pre/post command in FDBuild will fail on "No Output" projects
			if (project.Language == "as3")
				compiler = GetCompilerPath(project);
			
			if (project.PreBuildEvent.Trim().Length == 0 && project.PostBuildEvent.Trim().Length == 0) {
				// no output and no build commands
				if (project is AS2Project || project is AS3Project) {
					ErrorManager.ShowInfo("このプロジェクトはなにも出力せず、コマンドも実行しません。");
				}
				else {
					ErrorManager.ShowInfo("このプロジェクトは有効な AS2 / AS3 プロジェクトではありません。");
				}
				return false;
			}
		}
		else {
			// Ask the project to validate itself
			string error;
			project.ValidateBuild(out error);

			if (error != null) {
				ErrorManager.ShowInfo(TextHelper.GetString(error));
				return false;
			}

			// 出力先が空だったとき
			if (project.OutputPath.Length < 1) {
				String info = "プロジェクトをビルドするには、プロジェクトプロパティに SWF ファイルの有効な出力先を設定する必要があります。";
				ErrorManager.ShowInfo(info);
				return false;
			}

			compiler = GetCompilerPath(project);
			if (compiler == null || (!Directory.Exists(compiler) && !File.Exists(compiler))) {
				MessageBox.Show("Flex SDK へのパスが間違っています。プロジェクトプロパティのコンパイル設定を修正してください。", "エラー", MessageBoxButtons.OK);
				return false;
			}
		}

		return FDBuild(project, runOutput, compiler);
	}
	
	private static bool FDBuild(Project project, bool runOutput, string compiler)
	{
		string fdBuildDir = Path.Combine(PathHelper.ToolDir, "fdbuild");
		string fdBuildPath = Path.Combine(fdBuildDir, "fdbuild.exe");

		string arguments = "";
		
		if (compiler != null && compiler.Length > 0)
			arguments += " -compiler \"" + compiler + "\"";

		arguments += " -notrace";			
		arguments += " -library \"" + PathHelper.LibraryDir + "\"";

		foreach (string cp in ProjectManager.PluginMain.Settings.GlobalClasspaths)
			arguments += " -cp \"" + Environment.ExpandEnvironmentVariables(cp) + "\"";
		
		arguments += " -cp \"" + project.Directory + "\\libs-release\\" + "\"";		
		arguments = arguments.Replace("\\\"", "\""); // c# console args[] bugfix

		fdProcess.StartProcess(
			fdBuildPath,
			"\"" + project.ProjectPath + "\"" + arguments,
			project.Directory,
			delegate(bool success) {}
		);
		
		return true;
	}
	
	// Retrieve the project language's default compiler path
	private static string GetCompilerPath(Project project)
	{
		if (project.Language == "as3" && (project as AS3Project).CompilerOptions.CustomSDK.Length > 0)
		{
			usingProjectDefinedCompiler = true;
			return PathHelper.ResolvePath((project as AS3Project).CompilerOptions.CustomSDK, project.Directory);
		}
		usingProjectDefinedCompiler = false;

		Hashtable info = new Hashtable();
		info["language"] = project.Language;
		DataEvent de = new DataEvent(EventType.Command, "ASCompletion.GetCompilerPath", info);
		EventManager.DispatchEvent(project, de);
		if (de.Handled && info.ContainsKey("compiler")) 
			return PathHelper.ResolvePath(info["compiler"] as string, project.Directory);
		else return null;
	}
}
