$(function(){
	$(".collapse_menu_secad").click(function(){
		$(this).toggleClass("rotate-180")
			   .animate({
					transition: "all ease-in-out 0.2s"	
		});	
	});	
});