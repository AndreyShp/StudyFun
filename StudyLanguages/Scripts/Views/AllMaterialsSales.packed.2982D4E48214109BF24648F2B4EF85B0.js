﻿AllMaterialsSalesController={Init:function(){function u(n,t){var i={isSuccess:n,removeLabels:!0,container:$("#salesMessage"),text:t},r=new Message(i);r.Show()}var t="#emailField",i=0,r,n;GlobalBusiness!=null&&GlobalBusiness.TopPanel!=null&&(i=GlobalBusiness.TopPanel.GetHeight()),r=$("#selectedBlock"),r.sticky({topSpacing:i,center:!0}),n={consent:ko.observable(!1)},MailBlock({url:ServerData.Urls.SendToMail,mailBlockSelector:"#mailBlock",emailFieldSelector:t,showBtnBlockSelector:"#showMailBlockBtn",sendBtnSelector:"#sendUniqueIdBtn",showMessage:u,dataToPostModifier:function(n){n.uniqueDownloadId=ServerData.UniqueDownloadId}}),CopyToClipboard({copyToClipboardBtnSelector:"#copyToClipboardBtn",textSelector:"#salesUniqueId"}),$("#buySelectedBtn").click(function(){$("#consentCbx").prop("checked",!1),$(t).val(""),n.consent(!1),$("#salesFinishStepWindow").modal("show"),GlobalBusiness.Counter.reachGoal(GlobalBusiness.Counter.Ids.BuyClick)}),$("#payBtn").click(function(){var n='{ uniqueDownloadId:"'+ServerData.UniqueDownloadId+'"}';$.ajax({url:ServerData.Urls.Buy,type:"POST",dataType:"json",contentType:"application/json; charset=utf-8",data:n,success:function(n){var i=n!=null&&n.success,t;i?location.href=n.paymentUrl:(t=n!=null?n.message:"Извините, не удалось перейти к оплате.<br />Попробуйте еще раз через несколько секунд",u(!1,t))}}),GlobalBusiness.Counter.reachGoal(GlobalBusiness.Counter.Ids.PayClick)}),ko.applyBindings(n)}},$(function(){AllMaterialsSalesController.Init()});
