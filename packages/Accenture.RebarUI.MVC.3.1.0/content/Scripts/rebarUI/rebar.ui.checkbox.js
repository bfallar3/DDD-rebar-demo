/*
* jQuery UI Checkbox @VERSION
*
* Copyright 2011, AUTHORS.txt (http://jqueryui.com/about)
* Dual licensed under the MIT or GPL Version 2 licenses.
* http://jquery.org/license
* https://raw.github.com/bmsterling/jquery-ui/checkboxRadiobutton/ui/jquery.ui.checkbox.js
*
* http://wiki.jqueryui.com/w/page/12137730/Checkbox
*
* Depends:
*	jquery.ui.core.js
*	jquery.ui.widget.js
*
*/
(function ($) {

  $.widget("ui.checkbox", {
	baseClass: 'ui-checkbox',
	options: {
	  disabled: null,
	  label: null
	},

	_create: function () {
	  var _this = this,
					options = _this.options;

	  _this.checkBox = _this.element
								.addClass('ui-helper-reset')
								.css('opacity', 0)
								.wrap('<div class="' + _this.baseClass + ' ui-widget ui-state-default" role="checkbox">')
								.parent();

	  _this.label = _this._findLabel();

	  if (_this.element.hasClass("ui-state-alert")) {
		_this.checkBox.addClass("ui-state-alert");
	  } else if (_this.element.hasClass("ui-state-attention")) {
		_this.checkBox.addClass("ui-state-attention");
	  }

	  // check to see if the checkbox is disabled
	  if (_this.element.prop("disabled")) {
		_this._setOption("disabled", true);
	  }

	  _this.element
				.bind(
					"hasChanged.checkbox",
					function () {
					  _this._refresh();
					}
				)
				.bind(
					"focus.checkbox",
					function () {
					  _this.checkBox.addClass("ui-state-focus");
					}
				)
				.bind(
					"blur.checkbox",
					function () {
					  _this.checkBox.removeClass("ui-state-focus");
					}
				);

	  _this.checkBox
				.bind(
					"mouseenter.checkbox",
					function () {
					  if (options.disabled) {
						return;
					  }
					  _this.checkBox.addClass("ui-state-hover");
					}
				)
				.bind(
					"mouseleave.checkbox",
					function () {
					  if (options.disabled) {
						return;
					  }
					  _this.checkBox.removeClass("ui-state-hover");
					}
				);

	  _this.element
				.data(
					'ui.checkbox.interval',
					setInterval(
						$.proxy(function () {
						  var _this = this,
								isChecked;
						  if (_this.element.data('lastState') !== (isChecked = _this.element.prop('checked'))) {
							_this.element.trigger('hasChanged.checkbox');
							_this.element.data('lastState', isChecked);
						  }
						}, _this),
						10
					)
				);

	  _this._refresh();
	},

	check: function () {
	  this.element.prop('checked', true);

	  this._refresh();
	},

	uncheck: function () {
	  this.element.prop('checked', false);

	  this._refresh();
	},

	refresh: function () {
	  this._refresh();
	},

	_setOption: function (key, value) {
	  $.Widget.prototype._setOption.apply(this, arguments);
	  if (key === "disabled") {
		if (value) {
		  this.element.attr("disabled", true);
		  this.options.disabled = true;
		} else {
		  this.element.removeAttr("disabled");
		  this.options.disabled = false;
		}

		this.checkBox[value ? "addClass" : "removeClass"]("ui-state-disabled")
		  .attr("aria-disabled", value);
		if (this.label != null) {
		  this.label[value ? "addClass" : "removeClass"]("ui-state-disabled");
		}

		return;
	  }
	},

	enable: function () {
	  return this._setOption("disabled", false);
	},

	disable: function () {
	  return this._setOption("disabled", true);
	},

	_refresh: function () {
	  var isDisabled = this.element.prop('disabled'),
					isChecked = this.element.prop('checked');

	  if (isDisabled !== this.options.disabled) {
		this._setOption("disabled", isDisabled);
	  }

	  if (isChecked) {
		this.checkBox
						.addClass("ui-state-active ui-checkbox-checked")
						.attr("aria-checked", true);
	  }
	  else {
		this.checkBox
						.removeClass("ui-state-active ui-checkbox-unchecked")
						.attr("aria-checked", false);
	  }
	},

	widget: function () {
	  return this.checkBox;
	},

	destroy: function () {
	  clearInterval(this.element.data('ui.checkbox.interval'));
	  this.element.removeData('ui.checkbox.interval').css('opacity', '').unbind(".checkbox").unwrap();
	},

	_findLabel: function () {
	  var label = null;

	  // If a label selector or element is passed in via options
	  // use this label as it takes precendence
	  if (this.options.label != null) {
		label = $(this.options.label);
		if (label.length !== 0) {
		  return label;
		}
	  }

	  // if label option doesn't exist, try grabbing the closest wrapping label
	  label = this.element.closest("label");
	  if (!(label.length !== 0)) {
		// otherwise resort to finding a label using the 'for' attrib
		label = $(this.element[0].ownerDocument).find("label[for='" + this.element.attr('id') + "']");
	  }
	  return label;
	}
  }
	);
} (jQuery));