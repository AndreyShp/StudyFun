﻿ComparisonController={COUNT_ELEMENTS_IN_ROW:2,ELEMENTS_CLASS:".partial-comparison-item-container",rows:[],Init:function(){UserKnowledge.CheckExistenceIds(),GlobalBusiness.newVisitor(ServerData.Patterns.UrlNewVisitor),this.elems=$.makeArray($(this.ELEMENTS_CLASS)),$.each(this.elems,$.proxy(function(n,t){var i=$(t).parent(".row");n%this.COUNT_ELEMENTS_IN_ROW==0&&this.rows.push(i)},this))},HideOrShow:function(n){var t=$(this.elems).filter("#"+n).first(),r=t.is(":visible"),i="#"+String.format(ServerData.Patterns.CheckBoxId,n);return r?($(i).removeAttr("checked"),t.hide()):($(i).attr("checked",!0),t.show()),this.reorderIfNeed(),!1},reorderIfNeed:function(){if(!(this.rows<=1)){var n=0,t=$(this.elems).filter($.proxy(function(n){return this.elems[n].style.display!="none"},this)),i=this.COUNT_ELEMENTS_IN_ROW;$.each(this.rows,function(r,u){var f,e;(u.children().filter(this.ELEMENTS_CLASS).remove(),n>=t.length)||(f=t.length-n,f>i&&(f=i),e=t.slice(n,n+f),n+=f,u.append(e))})}}},$(function(){ComparisonController.Init()});
