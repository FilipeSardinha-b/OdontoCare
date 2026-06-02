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
// Cria um modal genérico reutilizável
function criarModal(titulo, conteudo) {
    const overlay = document.createElement('div');
    overlay.className = 'modal-overlay';
    overlay.id = 'modalOverlay';
    overlay.innerHTML = `
        <div class="modal">
            <div class="modal-header">
                <h3>${titulo}</h3>
                <button class="modal-close" onclick="fecharModal()">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <div class="modal-body">${conteudo}</div>
        </div>
    `;
    // Fecha ao clicar fora
    overlay.addEventListener('click', (e) => {
        if (e.target === overlay) fecharModal();
    });
    return overlay;
}

function fecharModal() {
    const modal = document.getElementById('modalOverlay');
    if (modal) modal.remove();
}

// Renderiza botões de paginação
function renderPaginacao(containerId, paginaAtual, totalPaginas, callback) {
    const div = document.getElementById(containerId);
    if (!div || totalPaginas <= 1) {
        if (div) div.innerHTML = '';
        return;
    }

    let html = '';
    for (let i = 1; i <= totalPaginas; i++) {
        html += `<button class="${i === paginaAtual ? 'active' : ''}"
            onclick="${callback.name}(${i})">${i}</button>`;
    }
    div.innerHTML = html;
}