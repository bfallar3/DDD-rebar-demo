
(function ($) {
    $.widget("ui.slider_x", $.ui.slider, {
        options: {
            // display labels for slider handle(s)
            handleLabels: true,
            ticksPosition: "bottom",

            //To label the numeric ticks with a piece of text (e.g. ?%? or ?M?)
            labelSuffix: "",
            labels: null,

            // scale settings
            tickMarks: true,
            tickMarksCount: 10,

            tickLabels: true,
            tickLabelsCount: 10,

            scale: null,

            // making all sliders have a fixed min value so that the range is colored from the min value to selected location.
            range: "min"
        },

        valuesActual: null,
        valuesPercent: null,

        _create: function () {

            $.ui.slider.prototype._create.apply(this);

            //adding palette fill to slider range
            this.element.find('.ui-slider-range').addClass('ui-fill-medium');

            // setting heterogeneous scale
            if (this.options.scale && this.options.scale.length) {
                this.options.min = this.options.scale[0];
                this.options.max = this.options.scale[this.options.scale.length - 1];
                this.options.tickLabelsCount = this.options.scale.length - 1;
            }

            // adding handle(s) labels
            if (this.options.handleLabels) {
                this.element.addClass("ui-slider-" + this.orientation + "-handle-labels");
                this.handleLabels = [];
                if (this.options.values && this.options.values.length) {
                    this.handleLabels[0] = $("<div>" + this.options.values[0] + "</div>")
					.insertAfter(this.handles[0])
					.addClass("ui-slider-handle-label ui-slider-handle-label-0");
                    this.handleLabels[1] = $("<div>" + this.options.values[1] + "</div>")
					.insertAfter(this.handles[1])
					.addClass("ui-slider-handle-label ui-slider-handle-label-1");
                } else {
                    this.handleLabels[0] = $("<div>" + this.options.value + "</div>")
					.insertAfter(this.handles[0])
					.addClass("ui-slider-handle-label");
                }
            }

            if (this.options.tickMarks) {
                this.element.addClass("ui-slider-" + this.orientation + "-ticks-marks");
                //slider ticks position
                if (this.orientation == 'horizontal')
                    if (this.options.ticksPosition == 'top') this.element.addClass('slider-ticks-top');
                    else this.element.addClass('slider-ticks-bottom');
                else
                    if (this.options.ticksPosition == 'left') this.element.addClass('slider-ticks-left');
                    else this.element.addClass('slider-ticks-right');

                this.tickMarks = [];
                var ticksMarksContainer = $("<div></div>").appendTo(this.element).addClass("ui-slider-ticks-marks");
                for (var i = 0; i <= (this.options.tickMarksCount); i++) {
                    var tick = (this._valueMin() + i * (this._valueMax() - this._valueMin()) / this.options.tickMarksCount);

                    this.tickMarks[i] = $("<div>&nbsp;</div>").appendTo(ticksMarksContainer).addClass("ui-slider-ticks-mark");
                    if (this.orientation == 'vertical') {
                        this.tickMarks[i].css({ "top": (i * this.element.innerHeight() / this.options.tickMarksCount)
					- this.tickMarks[i].height() / 2
                        });
                    } else {
                        this.tickMarks[i].css({ "left": (i * this.element.innerWidth() / this.options.tickMarksCount) - this.tickMarks[i].width() / 2 });
                    }
                }
            }

            if (this.options.tickLabels) {
                this.element.addClass("ui-slider-" + this.orientation + "-ticks-labels");

                //slider label position
                if (this.orientation == 'horizontal')
                    if (this.options.ticksPosition == 'top') this.element.addClass('slider-labels-top');
                    else this.element.addClass('slider-labels-bottom');

                else
                    if (this.options.ticksPosition == 'left') this.element.addClass('slider-labels-left')
                    else this.element.addClass('slider-labels-right');

                this.tickLabels = [];
                var ticksLabelsContainer = $("<div></div>").appendTo(this.element).addClass("ticks-labels");
                for (var i = 0; i <= this.options.tickLabelsCount; i++) {
                    var tickLabel = (this._valueMin() + i * (this._valueMax() - this._valueMin()) / this.options.tickLabelsCount);
                    if (this.options.scale && this.options.scale.length) {
                        tickLabel = this.options.scale[i];
                    }

                    if (this.options.labels != null) {
                        tickLabel = this.options.labels[i] ? this.options.labels[i] : '';
                    }

                    this.tickLabels[i] = $("<div>" + tickLabel + this.options.labelSuffix + "</div>").appendTo(ticksLabelsContainer).addClass("ticks-label");

                    if (this.orientation == 'vertical') {
                        this.tickLabels[i].css({ "bottom": (i * this.element.innerHeight() / this.options.tickLabelsCount) - this.tickLabels[i].height() / 2 });
                    } else {
                        this.tickLabels[i].css({ "left": (i * this.element.innerWidth() / this.options.tickLabelsCount) - this.tickLabels[i].width() / 2 });
                    }
                }

                //right aligning labels
                if (this.orientation == 'vertical') {
                    var maxWidth = 0;
                    $(this.element).find('.ticks-label').each(function (i) {
                        maxWidth = $(this).width() > maxWidth ? $(this).width() : maxWidth;
                    }).css('width', maxWidth);
                }
            }

            //assiging palette to ticks and labels
            $(this.element).find('.ticks-label').addClass('ui-label-color');
            $(this.element).find('.ui-slider-ticks-mark').addClass('ui-fill-darkest');
            $(this.element).find('.ui-widget-header').removeClass('ui-widget-header');
            this._refreshValue();
        },
        _refreshValue: function () {
            $.ui.slider.prototype._refreshValue.apply(this);
            var self = this;

            // update handles labels text and positions
            if (this.handleLabels) {
                this.handles.each(function (i, handle) {
                    if (self.options.scale && self.options.scale.length) {
                        var handleText = self.positionToValue(i).toFixed(0);
                        //find respective label if handle is on scale point
                        if (self.options.labels) {
                            var index = $.inArray(parseInt(handleText), self.options.scale);
                            if (index > -1 && self.options.labels[index]) 
                                handleText = self.options.labels[index];
                            else 
                                handleText = '';
                        }

                        self.handleLabels[i].text(handleText);
                    }
                    else {
                        self.handleLabels[i].text(self.values(i));
                    }

                    if (self.orientation == 'vertical') {
                        self.handleLabels[i].offset({ top: ($(handle).offset().top + $(handle).height() / 2) - self.handleLabels[i].height() / 2 });
                    } else {
                        self.handleLabels[i].offset({ left: ($(handle).offset().left + $(handle).width() / 2) - self.handleLabels[i].width() / 2 });
                    }
                });

            }
        },

        valueToLabel: function (value) {
            $.ui.slider.prototype._refreshValue.apply(this);
            var self = this;
            var labelText = value;
            if (self.options.scale && self.options.scale.length) {
                if (self.options.labels) {
                    var i = $.inArray(parseInt(labelText), self.options.scale);
                    if (i > -1) labelText = self.options.labels[i];
                }
                return labelText;
            }
        },

        positionToValue: function (index) {

            if (!this.options.scale && !this.options.scale.length) {
                return this.values(index);
            }

            var shiftRange = (this.values(index) - this._valueMin()) / (this._valueMax() - this._valueMin());

            var position = shiftRange / (1 / (this.options.scale.length - 1));
            if (position == 0) {
                return this.options.scale[0];
            }

            var scaleRange = Math.ceil(position) - 1;
            var rangeShift = position - scaleRange;

            return this.options.scale[scaleRange] + rangeShift * (this.options.scale[scaleRange + 1] - this.options.scale[scaleRange]);
        }
    });

    $.extend($.ui.slider_x, {
        version: "1.0.0"
    });

} (jQuery));
