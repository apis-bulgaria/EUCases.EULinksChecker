function scrtxt(a){
	switch(a){
	case 'innerHTML': return 'document.body.innerHTML';
	case 'innerText': return 'document.body.innerText';
	case 'Selection'://return 'window.getSelection().toString()';
	case 'popup-click': return 'window.getSelection().toString()';
	default: return 'None'
	}
}
function _a2(a,c,f,fa){chrome.tabs.query({active:true},function(t){chrome.tabs.executeScript(t[0].id,{code:scrtxt(a)},function(x){c(x[0]);f(fa)})})}
function AlX(x){alert(x)}
function _cls(a){a.close()}
function _html(e){_a2('innerHTML',AlX,_cls,self)}
function _text(e){_a2('innerText',AlX,_cls,self)}
function _sel(e){_a2('Selection',AlX,_cls,self)}
function _set(e){
	chrome.tabs.create({url:'chrome://extensions/?options='+chrome.runtime.id});
	self.close()
	}
function _si(i,t,h){
	var x=document.getElementById(i);
	if(x)with(x){
		try{setAttribute('title',t)}catch(_){}
		try{setAttribute('alt',t)}catch(_){}
		try{value=innerHTML=t}catch(_){}
		addEventListener('click',h);
	}else console.log(i+' not found')
}
function Init(){}
window.addEventListener('load',Init);
function _RmAllLinks(e){RmAllLinks()}
function _InsLink1(i,t){
	var rs=new Rsrc(),
	l=prompt(rs.cInsertLink,'http://');
	if(!l)return;
	chrome.tabs.query({active:true},function(t){
		//chrome.tabs.executeScript(t[0].id,{code:scrtxt(a)},function(x){c(x[0]);f(fa)
	chrome.tabs.executeScript(t[0].id,{code:'try{window.getSelection().toString()}catch(_){""}'},
	function(r){
		var tg=localStorage['NewLlinksIdx'];
		switch(tg){
		case '1':tg='_blank';break;
		case '2'://tg='_self';break;
		case '3':tg='_top';break;
		default:return}
		var lt='<a href="'+l+'" target="'+tg+'">'+r+'</a>';
		chrome.tabs.query({active:true},function(_t){
			chrome.tabs.executeScript(_t[0].id,{file:"cs1.js"},function(x){
				chrome.tabs.executeScript(_t[0].id,{code:'if(!window.EUCASES_UI_LANG)window.EUCASES_UI_LANG='+Qt()+LANG()+Qt()+';replaceSelectionURIEncoded("'+encodeURI(lt)+'")'},
					function(y){alert(y==''?rs.cNoSelection:rs.cInsLinkSucc)})
			})
		})
	})
})}
function _Chk4Links(e){Chk4Links()}
function _rmls(e){rmls()}
function _xml(e){xml()}
document.addEventListener('DOMContentLoaded',function(){
	var x=new Rsrc();
	_si('iCheckForLinks',x.cStPutLinksAndTerms,_Chk4Links);//x.cCheckFrLinks
	_si('iRemoveAllLinks',x.cStRemoveLinksAndTerms,_RmAllLinks);//x.cRemoveLinks
	_si('iAddNewLink',x.cStAddNewLink,_InsLink1);//x.cInsertLink
	_si('iRemoveLink',x.cStRemove1,_rmls);//x.cRemoveLinksSel
	_si('ixml',x.cStSave2Xml,_xml);//x.Save2Xml
	_si('iSettings',x.cStCredentials,_set)//x.cm_Credentials
})