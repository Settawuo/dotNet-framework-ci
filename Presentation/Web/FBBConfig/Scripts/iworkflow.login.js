/// <reference path="../lib/jquery.min.js" />
/// <reference path="../common/basecontrol.js" />
/// <reference path="../lib/helper.datatype.min.js" />

$(document).ready(function (e) {
    document.layoutObject = document.layouts || {};
    document.layoutObject.login = new Login();
});

var Login = function () {

    var SEL_MODULE = 'body#login';
    var COMPONENTS = {
    };

    return $.extend(
        new BaseControl(SEL_MODULE),
        {
            contructor: Login,
            init: function () {
                var me = this;
                me.addComponent(COMPONENTS);

                return this;
            }
        }
    ).init(SEL_MODULE);
};

function BaseControl(selector) {

    return {
        constructor: BaseControl,
        id: undefined,
        $: undefined,
        element: undefined,
        components: {},

        raiseEvent: function (eventName, param) { $(this).trigger(eventName, param); return this; },

        init: function (selector) {
            var me = this;

            $('body').addClass('loaded');

            me.components['element'] = (typeof (selector) === 'object') ? selector : $(selector);
            me.element = me.components['element'];
            me.$ = $(me);


            me.htmlHeadLostFix();
            return me;
        },

        addComponent: function (components) {
            if (!components) { return this; }
            for (var prop in components) {
                this.components[prop] = this.element.find(components[prop]);
                this.components[prop] = $.extend(this.components[prop], this.extensionComponentsFn);

                if (this.components[prop].length === 0) {
                    this.log('{0} : component {1} not found in selector {2}'.format(this.constructor.name, prop, this.components[prop].selector));
                }
            }
            return this;
        },

        log: function (message) {
            //if(console) console.log(message);
        },

        extensionComponentsFn: {
            disable: function () { this.enable(); $(this).addClass('disabled'); return this; },
            enable: function () { $(this).removeClass('disabled'); return this; },
            isDisabled: function () { return $(this).hasClass('disabled'); }
        },

        htmlHeadLostFix: function () {
            if ($('body').find('title').length) {

                $('body').find('title').appendTo('head');
                $('body').find('meta').appendTo('head');
                $('body').find('link').appendTo('head');

                var nodes = $(document.body.childNodes);

                nodes.each(function (index, el) {
                    el = $(el);
                    if (el.text().charCodeAt(0) === 65279) { el.remove(); }
                });

            }

        }


    }.init(selector);

}

