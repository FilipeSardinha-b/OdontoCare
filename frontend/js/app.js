// Inicializa o sistema após login
function iniciarApp() {
    document.getElementById('loginPage').classList.add('hidden');
    document.getElementById('registerPage').classList.add('hidden');
    document.getElementById('mainApp').classList.remove('hidden');

    // Exibe email e role do usuário na sidebar
    document.getElementById('userEmail').textContent = localStorage.getItem('email');
    document.getElementById('userRole').textContent = localStorage.getItem('role');

    navegarPara('dashboard');
}

// Navegação assíncrona sem recarregar a página
function navegarPara(pagina) {
    // Atualiza item ativo na sidebar
    document.querySelectorAll('.nav-item').forEach(item => item.classList.remove('active'));
    const navAtivo = document.querySelector(`.nav-item[onclick="navegarPara('${pagina}')"]`);
    if (navAtivo) navAtivo.classList.add('active');

    // Renderiza a página correspondente
    switch (pagina) {
        case 'dashboard': renderDashboard(); break;
        case 'pacientes': renderPacientes(); break;
        case 'consultas': renderConsultas(); break;
    }
}

// Verifica se já há sessão ativa ao carregar a página
window.addEventListener('load', () => {
    const token = localStorage.getItem('token');
    if (token) {
        iniciarApp();
    }
});

// Permite enviar formulários com Enter
document.addEventListener('keydown', (e) => {
    if (e.key === 'Enter') {
        const loginPage = document.getElementById('loginPage');
        if (!loginPage.classList.contains('hidden')) realizarLogin();
    }
});