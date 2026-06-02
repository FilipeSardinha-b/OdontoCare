let paginaPacientes = 1;

async function renderPacientes() {
    const content = document.getElementById('pageContent');
    content.innerHTML = `<div class="loading"><i class="fas fa-spinner"></i></div>`;

    const role = localStorage.getItem('role');

    content.innerHTML = `
        <div class="page-header">
            <h2>Pacientes</h2>
            <p>Gerencie os pacientes do consultório</p>
        </div>

        <div class="toolbar">
            <div class="search-box">
                <i class="fas fa-search"></i>
                <input type="text" id="searchPaciente" placeholder="Buscar por nome..."
                    oninput="buscarPacientes()">
            </div>
            <button class="btn btn-primary" onclick="abrirModalPaciente()">
                <i class="fas fa-plus"></i> Novo Paciente
            </button>
        </div>

        <div class="card">
            <div id="tabelaPacientes">
                <div class="loading"><i class="fas fa-spinner"></i></div>
            </div>
            <div id="paginacaoPacientes" class="pagination"></div>
        </div>
    `;

    await carregarPacientes();
}

async function carregarPacientes(pagina = 1) {
    paginaPacientes = pagina;
    const nome = document.getElementById('searchPaciente')?.value || '';
    const role = localStorage.getItem('role');

    try {
        const dados = await apiRequest(`/pacientes?pagina=${pagina}&tamanhoPagina=8&nome=${nome}`);
        const div = document.getElementById('tabelaPacientes');

        if (!dados?.dados?.length) {
            div.innerHTML = `
                <div class="empty-state">
                    <i class="fas fa-users"></i>
                    <p>Nenhum paciente encontrado</p>
                </div>`;
            document.getElementById('paginacaoPacientes').innerHTML = '';
            return;
        }

        div.innerHTML = `
            <div class="table-container">
                <table>
                    <thead>
                        <tr>
                            <th>Nome</th>
                            <th>CPF</th>
                            <th>Telefone</th>
                            <th>Email</th>
                            <th>Cadastro</th>
                            <th>Ações</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${dados.dados.map(p => `
                            <tr>
                                <td><strong>${p.nomeCompleto}</strong></td>
                                <td>${formatarCPF(p.cpf)}</td>
                                <td>${p.telefone}</td>
                                <td>${p.email}</td>
                                <td>${new Date(p.dataCadastro).toLocaleDateString('pt-BR')}</td>
                                <td>
                                    <div style="display:flex;gap:6px;">
                                        <button class="btn btn-outline btn-sm" onclick="verPaciente('${p.id}')">
                                            <i class="fas fa-eye"></i>
                                        </button>
                                        ${role === 'admin' ? `
                                        <button class="btn btn-primary btn-sm" onclick="editarPaciente('${p.id}')">
                                            <i class="fas fa-edit"></i>
                                        </button>
                                        <button class="btn btn-danger btn-sm" onclick="deletarPaciente('${p.id}', '${p.nomeCompleto}')">
                                            <i class="fas fa-trash"></i>
                                        </button>` : ''}
                                    </div>
                                </td>
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
            </div>`;

        // Paginação
        renderPaginacao('paginacaoPacientes', dados.pagina, dados.totalPaginas, carregarPacientes);

    } catch (err) {
        document.getElementById('tabelaPacientes').innerHTML =
            `<div class="alert alert-error">${err.message}</div>`;
    }
}

let debounceTimer;
function buscarPacientes() {
    clearTimeout(debounceTimer);
    debounceTimer = setTimeout(() => carregarPacientes(1), 400);
}

function abrirModalPaciente(paciente = null) {
    const titulo = paciente ? 'Editar Paciente' : 'Novo Paciente';
    const modal = criarModal(titulo, `
        <div class="form-row">
            <div class="form-group">
                <label>Nome Completo *</label>
                <input type="text" id="f_nome" value="${paciente?.nomeCompleto || ''}" placeholder="Nome completo">
            </div>
            <div class="form-group">
                <label>CPF *</label>
                <input type="text" id="f_cpf" value="${paciente?.cpf || ''}" placeholder="Apenas números" maxlength="11">
            </div>
        </div>
        <div class="form-row">
            <div class="form-group">
                <label>Data de Nascimento *</label>
                <input type="date" id="f_nascimento" value="${paciente?.dataNascimento?.substring(0,10) || ''}">
            </div>
            <div class="form-group">
                <label>Telefone *</label>
                <input type="text" id="f_telefone" value="${paciente?.telefone || ''}" placeholder="Apenas números">
            </div>
        </div>
        <div class="form-group">
            <label>Email *</label>
            <input type="email" id="f_email" value="${paciente?.email || ''}" placeholder="email@exemplo.com">
        </div>
        <div class="form-group">
            <label>Endereço *</label>
            <input type="text" id="f_endereco" value="${paciente?.endereco || ''}" placeholder="Rua, número, bairro">
        </div>
        <div class="form-group">
            <label>Histórico Médico</label>
            <textarea id="f_historico" rows="3" placeholder="Informações médicas relevantes...">${paciente?.historicoMedico || ''}</textarea>
        </div>
        <div class="form-actions">
            <button class="btn btn-outline" onclick="fecharModal()">Cancelar</button>
            <button class="btn btn-primary" onclick="salvarPaciente('${paciente?.id || ''}')">
                <i class="fas fa-save"></i> Salvar
            </button>
        </div>
    `);
    document.body.appendChild(modal);
}

async function salvarPaciente(id) {
    const dto = {
        nomeCompleto: document.getElementById('f_nome').value,
        cpf: document.getElementById('f_cpf').value,
        dataNascimento: document.getElementById('f_nascimento').value,
        telefone: document.getElementById('f_telefone').value,
        email: document.getElementById('f_email').value,
        endereco: document.getElementById('f_endereco').value,
        historicoMedico: document.getElementById('f_historico').value
    };

    try {
        if (id) {
            await apiRequest(`/pacientes/${id}`, 'PUT', dto);
        } else {
            await apiRequest('/pacientes', 'POST', dto);
        }
        fecharModal();
        await carregarPacientes(paginaPacientes);
    } catch (err) {
        alert('Erro: ' + err.message);
    }
}

async function verPaciente(id) {
    try {
        const p = await apiRequest(`/pacientes/${id}`);
        const modal = criarModal('Detalhes do Paciente', `
            <div style="display:grid;gap:12px;">
                <div><strong>Nome:</strong> ${p.nomeCompleto}</div>
                <div><strong>CPF:</strong> ${formatarCPF(p.cpf)}</div>
                <div><strong>Nascimento:</strong> ${new Date(p.dataNascimento).toLocaleDateString('pt-BR')}</div>
                <div><strong>Telefone:</strong> ${p.telefone}</div>
                <div><strong>Email:</strong> ${p.email}</div>
                <div><strong>Endereço:</strong> ${p.endereco}</div>
                <div><strong>Histórico:</strong> ${p.historicoMedico || 'Não informado'}</div>
                <div><strong>Cadastro:</strong> ${new Date(p.dataCadastro).toLocaleDateString('pt-BR')}</div>
            </div>
            <div class="form-actions mt-4">
                <button class="btn btn-outline" onclick="fecharModal()">Fechar</button>
            </div>
        `);
        document.body.appendChild(modal);
    } catch (err) {
        alert('Erro: ' + err.message);
    }
}

async function editarPaciente(id) {
    try {
        const p = await apiRequest(`/pacientes/${id}`);
        abrirModalPaciente(p);
    } catch (err) {
        alert('Erro: ' + err.message);
    }
}

async function deletarPaciente(id, nome) {
    if (!confirm(`Deseja realmente excluir o paciente "${nome}"?`)) return;
    try {
        await apiRequest(`/pacientes/${id}`, 'DELETE');
        await carregarPacientes(paginaPacientes);
    } catch (err) {
        alert('Erro: ' + err.message);
    }
}

function formatarCPF(cpf) {
    if (!cpf) return '';
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
}