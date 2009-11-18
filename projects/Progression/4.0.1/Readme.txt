===============================================================================
	FlashDevelop用 Progression 4.0.1 Public Beta 1.2 プロジェクトテンプレート
	http://d.hatena.ne.jp/ActionScript/20090921/fd_progression4_project
===============================================================================

■インストール方法
	FlashDevelopを起動します。
	メニューの [Tools] から [Application Files..] を開きます。
	出てきたフォルダに、解凍したテンプレートファイルから
	・Macros
	・Projects
	・Templates
	フォルダを移動します。

	【マクロのセットアップ】
	FlashDevelopを起動して、メニューの [Macros] から [Edit Macros...] を開きます。
	出てきたウインドウの右側のリストのあたりで右クリックします。
	[Import Macros...] をクリックします。
	同梱されている
	・リリースビルドマクロ_インストール.fdm
	を指定します。
	マクロがインストールされます。

	【マクロのセットアップがうまくいかない場合】
	うまくいかない場合は、手動で登録してください。
	Entriesに以下のように指定します。

	ExecuteScript|Development;$(UserAppDir)\Macros\P4ReleaseBuild.cs

	【仕上げ】
	仕上げに、FlashDevelopをいったん閉じて、再度起動します。

■使い方
	【プロジェクトの新規作成】
	FlashDevelopのメニューの [Project] から [New Project...] を選択します。
	すると、[New Project] ウィンドウが開かれるので、[Installed Templates] の
	[Progression] の項目から「Progression 4.0.1 Project」を選択します。
	あとは、通常のAS3 Projectと同様の手順でプロジェクトを新規作成します。

	※注意
	FlashDevelopでは、Flash CSで使用するProgression 4のように便利な新規作成
　　パネルはありません。「FlashPlayerのバージョン」「ステージの横幅、縦幅」
	「背景色」「フレームレート」などを変更する場合は、通常のプロジェクト設定に
	加えてPreloader.as3proj、出力先のindex.htmlファイルも一緒に変更する必要が
	あります。

	【テンプレート】
	Progressionでよく使う新規クラスを作成する場合は、テンプレートを利用できます。
	Projectパネルの任意の場所で [右クリック] > [Add] > [Progression 4.0.1] から
	任意のスーパークラスを選択します。
	ファイル名(クラス名)を指定すると、クラスが作成されます。

	【リリースビルド】
	Progressionでは、リリースビルド時にデバッグ用のクラスからメッセージを削除
	するなどしてリリース時のswfを軽量化しています。
	このテンプレートでは、マクロを使用することでそれを再現しています。

	メインメニューの「Macros」>「Progression4 リリースビルド」をクリックすると、
	ビルドが開始されます。
	Outputパネルに「Build succeeded」と表示されたら成功です。
	ビルド後、特にファイルは開きませんので、適宜必要なファイルを開いて確認してください。

	※重要
	リリースビルドすると、Progressionの動作状況の出力（トレース）機能がオフになります。
	ProgressionのTraceコマンドを使用している場合はトレースされません。
	直接trace()を使用した場合は表示されます。
	
■更新履歴
	2009.11.18 改変
		インストール先を変更
		リリースビルド用マクロ P4ReleaseBuild 1.0.1 を導入
	2009.11.13 公開
		FlashDevelop用 Progression 4.0.1 Public Beta 1.2 プロジェクトテンプレート
	2009.09.21 公開
		FlashDevelop用 Progression 4.0.1 Public Beta 1.1 プロジェクトテンプレート

■使用条件・免責事項
	・当テンプレートを使用する際はProgression 4のライセンスを遵守してください。
	  http://progression.jp/ja/overview/license/
	・当テンプレートの使用により生じたいかなる障害に対して責任を持ちません。
	  使用者の責任の上で使用してください。
