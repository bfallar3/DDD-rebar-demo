(function ($, document, window) {

  $.widget("ui.selectlist", {
    options: {
      multiselect: true
    },

    _create: function () {
      var self = this, o = this.options;

      $(".ui-selectlist-item", this.element)
        .live("mouseenter.selectlist", function () {
          if (!($(this).is(".ui-state-disabled") || $(this).parents(".ui-selectlist-listitem-group.ui-state-disabled").length !== 0)) {
            $(this).addClass("ui-state-hover");
          }
        }).live("mouseleave.selectlist", function () {
          $(this).removeClass("ui-state-hover");
        }).live("click.selectlist", function () {
          if ($(this).toggleClass("ui-state-active"), $(".ui-state-active")) {
            if (!o.multiselect) {
              self.widget().find(".ui-state-active").not($(this)).removeClass("ui-state-active");
            }
          }
        }).each(function (idx) {
          if (idx % 2) {
              $(this).addClass("ui-grey-bar"); //ui-priority-secondary
          }
        });
    }
  });

})(jQuery, document, window);