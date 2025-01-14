if(typeof (window.RadToolbarButtonNamespace)=="undefined"){
window.RadToolbarButtonNamespace=new Object();
}
function RadToolbarButton(_1,_2){
this.ClientID=_1;
this.ToolbarID=_2;
this.ControlElement=document.getElementById(this.ClientID);
this.ToolbarElement=document.getElementById(this.ToolbarID);
this.MouseEnterCheck=0;
this.ButtonGroup="";
var _3=this;
this.MouseOverHandler=function(e){
_3.OnMouseOver(e);
};
this.MouseOutHandler=function(e){
_3.OnMouseOut(e);
};
this.MouseClickHandler=function(e){
_3.OnMouseClick(e);
};
this.MouseDownHandler=function(e){
_3.OnMouseDown(e);
};
this.InternalAttachEvent(this.ControlElement,"mouseover",this.MouseOverHandler);
this.InternalAttachEvent(this.ControlElement,"mouseout",this.MouseOutHandler);
this.InternalAttachEvent(this.ControlElement,"mousedown",this.MouseDownHandler);
this.InternalAttachEvent(this.ControlElement,"click",this.MouseClickHandler);
}
RadToolbarButton.prototype.Dispose=function(){
this.InternalDetachEvent(this.ControlElement,"mouseover",this.MouseOverHandler);
this.InternalDetachEvent(this.ControlElement,"mouseout",this.MouseOutHandler);
this.InternalDetachEvent(this.ControlElement,"mousedown",this.MouseDownHandler);
this.InternalDetachEvent(this.ControlElement,"click",this.MouseClickHandler);
this.ControlElement=null;
this.ToolbarElement=null;
};
RadToolbarButton.prototype.Start=function(){
this.ToggleState();
};
RadToolbarButton.prototype.InternalAttachEvent=function(_8,_9,_a){
try{
if(_8.addEventListener){
_8.addEventListener(_9,_a,true);
}else{
if(_8.attachEvent){
_8.attachEvent("on"+_9,_a);
}
}
}
catch(error){
}
};
RadToolbarButton.prototype.InternalDetachEvent=function(_b,_c,_d){
try{
if(_b.removeEventListener){
_b.removeEventListener(_c,_d,true);
}else{
if(_b.detachEvent){
_b.detachEvent("on"+_c,_d);
}
}
}
catch(error){
}
};
RadToolbarButton.prototype.EventClick=function(e){
if(this.IsToggle){
this.ToggleLogic();
}else{
this.ToggleState();
}
this.ToolbarInstance.FireOnClientClick(this,e);
};
RadToolbarButton.prototype.EventOnMouseOver=function(e){
if(this.IsToggle&&this.Toggled){
this.ApplyStyle("hover_toggled",true);
}else{
this.ApplyStyle("hover",true);
}
this.ToolbarInstance.FireOnClientMouseOver(this,e);
};
RadToolbarButton.prototype.EventOnMouseOut=function(e){
this.ApplyStyle("normal",true);
this.ToggleState();
this.ToolbarInstance.FireOnClientMouseOut(this,e);
};
RadToolbarButton.prototype.EventOnMouseDown=function(e){
this.ApplyStyle("mousedown");
this.ToolbarInstance.FireOnClientMouseDown(this,e);
};
RadToolbarButton.prototype.OnMouseDown=function(e){
if(!this.Enabled){
return;
}
this.EventOnMouseDown(e);
};
RadToolbarButton.prototype.OnMouseOver=function(e){
if(!this.Enabled){
return;
}
if(!this.LeaveCheck){
this.EventOnMouseOver(e);
this.LeaveCheck=true;
}
};
RadToolbarButton.prototype.OnMouseOut=function(e){
if(!this.Enabled){
return;
}
var _15=RadToolbarButtonNamespace.GetEventSourceContainer(e);
var _16;
if(_15==this.ControlElement){
_16=false;
}else{
_16=RadToolbarButtonNamespace.IsParentOfObj(_15,this.ControlElement);
}
if(!_16){
this.EventOnMouseOut(e);
this.LeaveCheck=false;
}
};
RadToolbarButton.prototype.OnMouseClick=function(e){
if(!this.Enabled){
return;
}
this.EventClick(e);
};
RadToolbarButtonNamespace.IsIE=function(){
if(navigator.userAgent.match(/MSIE/i)!=null&&!window.opera){
return true;
}else{
return false;
}
};
RadToolbarButtonNamespace.IsParentOfObj=function(_18,_19){
if(!_18||!_19){
return false;
}
var _1a=_18;
while(((typeof (_1a)!="undefined")&&(_1a!=_19))&&_1a.nodeName!="BODY"&&(_1a.parentNode!=null)){
_1a=_1a.parentNode;
}
if(_1a==_19){
return true;
}
return false;
};
RadToolbarButtonNamespace.GetEventSourceContainer=function(_1b){
if(null==_1b){
return null;
}
if(_1b.toElement){
return _1b.toElement;
}else{
if(_1b.relatedTarget){
return _1b.relatedTarget;
}else{
return null;
}
}
};
RadToolbarButton.prototype.Serialize=function(){
var _1c=this.CommandName+","+(this.Enabled?"1":"0")+","+(this.Hidden?"1":"0")+","+(this.Toggled?"1":"0");
return _1c;
};
RadToolbarButton.prototype.ToggleState=function(){
if(this.Hidden){
this.ControlElement.style.display="none";
}else{
this.ControlElement.style.display="block";
}
if(this.IsToggle&&this.Toggled){
this.ApplyStyle("toggled");
}else{
this.ApplyStyle("normal");
}
if(!this.Enabled){
this.ApplyStyle("disabled");
}
};
RadToolbarButton.prototype.ApplyStyle=function(_1d,_1e){
_1e=_1e&&RadToolbarButtonNamespace.IsIE()&&window[this.ToolbarID].UseFadeEffect;
if(_1e){
this.ControlElement.style.filter="progid:DXImageTransform.Microsoft.Fade(Overlap=1.00,Duration=0.3)";
this.ControlElement.filters[0].Apply();
}
if(this.IsToggle){
if(this.DisplayType.toLowerCase()=="textimage"||this.DisplayType.toLowerCase()=="textonly"){
this.ControlElement["className"]=this.Skin+"_radtbutton_text_"+_1d;
}else{
this.ControlElement["className"]=this.Skin+"_radtbutton_"+_1d;
}
}else{
if(this.DisplayType.toLowerCase()=="textimage"||this.DisplayType.toLowerCase()=="textonly"){
this.ControlElement["className"]=this.Skin+"_radbutton_text_"+_1d;
}else{
this.ControlElement["className"]=this.Skin+"_radbutton_"+_1d;
}
}
if(_1e){
this.ControlElement.filters[0].Play();
}
};
RadToolbarButton.prototype.ToggleLogic=function(){
if(this.ButtonGroup!=""){
this.ToolbarInstance.ToggleButtonGroups(this);
}else{
this.ToggleButton(!this.Toggled);
}
};
RadToolbarButton.prototype.ToggleButton=function(_1f){
this.Toggled=_1f;
this.ToggleState();
this.ToolbarInstance.FireOnClientButtonToggled(this,_1f);
};
RadToolbarButton.prototype.DisableButton=function(){
this.Enabled=false;
this.ControlElement.setAttribute("disabled",true);
this.ToggleState();
};
RadToolbarButton.prototype.EnableButton=function(){
this.Enabled=true;
this.ControlElement.setAttribute("disabled",false);
this.ToggleState();
};
RadToolbarButton.prototype.HideButton=function(){
this.Hidden=true;
this.ToggleState();
};
RadToolbarButton.prototype.ShowButton=function(){
this.Hidden=false;
this.ToggleState();
};

//BEGIN_ATLAS_NOTIFY
if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
