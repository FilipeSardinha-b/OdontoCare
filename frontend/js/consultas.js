let paginaConsultas = 1;

async function renderConsultas() {
    const content = document.getElementById('pageContent');
    content.innerHTML = `<div class="loading"><i class="fas fa-spinner"></i></div>`;

    const role = localStorage.getItem('role');

    content.innerHTML = `
        <div class="page-header">
            <h2>Consultas</h2>
            <p>Gerencie as consultas do consultório</p>
        </div>

        <div class="toolbar">
            <div style="display:flex;gap:10px;flex-wrap:wrap;">
                <select id="filtroStatus" onchange="carregarConsultas(1)"
                    style="padding:8px 14px;border:1.5px solid #e2e8f0;border-radius:8px;font-size:14px;">
                    <option value="">Todos os status</option>
                    <option value="Agendada">Agendada</option>
                    <option value="Confirmada">Confirmada</option>
                    <option value="Realizada">Realizada</option>
                    <option value="Cancelada">Cancelada</option>
                </select>
            </div>
            <button class="btn btn-primary" onclick="abrirModalConsulta()">
                <i class="fas fa-plus"></i> Nova Consulta
            </button>
        </div>

        <div class="card">
            <div id="tabelaConsultas">
                <div class="loading"><i class="fas fa-spinner"></i></div>
            </div>
            <div id="paginacaoConsultas" class="pagination"></div>
        </div>
    `;

    await carregarConsultas();
}

async function carregarConsultas(pagina = 1) {
    paginaConsultas = pagina;
    const status = document.getElementById('filtroStatus')?.value || '';
    const role = localStorage.getItem('role');

    try {
        const dados = await apiRequest(`/consultas?pagina=${pagina}&tamanhoPagina=8&status=${status}`);
        const div = document.getElementById('tabelaConsultas');

        if (!dados?.dados?.length) {
            div.innerHTML = `
                <div class="empty-state">
                    <i class="fas fa-calendar"></i>
                    <p>Nenhuma consulta encontrada</p>
                </div>`;
            document.getElementById('paginacaoConsultas').innerHTML = '';
            return;
        }

        div.innerHTML = `
            <div class="table-container">
                <table>
                    <thead>
                        <tr>
                            <th>Paciente</th>
                            <th>Dentista</th>
                            <th>Especialidade</th>
                            <th>Data</th>
                            <th>Horário</th>
                            <th>Valor</th>
                            <th>Status</th>
                            <th>Ações</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${dados.dados.map(c => `
                            <tr>
                                <td><strong>${c.nomePaciente}</strong></td>
                                <td>${c.dentista}</td>
                                <td>${c.especialidade}</td>
                                <td>${new Date(c.dataConsulta).toLocaleDateString('pt-BR')}</td>
                                <td>${c.horario}</td>
                                <td>R$ ${c.valor.toFixed(2)}</td>
                                <td><span class="badge badge-${c.status.toLowerCase()}">${c.status}</span></td>
                                <td>
                                    <div style="display:flex;gap:6px;">
                                        ${role === 'admin' ? `
                                        <button class="btn btn-primary btn-sm" onclick="editarConsulta('${c.id}')">
                                            <i class="fas fa-edit"></i>
                                        </button>
                                        <button class="btn btn-danger btn-sm" onclick="deletarConsulta('${c.id}')">
                                            <i class="fas fa-trash"></i>
                                        </button>` : `
                                        <button class="btn btn-outline btn-sm" onclick="verConsulta('${c.id}')">
                                            <i class="fas fa-eye"></i>
                                        </button>`}
                                    </div>
                                </td>
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
            </div>`;

        renderPaginacao('paginacaoConsultas', dados.pagina, dados.totalPaginas, carregarConsultas);

    } catch (err) {
        document.getElementById('tabelaConsultas').innerHTML =
            `<div class="alert alert-error">${err.message}</div>`;
    }
}

async function abrirModalConsulta(consulta = null) {
    // Carrega lista de pacientes para o select
    const pacientesData = await apiRequest('/pacientes?tamanhoPagina=100');
    const pacientes = pacientesData?.dados || [];

    const titulo = consulta ? 'Editar Consulta' : 'Nova Consulta';
    const modal = criarModal(titulo, `
        <div class="form-group">
            <label>Paciente *</label>
            <select id="f_pacienteId">
                <option value="">Selecione um paciente</option>
                ${pacientes.map(p => `
                    <option value="${p.id}" ${consulta?.pacienteId === p.id ? 'selected' : ''}>
                        ${p.nomeCompleto}
                    </option>`).join('')}
            </select>
        </div>
        <div class="form-row">
            <div class="form-group">
                <label>Dentista *</label>
                <input type="text" id="f_dentista" value="${consulta?.dentista || ''}" placeholder="Nome do dentista">
            </div>
            <div class="form-group">
                <label>Especialidade *</label>
                <input type="text" id="f_especialidade" value="${consulta?.especialidade || ''}" placeholder="Ex: Ortodontia">
            </div>
        </div>
        <div class="form-row">
            <div class="form-group">
                <label>Data *</label>
                <input type="date" id="f_data" value="${consulta?.dataConsulta?.substring(0,10) || ''}">
            </div>
            <div class="form-group">
                <label>Horário *</label>
                <input type="time" id="f_horario" value="${consulta?.horario || ''}">
            </div>
        </div>
        <div class="form-row">
            <div class="form-group">
                <label>Status *</label>
                <select id="f_status">
                    ${['Agendada','Confirmada','Realizada','Cancelada'].map(s => `
                        <option value="${s}" ${consulta?.status === s ? 'selected' : ''}>${s}</option>
                    `).join('')}
                </select>
            </div>
            <div class="form-group">
                <label>Valor (R$)</label>
                <input type="number" id="f_valor" value="${consulta?.valor || '0'}" min="0" step="0.01">
            </div>
        </div>
        <div class="form-group">
            <label>Observações</label>
            <textarea id="f_obs" rows="3" placeholder="Observações sobre a consulta...">${consulta?.observacoes || ''}</textarea>
        </div>
        <div class="form-actions">
            <button class="btn btn-outline" onclick="fecharModal()">Cancelar</button>
            <button class="btn btn-primary" onclick="salvarConsulta('${consulta?.id || ''}')">
                <i class="fas fa-save"></i> Salvar
            </button>
        </div>
    `);
    document.body.appendChild(modal);
}

async function salvarConsulta(id) {
    const horarioRaw = document.getElementById('f_horario').value;
    const horario = horarioRaw.length === 5 ? horarioRaw : horarioRaw.substring(0, 5);

    const dto = {
        pacienteId: document.getElementById('f_pacienteId').value,
        dentista: document.getElementById('f_dentista').value,
        especialidade: document.getElementById('f_especialidade').value,
        dataConsulta: document.getElementById('f_data').value,
        horario: horario,
        status: document.getElementById('f_status').value,
        valor: parseFloat(document.getElementById('f_valor').value) || 0,
        observacoes: document.getElementById('f_obs').value
    };

    try {
        if (id) {
            await apiRequest(`/consultas/${id}`, 'PUT', dto);
        } else {
            await apiRequest('/consultas', 'POST', dto);
        }
        fecharModal();
        await carregarConsultas(paginaConsultas);
    } catch (err) {
        alert('Erro: ' + err.message);
    }
}

async function verConsulta(id) {
    try {
        const c = await apiRequest(`/consultas/${id}`);
        const modal = criarModal('Detalhes da Consulta', `
            <div style="display:grid;gap:12px;">
                <div><strong>Paciente:</strong> ${c.nomePaciente}</div>
                <div><strong>Dentista:</strong> ${c.dentista}</div>
                <div><strong>Especialidade:</strong> ${c.especialidade}</div>
                <div><strong>Data:</strong> ${new Date(c.dataConsulta).toLocaleDateString('pt-BR')}</div>
                <div><strong>Horário:</strong> ${c.horario}</div>
                <div><strong>Status:</strong> <span class="badge badge-${c.status.toLowerCase()}">${c.status}</span></div>
                <div><strong>Valor:</strong> R$ ${c.valor.toFixed(2)}</div>
                <div><strong>Observações:</strong> ${c.observacoes || 'Nenhuma'}</div>
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

async function editarConsulta(id) {
    try {
        const c = await apiRequest(`/consultas/${id}`);
        await abrirModalConsulta(c);
    } catch (err) {
        alert('Erro: ' + err.message);
    }
}

async function deletarConsulta(id) {
    if (!confirm('Deseja realmente excluir esta consulta?')) return;
    try {
        await apiRequest(`/consultas/${id}`, 'DELETE');
        await carregarConsultas(paginaConsultas);
    } catch (err) {
        alert('Erro: ' + err.message);
    }
}