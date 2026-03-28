// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
    var floatingContact = document.querySelector('[data-floating-contact]');
    var toggle = document.querySelector('[data-floating-contact-toggle]');
    var revealItems = document.querySelectorAll('[data-reveal]');

    if (floatingContact && toggle) {
        window.setTimeout(function () {
            floatingContact.classList.add('is-ready');
        }, 120);

        toggle.addEventListener('click', function () {
            var isCollapsed = floatingContact.classList.toggle('is-collapsed');
            toggle.setAttribute('aria-expanded', (!isCollapsed).toString());
        });
    }

    var updateNavbarState = function () {
        document.body.classList.toggle('nav-scrolled', window.scrollY > 24);
    };

    updateNavbarState();
    window.addEventListener('scroll', updateNavbarState, { passive: true });

    document.querySelectorAll('[data-tour-thumb]').forEach(function (thumb) {
        thumb.addEventListener('click', function () {
            var mainImage = document.getElementById('tour-main-image');
            if (!mainImage) {
                return;
            }

            mainImage.src = thumb.getAttribute('data-tour-thumb');
            document.querySelectorAll('[data-tour-thumb]').forEach(function (item) {
                item.classList.remove('is-active');
            });
            thumb.classList.add('is-active');
        });
    });

    if (!revealItems.length) {
        return;
    }

    if ('IntersectionObserver' in window) {
        var revealObserver = new IntersectionObserver(function (entries, observer) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    observer.unobserve(entry.target);
                }
            });
        }, {
            threshold: 0.16,
            rootMargin: '0px 0px -40px 0px'
        });

        revealItems.forEach(function (item) {
            revealObserver.observe(item);
        });
    } else {
        revealItems.forEach(function (item) {
            item.classList.add('is-visible');
        });
    }
});
