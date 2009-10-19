		FlashDevelop.jp の皆で FlashDevelop がちょっと便利になるPluginを作ろう的な企画

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
		Ctrl + Tab で 次のワードへ
		Ctrl + Shift + Tab で前のワードへ
		WrodSelect を設定することで、移動したときにワードを選択できます。
	・Always Compile
		Alt + A で 現在のドキュメントを AlwaysCompile に設定します。。
		AlwaysCompileAfterCompile を設定することで 設定を切り替えた後コンパイルされます。

	それぞれの設定は Tools -> Program Settings(F10) -> FDjpPlugin で設定・確認ができます。


インストール方法

	同梱した DLL ファイルを
	C:\Documents and Settings\user名\Local Settings\Application Data\FlashDevelop\Plugins
	※ FlashDevelop を開いている人は Tools -> Application Files をクリックしたら開くディレクトリです。
	に入れる。
	FlashDevelop を開いている人は FlashDevelop を再起動してください。
	以上です。


その他

	FlashDevelop.jp に SVNがほしい。
	GoogleCode って自由に使えるのかな？


更新日
	2009/10/17	忘れた	bkzen	(1.0.0.0)	とりあえず作った
	2009/10/17	21:35	bkzen	(1.0.0.1)	ワードジャンプに WordSelect 作った
	          	     	     	         	Readme 書いた。



