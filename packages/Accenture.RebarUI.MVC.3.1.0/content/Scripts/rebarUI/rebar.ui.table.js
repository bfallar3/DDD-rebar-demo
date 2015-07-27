(function ($) {
  $.widget("ui.table", {

    defaultOptions: {
      selectable: false,
      multiselect: false
    },
    options: {},

    _create: function () {
      var self = this, o = this.options;

      self._mergeDataOptions();

      $("tbody tr", this.element)
        .live("mouseenter", function () {
          if (self._isHoverable()) {
            $(this).addClass("ui-state-hover");
          }
        })
        .live("mouseleave", function () {
          $(this).removeClass("ui-state-hover");
        })
        .live("click", function () {
          var $el = $(this);
          if (self.isSelectable($el)) {
            $el.toggleClass("ui-state-active");
            if (!o.multiselect) {
              $el.siblings().removeClass("ui-state-active");
            }
          }
        });
    },

    _init: function () {
      this.refresh();
    },

    refresh: function () {
      var $rows = this.widget().find("tr");
      $rows.filter(":even").removeClass("ui-priority-secondary");
      $rows.filter(":odd").addClass("ui-priority-secondary");

    },

    disable: function(row) {
      
    },
    
    enable: function(row) {
      
    },
    
    isSelectable: function (el) {
      var $el = $(el);
      return this.isEnabled($el) && this.options.selectable;
    },

    _isHoverable: function (el) {
      return this.isSelectable(el);
    },

    isDisabled: function (el) {
      var $el = $(this), disabledClass = ".ui-state-disabled";

      return this.widget().is(disabledClass) ||
        ($el.length !== 0 && ($(el).is(disabledClass) || $(el).closest("tr").is(disabledClass)));
    },

    isEnabled: function (el) {
      return !this.isDisabled(el);
    },

    /* Read html5 data- attributes off the table and merge them into 
    * standard widget options
    */
    _mergeDataOptions: function () {
      var self = this, o = self.options, $tbl = self.element;
      var attrOpts = {
        multiselect: $tbl.data("tbl-multiple"),
        selectable: $tbl.data("tbl-selectable")
      };
      self.options = $.extend({}, $.ui.table.prototype.defaultOptions, attrOpts, o);

      // Ensure that selectable is true if multiselect flag is set
      self.options.selectable = self.options.multiselect ? true : self.options.selectable;
    }
  });
})(jQuery);