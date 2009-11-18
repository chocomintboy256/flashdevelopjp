/**
 * Progression 4
 * 
 * @author Copyright (C) 2007-2009 taka:nium.jp, All Rights Reserved.
 * @version 4.0.1 Public Beta 1.2
 * @see http://progression.jp/
 * 
 * Progression Software is released under the Progression Software License:
 * http://progression.jp/ja/overview/license
 * 
 * Progression Libraries is released under the MIT License:
 * http://www.opensource.org/licenses/mit-license.php
 */

var progression; ( function() {
	progression = function() {};
	
	var doc = document,
		swf = swfobject,
		$ = function( id ) { return doc.getElementById( id ); },
		$t = function( tagName ) { return doc.getElementsByTagName( tagName ); },
		$i = function( value, defaultValue ) {
			if( $x( value ) ) { return value; }
			else { return defaultValue };
		},
		$x = function( value ) {
			switch ( value ) {
				case ""			:
				case null		:
				case undefined	: { return false; }
			}
			return true;
		},
		$p = function( t1, t2 ) {
			if ( !$x( t1 ) || !$x( t2 ) ) { return t1; }
			for ( var p in t2 ) {
				var tt1 = t1[p], tt2 = t2[p];
				if ( typeof( tt1 ) == "object" ) { $p( tt1, tt2 ); }
				else { t1[p] = $i( tt1, tt2 ); }
			}
			return t1;
		},
		$css = function( idOrTagName, attribute ) {
			if ( idOrTagName.charAt( 0 ) == "#" ) { var target = $( idOrTagName.slice( 1 ) ); }
			else { var target = $t( idOrTagName )[0]; }
			
			if ( target ) {
				$p( target, { style:attribute } );
			}
			else {
				var attr = "";
				for ( var p in attribute ) {
					attr += $decamelize( p ) + ":" + attribute[p] + ";";
				}
				swf.createCSS( idOrTagName, attr );
			}
		},
		$decamelize = function( name ) {
			return name.replace( new RegExp( "[A-Z]", "g" ), function( $0 ) { return "-" + $0.toLowerCase(); } );
		},
		config = {},
		defaultConfig = {
			version				:"9.0.45",
			url					:"index.swf",
			width				:800,
			height				:600,
			halign				:"center",
			valign				:"middle",
			hscale				:"default",
			vscale				:"default",
			contentId			:"content",
			htmlContentId		:"htmlcontent",
			flashContentId		:"flashcontent",
			useExpressInstall	:true,
			bgcolor				:"#FFFFFF",
			params				:{
				allowscriptaccess	:"always"
			},
			flashvars			:{
			},
			attributes			:{
			}
		};
	
	progression.embedSWF = function( customConfig ) {
		config = $p( customConfig, defaultConfig );
		
		config.attributes.id = config.attributes.name = "external_" + config.contentId;
		config.params.bgcolor = config.bgcolor;
		config.params.wmode = "window";
		config.params.allowfullscreen = "true";
		
		switch ( config.hscale ) {
			case "window"	: { var w = "100%", left = "0", marginLeft = "0"; break; }
			default			: {
				var w = config.width + "px";
				switch ( config.halign ) {
					case "center"	: { var left = "50%", marginLeft = "-" + Math.ceil( config.width / 2 ) + "px"; break; }
					case "right"	: { var left = "100%", marginLeft = "-" + config.width + "px"; break; }
					default			: { var left = "0", marginLeft = "0"; }
				}
			}
		}
		
		switch ( config.vscale ) {
			case "window"	: { var h = "100%", top = "0", marginTop = "0"; break; }
			default			: {
				var h = config.height + "px";
				switch ( config.valign ) {
					case "middle"	: { var top = "50%", marginTop = "-" + Math.ceil( config.height / 2 ) + "px"; break; }
					case "bottom"	: { var top = "100%", marginTop = "-" + config.height + "px"; break; }
					default			: { var top = "0", marginTop = "0"; }
				}
			}
		}
		
		var xiURL = config.useExpressInstall ? "commons/scripts/swfobject/expressinstall.swf" : undefined;
		
		var css = { width:"100%", height:"100%", overflow:"auto", margin:"0", padding:"0", background:config.bgcolor };
		$css( "html", css );
		$css( "body", css );
		$css( "#" + config.contentId, { position:"absolute", width:w, height:h, left:left, top:top, marginLeft:marginLeft, marginTop:marginTop, lineHeight:0 } );
		$css( "#" + config.htmlContentId, { display:"none" } );
		
		swf.addLoadEvent( initSWF );
		swf.embedSWF( config.url, config.flashContentId, "100%", "100%", config.version, xiURL, config.flashvars, config.params, config.attributes );
	};
	
	var initSWF = function() {
		( swfobject.getObjectById( config.attributes.id ) || {} ).focus();
		$p( $( "disabled_javascript" ), { style:{ display:"none" } } )
	};
} )();
