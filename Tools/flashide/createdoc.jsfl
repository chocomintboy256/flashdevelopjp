/*
 * 設定した値で、新規ドキュメント（幅、高さ、フレームレート、ドキュメントクラス、背景色）を作成してくれるJSFL
 *
 * Copyright (C) 2010 flabaka
 * version 1.1.0
 * CreateDate 2010/06/05
 *
 * Developed by flabaka
 * http://flabaka.com/blog/
 *
 * (C) 2010 flabaka and is released under the MIT License:
 * http://www.opensource.org/licenses/mit-license.php
 *
 * このJSFLの使い方は、以下の記事を参照してください。
 * http://flabaka.com/blog/?p=2487
 * http://flabaka.com/blog/?p=2498
 *
 */

//ダイアログボックスを表示
var str = prompt("幅、高さ、フレームレート、ドキュメントクラス、カラー", "550,400,30,Main,#000000");

//値が空だったら…
if(str == ""){
	str = prompt("数値をカンマ区切りで入力してください","");
}

//配列にでも入れておく
var myarray = str.split(",");

//新規ドキュメントを作成
fl.createDocument();

var path = fl.getDocumentDOM();

//ドキュメントサイズの幅
path.width = Number(myarray[0]);

//ドキュメントサイズの高さ
path.height = Number(myarray[1]);

//フレームレート
path.frameRate = Number(myarray[2]);

//ドキュメントクラス
path.docClass = myarray[3];

//背景色
path.backgroundColor = myarray[4];

//ドキュメントを保存
fl.saveDocumentAs(fl.documents[0]);
