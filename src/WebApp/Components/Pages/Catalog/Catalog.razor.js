export function setupIntersectionObserver(element, dotNetRef) {
    console.log('Setting up intersection observer for element:', element);
    
    if (!element) {
        console.error('Element is null or undefined');
        return null;
    }

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            console.log('Intersection observed:', entry.isIntersecting);
            if (entry.isIntersecting) {
                console.log('Element is intersecting, calling OnIntersection');
                dotNetRef.invokeMethodAsync('OnIntersection');
            }
        });
    }, {
        root: null,
        rootMargin: '100px',
        threshold: 0.1
    });

    observer.observe(element);
    console.log('Observer attached to element');
    
    return {
        disconnect: () => {
            console.log('Disconnecting observer');
            observer.disconnect();
        }
    };
}
