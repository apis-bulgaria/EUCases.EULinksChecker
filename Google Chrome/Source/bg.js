function SetRes(){
var x=new Rsrc();
	try{chrome.contextMenus.update('cmiInsLink',{title:x.cInsertLink})}catch(_){}
	try{chrome.contextMenus.update('cmiRmLinksSel',{title:x.cRemoveLinksSel})}catch(_){}
	try{chrome.contextMenus.update('cmiChk4Links',{title:x.cCheckFrLinks})}catch(_){}
	try{chrome.contextMenus.update('cmiRmAllLinks',{title:x.cRemoveLinks})}catch(_){}
}
var contexts=['selection'],c1=['page','selection'];
var lti=0;
function InsLink(i,t){
	var rs=new Rsrc(),
	l=prompt(rs.cInsertLink,'http://');
	if(!l)return;
	chrome.tabs.executeScript(lti,{code:'try{window.getSelection().toString()}catch(_){""}'},
		function(r){
			var tg=localStorage['NewLlinksIdx'];
			switch(tg){
			case '1':tg='_blank';break;
			case '2'://tg='_self';break;
			case '3':tg='_top';break;
			default:return}
			var lt='<a href="'+l+'" target="'+tg+'">'+r+'</a>';
			chrome.tabs.executeScript(lti,{file:"cs1.js"},function(x){
				chrome.tabs.executeScript(lti,{code:'if(!window.EUCASES_UI_LANG)window.EUCASES_UI_LANG='+Qt()+LANG()+Qt()+';if(lt!="")replaceSelectionURIEncoded("'+encodeURI(lt)+'")'},
					function(y){alert(rs.cInsLinkSucc)})
			})
	})
}
chrome.contextMenus.create({id:'cmiInsLink',title:'cmiInsLink',contexts:contexts,onclick:InsLink});
function RmLSB(i,t){rmls(true)}
chrome.contextMenus.create({id:'cmiRmLinksSel',title:'cmiRmLinksSel',contexts:contexts,onclick:RmLSB});
function _Chk4LinksB(i,t){Chk4Links(true)}
chrome.contextMenus.create({id:'cmiChk4Links',title:'cmiChk4Links',contexts:c1,onclick:_Chk4LinksB});
function RmLinksB(i,t){RmAllLinks(true)}
chrome.contextMenus.create({id:'cmiRmAllLinks',title:'cmiRmAllLinks',contexts:c1,onclick:RmLinksB});


SetRes();
chrome.tabs.onActivated.addListener(function(activeInfo){lti=activeInfo.tabId});
var resx='';
function scrtxt(a){
	switch(a){
	case 'innerHTML': return 'document.body.innerHTML';
	case 'innerText': return 'document.body.innerText';
	case 'Selection':
	case 'popup-click': return 'window.getSelection().toString()';
	default: return 'None'
	}
}
chrome.extension.onRequest.addListener(function(q,s,r){
	try{
	if(q.action==='innerHTML')chrome.tabs.executeScript(lti/*chrome.tabs[lti]*/,{code:'document.body.innerHTML'},function(x){r({res:x})});
	else if(q.action==='eucasesSetLinks')chrome.tabs.executeScript(lti/*chrome.tabs[lti]*/,{file:'cs1.js',code:'eucasesSetLinks()'},function(){r({res:'OK'})});
	else if(q.action==='ping')r({res:'pong'});
	else if(q.action==='getUiLang'){
		var _r=new Rsrc();
		r({res:LANG(),cSuccess:_r.cSuccess})
	}else if(q.action==='cLinksRemoved'){
		var _r=new Rsrc();
		r({res:LANG(),cLinksRemoved:_r.cLinksRemoved})
	}else if(q.action==='getSel'){
		chrome.tabs.executeScript(lti,{code:'try{window.getSelection().toString()}catch(_){""}'},function(x){r({res:x})})
	}
	}catch(_){alert(_.message)}
});
chrome.extension.onMessage.addListener(
	function(q,s,r){
	if(q.directive==='langchanged'){
		SetRes();
		r({});
		return
	}
	if(q.directive==='eucasesSetLinksCompleted'){
		InjectCodeIntoTab(lti);
		return
	}
		chrome.tabs.executeScript(lti,{code:scrtxt(q.directive)},function(x){resx=x[0]});
		r({res: resx});
		resx=''
		return true
	}
)