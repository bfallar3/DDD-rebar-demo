/*
* jQuery UI Radio Button @VERSION
*
* Copyright 2011, AUTHORS.txt (http://jqueryui.com/about)
* Dual licensed under the MIT or GPL Version 2 licenses.
* http://jquery.org/license
*
* http://docs.jquery.com/UI/ ???
* https://raw.github.com/bmsterling/jquery-ui/checkboxRadiobutton/ui/jquery.ui.radiobutton.js
*
* Depends:
*	jquery.ui.core.js
*	jquery.ui.widget.js
*/
(function ($) {

  // borrowed from $.ui.button
  var radioGroup = function (radio) {
    var name = radio.name,
			form = radio.form,
			radios = $([]);
    if (name) {
      if (form) {
        radios = $(form).find("[name='" + name + "']");
      } else {
        radios = $("[name='" + name + "']", radio.ownerDocument)
					.filter(function () {
					  return !this.form;
					});
      }
    }
    return radios;
  };

  $.widget("ui.radiobutton", $.ui.checkbox, {
    baseClass: 'ui-radio',
    _refresh: function () {
      var _this = this;
      $.ui.checkbox.prototype._refresh.apply(this, arguments);

      // partially borrowed from $.ui.button
      radioGroup(_this.element[0])
				.each(
					function () {
					  var el = $(this);
					  if (el.prop('checked')) {
					    el.parent()
								.addClass("ui-state-active")
								.attr("aria-pressed", true);
					  }else {
					    el.parent()
								.removeClass("ui-state-active")
								.attr("aria-pressed", false);
					  }
					}
				);
    }
  }
	);
} (jQuery));