(function ($) {
    /* Initialization of jQuery UI Dialogs
    * This sets default properties to ensure the datepicker meets
    * CIO standards
    */

    // Get the path to where the images are stored relative to the rebar style sheet
    var rebarStyleSheet = "rebar.ui.all.css";
    var rebarStyleLink = $("head").find('[data-rebar_css="base"]').get(0);
    var imagePath = rebarStyleLink.href.replace(rebarStyleSheet, "images/");

    /* Set some CIO defaults that will apply to all dialogs, this code must run before
    * any jQuery UI dialogs are initialized, so this file inclusion should happen directly
    * after including the jQuery UI javascript file
    *
    * All the options being set can be easily overridden at run time by just
    * passing the options directly when calling $.dialog({})
    *
    * Here we are monkey patching the dialog to run our custom code
    * after the widget has been created.
    */

    var isEmpty = function (map) {
        for (var key in map) {
            if (map.hasOwnProperty(key)) {
                return false;
            }
        }
        return true;
    };

    if ($.ui && $.ui.dialog) {
        $.extend($.ui.dialog.prototype.options, {
            modal: true,
            resizable: false,
            draggable: false,
            autoOpen: false
        });

        if ($.ui.dialog.prototype._create) {
            var originalCreate = $.ui.dialog.prototype._create;
            $.ui.dialog.prototype._create = function () {

                // Add a close button to the bottom right if the user did not add a close button
                if (!this.options.buttons || isEmpty(this.options.buttons)) {
                    this.options.buttons = { Close: function () { $(this).dialog("close"); } };
                }

                /* Call the original function to get the default behavior */
                originalCreate.apply(this, arguments);

                /* Apply our overrides */
                var $dialog = $(this.uiDialog)
                // Add a shadow
          .addClass("ui-shadow");

                var $title = $dialog.find(".ui-dialog-titlebar")
                // Remove rounded corners on title
          .removeClass("ui-corner-all")
                // Remove the default close button
          .find(".ui-dialog-titlebar-close").remove().end();

                var $content = $dialog.find(".ui-widget-content");

                var rewriteHtml = function (cssClass) {
                    /* Add a span element before the title text to add the icon */
                    var iconClassName = cssClass.replace("ui-state", "rui-icon-triangle");
                    $title.removeClass("ui-corner-all").prepend($("<span></span>", { 'class': "rui-icon " + iconClassName, 'style': 'float:left;' }));

                    /* Remove the state class from the content div, push it up to the ui-widget div */
                    $content.removeClass(cssClass).parent()
            .addClass(cssClass);
                };

                /* Rewrite the HTML for our default classes */
                if ($content.get(0)) {
                    if ($content.hasClass("ui-state-alert")) {
                        rewriteHtml("ui-state-alert");
                    } else if ($content.hasClass("ui-state-attention")) {
                        rewriteHtml("ui-state-attention");
                    } else {
                        /* By default we do nothing */
                    }
                }

                this.element.bind("dialogopen", function () {
                    var maxButtonWidth = 0;
                    $dialog.find(".ui-dialog-buttonset button").each(function () {
                        maxButtonWidth = maxButtonWidth < $(this).width() ? $(this).width() : maxButtonWidth;
                    }).width(maxButtonWidth + 2); //button need 2px for border
                });

            };
        }
    }
})(jQuery);