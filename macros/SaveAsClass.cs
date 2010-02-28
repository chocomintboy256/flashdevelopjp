using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using FlashDevelop;
using PluginCore;
using PluginCore.Managers;
using PluginCore.Helpers;
using ProjectManager.Projects;
using ScintillaNet;
using ProjectManager.Actions;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;

public class SaveAsClass
{
	public static void Execute()
	{
		if ( Globals.SciControl == null ) return;
		ScintillaControl sci = Globals.SciControl;

		//Project project = (Project) PluginBase.CurrentProject;
		//if (project == null) return;

		MainForm mainForm = (MainForm) Globals.MainForm;

		ITabbedDocument document = mainForm.CurrentDocument;

		String dir = Path.GetDirectoryName(document.FileName);
		Hashtable details = ASComplete.ResolveElement(sci, null);

		IASContext context = ASContext.Context;
		ClassModel cClass = context.CurrentClass;
		string package = context.CurrentModel.Package;

		String content = Regex.Replace(
		sci.SelText,
		@"^",
		"\t",
		RegexOptions.Multiline);

		String contents = "package " + package + " {\n" + content + "\n}";
		//TraceManager.Add(contents);

		Encoding encoding = sci.Encoding;
		String file = dir + "\\" + cClass.Name + ".as";
		FileHelper.WriteFile(file, contents, encoding);
	}
}