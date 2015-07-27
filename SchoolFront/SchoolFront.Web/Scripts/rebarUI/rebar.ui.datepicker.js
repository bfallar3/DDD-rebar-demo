(function ($) {
  /* Initialization of jQuery UI Datepicker
  * This sets default properties to ensure the datepicker meets
  * CIO standards
  */

  // Get the path to where the images are stored relative to the rebar style sheet
  var rebarStyleSheet = "rebar.ui.all.css";
  var imagePath = $("head").find("link[type='text/css'][href$='" + rebarStyleSheet + "']")
        .get(0).href.replace(rebarStyleSheet, "images/");

  /* Set some CIO defaults that will apply to all datepickers, this code must run before
  * any jQuery UI datepickers are initialized, so this file inclusion should happen directly
  * after including the jQuery UI javascript file
  */

  $.datepicker.setDefaults({
    showOn: "both",
    buttonText: "",
    buttonImageOnly: true,
    buttonImage: imagePath + "Calendar.png",
    prevText: '&lt;',
    prevStatus: '',
    nextText: '&gt;',
    nextStatus: '',
    defaultStatus: '',
    dayNamesMin: ['S', 'M', 'T', 'W', 'T', 'F', 'S']
    // beforeShowDay: $.datepicker.noWeekends
  });

  $(".ui-datepicker-prev, .ui-datepicker-next").live('mouseover', function () {
    $(this).prop("title", "");
  });

})(jQuery);