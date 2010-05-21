/**
*　@author flabaka
*　@see http://flabaka.com/blog/
*　@version 1.1.1
*
*　■FlashDevelopのASDocジェネレーターを使って、ASDocを作成■
*
*　FlexSDK4を使っている場合、HTMLタグの閉じ忘れとかがあると、処理が中断してしまいます。（-lenientオプションを設定した場合は、ASDocは作成されますが…）
*
*　処理が中断された場合＆ASDocは作成されたんだけど、HTMLタグなどの記述に問題があった場合、validation_errors.logというファイルが作成されます。
*　
*　いちいちエラーログの中身を見るのは面倒だよね！　ということで、エラーログの内容をFlashDevelopの出力パネルに表示出来るマクロを作ってみました。
*
*　エラーログが作成されたよ！　とASDocジェネレーターのOutPutパネルに表示があったら、ASDocプロジェクトを任意の場所に保存してください。
*　
*　そして、FlashDevelop>マクロ>Execute Scriptより、このマクロを実行。ダイアログボックスが表示されるので、先程保存したASDocのドキュメントプロジェクトを選択します。
*
*　するとエラーログの内容が、FlashDevelopの出力パネルに表示されます。
*
*　それを見て、HTML記述の修正→FlashDevelop＆Flex4SDKでASDocを再度作成してみるのも、いいんじゃないでしょうか？
*
*/

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;

public class ASDocError
{

    public static void Execute()
    {

        //OpenFileDialogクラスのインスタンスを作成
        OpenFileDialog dialog = new OpenFileDialog();

        //デフォルトのファイル名の設定
        dialog.FileName = ".docproj";

        //デフォルトのフォルダの指定
        dialog.InitialDirectory = @"C:\";

        //ファイルの種類の設定
        dialog.Filter = "ASDocプロジェクト(*.docproj)|*.docproj";

        dialog.FilterIndex = 2;

        //ダイアログのタイトルを設定する
        dialog.Title = "ASDocプロジェクトを選択してください";

        //現在のディレクトリを復元
        dialog.RestoreDirectory = true;

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            //XMLドキュメントを読み込む
            XmlDocument doc = new XmlDocument();
            doc.Load(dialog.FileName);

            //outputDirectoryのノードを取得
            XmlNodeList list = doc.SelectNodes("/docProject/outputDirectory");
            foreach (XmlNode node in list)
            {
                string path = node.InnerText + "\\" + "validation_errors.log";
                //TraceManager.Add(path);

                //エラーログがある場合
                if (File.Exists(path))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(
                    path,
                    System.Text.Encoding.GetEncoding("utf-8"));

                    //ログの内容を、すべて読み込む
                    string s = sr.ReadToEnd();

                    //閉じる
                    sr.Close();

                    //エラーログの内容を、出力パネルに表示する
                    TraceManager.Add(s);

                }
                //エラーログがない場合
                else
                {
                    TraceManager.Add("エラーログファイルはありません！");
                }

            }
        }
    }
}