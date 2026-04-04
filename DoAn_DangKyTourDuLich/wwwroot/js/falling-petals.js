/**
 * Falling Petals Effect — uses /images/LaDao.png
 * Creates cherry-blossom petals that drift across the viewport.
 */
(function () {
    'use strict';

    const CONFIG = {
        imageSrc: '/images/LaDao.png',
        /** max petals alive at once */
        maxPetals: 18,
        /** ms between new petal spawns */
        spawnInterval: 900,
        /** size range (px) */
        sizeMin: 22,
        sizeMax: 48,
        /** fall duration range (s) */
        durationMin: 7,
        durationMax: 14,
        /** horizontal sway amplitude (px) */
        swayMin: 40,
        swayMax: 120
    };

    /* ---------- container ---------- */
    const container = document.createElement('div');
    container.id = 'falling-petals';
    container.setAttribute('aria-hidden', 'true');
    document.body.appendChild(container);

    /* ---------- inject dynamic styles ---------- */
    const style = document.createElement('style');
    style.textContent = `
        #falling-petals {
            position: fixed;
            inset: 0;
            pointer-events: none;
            overflow: hidden;
            z-index: 9998;
        }

        .petal {
            position: absolute;
            top: -60px;
            will-change: transform, opacity;
            pointer-events: none;
            filter: drop-shadow(0 2px 6px rgba(255,182,193,0.35));
        }

        .petal img {
            width: 100%;
            height: 100%;
            object-fit: contain;
        }

        @keyframes petalFall {
            0%   { transform: translateY(0)   translateX(0)          rotate(0deg);   opacity: 0; }
            5%   { opacity: 1; }
            25%  { transform: translateY(25vh) translateX(var(--sway)) rotate(90deg); }
            50%  { transform: translateY(50vh) translateX(0)          rotate(200deg); }
            75%  { transform: translateY(75vh) translateX(calc(var(--sway) * -0.6)) rotate(290deg); }
            95%  { opacity: 0.8; }
            100% { transform: translateY(110vh) translateX(var(--sway)) rotate(400deg); opacity: 0; }
        }
    `;
    document.head.appendChild(style);

    /* ---------- helpers ---------- */
    function rand(min, max) {
        return Math.random() * (max - min) + min;
    }

    function spawnPetal() {
        if (container.childElementCount >= CONFIG.maxPetals) return;

        const size = rand(CONFIG.sizeMin, CONFIG.sizeMax);
        const duration = rand(CONFIG.durationMin, CONFIG.durationMax);
        const sway = rand(CONFIG.swayMin, CONFIG.swayMax) * (Math.random() > 0.5 ? 1 : -1);
        const left = rand(-5, 100); // % across viewport
        const delay = rand(0, 2);

        const el = document.createElement('div');
        el.className = 'petal';
        el.style.cssText = `
            left: ${left}%;
            width: ${size}px;
            height: ${size}px;
            --sway: ${sway}px;
            animation: petalFall ${duration}s ${delay}s ease-in-out forwards;
            opacity: 0;
        `;

        const img = document.createElement('img');
        img.src = CONFIG.imageSrc;
        img.alt = '';
        img.loading = 'lazy';
        el.appendChild(img);
        container.appendChild(el);

        /* remove after animation ends */
        el.addEventListener('animationend', () => el.remove());
    }

    /* ---------- loop ---------- */
    // Respect reduced-motion preference
    if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;

    setInterval(spawnPetal, CONFIG.spawnInterval);

    // spawn a small initial burst
    for (let i = 0; i < 6; i++) {
        setTimeout(spawnPetal, i * 250);
    }
})();
