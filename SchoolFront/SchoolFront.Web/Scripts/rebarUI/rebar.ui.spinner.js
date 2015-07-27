(function ($) {

    $.fn.spinner = function (options) {

        var opts = $.extend({
            displayMode: 'page',
            caption: '',
            timeout: 30000

        }, options);

        var settings;
        var is_local = false;

        if (opts.displayMode == 'page') {

            settings = {
                lines: 16, // The number of lines to draw
                length: 14, // The length of each line
                width: 6, // The line thickness
                radius: 19, // The radius of the inner circle
                color: '#fff', // #rgb or #rrggbb
                speed: 1, // Rounds per second
                trail: 100, // Afterglow percentage
                shadow: true, // Whether to render a shadow
                hwaccel: false, // Whether to use hardware acceleration
                className: 'ui-spinner ui-spinner-page', // The CSS class to assign to the spinner
                zIndex: 2e9, // The z-index (defaults to 2000000000)
                top: 'auto', // Top position relative to parent in px
                left: 'auto' // Left position relative to parent in px
            };

        }
        else {
            settings = {
                lines: 16, // The number of lines to draw
                length: 5, // The length of each line
                width: 2, // The line thickness
                radius: 4, // The radius of the inner circle
                color: '#fff', // #rgb or #rrggbb
                speed: 1, // Rounds per second
                trail: 100, // Afterglow percentage
                shadow: true, // Whether to render a shadow
                hwaccel: false, // Whether to use hardware acceleration
                className: 'ui-spinner ui-spinner-local', // The CSS class to assign to the spinner
                zIndex: 2e9, // The z-index (defaults to 2000000000)
                top: 'auto', // Top position relative to parent in px
                left: 'auto' // Left position relative to parent in px
            };
            is_local = true; 
//            settings = {
//                lines: 14, // The number of lines to draw
//                length: 6, // The length of each line
//                width: 3, // The line thickness
//                radius: 8, // The radius of the inner circle
//                color: '#fff', // #rgb or #rrggbb
//                speed: 1, // Rounds per second
//                trail: 100, // Afterglow percentage
//                shadow: true, // Whether to render a shadow
//                hwaccel: false, // Whether to use hardware acceleration
//                className: 'ui-spinner ui-spinner-local', // The CSS class to assign to the spinner
//                zIndex: 2e9, // The z-index (defaults to 2000000000)
//                top: 'auto', // Top position relative to parent in px
//                left: 'auto' // Left position relative to parent in px
//            };
//            is_local = true;
        }

        if (Spinner) {
            return this.each(function () {
                //debugger;
                var $this = $(this);
                var data = $this.data();
                if (data.spinner) {
                    stop_spinner(this);
                }

                if (options !== false) {

                    //adding spinner instance with data
                    data.spinner = new Spinner(settings).spin(this);

                    if (is_local) {
                        //LOCAL spinner
                        //debugger;
                        var top = $(this).height() / 2;
                        var left = $(this).width() / 2;
                        //var zi = $(data.spinner.el).css('z-index') - 1;
                        var container = $("<div class='ui-spinner-container-local'></div>").css({
                            'left': left -1,
                            'top': top - 18
                        });


                        $("<div class='ui-spinner-container-shadow  ui-shadow ui-corner-all'></div>").prependTo(container);
                        $(container).prependTo(this);
                        $(this).addClass('ui-spinner-container-div');


                    }

                    else {
                        //PAGE spinner
                        //debugger;
                        $(data.spinner.el).css({
                            'position': '',
                            'left': '',
                            'top': ''
                        });
                        //creating box
                        var zi = $(data.spinner.el).css('z-index');
                        var container = $("<div class='ui-spinner-container-page ui-corner-all ui-shadow'></div>").css('z-index', zi - 2).appendTo(this);
                        //$(this).append(container);
                        //adding caption inside box
                        if (opts.caption !== '') {
                            var caption = $("<div class='ui-spinner-caption'><span>" + opts.caption + "</span></div>");
                            $(container).addClass('spinner-container-with-caption');
                            $(data.spinner.el).append(caption.css('z-index', zi - 1)).addClass('spinner-with-caption');
                        }

                        //adding modal behaviour
                        $("<div class='ui-spinner-overlay'></div>").css('z-index', zi - 3).appendTo('body');

                    }

                    //Timeout for both types of spinner
                    var timer = setTimeout(function () {
                        //debugger;
                        if (data.spinner.opts.className === "ui-spinner ui-spinner-page") {
                            //remove page spinner
                            $('body').find('.ui-spinner-overlay').remove();
                            $($this).find('.ui-spinner-caption, .ui-spinner-container-page').remove();
                        }
                        else {
                            //remove specific local spinner
                            $($this).find('.ui-spinner-container-local').remove();
                        }

                        data.spinner.stop();
                        delete data.spinner;

                    }, opts.timeout);

                    $(this).data("timer", timer);

                }
            });
        }
        else {
            throw "Spinner class not available.";
        }

        function stop_spinner(e) {
            //debugger;
            var container = $(e);
            var data = container.data();

            if (data.spinner.opts.className === "ui-spinner ui-spinner-page") {
                //remove page spinner
                $('body').find('.ui-spinner-overlay').remove();
                $(container).find('.ui-spinner-caption, .ui-spinner-container-page').remove();
            }
            else {
                //remove specific local spinner
                $(container).find('.ui-spinner-container-local').remove();
            }

            var timer = $(e).data('timer');
            clearTimeout(timer)

            data.spinner.stop();
            delete data.spinner;
        }

    } //namespace end


})(jQuery);


