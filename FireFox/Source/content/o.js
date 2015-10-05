//'use strict';
var cIdBusyBox='IdBusyBox',cIdModalBox='IdModalBox',cNone='none',cBlock='block',cHidden='hidden',cVisible='visible',cA='A',cDiv='div',cSpan='SPAN',
	cCnEuCasesLink='eucases-link',cCnEucasesTerm='eucases-term',cStyleId='StyleId',cVarsId='VarsId',cVars1Id='Vars1Id',cCommonProcsId='CommonProcsId',cModalProcsId='ModalProcsId',cModalResScrId='ModalResScrId',cModalResId='ModalResId',cUiLangId='UiLangId',cNotify='notifyme',
	cSymbol='symbol',cGET='GET',cUiLangIdx='UiLangIdx',cNewLinkInIdx='NewLinkInIdx',cTitle='title',cModalResult='ModalResult',cHead='head',ctA='A',ctI='I',ctE='E',ctX='X',ctW='W',ctQ='Q',cType='type',cMsg='msg',cOk='OK',cCancel='Cancel',cYes='Yes',cNo='No',cBut='but',cRes='res',cHttp_='http://';
function _alrt(a){console.log(a)}
function qArg(n,q){
	if(!q)q=document.location.href;
	try{
		let a=q.split('?')[1].split('&');
		for(let i=0;i<a.length;i++){
			let b=a[i].split('=');
			if(b[0]==n)return b[1]
		}
	}catch(_){}
	return ''
}
function Msg(m,t,b){
	if(!b)b=cOk;
	if(!t)t=ctI;
	let x=Components.classes['@mozilla.org/embedcomp/window-watcher;1'].getService(Components.interfaces.nsIWindowWatcher);
	x.openWindow(null,'chrome://MNU/content/dlg.xul?'+cType+'='+t+'&'+cMsg+'='+encodeURI(m)+'&'+cBut+'='+encodeURI(b),'','chrome,dialog,modal,centerscreen,alwaysRaised',null).focus()
}
function OkCancel(){return [cOk,cCancel].join(',')}
function ResOkCancel(){return [cRes,cOk,cCancel].join(',')}
function Qst(m,t,b){
	if(!b)b=OkCancel();
	if(!t)t=ctQ;
	let r=false,x=Components.classes['@mozilla.org/embedcomp/window-watcher;1'].getService(Components.interfaces.nsIWindowWatcher);
	x.openWindow(null,'chrome://MNU/content/dlg.xul?'+cType+'='+t+'&'+cMsg+'='+encodeURI(m)+'&'+cBut+'='+encodeURI(b),'','chrome,dialog,modal,centerscreen,alwaysRaised',null).focus();
	let y=GetStrPref(cModalResult);
	if(y)r=y=='1';
	SetStrPref(cModalResult,null);
	return r
}
function Inp(m,t,r){
	if(!t)t=ctQ;
	let x=Components.classes['@mozilla.org/embedcomp/window-watcher;1'].getService(Components.interfaces.nsIWindowWatcher),b=ResOkCancel();
	x.openWindow(null,'chrome://MNU/content/dlg.xul?'+cType+'='+t+'&'+cMsg+'='+encodeURI(m)+'&'+cBut+'='+encodeURI(b)+'&'+cRes+'='+encodeURI(r.v),'','chrome,dialog,modal,centerscreen,alwaysRaised',null).focus();
	let y=GetStrPref(cModalResult);
	if(y)r.v=y;
	SetStrPref(cModalResult,null);
	return y?true:false
}
function HG(u){
	let x=new XMLHttpRequest(),s='';
	x.open(cGET,u,false);
	x.send(null);
	if(x.status===200)s=x.responseText;
	return s
}
function Doc(){
	let x=window.content;
	return x?x.document:null
}
function DocBody(){
	let x=Doc();
	return x?x.body:null
}
function DocBodyHtml(){
	let x=DocBody();
	return x?x.innerHTML:null
}
function GetElById(i){let x=document.getElementById(i);if(!x){x=Doc();if(x)x=x.getElementById(i)}return x}
function GetElsByTagName(i){let x=Doc();return x?x.getElementsByTagName(i):null}
function ElsAtCursor(){
	let d=Doc();
	if(d)if(d.querySelectorAll)return d.querySelectorAll(':hover');
	return null
}
function LinkAtCursor1(e){
	let x=e.clientX,y=e.clientY;
	if(document.elementFromPoint)return document.elementFromPoint(x,y);
	let d=Doc();
	if(d.elementFromPoint)return d.elementFromPoint(x,y);
	return null
}
function LinkAtCursor(e){
	let l=ElsAtCursor();
	if(!l)return null;
	for(let i=0;i<l.length;i++){let e=l[i];if(e.tagName==cA)return e}
	return LinkAtCursor1(e)
}
function GSN(){
	let s=content.getSelection();
	if(s.isCollapsed)return [];
	let x=s.anchorNode,y=s.focusNode,a=_CA(x,y);
	if(!a)return [];
	return GNB(a,x,y)
}
function _Pr(n){
	let a=[n]
	for(;n;n=n.parentNode)a.unshift(n);
	return a
}
function _CA(x,y){
	let a=_Pr(x),b=_Pr(y);
	if(a[0]!=b[0])throw 'No common ancestor!'
	for(let i=0;i<a.length;i++)if(a[i]!=b[i])return a[i-1]
}
function _IsD(p,c){
	let n=c;
	while(n!=null){
		if(n==p)return true;
		n=n.parentNode
	}
	return false
}
function GNB(r,x,y){
	let z=[],b=false;
	for(let i=0;i<r.childNodes.length;i++){
		let e=r.childNodes[i]
		if(_IsD(e,x)||_IsD(e,y)){
			b=z.length==0;
			z.push(e)
		}else if(z.length==0){}
		else if(b)z.push(e);
		else return z
	}
	if(z.length==0)return [r];
	else if(_IsD(z[z.length-1],x)||_IsD(z[z.length-1],y))return z;
	else return [z[0]]
}
function _s(h,b){
	let s=content.getSelection(),r,f;
	if(s.getRangeAt&&s.rangeCount){
		r=content.getSelection().getRangeAt(0);
		r.deleteContents();
		if(r.createContextualFragment)f=r.createContextualFragment(h);
		else{
			let d=Doc().createElement(cDiv),c;
			d.innerHTML=h;
			f=Doc().createDocumentFragment();
			while((c=d.firstChild))f.appendChild(c)
		}
		let n=f.firstChild,l=f.lastChild;
		r.insertNode(f);
		if(b){
			if(n){
				r.setStartBefore(n);
				r.setEndAfter(l)
			}
			s.removeAllRanges();
			s.addRange(r)
		}
	}
}
function OurLinksInSel(){
	let x=GSN();
	if(!x)if(x.length==0)return null;
	let r=[];
	for(let i=0;i<x.length;i++){
		let e=x[i],c=e.className;
		try{
			let s=e.getElementsByTagName(cA);
			for(let j=0;j<s.length;j++){let t=s[j],f=t.className;if(f==cCnEuCasesLink||f==cCnEucasesTerm)r.push(t)}
			s=e.getElementsByTagName(cSpan);
			for(let j=0;j<s.length;j++){let t=s[j],f=t.className;if(f==cCnEuCasesLink||f==cCnEucasesTerm)r.push(t)}
		}catch(_){continue}
		if(c!=cCnEuCasesLink&&c!=cCnEucasesTerm)continue;
		r.push(e)
	}
	return r
}
function LinksInSel(){
	let x=GSN();
	if(!x)if(x.length==0)return null;
	let r=[];
	for(let i=0;i<x.length;i++){
		let e=x[i];
		if(e.tagName==cA)r.push(e);
		try{
			let s=e.getElementsByTagName(cA);
			for(let j=0;j<s.length;j++)r.push(s[j])
		}catch(_){}
	}
	return r
}
function Rm1Lnk(l){if(l)l.outerHTML=l.innerHTML}
function _RmT(t){
	let f;
	do{
		f=false;
		let a=GetElsByTagName(t);
		if(a.length==0)break;
		for(let i=0;i<a.length;i++){
			let e=a[i],c=e.className;
			if(c!=cCnEuCasesLink&&c!=cCnEucasesTerm)continue;
			f=true;
			Rm1Lnk(e)
		}
	}while(f)
}
function removehyperlinks(){
	let x=new Rsrc();
	if(!Qst(x.cPlzConfirmRmLnkSel))return;
	try{
		_RmT(cA);
		_RmT(cSpan)
		Msg(x.cLinksRemoved)
	}catch(_){Msg(_,ctX)}
}
function _x(a){return document.getElementById('MNU-string-bundle').getString(a)}
function GetSelTxt(){
	let t='';
	try{t=content.getSelection().getRangeAt(0)}catch(_){}
	return t
}
function IsLinkInSel(){
	let t=false;
	try{
		let x=content.getSelection().getElementsByTagName(cA);
		t=x.length>0
	}catch(_){console.log(_.message)}
	return t
}
function ShowBusy(v){
	if(v){
		let r=new Rsrc();
		MNU.o.CurrentDoc=Doc();
		MNU.o.CurrentDocBody=DocBody();
		MNU.o.CurrentBusy=MNU.o.CurrentDoc.createElement(cDiv);
		MNU.o.CurrentBusy.id=cIdBusyBox;
		MNU.o.CurrentBusy.innerHTML=r.processingLinksMessage;
		let s=MNU.o.CurrentBusy.style;
		s.position='fixed';
		s.top='10px';
		s.left='10px';
		s.zIndex='1000';
		s.padding='10px';
		s.paddingLeft='20px';
		s.paddingRight='20px';
		s.border='5px solid grey';
		s.backgroundColor='white';
		s.fontSize='16px';
		s.fontWeight='bold';
		s.visibility=cVisible;
		s.display=cBlock;
		MNU.o.CurrentDocBody.appendChild(MNU.o.CurrentBusy)
	}else{
		let x=MNU.o.CurrentDoc.getElementById(cIdBusyBox);
		if(!x)x=GetElById(cIdBusyBox);
		if(!x)return;
		try{MNU.o.CurrentDocBody.removeChild(MNU.o.CurrentBusy)}catch(_){}
		try{MNU.o.CurrentDocBody.removeChild(x)}catch(_){}
		try{
			let s=x.style;
			s.visibility=cHidden;
			s.display=cNone
		}catch(_){}
	}
}
function SetStrPref(k,v){
	let p=Components.classes['@mozilla.org/preferences-service;1'].getService(Components.interfaces.nsIPrefService).getBranch('extensions.MNU.'),
	s=Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString);
	s.data=v;
	p.setComplexValue(k,Components.interfaces.nsISupportsString,s)
}
function GetStrPref(k){
	let r='',p=Components.classes['@mozilla.org/preferences-service;1'].getService(Components.interfaces.nsIPrefService).getBranch('extensions.MNU.');
	if(p.getPrefType(k))r=p.getComplexValue(k,Components.interfaces.nsISupportsString).data;//nsIPrefLocalizedString
	return r
}
function GetPrefUiLang(){
	let p=Components.classes['@mozilla.org/preferences-service;1'].getService(Components.interfaces.nsIPrefService).getBranch('extensions.MNU.');
	try{p.getIntPref(cUiLangIdx)}catch(_){p.setIntPref(cUiLangIdx,4)}//en!
	return p.getIntPref(cUiLangIdx)
}
function GetPrefNewLinkTarget(){
	let p=Components.classes['@mozilla.org/preferences-service;1'].getService(Components.interfaces.nsIPrefService).getBranch('extensions.MNU.');
	try{p.getIntPref(cNewLinkInIdx)}catch(_){p.setIntPref(cNewLinkInIdx,1)}//_blank
	if(p.getIntPref(cNewLinkInIdx)==1)return '_blank';
	return '_top'
}
function LANG(){
	let x=new Rsrc();
	return x.fLangByNum(GetPrefUiLang())
}
function Rsrc(aLang){
	this.Lang='';
	this.LangId=0;
	this.DocLang='';
	this.DocLangId=0;
	//
	this.allDocs=
	this.euLegislation=
	this.euCaseLaw=
	this.natLegislation=
	this.natCaseLaw=
	this.removeLink=
	this.processingLinksMessage=
	this.confirmCheckForLinks=
	this.confirmRemoveLinks=
	this.cMsgNoLinksSetByELCFound=
	this.cPlsSelLink=
	this.cPlzConfirmRmLnkSel=
	this.cSuccess=
	this.cWaitUntilAsyncEnd=
	this.msgLoading=
	this.msgErrorHintLoading=
	this.cLang=
	this.OK=
	this.Cancel=
	this.cYes=
	this.cNo=
	this.cMsgChgLng=
	this.cCheckFrLinks=
	this.cRemoveLinks=
	this.cInsertLink=
	this.cRemoveLinksSel=
	this.Save2Xml=
	this.cm_Credentials=
	this.cLinksRemoved=
	this.cLinksRemovedFromSel=
	this.cPlzInpulLink=
	this.cInformation=
	this.cWarning=
	this.cError=
	this.cException=
	this.cQuestion=
	this.cNoLinksInSel=
	this.cNoSelection=
	this.cInsLinkSucc=
	this.cAttention=
	this.shortCite=
	this.fullCite=
	this.cTarget4NewLink=
	this.cTarget4NewLink_Blank=
	this.cTarget4NewLink_Self=
	this.cStAddNewLink=
	this.cStCredentials=
	this.cStPutLinksAndTerms=
	this.cStRemove1=
	this.cStRemoveLinksAndTerms=
	this.cStSave2Xml='';
	
	this.fLangNum=function(l){
		switch(l){
		case 'bg': return 1;
		case 'de': return 2;
		case 'fr': return 3;
		case 'en': return 4;
		case 'it': return 5;
		default: return 4
		}
	};
	this.fLangByNum=function(i){
		switch(i){
		case 1: return 'bg';
		case 2: return 'de';
		case 3: return 'fr';
		case 4: return 'en';
		case 5: return 'it';
		default: return 'en'
		}
	};
	this.WinLangNum=function(l){
		switch(l){
		case 'bg': return 1026;
		case 'de': return 1031;
		case 'fr': return 1036;
		case 'en': return 2057;
		case 'it': return 1040
		default: return 2057
		}
	};
	this.SetLangIdx=function(i){this.SetLang(this.fLangByNum(i))};
	this.SetLang=function(l){
		if(l==this.Lang)return;
		this.Lang=l;
		this.LangId=this.fLangNum(l);
		switch(l){
		case 'bg':
			this.allDocs='Всички документи';
			this.euLegislation='Европейско законодателство';
			this.euCaseLaw='Европейска съдебна практика';
			this.natLegislation='Национално законодателство';
			this.natCaseLaw='Национална съдебна практика';
			this.removeLink='Премахни линк';
			this.processingLinksMessage='Провери за връзки...';
			this.confirmCheckForLinks='EULinksChecker ще провери дали съществуват препратки към законодателството и съдебната практика на Европейския съюз и ще постави връзки към цитираните нормативни актове. Това може да отнеме известно време. Моля, потвърдете!';
			this.confirmRemoveLinks='Моля, потвърдете изтриването на връзките, поставени от EULinksChecker.';
			this.cMsgNoLinksSetByELCFound='Не са намерени връзки, поставени от EULinksChecker.';
			this.cPlsSelLink='Моля, маркирайте текст с връзки, поставени от EULinksChecker.';
			this.cPlzConfirmRmLnkSel='Връзките, поставени от EULinksChecker ще бъдат изтрити. Моля, потвърдете!';
			this.cSuccess='Проверката за връзки приключи.';
			this.cWaitUntilAsyncEnd='Моля, изчакайте завършването на асинхронната операция.';
			this.msgLoading='Зареждане...';
			this.msgErrorHintLoading='Грешка при зареждането! За повече информация: <a href="http://www.help.eucases.eu/ieaddin" target="_blank">www.help.eucases.eu/ieaddin</a>';
			this.cLang='Език на интерфейса';
			this.OK='Потвърди';
			this.Cancel='Откажи';
			this.cYes='Да';
			this.cNo='Не';
			this.cMsgChgLng='С това действие ще промените езика на потребителския интерфейс.';
			this.cCheckFrLinks='Провери за връзки';
			this.cRemoveLinks='Премахни връзките';
			this.cInsertLink='Въведи линк';
			this.cRemoveLinksSel='Премахни връзките от избраното';
			this.Save2Xml='Запиши текста в XML-file';
			this.cm_Credentials='Настройки';
			this.cLinksRemoved='Премахването на връзките приключи';
			this.cLinksRemovedFromSel='Премахването на връзките от селекцията приключи';
			this.cPlzInpulLink='Моля, въведете адрес:';
			this.cInformation='Информация';
			this.cWarning='Предупреждение';
			this.cError='Грешка';
			this.cException='Изключение';
			this.cQuestion='Въпрос';
			this.cNoLinksInSel='Няма линкове в избраното';
			this.cNoSelection='Няма нищо избрано.';
			this.cInsLinkSucc='Поставянето на линк премина успешно.';
			this.cAttention='Внимание';
			this.shortCite='Кратък цитат';
			this.fullCite='Пълен цитат';
			this.cTarget4NewLink='Новите линкове да се отварят в:';
			this.cTarget4NewLink_Blank='нов прозорец';
			this.cTarget4NewLink_Self='същия прозорец';
			this.cStPutLinksAndTerms='Разпознаване и маркиране на препратки към законодателството и съдебната практика на Европейския съюз';
			this.cStAddNewLink='Ръчно поставяне на връзка към външен уеб ресурс';
			this.cStCredentials='Настройки';
			this.cStRemove1='Премахване в маркирания текст на поставените от EULinksChecker връзки към законодателството и съдебната практика на Европейския съюз';
			this.cStRemoveLinksAndTerms='Премахване на връзките към законодателството и съдебната практика на Европейския съюз, поставени от EULinksChecker с функцията "Провери за връзки"';
			this.cStSave2Xml='Запиши текста в XML-file';
			break;
		case 'de':
			this.allDocs='Alle Dokumente';
			this.euLegislation='EU-Gesetzgebung';
			this.euCaseLaw='EU-Rechtsprechung';
			this.natLegislation='Nationale Gesetzgebung';
			this.natCaseLaw='Nationale Rechtsprechung';
			this.removeLink='Link entfernen';
			this.processingLinksMessage='Überprüfen Sie für Links...';
			this.confirmCheckForLinks='EULinksChecker wird überprüfen ob Verweise auf EU-Gesetzgebung oder Rechtsprechung  vorhanden sind und wird Links zu den zitierten Rechtsakten einfügen. Dies kann eine Weile dauern. Bitte, bestätigen!';
			this.confirmRemoveLinks='Bitte bestätigen Sie die Entfernung von Links gesetzt von EULinksCheker.';
			this.cMsgNoLinksSetByELCFound='Keine Links vom EULinksChecker gefunden.';
			this.cPlsSelLink='Bitte, wählen Sie einen Text mit Links gesetzt vom EULinksChecker.';
			this.cPlzConfirmRmLnkSel='Die vom EULinksChecker gesetzten Links werden gelöscht. Bitte, bestätigen!';
			this.cSuccess='Überprüfung für Links abgeschlossen.';
			this.cWaitUntilAsyncEnd='Bitte warten Sie bis der asynchronische Vorgang abgeschlossen ist.';
			this.msgLoading='Laden...';
			this.msgErrorHintLoading='Fehler beim Laden! Für mehr Information: <a href="http://www.help.eucases.eu/ieaddin" target="_blank">www.help.eucases.eu/ieaddin</a>';
			this.cLang='Sprache';
			this.OK='OK';
			this.Cancel='Stornier';
			this.cYes='Ja';
			this.cNo='Nein';
			this.cMsgChgLng='Sie sind dabei, die Sprache der Benutzeroberfläche zu ändern.';
			this.cCheckFrLinks='Überprüfung für Links';
			this.cRemoveLinks='Links entfernen';
			this.cInsertLink='Link setzen';
			this.cRemoveLinksSel='Links von Selektion entfernen';
			this.Save2Xml='Text in XML-Datei speichern';
			this.cm_Credentials='Einstellungen';
			this.cLinksRemoved='Links wurden entfernt.';
			this.cLinksRemovedFromSel='Links wurden entfernt.';
			this.cPlzInpulLink='Bitte, enter address:';
			this.cInformation='Information';
			this.cWarning='Warnung';
			this.cError='Fehler';
			this.cException='Ausnahme';
			this.cQuestion='Frage';
			this.cNoLinksInSel='Keine Links vorhanden';
			this.cNoSelection='Es ist nichts ausgewählt.';
			this.cInsLinkSucc='Linkssetzung erfolgreich abgelaufen.';
			this.cAttention='Achtung';
			this.shortCite='Kurzes Zitat';
			this.fullCite='Vollständiges Zitat';
			this.cTarget4NewLink='Die neuen Links sind in ….geöffnet:';
			this.cTarget4NewLink_Blank='ein neues Fenster';
			this.cTarget4NewLink_Self='das gleiche Fenster';
			this.cStPutLinksAndTerms='Erkennung und Hervorhebung von Verweisen auf EU-Gesetzgebung und Rechtsprechung';
			this.cStAddNewLink='Manuelle Einfügung vom Link zur externen Web-Ressource';
			this.cStCredentials='Einstellungen';
			this.cStRemove1='Entfernung von den vom EULinksChecker gesetzten Links die auf EU-Gesetzgebung und Rechtsprechung verweisen';
			this.cStRemoveLinksAndTerms='Entfernung der Links gesetzt von "Überprüfen Sie für Links" Funktion des EULinksCheckers';
			this.cStSave2Xml='Speichern Sie den Text in XML-file';
			break;
		case 'fr':
			this.allDocs='Tous les documents';
			this.euLegislation='Législation de l\'UE';
			this.euCaseLaw='Jurisprudence de l\'UE';
			this.natLegislation='Législation nationale';
			this.natCaseLaw='Jurisprudence nationale';
			this.removeLink='Supprimer le lien';
			this.processingLinksMessage='Vérifier les liens...';
			this.confirmCheckForLinks='EULinksChecker vérifiera s’il existe des références menant vers la législation et la jurisprudence de l\'Union européenne et mettra des liens vers les dispositions cités. Cela peut prendre un certain temps.Veuillez  confirmer!';
			this.confirmRemoveLinks='Veuillez confirmer la suppression des liens établis par EULinksChecker.';
			this.cMsgNoLinksSetByELCFound='Aucun lien établi par EULinksChecker.';
			this.cPlsSelLink='Veuillez sélectionner le texte avec des liens établis par EULinksChecker.';
			this.cPlzConfirmRmLnkSel='Les liens établis par EULinksChecker seront supprimés. Veuillez confirmer!';
			this.cSuccess='La vérification des liens est terminée.';
			this.cWaitUntilAsyncEnd='Veuillez attendre la fin de l\'opération asynchrone';
			this.msgLoading='Chargement...';
			this.msgErrorHintLoading='Erreur de chargement! Pour plus de renseignements: <a href="http://www.help.eucases.eu/ieaddin" target="_blank">www.help.eucases.eu/ieaddin</a>';
			this.cLang='Langue de l\'interface';
			this.OK='Confirmer';
			this.Cancel='Annuler';
			this.cYes='Oui';
			this.cNo='Non';
			this.cMsgChgLng='Vous êtes sur le point de changer la langue de l\'interface utilisateur.';
			this.cCheckFrLinks='Vérifier les liens';
			this.cRemoveLinks='Supprimer les liens';
			this.cInsertLink='Insérer un lien';
			this.cRemoveLinksSel='Supprimer les liens de la selection';
			this.Save2Xml='Enregistrer le texte au format XML';
			this.cm_Credentials='Paramètres';
			this.cLinksRemoved='Les liens sont supprimés.';
			this.cLinksRemovedFromSel='Les liens de la sélection sont supprimés.';
			this.cPlzInpulLink='Veuillez entrer une adresse:';
			this.cInformation='Information';
			this.cWarning='Avertissement';
			this.cError='Erreur';
			this.cException='Exception';
			this.cQuestion='Question';
			this.cNoLinksInSel='Aucun lien dans la sélection';
			this.cNoSelection='Aucune sélection.';
			this.cInsLinkSucc='L\'établissement du lien a reussi.';
			this.cAttention='Attention';
			this.shortCite='Courte citation';
			this.fullCite='Citation complète';
			this.cTarget4NewLink='Les nouveaux liens s\'ouvrent dans:';
			this.cTarget4NewLink_Blank='une nouvelle fenêtre';
			this.cTarget4NewLink_Self='la même fenêtre';
			this.cStPutLinksAndTerms='Identification et le marquage des références vers la législation et la jurisprudence de l\'Union européenne';
			this.cStAddNewLink='Insérez manuellement un lien vers une ressource Web externe';
			this.cStCredentials='Paramètres';
			this.cStRemove1='Retrait de liens menant vers la législation et la jurisprudence de l\'UE établis par EULinksChecker à partir du texte sélectionné';
			this.cStRemoveLinksAndTerms='Retrait de liens menant vers la législation et la jurisprudence de l\'UE établis par EULinksChecker avec la fonction "Vérifier les liens"';
			this.cStSave2Xml='Enregistrer le texte au format XML';
			break;
		case 'en':
			this.allDocs='All documents';
			this.euLegislation='EU legislation';
			this.euCaseLaw='EU case law';
			this.natLegislation='National legislation';
			this.natCaseLaw='National case law';
			this.removeLink='Remove a link';
			this.processingLinksMessage='Check for links...';
			this.confirmCheckForLinks='EULinksChecker will check for the existence of references to EU legislation and case law and will set links to the cited legal acts. This may take a while. Please, confirm!';
			this.confirmRemoveLinks='Please confirm the removal of links set by EULinksChecker.';
			this.cMsgNoLinksSetByELCFound='No links set by EULinksChecker have been found.';
			this.cPlsSelLink='Please select text with links set by EULinksChecker.';
			this.cPlzConfirmRmLnkSel='The links set by EULinksChecker will be removed. Please confirm!';
			this.cSuccess='Check for links completed.';
			this.cWaitUntilAsyncEnd='Please wait for the asynchronous operation to end.';
			this.msgLoading='Loading...';
			this.msgErrorHintLoading='An error during loading occured! For more information: <a href="http://www.help.eucases.eu/ieaddin" target="_blank">www.help.eucases.eu/ieaddin</a>';
			this.cLang='User interface language';
			this.OK='OK';
			this.Cancel='Cancel';
			this.cYes='Yes';
			this.cNo='No';
			this.cMsgChgLng='You are about to change the user interface language.';
			this.cCheckFrLinks='Check for links';
			this.cRemoveLinks='Remove the links';
			this.cInsertLink='Insert a link';
			this.cRemoveLinksSel='Remove the links from the selection';
			this.Save2Xml='Save text as XML-file';
			this.cm_Credentials='Settings';
			this.cLinksRemoved='The links have been removed.';
			this.cLinksRemovedFromSel='The links have been removed from the selection.';
			this.cPlzInpulLink='Please, enter an address:';
			this.cInformation='Information';
			this.cWarning='Warning';
			this.cError='Error';
			this.cException='Exception';
			this.cQuestion='Question';
			this.cNoLinksInSel='No links in the selection';
			this.cNoSelection='Nothing is selected.';
			this.cInsLinkSucc='The setting of a link secceeded.';
			this.cAttention='Attention';
			this.shortCite='Short citation';
			this.fullCite='Full citation';
			this.cTarget4NewLink='Let new links open in:';
			this.cTarget4NewLink_Blank='a new window';
			this.cTarget4NewLink_Self='the same window';
			this.cStPutLinksAndTerms='Recognition and highlighting of references to EU legislation and case law';
			this.cStAddNewLink='Manual insertion of a link to external web resource';
			this.cStCredentials='Settings';
			this.cStRemove1='Removing links to EU legislation and case law set by EULinksChecker from the selected text';
			this.cStRemoveLinksAndTerms='Removal of links to EU legislation and case law set by "Check for links" function of EULinksChecker';
			this.cStSave2Xml='Save text to XML-file';
			break;
		case 'it':
			this.allDocs='Tutti i documenti';
			this.euLegislation='Legislazione UE';
			this.euCaseLaw='Giurisprudenza dell\'UE';
			this.natLegislation='Legislazione nazionale';
			this.natCaseLaw='Giurisprudenza nazionale';
			this.removeLink='Rimuovi collegamento';
			this.processingLinksMessage='Verificare la presenza dei collegamenti...';
			this.confirmCheckForLinks='EULinksChecker verificherà la presenza di riferimenti alla legislazione e alla giurisprudenza dell\'UE e metterà collegamenti ai testi normativi citati. Questo potrebbe richiedere del tempo. Si prega di confermare!';
			this.confirmRemoveLinks='Si prega di confermare l\'eliminazione dеi collegamenti messi dal EULinksChecker.';
			this.cMsgNoLinksSetByELCFound='Nessun collegamento messo dal EULinksChecker e stato trovato.';
			this.cPlsSelLink='Si prega di selezionare testo con collegamenti messi dal EULinksChecker.';
			this.cPlzConfirmRmLnkSel='I collegamenti messi dal EULinksChecker saranno rimossi. Si prega di confermare!';
			this.cSuccess='Verificazione dei collegamenti compiuta.';
			this.cWaitUntilAsyncEnd='Si prega di attendere il completamento dell\'operazione asincrona.';
			this.msgLoading='Caricamento...';
			this.msgErrorHintLoading='Errore! Per informazione: <a href="http://www.help.eucases.eu/ieaddin" target="_blank">www.help.eucases.eu/ieaddin</a>';
			this.cLang='Lingua';
			this.OK='Conferma';
			this.Cancel='Cancellare';
			this.cYes='Si';
			this.cNo='No';
			this.cMsgChgLng='Questa azione modificherà la lingua dell\'interfaccia utente.';
			this.cCheckFrLinks='Verificare la presenza di collegamenti';
			this.cRemoveLinks='Rimuovere collegamenti';
			this.cInsertLink='Inserisci collegamento';
			this.cRemoveLinksSel='Rimuovere collegamenti dal selezione';
			this.Save2Xml='Salva il testo come file XML';
			this.cm_Credentials='Impostazioni';
			this.cLinksRemoved='I collegamenti sono stati rimossi.';
			this.cLinksRemovedFromSel='I collegamenti sono stati rimossi dalla selezione.';
			this.cPlzInpulLink='Si prega di inserire indirizzo:';
			this.cInformation='Informazione';
			this.cWarning='Avvertimento';
			this.cError='Errore';
			this.cException='Eccezione';
			this.cQuestion='Domanda';
			this.cNoLinksInSel='Nessun link nella selezione';
			this.cNoSelection='Niente selezionato.';
			this.cInsLinkSucc='Il link e messo successivamente.';
			this.cAttention='Attenzione';
			this.shortCite='Citazione breve';
			this.fullCite='Citazione completa';
			this.cTarget4NewLink='I nuovi link si aprono in :';
			this.cTarget4NewLink_Blank='una nuova finestra';
			this.cTarget4NewLink_Self='la stessa finestra';
			this.cStPutLinksAndTerms='Identificazione e marcatura di riferimenti alla legislazione e alla giurisprudenza dell\'Unione europea';
			this.cStAddNewLink='Inserire manualmente un collegamento ad una risorsa web esterna';
			this.cStCredentials='Impostazioni';
			this.cStRemove1='Rimuovere i collegamenti alla legislazione e alla giurisprudenza dell\'UE messi dal EULinksChecker dal testo selezionato';
			this.cStRemoveLinksAndTerms='Rimuovere i collegamenti alla legislazione e alla giurisprudenza dell\'UE messi dal EULinksChecker con la funzione "Verifica la presenza di collegamenti".';
			this.cStSave2Xml='Salva il testo come file XML';
			break
		}
	};
	if(aLang)this.SetLang(aLang);
	else this.SetLangIdx(GetPrefUiLang());
	return this
}
if(!MNU)var MNU={};
MNU.o={
	CurrentDoc: null,
	CurrentDocBody: null,
	CurrentBusy: null,
	mLinkAtCrs: null,
	eucasesSetLinks: function(){
		if(MNU.o.CurrentDocBody){let x=new Rsrc();Msg(x.cWaitUntilAsyncEnd);return}
		ShowBusy(true);
AddScrHT(Doc(),'var EUCASES_UI_LANG="'+LANG()+'";',cVarsId);
		let t=DocBodyHtml();
		if(t==''){Msg('No selection');return}
		let x=new XMLHttpRequest();
		x.onreadystatechange=function(){
			let r=new Rsrc();
			AddScrHT(MNU.o.CurrentDoc,'var EUCASES_UI_LANG="'+LANG()+'";',cVarsId);
			AddScrHT(MNU.o.CurrentDoc,'EUCASES_UI_LANG="'+LANG()+'";',cVars1Id);
try{MNU.o.CurrentDoc.EUCASES_UI_LANG=LANG()}catch(_){}
			if(x.readyState==4&&x.status==200){
				AddCssH(MNU.o.CurrentDoc,cStyleId);
				AddScrHT(MNU.o.CurrentDoc,'var EUCASES_UI_LANG="'+LANG()+'";',cVarsId);
				MNU.o.CurrentDocBody.innerHTML=x.responseText;
				AddScrH(MNU.o.CurrentDoc,cCommonProcsId);
				ShowBusy(false);
				MNU.o.CurrentBusy=null;
				MNU.o.CurrentDocBody=null
				MNU.o.CurrentDoc=null;
				Msg(r.cSuccess)
			}
		}
		x.open('POST','http://techno.eucases.eu/FrontEndREST/api/Links/PutHtmlLinks/',true);
		x.setRequestHeader('Content-type','application/x-www-form-urlencoded');
		x.setRequestHeader('UI-Language',LANG());
		x.send(t)
	},
	eucasesDownloadXml: function(){
		let d=Doc(),f=d.createElement('form');
		f.action='http://techno.eucases.eu/FrontEndREST/api/Links/GenerateXmlFromDoubleEncoded';
		f.method='post';
		let i=d.createElement('input');
		i.type='text';
		i.name='html';
		i.value=encodeURI(DocBody().outerHTML);
		f.appendChild(i);
		let s=d.createElement('input');
		s.type='submit';
		f.appendChild(s);
		d=d.body;
		d.appendChild(f);
		s.click();
		d.removeChild(f)
	},
	HasSelection: function(){return GetSelTxt()!=''},
	RemoveLink: function(e){Rm1Lnk(MNU.o.mLinkAtCrs)},
	RemoveLinksSel: function(e){
		let a=OurLinksInSel();
		if(!a)return;
		let r=new Rsrc();
		if(!Qst(r.cPlzConfirmRmLnkSel))return;
		a.forEach(function(_){Rm1Lnk(_)});//for(let i=0;i<a.length;i++)Rm1Lnk(a[i]);
		Msg(r.cLinksRemovedFromSel)
	},
	InsertLink: function(e){
		let b=MNU.o.HasSelection(),r=new Rsrc();
		if(!b){Msg(r.cNoSelection);return}
		let l={v:cHttp_};if(!Inp(r.cPlzInpulLink,ctQ,l))return;
		let s=GetSelTxt();
		_s('<a href="'+l.v+'" target="'+GetPrefNewLinkTarget()+'">'+s+'</a>',false)
		Msg(r.cInsLinkSucc)
	},
	EI1: function(e){this.eucasesSetLinks()},
	EI2: function(e){removehyperlinks()},
	tstTB1: function(e){Msg('You\'ve just pressed me')},
	Settings: function(e){
		let x=Components.classes['@mozilla.org/embedcomp/window-watcher;1'].getService(Components.interfaces.nsIWindowWatcher),
		r=new Rsrc(),
		y=x.openWindow(
			null,
			'chrome://MNU/content/pref.xul?Lang='+LANG(),
			r.cm_Credentials,
			'chrome,dialog,modal,centerscreen,alwaysRaised',
			null
		);
		this.startup(e)
	},
	Export2Xml: function(e){this.eucasesDownloadXml()},
	startup: function(e){
		let x=new Rsrc(),
		o=document.getElementById('tbbCheck4LinksId');if(o){o.label=x.cCheckFrLinks;o.tooltipText=x.cStPutLinksAndTerms}
		o=document.getElementById('tbbRmAllLinksId');if(o){o.label=x.cRemoveLinks;o.tooltipText=x.cStRemoveLinksAndTerms}
		o=document.getElementById('tbbInsertLink');if(o){o.label=x.cInsertLink;o.tooltipText=x.cStAddNewLink}
		o=document.getElementById('tbbRmLinksSelId');if(o){o.label=x.cRemoveLinksSel;o.tooltipText=x.cStRemove1}
		o=document.getElementById('tbbXmlId');if(o){o.label=x.Save2Xml;o.tooltipText=x.cStSave2Xml}
		o=document.getElementById('tbbSettingsId');if(o){o.label=x.cm_Credentials;o.tooltipText=x.cStCredentials}
		o=document.getElementById('MNU-EUL-menu-item1');if(o)o.label=x.cCheckFrLinks;
		o=document.getElementById('MNU-EUL-menu-item2');if(o)o.label=x.cRemoveLinks;
		o=document.getElementById('miXml');if(o)o.label=x.Save2Xml;
		o=document.getElementById('miSettingsId');if(o)o.label=x.cm_Credentials
	}
};
window.addEventListener('load',function(event){MNU.o.startup(event)},false);
function HelloTo_callBack(r){Msg(r)}
function _st(o,b,l){
	o=GetElById(o);
	if(!o)return;
	o.disabled=o.hidden=b;
	if(b)return;
	o.label=l
}
function PROC(event){
	MNU.o.mLinkAtCrs=LinkAtCursor(event);
	let x=Rsrc(),s=OurLinksInSel(),l=LinksInSel(),t=GetSelTxt(),b,r=s?s.length==0:true;
	if(t=='')b=true;else if(l&&l.length>0)b=true;else b=false;
	try{
		_st('RemoveLinksSel',r,x.cRemoveLinksSel);
		_st('RemoveLink',MNU.o.mLinkAtCrs!=null,x.cRemoveLinks);
		_st('InsertLink',b,x.cInsertLink)
	}catch(_){console.log(_.message)}
	updateEditUIVisibility;
	gContextMenu=new nsContextMenu(window.content.document,event.shiftKey);
	if(gContextMenu.shouldDisplay)updateEditUIVisibility();
	return gContextMenu.shouldDisplay
}
function AddScrHT(d,t,i){
	if(t=='')return;
//	if(i)if(d.getElementById(i))return;
	if(i){
		let e=d.getElementById(i);
		if(e)e.parentNode.removeChild(e)
	}
	let h=d.getElementsByTagName(cHead)[0],s=d.createElement('script');
	if(i)s.id=i;
	s.type='text/javascript';
	//s.defer='defer';
	s.text=t;
	if(h)h.appendChild(s)
}
function AddScrH(d,i){AddScrHT(d,HG('http://techno.eucases.eu/FrontEndREST/api/Links/GetResourceFile?fileName=context-menu.js'),i)}
function AddCssH(d,i){
	if(i)if(d.getElementById(i))return;
	let t=HG('http://techno.eucases.eu/FrontEndREST/api/Links/GetResourceFile?fileName=context-menu.css');
	if(t=='')return;
	let h=d.getElementsByTagName(cHead)[0],s=d.createElement('style');
	if(i)s.id=i;
	s.type='text/css';
	if(s.styleSheet)s.styleSheet.cssText=t;else s.appendChild(d.createTextNode(t));
	//s.text=t;
	//s.src='path/to/your-script.js';
	if(h)h.appendChild(s)
}
function _PODBA(){let r=new Rsrc();Msg(r.cMsgChgLng)}
function _PODA(e){MNU.o.startup(e)}
function _POL(){
	let r=new Rsrc(),x=GetElById('UILangLblId');if(x)x.value=r.cLang;
	x=GetElById('Settings-pane');if(x)x.label=r.cm_Credentials;
	x=GetElById('NewLinkInLblId');if(x)x.value=r.cTarget4NewLink;
	x=GetElById('NewLlinksIdx_1');if(x)x.label=r.cTarget4NewLink_Blank;
	x=GetElById('NewLlinksIdx_2');if(x)x.label=r.cTarget4NewLink_Self;
	x=GetElById('MNU-prefs');
	if(x){
		x.title=r.cm_Credentials;x.setAttribute(cTitle,r.cm_Credentials)
		let b=x.getButton('accept');if(b)b.label=r.OK;//b.setAttribute('image','chrome://MNU/skin/b/OK.png')
		b=x.getButton('cancel');if(b)b.label=r.Cancel
	}
}