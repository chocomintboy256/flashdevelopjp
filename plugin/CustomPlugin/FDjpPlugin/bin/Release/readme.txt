﻿		FlashDevelop.jp の皆で FlashDevelop がちょっと便利になるPluginを作ろう的な企画

起案

	2009/10/17	bkzen
	

概要

	FlashDevelop のプラグインを作ってみたいけど最初から作るのはだるい。
	FlashDevelop のプラグインを作ってみたいけどこんな小さい機能をプラグインにするのはどうなの？
	FlashDevelop にこんな機能があったらいいけど、本家に頼んでも見込みがない。
	的な悩みを、まとめてここにぶち込んじゃって皆で便利にしていこうという企画。
	FlashDevelop.jp #0 以降にソースを共有します！(FlashDevelop.jp #0 のお土産的な感じでｗ)


現在の機能

	・ワードジャンプ
		Ctrl + . で 次のワードへ
		Ctrl + , で前のワードへ
		WrodSelect を設定することで、移動したときにワードを選択できます。
	・Always Compile
		Alt + A で 現在のドキュメントを AlwaysCompile に設定します。。
		AlwaysCompileAfterCompile を設定することで 設定を切り替えた後コンパイルされます。
	・行移動
		Alt + ArrowDown で次の行へ移動
		Alt + ArrowUp で前の行へ移動
		複数行の選択も可
	・複数行コメントの一括折りたたみ・展開
		Ctrl + Alt + F で折りたたみ
		Ctrl + Alt + E で展開

	それぞれの設定は Tools -> Program Settings(F10) -> FDjpPlugin で設定・確認ができます。
	現在はショートカットが変更された場合は FlashDevelop を再起動するか、
	現在開いているドキュメント全てを開きなおす必要がある。

インストール方法

	同梱した DLL ファイルを
	C:\Documents and Settings\user名\Local Settings\Application Data\FlashDevelop\Plugins
	※ FlashDevelop を開いている人は Tools -> Application Files をクリックしたら開くディレクトリです。
	に入れる。
	FlashDevelop を開いている人は FlashDevelop を再起動してください。
	以上です。


更新日
	2009/10/17	忘れた	bkzen	(1.0.0.0)
		とりあえず作った
	2009/10/17	21:35	bkzen	(1.0.0.1)
		ワードジャンプに WordSelect 作った
		Readme 書いた。
	2009/10/19	22:06	bkzen	(1.0.0.2)
		ショートカットの取り方を変えた。
		メニューを追加。ボタンでも操作可能(多分使わない)
		Google Code のSVNを作った。
	2009/10/20	07:46	bkzen	(1.0.0.3)
		WordSelect を True にしたときの文頭、文末での範囲外エラーを修正。
	2009/10/21	09:02	bkzen	(1.0.0.4)
		WordSelect を True にしたときの日本語越えバグを修正。
	2009/10/24	13:17	bkzen	(1.0.0.4)
		行移動を実装。
		バージョンチェックで異なるバージョンの時は、
		Keys.NONE からデフォルトのキーを設定する機能を実装。
	2009/11/16	05:31	matsumos	(1.0.0.6)
		複数行コメントの一括折りたたみ・展開を実装。
	2009/11/16	07:41	matsumos	(1.0.0.7)
		複数行コメントの一括折りたたみ・展開機能をトグルできるようにするオプションを追加。
	2009/11/16	20:10	matsumos	(1.0.0.8)
		複数行コメントの一括折りたたみ・展開機能のトグルのバグを修正


TODO
	ScintillaControl をラップしてショートカットを即時反映させるクラスを作成。
	WonderFD を作る。
	コメントを書く。
	ASCompressor はちょっとむずい。

