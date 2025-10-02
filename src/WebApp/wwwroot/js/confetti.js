window.eshopConfetti = window.eshopConfetti || {
    launch: () => {
        if (typeof confetti !== "function") {
            return;
        }

        const duration = 2000;
        const animationEnd = Date.now() + duration;
        const defaults = {
            spread: 360,
            ticks: 180,
            gravity: 0.8,
            decay: 0.94,
            startVelocity: 60,
            zIndex: 1000,
            scalar: 1.2,
        };

        const randomInRange = (min, max) => Math.random() * (max - min) + min;

        const shoot = () => {
            confetti({
                ...defaults,
                particleCount: 180,
                origin: { x: randomInRange(0.1, 0.9), y: randomInRange(0.1, 0.3) },
                colors: ["#ff577f", "#ff884b", "#ffd384", "#fff9b0", "#62cdff", "#8c52ff"],
            });
        };

        const frame = () => {
            shoot();
            if (Date.now() < animationEnd) {
                requestAnimationFrame(frame);
            }
        };

        frame();
    },
};
