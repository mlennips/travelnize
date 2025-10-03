export function scrollToId(id, options) {
    const maxTries = options?.maxTries ?? 10;
    const highlight = options?.highlight ?? true;
    const behavior = options?.behavior ?? 'smooth';
    let tries = 0;

    function attempt() {
        const el = document.getElementById(id);
        if (el) {
            el.scrollIntoView({ behavior, block: 'start', inline: 'nearest' });
            if (highlight) {
                el.classList.add('scroll-target-highlight');
                setTimeout(() => el.classList.remove('scroll-target-highlight'), 2000);
            }
        } else if (tries++ < maxTries) {
            requestAnimationFrame(attempt);
        }
    }
    attempt();
}