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
using PluginCore.FRService;
using System.Collections.Generic;
using FlashDevelop.Utilities;

public class SaveAsClasses
{
	public static void Execute()
	{
		if ( Globals.SciControl == null ) return;
		ScintillaControl sci = Globals.SciControl;

		MainForm mainForm = (MainForm) Globals.MainForm;

		ITabbedDocument document = mainForm.CurrentDocument;

		String documentDirectory = Path.GetDirectoryName(document.FileName);

		ASContext context = (ASContext) ASContext.Context;
		string currentPackage = context.CurrentModel.Package;
		List<ClassModel> classes = context.CurrentModel.Classes;
		MemberList imports = context.CurrentModel.Imports;
		
		sci.BeginUndoAction();
		
		String pattern = "}";
		FRSearch search = new FRSearch(pattern);
		search.Filter = SearchFilter.None;
		List<SearchMatch> matches = search.Matches(sci.Text);
		if (matches == null || matches.Count == 0) return;
		if (classes.Count < 2) return;
		
		SearchMatch match = GetNextDocumentMatch(sci, matches, sci.PositionFromLine(classes[0].LineTo + 1));
		
		Int32 packageEnd = sci.MBSafePosition(match.Index) + sci.MBSafeTextLength(match.Value); // wchar to byte text length
		
		String strImport = "";
		
		if (imports.Count > 0)
		{
			for (int i=imports.Count-1; i>0; --i ) 
			{
				MemberModel import = imports[i];
				if(packageEnd < sci.PositionFromLine(import.LineFrom)) {
					sci.SelectionStart = sci.PositionFromLine(import.LineFrom);
					sci.SelectionEnd = sci.PositionFromLine(import.LineTo + 1);
					strImport = sci.SelText + strImport;
					sci.Clear();
				}
			}
		}
		
		context.UpdateCurrentFile(true);
		
		Int32 prevClassEnd = packageEnd;
		
		foreach (ClassModel currentClass in classes)
		{
			// 最初のクラスは無視
			if (currentClass == classes[0]) continue;
			
			sci.SelectionStart = prevClassEnd;
			sci.SelectionEnd = sci.PositionFromLine(currentClass.LineTo + 1);
			
			String content = "package "
					   + currentPackage
					   + "\n{\n"
					   + Regex.Replace( strImport, @"^", "\t", RegexOptions.Multiline)
					   + Regex.Replace( sci.SelText, @"^", "\t", RegexOptions.Multiline)
					   + "\n}\n";
					   
			prevClassEnd = sci.PositionFromLine(currentClass.LineTo + 1);
			
			Encoding encoding = sci.Encoding;
			String file = documentDirectory + "\\" + currentClass.Name + ".as";
			FileHelper.WriteFile(file, content, encoding);
		}
		
		sci.SelectionStart = packageEnd;
		sci.Clear();		
		sci.EndUndoAction();
	}

	public static SearchMatch GetNextDocumentMatch(ScintillaControl sci, List<SearchMatch> matches, int position)
	{
		SearchMatch nearestMatch = matches[0];
		Int32 currentPosition = sci.MBSafeCharPosition(position);
		for (Int32 i = 0; i < matches.Count; i++)
		{
			if (currentPosition > matches[matches.Count - 1].Index)
			{
				return matches[0];
			}
			if (matches[i].Index >= currentPosition)
			{
				return matches[i];
			}
		}
		return nearestMatch;
	}
	
}