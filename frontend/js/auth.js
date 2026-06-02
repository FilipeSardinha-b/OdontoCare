// Realiza login e salva token no localStorage
async function realizarLogin() {
    const email = document.getElementById('loginEmail').value;
    const senha = document.getElementById('loginSenha').value;
    const errorDiv = document.getElementById('loginError');

    errorDiv.classList.add('hidden');

    try {
        const dados = await apiRequest('/auth/login', 'POST', { email, senha });
        if (!dados) return;

        localStorage.setItem('token', dados.token);
        localStorage.setItem('email', dados.email);
        localStorage.setItem('role', dados.role);

        iniciarApp();
    } catch (err) {
        errorDiv.textContent = err.message;
        errorDiv.classList.remove('hidden');
    }
}

// Realiza o registro de novo usuário
async function realizarRegistro() {
    const email = document.getElementById('registerEmail').value;
    const senha = document.getElementById('registerSenha').value;
    const role = document.getElementById('registerRole').value;
    const errorDiv = document.getElementById('registerError');

    errorDiv.classList.add('hidden');

    try {
        const dados = await apiRequest('/auth/registrar', 'POST', { email, senha, role });
        if (!dados) return;

        localStorage.setItem('token', dados.token);
        localStorage.setItem('email', dados.email);
        localStorage.setItem('role', dados.role);

        iniciarApp();
    } catch (err) {
        errorDiv.textContent = err.message;
        errorDiv.classList.remove('hidden');
    }
}

function mostrarRegistro() {
    document.getElementById('loginPage').classList.add('hidden');
    document.getElementById('registerPage').classList.remove('hidden');
}

function mostrarLogin() {
    document.getElementById('registerPage').classList.add('hidden');
    document.getElementById('loginPage').classList.remove('hidden');
}

function logout() {
    localStorage.clear();
    document.getElementById('mainApp').classList.add('hidden');
    document.getElementById('loginPage').classList.remove('hidden');
}