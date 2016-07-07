$.fn.extend({
  insert: function (text) {
    return this.each(function () {
      var textarea = $(this);
      if (textarea[0].nodeName == "TEXTAREA") {
        textarea.val(textarea.val().substr(0, textarea.prop("selectionStart")) + text + textarea.val().substr(textarea.prop("selectionEnd")));
      }
    });
  },
  error: function (error, multiple) {
    return this.each(function () {
      if(!multiple)
        $(this).siblings('small.error').remove();
      $(this).after('<small class="error no-margin">' + error + '</small>');
    });
  },
  confirm: function (text) {
    return this.each(function () {
      $(this).click(function (e) {
        return confirm(text);
      })
    });
  }
});

function Application() { }
Application.prototype = {
  queryString: {},
  init: function () {
    var self = this;

    $(document.body).addClass("js-enabled");

    this.initWideScreen();

    $(window).resize(function () {
      self.windowResize();
    }).resize();

    $('input.datepicker').fdatepicker({
      format: 'dd-mm-yyyy',
      weekStart: 1
    });

    $('a[data-confirm]').click(function() {
      var text = $(this).data('confirm');
      return confirm(text);
    });
  },

  initWideScreen: function () {
    var self = this;
    $('a.toggle-widescreen').click(function (e) {
      e.preventDefault();
      if ($('body').hasClass('wide')) {
        $('body').removeClass('wide');
        self.setCookie('wide', 'false', 365);
      } else {
        $('body').addClass('wide');
        self.setCookie('wide', 'true', 365);
      }
    });
    if (self.getCookie('wide') == 'true') {
      $('body').addClass('wide');
    }
  },

  windowResize: function () {
    if ($(window).height() < $(document).height()) {
      $(document.body).addClass("scroll");
    } else {
      $(document.body).removeClass("scroll");
    }
  },

  initOnLoad: function () {

  },

  setCookie: function (cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
  },

  getCookie: function (cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
      var c = ca[i];
      while (c.charAt(0) == ' ') {
        c = c.substring(1);
      }
      if (c.indexOf(name) == 0) {
        return c.substring(name.length, c.length);
      }
    }
    return "";
  },


};

var app = null;
$(document).ready(function () {
  app = new Application();
  app.init();
});

$(window).load(function () {
  app.initOnLoad();
});