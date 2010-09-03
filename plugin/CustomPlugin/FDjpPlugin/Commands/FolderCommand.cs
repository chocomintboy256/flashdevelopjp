using System;
using System.Collections.Generic;
using System.Text;
using ScintillaNet;
using PluginCore.FRService;

namespace FDjpPlugin.Commands
{
    class FolderCommand : SCICommand
    {
        private bool fold;
        private bool foldCommentsToggle;

        public FolderCommand(ScintillaControl sci, bool isFold, bool isFoldCommentsToggle)
            : base(sci)
        {
            fold = isFold;
            foldCommentsToggle = isFoldCommentsToggle;
        }

        public override void Execute()
        {
            // スクロール位置を保持
            Int32 scrollTop = sci.FirstVisibleLine;

            // コメントの開始を検索。検索だと重いので他の方法がいいんですけどわからなくて・・・
            //String commentStart = ScintillaManager.GetCommentStart(sci.ConfigurationLanguage); //これ使いたいんですけどプロテクトがかかってる・・・
            String commentStart = "/*";
            List<SearchMatch> matches = this.searchString(sci, commentStart);

            if (matches != null && matches.Count != 0)
            {
                Int32 lexer = sci.Lexer;
                Int32 fldNum = 0; // 折りたたみ/展開の対象になった個数
                Int32 expNum = 0; // 折りたたみ/展開されなかった個数
                foreach (SearchMatch match in matches)
                {
                    Int32 pos = sci.MBSafePosition(match.Index);
                    Int32 line = sci.LineFromPosition(pos);
                    Int32 foldParentLine = sci.FoldParent(line + 1);
                    if (foldParentLine == line)
                    {
                        fldNum++;
                        Boolean isExpanded = sci.FoldExpanded(foldParentLine);
                        if (fold)
                        {
                            if (isExpanded)
                            {
                                sci.ToggleFold(line);
                            }
                            else
                            {
                                expNum++;
                            }
                        }
                        else
                        {
                            if (!isExpanded)
                            {
                                sci.ToggleFold(line);
                            }
                            else
                            {
                                expNum++;
                            }
                        }
                    }
                }

                // 折りたたみ・展開するものがなかったら逆の動きをする
                if (fldNum == expNum && foldCommentsToggle)
                {
                    fold = !fold;
                    Execute();

                    return;
                }
            }

            // スクロール位置を元に戻す
            if (scrollTop != sci.FirstVisibleLine)
            {
                sci.LineScroll(0, scrollTop - sci.FirstVisibleLine);
            }
        }

        // 文字列検索
        private List<SearchMatch> searchString(ScintillaControl sci, String text)
        {
            FRSearch search = new FRSearch(text);
            return search.Matches(sci.Text);
        }
    }
}
