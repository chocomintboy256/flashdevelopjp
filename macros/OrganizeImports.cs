using System;
using System.IO;
using FlashDevelop;
using PluginCore;
using PluginCore.Managers;
using ScintillaNet;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore.FRService;
using System.Collections.Generic;

public class OrganizeImports
{
    private static MainForm mainForm;

    public static void Execute()
    {
        if (Globals.SciControl == null) return;

        ScintillaControl sci = Globals.SciControl;

        Int32 pos = sci.CurrentPos;
        sci.BeginUndoAction();

        ASContext context = (ASContext)ASContext.Context;
        List<ClassModel> classes = context.CurrentModel.Classes;

        FileModel currentModel = context.CurrentModel;
        MemberModel currentMember = classes[0].ToMemberModel();

        List<MemberModel> deleteList = new List<MemberModel>();

        List<String> usedClassPackageList = new List<String>();

        Int32 startPos = sci.PositionFromLine(context.CurrentModel.GetPublicClass().LineFrom);
        Int32 endPos = sci.PositionFromLine(context.CurrentModel.GetPublicClass().LineTo);

        string cword = "";
        string nword = "";

        Int32 lexer = sci.Lexer;
        sci.SetSel(startPos, startPos);

        while (sci.CurrentPos < endPos)
        {
            cword = sci.GetWordFromPosition(sci.CurrentPos);
            sci.WordRightEnd();
            nword = sci.GetWordFromPosition(sci.CurrentPos);
            if (cword != nword && nword != null)
            {
                cword = sci.GetWordFromPosition(sci.CurrentPos);
                sci.WordRightEnd();
                nword = sci.GetWordFromPosition(sci.CurrentPos);
                if (cword != nword) sci.WordLeftEnd();
            }
            else
            {
                while ((nword != cword && !(cword == null && nword != null)) || (cword == null && nword == null) && sci.Length > sci.CurrentPos)
                {
                    cword = sci.GetWordFromPosition(sci.CurrentPos);
                    sci.WordRightEnd();
                    nword = sci.GetWordFromPosition(sci.CurrentPos);
                }
            }

            if (sci.PositionIsOnComment(sci.CurrentPos, lexer)) continue;

            int position = sci.WordEndPosition(sci.CurrentPos, true);
            ASResult result = ASComplete.GetExpressionType(sci, position);

            if (!result.IsNull())
            {
                ClassModel oClass = (result.inClass != null) ? result.inClass
                                                             : result.Type;
                if (oClass != null)
                {
                    usedClassPackageList.Add(oClass.InFile.Package);
                }
            } 
            
        }

        sci.SetSel(sci.PositionFromLine(context.CurrentModel.GetPublicClass().LineFrom), sci.PositionFromLine(context.CurrentModel.GetPublicClass().LineTo));
        String publicClassText = sci.SelText;

        foreach (MemberModel import in currentModel.Imports)
        {
            if (import.Name == "*")
            {
                Boolean checker = false;

                foreach (String usedClassPackage in usedClassPackageList)
                {
                    if (usedClassPackage == import.Type.Substring(0, import.Type.Length - 2)) checker = true;
                }

                if (!checker) deleteList.Add(import);
                
            }
            else if (!TypeUsed(import.Name, publicClassText))
            {
                deleteList.Add(import);
            }
        }

        deleteList.Reverse();

        foreach (MemberModel import in deleteList)
        {
            sci.GotoLine(import.LineFrom);
            sci.LineDelete();
        }
        sci.SetSel(pos, pos);
        sci.EndUndoAction();

    }

    private static Boolean TypeUsed(String type, String searchInText)
    {
        SearchMatch result;
        String pattern = type;
        FRSearch search = new FRSearch(pattern);
        search.WholeWord = true;
        search.NoCase = !true;
        search.Filter |= SearchFilter.OutsideCodeComments;
        search.Filter |= SearchFilter.OutsideStringLiterals;
        search.NoCase = false;
        search.WholeWord = true;
        result = search.Match(searchInText);
        return result != null;
    }
}
