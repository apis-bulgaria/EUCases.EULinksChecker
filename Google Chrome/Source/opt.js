/*
To save a string to localStorage, use the following code, replacing mysetting and myvalue with your own:

localStorage["mysetting"] = "myvalue";
Or, equivalently:

1
localStorage.mysetting = "myvalue";

localStorage.removeItem("mysetting");
*/
function SetRes(){
	var x=new Rsrc();
	document.getElementById('settit').innerHTML=x.cm_Credentials;
	document.getElementById('UILangLblId').innerHTML=x.cLang;
	document.getElementById('Save').innerHTML=x.cSaveSettings;
	document.getElementById('Del').innerHTML=x.cRevertSettings;
	document.getElementById('NewLlinksLblId').innerHTML=x.cTarget4NewLink
	document.getElementById('NewLlinksIdx_1').innerHTML=x.cTarget4NewLink_Blank
	document.getElementById('NewLlinksIdx_2').innerHTML=x.cTarget4NewLink_Self
}
var cUILang='UILang',cClick='click',cUILangIdx='UILangIdx',cNewLlinksIdx='NewLlinksIdx';
function Init(){
	SetRes();
	document.getElementById("Save").addEventListener(cClick,saveOptions);
	document.getElementById("Del").addEventListener(cClick,eraseOptions);
	loadOptions()
}
window.addEventListener('load',Init);
var defaultColor="blue",defaultUILang=4;
function lo(k,vs,dflt,id){
	var v=localStorage[k];
	if(!v)v=dflt;
	else{
		var b=false;
		vs.forEach(function(e){if(e==v)b=true});
		if(!b)v=dflt
	}
	var s=document.getElementById(id);
	if(!s)return;
	s=s.children;
	for(var i=0;i<s.length;i++){
		var e=s[i];
		if(e.value==v){
			e.selected=true;
			break
		}
	}
}
function loadOptions(){
	lo(cUILang,['1','2','3','4','5'],4,cUILangIdx)
	lo(cNewLlinksIdx,['1','2'],1,cNewLlinksIdx)
}
function so(k,i){
	var x=document.getElementById(i),v=x.children[x.selectedIndex].value;
	localStorage[k]=v
}
function saveOptions(e){
	var x=new Rsrc();
	alert(x.cMsgChgLng);
	so(cUILang,cUILangIdx);
	so(cNewLlinksIdx,cNewLlinksIdx);
	chrome.extension.sendMessage({directive:'langchanged'},function(r){});
	self.close()
}
function eraseOptions(e){
	localStorage.removeItem(cUILang);
	localStorage.removeItem(cNewLlinksIdx);
	location.reload()
}