/// <reference path="jquery-1.6.4-vsdoc.js" />
/// <reference path="jquery-ui-1.8.16.js" />
/// <reference path="jquery.ba-throttle-debounce.min.js" />

(function ($) {
/// <summary>JQuery plugin for unobtrusively wiring up auto-postback select lists.</summary>
/// <param name="$" type="jQuery">JQuery</param>
    $("select[data-autopost]").live("change", $.debounce(500, function () {
        $(this).parents("form:first").submit();
     }));
})(jQuery);


(function ($) {
    $.fn.openDialogRelativeTo = function(elem, my, at) {
        var $rel = $(elem);
        var $e = $(this);
        return $e.dialog( 'option', 'position', {
                of: $rel,
                my: "left top",
                at: "left bottom"})
          .dialog('open');
    };

    $.fn.wireupAutocomplete = function() {
    /// <summary>Wires up an element for jquery-ui autocompletion.
    ///   {data-ui-autocomplete-source} = URL for remote ajax call for search term
    ///   {data-ui-autocomplete-field} = jquery DOM selector for hidden field to bind id value of item
    /// </summary>
        return $(this).autocomplete( {
            source: $(this).attr("data-ui-autocomplete-source"),
            minLength: 2,
            selectFirst: true,
            select: function(event, ui) {
                $($(this).attr("data-ui-autocomplete-field")).val(ui.item ? ui.item.id : "");
            },
            change: function(event, ui) {
                $($(this).attr("data-ui-autocomplete-field")).val(ui.item ? ui.item.id : "");
            }
        });
    };

    $.extend({
        wireupDialogs: function() {
        /// <summary>Wires an onclick handler for any anchor elements on a page that have an html attribute
        ///          named data-dialog.  The value of the attribute should be a jquery selector pointing to
        ///          the dialog html element.</summary>
        /// <returns type="jQuery" />
            return $("*[data-dialog=true]").dialog( { 
                autoOpen: false, 
                modal: true, 
                resizable: false, 
                draggable: false,
                width: 520,
                show: ["scale", {direction: "both", origin: ['top', 'left'], scale: "box"}, "slow"],
                hide: ["scale", {direction: "both", origin: ['top', 'left'], scale: "box"}, "slow"]
               } );
        }
    });

    // Unobtrusive wireup of dialog open handlers
    $("*[data-dialog-open]").live("click", function() {
        // The value of the data-popup attribute should be a jquery selector to the element to popup
        var $elem = $(this);

        var dialogSelector = $(this).attr("data-dialog-open");

        $(dialogSelector)
            /* Set the position of dialog underneath and to the left of the opener */
            .dialog( 'option', 'position', { 
                of: $elem, 
                my: "left top", 
                at: "left bottom" } )
            /* Open the dialog */
            .dialog('open');
                
        /* return false if the opener was an anchor tag so we don't navigate away */
        if ($(this).prop("tagName").toLowerCase() === "a") {
            return false;
        }
    });

    // Unobtrusive wireup of dialog close handlers
    $("*[data-dialog-close]").live("click", function() {
        // The value of the data-popdown attribute should be a jquery selector to the element to popup
        var popdownSelector = $(this).attr("data-dialog-close");
        $(popdownSelector).dialog('close');
        if ($(this).prop("tagName").toLowerCase() === "a") {
            return false;
        }
    });

    // Wires up select first functionality for auto complete
    $( ".ui-autocomplete-input" ).live( "autocompleteopen", function() {
	    var autocomplete = $( this ).data( "autocomplete" ),
		    menu = autocomplete.menu;

	    if ( !autocomplete.options.selectFirst ) {
		    return;
	    }

	    menu.activate( $.Event({ type: "mouseenter" }), menu.element.children().first() );
    });
})(jQuery);

(function ($) {
/// <summary>JQuery plugin for handling cascading html select lists</summary>
/// <param name="$" type="jQuery">JQuery</param>

    $.fn.cascadingSelect = function() {
    /// <summary>Initializes an element has a cascading drop down list</summary>
        this.each(function() {
            var $e = $(this);
            var $p = $($e.attr("data-ui-cascading-parent"));
            $e.data('cascading-opts', $e.find("option:gt(0)")); // save all the original options

            $p.bind("change", function() {
                $e.find("option:gt(0)").remove();
                $e.append($e.data('cascading-opts').filter('[data-ui-cascading-parent=' + this.value + ']'));
                if ( $e.children().size() == 2 ) {
                  $($e.children().get(1)).prop("selected", true);
                }
                $e.change();
            });

            $e.find("option:gt(0)").filter(":not(:selected)").remove();
        });
        return this;
    };

    $.extend({
        wireupCascadingSelects: function() {
        /// <summary>Uses unobtrusive html 5 data- attributes to wireup cascading select lists</summary>
            return $("select[data-ui-cascading-parent]").cascadingSelect();
        } 
    });

    $.wireupCascadingSelects();
})(jQuery);

(function ($) {
/// <param name="$" type="jQuery">JQuery</param>
    $.fn.iebuttonfix = function() {
    /// <summary>Fixes IE bugs around using the html &lt;button&gt; tag.  IE <= 7 will submit all buttons
    /// within the form, not just the button that was clicked.  It will also submit the innerHtml
    /// of the button instead of the value of the value attribute of the button element.
    /// This plugin attempts to fix the problem by wiring up an onSubmit handler for the form
    /// and onClick handlers for all the buttons on the form which will disable all buttons that
    /// were not clicked causing them not be submitted with the form data, and also replacing the
    /// innerHtml of a clicked button element the value of the value attribute on the form element.
    ///
    /// This plugin can be called using normal jquery plugin syntax of $(selector).iebuttonfix() where
    /// selector is a selector for html form elements that you would like to apply this to.
    ///
    /// Note that this plugin does not test the version of the browser so it should only be included
    /// and executed using conditional html comments for ie version < 7
    /// 
    /// </summary>
        return $(this).each( function() {
            // enumerate each jquery element from the selector
            var $f = $(this);
            if ($f.prop("nodeName").toLowerCase() === "form") {
                // ensure we only apply a submit handler to an html form node
                $f.submit(function() {
                /// <summary>onSubmit handler for the form which fixes the IE bug by ensuring the correct html button and value are submitted.</summary>

                    $("button", $f).each(function() {
                        /// <summary>Ensures only the clicked button is submitted with the appropriate value</summary>

                        var $b = $(this);
                        if ($b.data("ie.btnfix.clicked")) {
                            $b.html($b.val()); // replace the innerHtml of the button with the value
                            $b.data("ie.btnfix.clicked", false); // clear the clicked flag
                        } else {
                            // if the button wasn't clicked, disable it so it is not submitted with the form
                            $b.prop('disabled', true);
                        }
                    });
                });

                $("button", $f).click(function() {
                  /// <summary>Adds an onClick handler to all buttons on the form that sets a flag
                  ///  onClick to true so we can identify which button was clicked when the form
                  ///  is submitted </summary>
                  $(this).data("ie.btnfix.clicked", true);
                });
            }
        });
    };
})(jQuery);