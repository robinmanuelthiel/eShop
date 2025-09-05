(() => {
  const toggle = document.getElementById('theme-toggle');
  if (!toggle) return;
  toggle.addEventListener('click', () => {
    document.documentElement.classList.toggle('dark');
    const mode = document.documentElement.classList.contains('dark') ? 'dark' : 'light';
    localStorage.setItem('theme', mode);
  });
})();
