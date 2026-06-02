// URL base da API - ajuste a porta se necessário
const API_URL = 'http://localhost:5088/api';

// Função central para todas as chamadas à API
async function apiRequest(endpoint, method = 'GET', body = null) {
    const token = localStorage.getItem('token');

    const headers = { 'Content-Type': 'application/json' };
    if (token) headers['Authorization'] = `Bearer ${token}`;

    const config = { method, headers };
    if (body) config.body = JSON.stringify(body);

    const response = await fetch(`${API_URL}${endpoint}`, config);

    // Token expirado - redireciona para login
    if (response.status === 401) {
        logout();
        return null;
    }

    if (!response.ok) {
        const erro = await response.json().catch(() => ({ mensagem: 'Erro desconhecido' }));
        throw new Error(erro.mensagem || `Erro ${response.status}`);
    }

    if (response.status === 204) return null;
    return await response.json();
}