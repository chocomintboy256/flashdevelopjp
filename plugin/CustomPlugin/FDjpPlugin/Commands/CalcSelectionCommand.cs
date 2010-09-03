using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;
using PluginCore.Managers;
using ScintillaNet;

namespace FDjpPlugin.Commands
{
    class CalcSelectionCommand : SCICommand
    {
        public CalcSelectionCommand(ScintillaControl sci) : base(sci) { }

        public override void Execute()
        {
            try
            {

                //計算式
                string exp = sci.SelText;

                //計算するためのコード
                string source =
                @"package Evaluator
{
    class Evaluator
    {
        public function Eval(expr : String) : String 
        { 
            return eval(expr); 
        }
    }
}";

                //コンパイルするための準備
                CodeDomProvider cp = new Microsoft.JScript.JScriptCodeProvider();
                CompilerParameters cps = new CompilerParameters();
                CompilerResults cres;
                //メモリ内で出力を生成する
                cps.GenerateInMemory = true;
                //コンパイルする
                cres = cp.CompileAssemblyFromSource(cps, source);

                //コンパイルしたアセンブリを取得
                Assembly asm = cres.CompiledAssembly;
                //クラスのTypeを取得
                Type t = asm.GetType("Evaluator.Evaluator");
                //インスタンスの作成
                object eval = Activator.CreateInstance(t);
                //Evalメソッドを実行し、結果を取得
                string result = (string)t.InvokeMember("Eval",
                    BindingFlags.InvokeMethod,
                    null,
                    eval,
                    new object[] { exp });

                replaceSelection(result.ToString());

            }
            catch (Exception exp)
            {
                ErrorManager.ShowError(exp);
            }
        }

        public void replaceSelection(string changeString)
        {
            sci.BeginUndoAction();
            sci.Clear();
            sci.InsertText(sci.SelectionStart, changeString);
            sci.EndUndoAction();
        }
    }
}
