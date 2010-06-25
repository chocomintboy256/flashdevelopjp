using System;
using System.Collections.Generic;
using System.Text;
using FlashDevelop;
using PluginCore.Managers;
using ScintillaNet;
using FlashDevelop.Utilities;

namespace FDjpPlugin.Commands
{
    class AlignAssignments
    {
        private static ScintillaControl sci;

        public static void Execute()
        {
            if (Globals.SciControl == null) return;
            sci = (ScintillaControl)Globals.SciControl;

            if (charsThatAssociateWithEquals == null)
            {
                charsThatAssociateWithEquals = new List<char>();
                charsThatAssociateWithEquals.Add('+');
                charsThatAssociateWithEquals.Add('-');
                charsThatAssociateWithEquals.Add('.');
                charsThatAssociateWithEquals.Add('<');
                charsThatAssociateWithEquals.Add('>');
                charsThatAssociateWithEquals.Add('/');
                charsThatAssociateWithEquals.Add(':');
                charsThatAssociateWithEquals.Add('\\');
                charsThatAssociateWithEquals.Add('*');
                charsThatAssociateWithEquals.Add('&');
                charsThatAssociateWithEquals.Add('^');
                charsThatAssociateWithEquals.Add('%');
                charsThatAssociateWithEquals.Add('$');
                charsThatAssociateWithEquals.Add('#');
                charsThatAssociateWithEquals.Add('@');
                charsThatAssociateWithEquals.Add('!');
                charsThatAssociateWithEquals.Add('~');
                charsThatAssociateWithEquals.Add('|');
            }

            // Find all lines above and below with = signs

            int currentLineNumber = sci.LineFromPosition(sci.SelectionStart);

            Dictionary<int, ColumnAndOffset> lineNumberToEqualsColumn = new Dictionary<int, ColumnAndOffset>();

            // Start with the current line
            int line = sci.LineFromPosition(sci.SelectionStart);
            int start = sci.PositionFromLine(line);
            int end = sci.LineEndPosition(line);
            ColumnAndOffset columnAndOffset = GetColumnNumberOfFirstEquals(new TextSnapshotLine(sci.GetLine(line), end - start, start));

            if (columnAndOffset.Column == -1) return;

            lineNumberToEqualsColumn[currentLineNumber] = columnAndOffset;

            int lineNumber = currentLineNumber;
            int minLineNumber = 0;
            int maxLineNumber = sci.LineFromPosition(sci.Length);

            // If the selection spans multiple lines, only attempt to fix the lines in the selection
            if (sci.SelText != null)
            {
                int selectionStartLine = sci.LineFromPosition(sci.SelectionStart);
                if (sci.SelectionEnd > sci.LineEndPosition(selectionStartLine))
                {
                    minLineNumber = selectionStartLine;
                    maxLineNumber = sci.LineFromPosition(sci.SelectionEnd);
                }
            }

            // Moving backwards        
            for (lineNumber = currentLineNumber - 1; lineNumber >= minLineNumber; lineNumber--)
            {
                start = sci.PositionFromLine(lineNumber);
                end = sci.LineEndPosition(lineNumber);
                columnAndOffset = GetColumnNumberOfFirstEquals(new TextSnapshotLine(sci.GetLine(lineNumber), end - start, start));

                if (columnAndOffset.Column == -1) break;

                lineNumberToEqualsColumn[lineNumber] = columnAndOffset;
            }

            // Moving forwards
            for (lineNumber = currentLineNumber + 1; lineNumber <= maxLineNumber; lineNumber++)
            {
                start = sci.PositionFromLine(lineNumber);
                end = sci.LineEndPosition(lineNumber);
                columnAndOffset = GetColumnNumberOfFirstEquals(new TextSnapshotLine(sci.GetLine(lineNumber), end - start, start));

                if (columnAndOffset.Column == -1) break;

                lineNumberToEqualsColumn[lineNumber] = columnAndOffset;
            }

            // Perform the actual edit
            if (lineNumberToEqualsColumn.Count > 1)
            {
                int columnToIndentTo = 0;
                foreach (ColumnAndOffset cof in lineNumberToEqualsColumn.Values)
                {
                    columnToIndentTo = Math.Max(columnToIndentTo, cof.Column);
                }

                sci.BeginUndoAction();

                foreach (KeyValuePair<int, ColumnAndOffset> pair in lineNumberToEqualsColumn)
                {
                    if (pair.Value.Column >= columnToIndentTo) continue;

                    string spaces = new string(' ', columnToIndentTo - pair.Value.Column);

                    if (isComment((sci.PositionFromLine(pair.Key) + pair.Value.Offset))) continue;

                    sci.InsertText((sci.PositionFromLine(pair.Key) + pair.Value.Offset), spaces);
                }

                sci.EndUndoAction();
            }
        }

        private static ColumnAndOffset GetColumnNumberOfFirstEquals(TextSnapshotLine line)
        {
            String snapshot = line.Text;

            int tabSize = Globals.MainForm.Settings.TabWidth;
            int column = 0;
            int nonWhiteSpaceCount = 0;
            for (int i = 0; i < line.Length; i++)
            {
                char ch = snapshot[i];
                if (ch == '=')
                {
                    if (isComment(line.Position + i))
                    {
                        return new ColumnAndOffset(1, i - nonWhiteSpaceCount);
                    }
                    else
                    {
                        return new ColumnAndOffset(column, i - nonWhiteSpaceCount);
                    }
                }

                // For the sake of associating characters with the '=', include only 
                if (!CharAssociatesWithEquals(ch))
                    nonWhiteSpaceCount = 0;
                else
                    nonWhiteSpaceCount++;

                if (ch == '\t')
                    column += tabSize - (column % tabSize);
                else
                    column++;

                // Also, check to see if this is a surrogate pair.  If so, skip the next character by incrementing
                // the loop counter and increment the nonWhiteSpaceCount without incrementing the column
                // count.
                if (char.IsHighSurrogate(ch) &&
                    i < line.Length - 1 && char.IsLowSurrogate(snapshot[i + 1]))
                {
                    nonWhiteSpaceCount++;
                    i++;
                }
            }

            return new ColumnAndOffset(-1, -1);
        }

        private static List<char> charsThatAssociateWithEquals;
        private static bool CharAssociatesWithEquals(char ch)
        {
            return charsThatAssociateWithEquals.Contains(ch);
        }

        private static bool isComment(int pos)
        {
            if (sci.StyleAt(pos) == 1 || sci.StyleAt(pos) == 2)
            {
                return true;
            }
            return false;
        }
    }

    class ColumnAndOffset
    {
        public int Column;
        public int Offset;
        public ColumnAndOffset(int column, int offset)
        {
            Column = column;
            Offset = offset;
        }
    }

    class TextSnapshotLine
    {
        public String Text;
        public int Length;
        public int Position;
        public TextSnapshotLine(String text, int length, int pos)
        {
            Text = text;
            Length = length;
            Position = pos;
        }
    }
}