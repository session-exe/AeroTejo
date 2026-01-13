// AeroTejo - Toggle entre Dark Mode e Light Mode

document.addEventListener('DOMContentLoaded', function () {
    // Verificar se há preferência salva no localStorage
    const savedTheme = localStorage.getItem('aerotejo-theme');
    if (savedTheme === 'light') {
        document.body.classList.add('light-mode');
        updateThemeIcon();
    }

    // Criar botão de toggle se não existir
    if (!document.querySelector('.theme-toggle')) {
        const toggleButton = document.createElement('button');
        toggleButton.className = 'theme-toggle';
        toggleButton.innerHTML = '<i class="fas fa-moon"></i>';
        toggleButton.setAttribute('aria-label', 'Alternar tema');
        toggleButton.onclick = toggleTheme;
        document.body.appendChild(toggleButton);
    }
});

function toggleTheme() {
    const body = document.body;
    body.classList.toggle('light-mode');

    // Salvar preferência no localStorage
    if (body.classList.contains('light-mode')) {
        localStorage.setItem('aerotejo-theme', 'light');
    } else {
        localStorage.setItem('aerotejo-theme', 'dark');
    }

    updateThemeIcon();
}

function updateThemeIcon() {
    const toggleButton = document.querySelector('.theme-toggle');
    if (toggleButton) {
        if (document.body.classList.contains('light-mode')) {
            toggleButton.innerHTML = '<i class="fas fa-sun"></i>';
        } else {
            toggleButton.innerHTML = '<i class="fas fa-moon"></i>';
        }
    }
}
