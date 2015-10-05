//http://192.168.2.33/1/EuLinksChecker.crx
var cIdBusyBox='IdBusyBox',cIdModalBox='IdModalBox',cNone='none',cBlock='block',cHidden='hidden',cVisible='visible',cA='A',cDiv='div',cSpan='SPAN',
	cCnEuCasesLink='eucases-link',cCnEucasesTerm='eucases-term',cStyleId='StyleId',cVarsId='VarsId',cVars1Id='Vars1Id',cCommonProcsId='CommonProcsId',cModalProcsId='ModalProcsId',cModalResScrId='ModalResScrId',cModalResId='ModalResId',cUiLangId='UiLangId',cNotify='notifyme',
	cSymbol='symbol',cGET='GET',cUiLangIdx='UiLangIdx',cTitle='title',cModalResult='ModalResult',cHead='head',ctA='A',ctI='I',ctE='E',ctX='X',ctW='W',ctQ='Q',cType='type',cMsg='msg',cOk='OK',cCancel='Cancel',cYes='Yes',cNo='No',cBut='but',cRes='res',cHttp_='http://';
//-----
function LANG(){
	var x=new Rsrc();
	return x.fLangByNum(GetPrefUiLang())
}
function Qt(){return String.fromCharCode(39)}
function _eucasesSetLinks(p){
	chrome.tabs.query({active:true},function(t){
		chrome.tabs.executeScript(t[0].id,{file:'cs1.js'},function(r){
			chrome.tabs.executeScript(t[0].id,{code:'if(!window.EUCASES_UI_LANG)window.EUCASES_UI_LANG='+Qt()+LANG()+Qt()+';eucasesSetLinks()'},function(s){_CS(self,p)})
		});
	})
}
function _CS(a,p){
	if(p)return;
try{
//console.log('_CS: name='+a.name)
	if(a)if(!a.closed)a.close()
}catch(_){}
}
function RmLinksInCT(){
	var x=new Rsrc();
	if(confirm(x.confirmRemoveLinks))chrome.tabs.query({active:true,status:'complete'},function(t){
		chrome.tabs.executeScript(t[0].id,{code:
'function RmT(t){var a,e,f,i;do{f=false;a=document.getElementsByTagName(t);if(a.length==0)break;for(i=0;i<a.length;i++){e=a[i],c=e.className;if(c!="eucases-link"&&c!="eucases-term")continue;f=true;e.outerHTML=e.innerHTML}}while(f)}RmT("A");RmT("SPAN")'
		},function(r){
				if(chrome.runtime.lastError)console.error(chrome.runtime.lastError.message);
				else alert(x.cLinksRemoved)
			}
		)
	})
}
function InjVar(){chrome.tabs.query({active:true},function(t){chrome.tabs.executeScript(t[0].id,{code:'if(!window.EUCASES_UI_LANG)window.EUCASES_UI_LANG='+Qt()+LANG()+Qt()+';'})})}
function RmAllLinks(p){
InjVar();
	var x=new Rsrc();
	if(confirm(x.confirmRemoveLinks))chrome.tabs.query({active:true},function(t){
		chrome.tabs.executeScript(t[0].id,{file:'cs1.js'},function(r){
			chrome.tabs.executeScript(t[0].id,{code:'if(!window.EUCASES_UI_LANG)window.EUCASES_UI_LANG='+Qt()+LANG()+Qt()+';eucasesRemoveAllLinks()'},function(s){_CS(self,p)})
		});
	})
}
function Chk4Links(p){
	var x=new Rsrc();
	InjVar();
	if(confirm(x.confirmCheckForLinks))_eucasesSetLinks(p)
}
function rmls(p){
InjVar();
var rs=new Rsrc();
if(!confirm(rs.cPlzConfirmRmLnkSel))return;
if(p)chrome.tabs.executeScript(lti,{code:'try{window.getSelection().toString()}catch(_){""}'},
	function(r){
		chrome.tabs.executeScript(lti,{file:'cs1.js'},function(x){
			chrome.tabs.executeScript(lti,{code:'if(!window.EUCASES_UI_LANG)window.EUCASES_UI_LANG='+Qt()+LANG()+Qt()+';replaceSelectionURIEncoded("'+encodeURI(r)+'")'},
				function(y){alert(rs.cLinksRemoved)})
		})
	}
);else chrome.extension.sendRequest({action:'getSel'},function(r){
	if(r)if(r.res)
	chrome.tabs.query({active:true},function(t){
		chrome.tabs.executeScript(t[0].id,{file:'cs1.js'},function(x){
			chrome.tabs.executeScript(t[0].id,{code:'if(!window.EUCASES_UI_LANG)window.EUCASES_UI_LANG='+Qt()+LANG()+Qt()+';replaceSelectionURIEncoded("'+encodeURI(r.res)+'")'},
				function(y){alert(rs.cLinksRemoved);_CS(self,p)}
			)
		});
		}
	)}
)}
function xml(){
	InjVar();
	chrome.tabs.query({active:true},function(t){
		chrome.tabs.executeScript(t[0].id,{file:'cs1.js'},function(r){
			chrome.tabs.executeScript(t[0].id,{code:'if(!window.EUCASES_UI_LANG)window.EUCASES_UI_LANG='+Qt()+LANG()+Qt()+';eucasesDownloadXml()'},function(s){_CS(self)})
		});
	})
}
function GetPrefUiLang(){try{return parseInt(localStorage['UILang'])}catch(_){return 4}}
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
	this.cSaveSettings=
	this.cRevertSettings=
	this.cTarget4NewLink=
	this.shortCite=
	this.fullCite=
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
			this.cSaveSettings='Запомни настройките';
			this.cRevertSettings='Възстанови настройките';
			this.cTarget4NewLink='Новите линкове да се отварят в:';
			this.shortCite='Кратък цитат';
			this.fullCite='Пълен цитат';
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
			this.cSaveSettings='Einstellungen speichern';
			this.cRevertSettings='Zurücksetzen der Einstellungen';
			this.cTarget4NewLink='Die neuen Links sind in ….geöffnet:';
			this.shortCite='Kurzes Zitat';
			this.fullCite='Vollständiges Zitat';
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
			this.cSaveSettings='Enregistrer les paramètres';
			this.cRevertSettings='Retirer les paramètres';
			this.cTarget4NewLink='Les nouveaux liens s\'ouvrent dans:';
			this.shortCite='Courte citation';
			this.fullCite='Citation complète';
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
			this.cSaveSettings='Save the settings';
			this.cRevertSettings='Reset the settings';
			this.cTarget4NewLink='Let new links open in:';
			this.shortCite='Short citation';
			this.fullCite='Full citation';
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
			this.cSaveSettings='Salva impostazioni';
			this.cRevertSettings='Ripristinare le impostazioni';
			this.cTarget4NewLink='I nuovi link si aprono in :';
			this.shortCite='Citazione breve';
			this.fullCite='Citazione completa';
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