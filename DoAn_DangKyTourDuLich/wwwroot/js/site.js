// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
    var floatingContact = document.querySelector('[data-floating-contact]');
    var toggle = document.querySelector('[data-floating-contact-toggle]');

    if (!floatingContact || !toggle) {
        return;
    }

    window.setTimeout(function () {
        floatingContact.classList.add('is-ready');
    }, 120);

    toggle.addEventListener('click', function () {
        var isCollapsed = floatingContact.classList.toggle('is-collapsed');
        toggle.setAttribute('aria-expanded', (!isCollapsed).toString());
    });
});
