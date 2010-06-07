===============================================================================
	FlashDevelop用 Progression 4.0.2 プロジェクトテンプレート
	http://j.mp/flashdevelop_progression_project_template
===============================================================================

■インストール方法
	FlashDevelopを起動します。
	メニューの [Tools] から [Application Files..] を選びます。
	表示されたフォルダに、解凍したテンプレートファイルから
	・Macros
	・Projects
	・Templates
	の3つのフォルダを移動（上書き）します。
	※Progression 4.0.1 以前から 4.0.2 にバージョンアップする際は、フォルダ構成が一部変わっているため、後述のアンインストール方法でアンインストールを行ってから上書きしてください。

	【マクロのセットアップ手順】
	リリースビルドの書き出しに必要なマクロを、以下の手順で登録します。
	※FlashDevelopをStandalone Modeで使用されている場合は、後述の手動による
	  方法でマクロを設定してください。
	(1) メニューの [Macros] から [Edit Macros...] を開きます。
	(2) 表示されたウィンドウの左側にある一覧リストのあたりで右クリックします。
	(3) [Import Macros...] をクリックします。
	(4) 同梱されている「リリースビルドマクロ_インストール.fdm」を指定します。
	(5) エラーが出た場合は、かまわず [Continue] をクリックして進みます。
	これでマクロがインストールされます。
	インストールが終われば、fdmファイルは今後使わないので破棄してもかまいません。

	【マクロの登録がうまくいかない場合】
	どうしてもうまくいかない場合は、手動で登録してください。
	Entriesに以下のように指定します。
	
	◆デフォルトの設定でインストールしている場合（通常はこちらです）
	ExecuteScript|Development;$(UserAppDir)\Macros\P4ReleaseBuild.cs

	◆Standalone Modeで使用されている場合
	ExecuteScript|Development;$(AppDir)\Macros\P4ReleaseBuild.cs

	【仕上げ】
	仕上げに、FlashDevelopをいったん閉じて、再度起動します。

■使い方
	【プロジェクトの新規作成】
	FlashDevelopのメニューの [Project] から [New Project...] を選択します。
	すると、[New Project] ウィンドウが開かれるので、[Installed Templates] の
	[Progression] の項目から「Progression 4.0 Project」を選択します。
	あとは、通常のAS3 Projectと同様の手順でプロジェクトを新規作成します。

	※注意
	FlashDevelopでは、Flash IDEで使用するProgression 4のように便利な新規作成パネル
	はありません。「Flash Playerのバージョン」「ステージの横幅、縦幅」「背景色」
	「フレームレート」などを変更する場合は、通常のプロジェクト設定に加えて
	Preloader.as3proj、出力先のindex.htmlファイルも一緒に変更する必要があります。

	【ファイルテンプレート】
	Progressionでよく使う新規クラスを作成する場合は、テンプレートを利用できます。
	Projectパネルの任意の場所で [右クリック] -> [Add] -> [Progression 4.0] から
	任意のスーパークラスを選択します。
	ファイル名(クラス名)を指定すると、クラスが作成されます。

	【リリースビルド】
	Progression 4では、リリースビルド時にデバッグ用のクラスからメッセージを削除する
	などしてリリース時のswfファイルを軽量化しています。
	このテンプレートでは、マクロを使用することでそれを再現しています。

	メインメニューの「Macros」->「Progression 4 リリースビルド」をクリックすると、
	リリース用のビルドが開始されます。
	Outputパネルに「Build succeeded」と表示されたら成功です。
	ビルド後、swfファイルは開きませんので、適宜必要なファイルを開いて確認します。

	※重要
	リリースビルドをすると、Progression 4の動作状況の出力（トレース）機能がオフに
	なります。Progression 4のTraceコマンドを使用している場合はトレースされません。
	直接trace()を使用した場合は表示されます。
	
■アンインストール
	FlashDevelopメニューの [Tools] から [Application Files..] を選びます。
	開いたフォルダから、以下のファイルを削除します。
	・Macrosフォルダ
		P4ReleaseBuild.cs ファイルを削除
	・Projectsフォルダ
		201 Progression - Progression 4.0 Project フォルダを削除
	・Templatesフォルダ
		ProjectFiles
			AS3Project
				Progression 4.0 フォルダを削除

	FlashDevelopメニューの [Macros] から [Edit Macros...] を選びます。
	登録したマクロ、[Progression 4 リリースビルド]を選択します。
	[Delete]を押下します。

■リリースノート
	2010/04/22 更新 dkgkAs
		Progression 4.0.2 リリースに伴う更新。
		プロジェクト作成画面やクラス生成テンプレートに表示される Progression のバージョン表記を 3 桁から 2 桁（「4.0.1」→「4.0」）に変更。
		クラス生成用テンプレートのファイル名に接頭辞「My」を追加。
	2010/04/16 修正 dkgkAs
		プロジェクトプロパティのCompiler OptionsにおいてIntrinsic Librariesの部分にFlash IDE用のパスが含まれていたのを削除。
	2010/04/03 更新 dkgkAs
		Progression 4.0.1 RC1 リリースに伴う更新。
	2010/03/29 修正 matsumos
		クラス生成用テンプレートの command.as.fdt においてクラス名を指定する部分にアーギュメントが設定されていなかったのを修正。
	2010/01/12 更新 matsumos
		Progression 4.0.1 Public Beta 1.3 リリースに伴う更新。
	2009/11/22 修正 matsumos
		クラス生成用テンプレートのフォルダ配置が間違っていたのを修正。
	2009/11/18 改変 matsumos
		インストール先を変更
		リリースビルド用マクロ P4ReleaseBuild 1.0.1 を導入。
	2009/11/13 更新 dkgkAs
		Progression 4.0.1 Public Beta 1.2 リリースに伴う更新。
	2009/09/21 公開 dkgkAs
		FlashDevelop 用 Progression 4.0.1 Public Beta 1.1 プロジェクトテンプレートを作成。

■使用条件・免責事項
	・当テンプレートを使用する際はProgression 4のライセンスを遵守してください。
	  http://progression.jp/ja/overview/license/
	・当テンプレートの使用により生じたいかなる障害に対して責任を持ちません。
	  使用者の責任の上で使用してください。
