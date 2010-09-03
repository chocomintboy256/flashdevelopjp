using System;
using System.Collections.Generic;
using ASCompletion.Completion;
using PluginCore.FRService;
using CodeRefactor.Commands;
using PluginCore.Localization;
using PluginCore.Managers;
using ScintillaNet;
using PluginCore;
using FlashDevelop;
using FDjpPlugin.Provider;
using System.Windows.Forms;
using FDjpPlugin.Resources;

namespace FDjpPlugin.Commands
{
    public class GotoSameIdentifierCommand : RefactorCommand<IDictionary<String, List<SearchMatch>>>
    {
        private ASResult currentTarget;
        private Boolean outputResults;
        private Boolean ignoreDeclarationSource;
        public int curStartPos;
        public int curEndPos;
        public int spos;
        public int epos;
        public bool back = false;

        /// <summary>
        /// The current declaration target that references are being found to.
        /// </summary>
        public ASResult CurrentTarget
        {
            get { return this.currentTarget; }
        }

        /// <summary>
        /// A new FindAllReferences refactoring command. Outputs found results.
        /// Uses the current text location as the declaration target.
        /// </summary>
        public GotoSameIdentifierCommand()
            : this(true)
        {
        }

        /// <summary>
        /// A new FindAllReferences refactoring command.
        /// Uses the current text location as the declaration target.
        /// </summary>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        public GotoSameIdentifierCommand(Boolean output)
            : this(RefactoringHelpera.GetDefaultRefactorTarget(), output)
        {
            this.outputResults = output;
        }

        /// <summary>
        /// A new FindAllReferences refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        public GotoSameIdentifierCommand(ASResult target, Boolean output)
            : this(target, output, false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        public GotoSameIdentifierCommand(ASResult target, Boolean output, Boolean ignoreDeclarations)
        {
            this.currentTarget = target;
            this.outputResults = output;
            this.ignoreDeclarationSource = ignoreDeclarations;
        }

        #region RefactorCommand Implementation

        /// <summary>
        /// Entry point to execute finding.
        /// </summary>
        protected override void ExecutionImplementation()
        {
            RefactoringHelpera.FindTargetInCurrentFile(currentTarget, new FRFinishedHandler(this.FindFinished), true);
        }

        /// <summary>
        /// Indicates if the current settings for the refactoring are valid.
        /// </summary>
        public override Boolean IsValid()
        {
            return this.currentTarget != null;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Invoked when the FRSearch completes its search
        /// </summary>
        private void FindFinished(FRResults results)
        {
            GotoNextMatch(results, currentTarget);
            this.FireOnRefactorComplete();
        }

        /// <summary>
        /// Filters the initial result set by determining which entries actually resolve back to our declaration target.
        /// </summary>
        private void GotoNextMatch(FRResults results, ASResult target)
        {
            IDictionary<String, List<SearchMatch>> initialResultsList = RefactoringHelpera.GetInitialResultsList(results);
            Boolean foundDeclarationSource = false;
            foreach (KeyValuePair<String, List<SearchMatch>> entry in initialResultsList)
            {
                String currentFileName = entry.Key;
                ScintillaControl sci = (ScintillaControl) this.AssociatedDocumentHelper.LoadDocument(currentFileName);
                if(back) entry.Value.Reverse();
                
                foreach (SearchMatch match in entry.Value)
                {
                    // if the search result does point to the member source, store it
                    if (RefactoringHelpera.DoesMatchPointToTarget(sci, match, target, this.AssociatedDocumentHelper))
                    {
                        if (ignoreDeclarationSource && !foundDeclarationSource && RefactoringHelpera.IsMatchTheTarget(sci, match, target))
                        {
                            //ignore the declaration source
                            foundDeclarationSource = true;
                        }
                        else
                        {
                            int ws = sci.PositionFromLine(match.Line - 1) + match.Column;
                            if (!back)
                            {
                                if (ws > curEndPos)
                                {
                                    sci.SelectionStart = ws;
                                    sci.SelectionEnd = ws + match.Length;
                                    sci.ScrollCaret();
                                    return;
                                }
                            }
                            else
                            {
                                if (ws < curStartPos)
                                {
                                    sci.SelectionStart = ws;
                                    sci.SelectionEnd = ws + match.Length;
                                    sci.ScrollCaret();
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            Globals.SciControl.SelectionStart = spos;
            Globals.SciControl.SelectionEnd = epos;
            string str = Globals.SciControl.GetWordFromPosition(spos);
            MessageBox.Show(LocaleHelper.GetString("MESSAGE_NOT_FOUND") + str, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        #endregion

    }

}
