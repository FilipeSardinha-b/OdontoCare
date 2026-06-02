async function renderDashboard() {
    const content = document.getElementById('pageContent');
    content.innerHTML = `<div class="loading"><i class="fas fa-spinner"></i></div>`;

    try {
        const [pacientes, consultas] = await Promise.all([
            apiRequest('/pacientes?tamanhoPagina=1'),
            apiRequest('/consultas?tamanhoPagina=1')
        ]);

        const agendadas = await apiRequest('/consultas?status=Agendada&tamanhoPagina=1');
        const realizadas = await apiRequest('/consultas?status=Realizada&tamanhoPagina=1');

        content.innerHTML = `
            <div class="page-header">
                <h2>Dashboard</h2>
                <p>Visão geral do consultório</p>
            </div>

            <div class="stats-grid">
                <div class="stat-card">
                    <div class="stat-icon blue"><i class="fas fa-users"></i></div>
                    <div class="stat-info">
                        <h3>${pacientes?.total ?? 0}</h3>
                        <p>Total de Pacientes</p>
                    </div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon green"><i class="fas fa-calendar-check"></i></div>
                    <div class="stat-info">
                        <h3>${consultas?.total ?? 0}</h3>
                        <p>Total de Consultas</p>
                    </div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon yellow"><i class="fas fa-clock"></i></div>
                    <div class="stat-info">
                        <h3>${agendadas?.total ?? 0}</h3>
                        <p>Consultas Agendadas</p>
                    </div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon red"><i class="fas fa-check-circle"></i></div>
                    <div class="stat-info">
                        <h3>${realizadas?.total ?? 0}</h3>
                        <p>Consultas Realizadas</p>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="page-header">
                    <h2>Próximas Consultas</h2>
                    <p>Consultas agendadas recentemente</p>
                </div>
                <div id="proximasConsultas">
                    <div class="loading"><i class="fas fa-spinner"></i></div>
                </div>
            </div>
        `;

        // Carrega as próximas consultas
        const proximas = await apiRequest('/consultas?status=Agendada&tamanhoPagina=5');
        const divProximas = document.getElementById('proximasConsultas');

        if (!proximas?.dados?.length) {
            divProximas.innerHTML = `
                <div class="empty-state">
                    <i class="fas fa-calendar"></i>
                    <p>Nenhuma consulta agendada</p>
                </div>`;
            return;
        }

        divProximas.innerHTML = `
            <div class="table-container">
                <table>
                    <thead>
                        <tr>
                            <th>Paciente</th>
                            <th>Dentista</th>
                            <th>Data</th>
                            <th>Horário</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${proximas.dados.map(c => `
                            <tr>
                                <td>${c.nomePaciente}</td>
                                <td>${c.dentista}</td>
                                <td>${new Date(c.dataConsulta).toLocaleDateString('pt-BR')}</td>
                                <td>${c.horario}</td>
                                <td><span class="badge badge-${c.status.toLowerCase()}">${c.status}</span></td>
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
            </div>`;
    } catch (err) {
        content.innerHTML = `<div class="alert alert-error">${err.message}</div>`;
    }
}