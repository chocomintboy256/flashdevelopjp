using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using FlashDevelop;
using PluginCore;
using PluginCore.Managers;
using PluginCore.Helpers;
using ProjectManager.Projects;
using ScintillaNet;
using ProjectManager.Actions;

/// <summary>
/// Progression でコーディング中によく使うコマンドパターンがあった場合に、そのパターンをカスタムコマンド化してくれる拡張機能です。
/// </summary>
/// <remarks>
/// 1. カスタムコマンドに変換したいコード内のコマンド群を選択。
/// 2. マクロ「カスタムコマンドに変換」を実行。
/// 3. クラス名入力ダイアログが表示される。
/// 4. 入力されたクラス名が設定された以下(106行目から)のようなクラスファイルがコピー元のクラスと同じ階層に自動作成される。
/// 5. 変換前のコード部分が新しいコマンドに置換される。
/// 
/// 使い方
/// 1. このファイルの46行目、TEMP_PATH は、テンプレートを保存する場所です。
///    FlashDevelop のメニューから Tools > Application Files... を開いて出たフォルダの中の ProjectFiles/AS3Project/ になります。
///    デフォルトではこのテンプレートは Project パネルの右クリックメニュー > Add > 以下にも登録されます。
///    (こちらでも使用できます。一部の機能、変換前のコード部分の新しいコマンドへの置換などは実行されません。)
///    好みでなければ任意のパスに変更してください。なお、パスが存在しない場合はエラーとなりますのであらかじめフォルダを作成しておいてください。
///    メニューに登録されたくない場合は、PathHelper.TemplateDir をはずします。
///    迷ったら以下のような場所がおすすめです。
///    PathHelper.BaseDir + \\Macros\\CommandFromSelection.as.fdt
/// 2. FlashDevelop のメニューから Tools > Application Files... を開きます。
/// 4. Macros フォルダ （なければ作成してください） の中に、このファイルをコピーしてください。
/// 5. FlashDevelop に戻って、メニューから Macros >  Edit Macros... を開きます。
/// 6. 出てきた　Macros ウインドウで、Add ボタンをクリックします。
/// 7. 一覧に Untitled が作成されますのでクリックします。
/// 8. 右側の編集画面の Entries 欄の右端の[...]ボタンをクリックし、出てきたウインドウに以下のように記入します。
/// 	ExecuteScript|Development;$(UserAppDir)\Macros\CommandFromSelection.cs
/// 9.Label の欄に「カスタムコマンドに変換」と入力して完成です。Shortcut 欄でショートカットを設定することもできます。
/// 
/// 詳しい解説はこちら
/// http://flabaka.com/blog/?p=2102
/// </remarks>
public class CommandFromSelection
{
	private static String TEMP_PATH = PathHelper.TemplateDir + "\\ProjectFiles\\AS3Project\\CommandFromSelection.as.fdt";

	public static void Execute()
	{
		if ( Globals.SciControl == null ) return;

		ScintillaControl sci = Globals.SciControl;

		string selectStr = sci.SelText;
		if ( selectStr.Length < 1 ) {
			MessageBox.Show("クラスにするコマンドを選択してください","エラー",MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}

		Project project = (Project) PluginBase.CurrentProject;
		if (project == null) return;

		MainForm mainForm = (MainForm) Globals.MainForm;
		FlashDevelopActions flashDevelopActions = new FlashDevelopActions(mainForm);
		FileActions fileActions = new FileActions(mainForm, flashDevelopActions);

		if (!File.Exists(TEMP_PATH))
		{
			StreamWriter tmpWriter = new StreamWriter(TEMP_PATH, false, System.Text.Encoding.UTF8, 512);
			tmpWriter.Write(TMP_FILE);
			tmpWriter.Close();
		}

		ITabbedDocument currentDocument = (ITabbedDocument) mainForm.CurrentDocument;
		String parentPath = System.IO.Path.GetDirectoryName(currentDocument.FileName);

		fileActions.AddFileFromTemplate(project,parentPath,TEMP_PATH);
		String fileName = fileActions.ProcessArgs(project, "$(FileName)");

		String newFilePath = parentPath + "\\" + fileName + ".as";
		if (!File.Exists(newFilePath)) {
			TraceManager.Add( "キャンセルされました" );
			return;
		}

		StreamReader reader = new StreamReader(newFilePath);
		String value = reader.ReadToEnd();
		reader.Close();

		StreamWriter writer = new StreamWriter(newFilePath, false, System.Text.Encoding.UTF8, 512);
		writer.Write(fileActions.ProcessArgs(project, value));
		writer.Close();

		string insText = "new "+fileName+"()";
		sci.BeginUndoAction();
		sci.Clear();
		sci.InsertText(sci.CurrentPos, insText);
		sci.SelectionStart = sci.CurrentPos;
		sci.SelectionEnd = sci.CurrentPos + insText.Length;
		sci.EndUndoAction();
		TraceManager.Add( fileName + " が作成されました" );
	}

	private const String TMP_FILE =
@"package $(Package) $(CSLB){
	import jp.progression.casts.*;
	import jp.progression.commands.display.*;
	import jp.progression.commands.lists.*;
	import jp.progression.commands.net.*;
	import jp.progression.commands.tweens.*;
	import jp.progression.commands.*;
	import jp.progression.data.*;
	import jp.progression.events.*;
	import jp.progression.scenes.*;
	
	/**
	$(CBI)* ...
	$(CBI)* @author $(DefaultUser)
	$(CBI)*/
	public class $(FileName) extends $$(ListType=SerialList,ParallelList) $(CSLB) {
		
		/**
		$(CBI)* 新しい $(FileName) インスタンスを作成します。
		$(CBI)*/
		public function $(FileName)( initObject:Object = null ) $(CSLB){
			super( initObject );
			
			addCommand(
				$(SelText)
			);
		}
		
		/**
		$(CBI)* インスタンスのコピーを作成して、各プロパティの値を元のプロパティの値と一致するように設定します。
		$(CBI)*/
		public override function clone():Command $(CSLB){
			return new $(FileName)( this );
		}
	}
}";
}