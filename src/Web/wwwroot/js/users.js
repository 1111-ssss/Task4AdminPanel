$(function () {
    const state = {
        page: 1,
        pageSize: 20,
        orderBy: 'email',
        isAsc: true,
        search: '',
        selected: new Set()
    };

    loadUsers();

    let searchTimer;
    $('#searchInput').on('input', function () {
        clearTimeout(searchTimer);
        searchTimer = setTimeout(() => {
            state.search = $(this).val().trim();
            state.page = 1;
            loadUsers();
        }, 300);
    });

    $('#selectAll').on('change', function () {
        const checked = this.checked;
        $('#usersBody input.row-check').prop('checked', checked).trigger('change');
    });

    $('.sortable').on('click', function () {
        const order = $(this).data('order');
        if (state.orderBy === order) {
            state.isAsc = !state.isAsc;
        } else {
            state.orderBy = order;
            state.isAsc = true;
        }
        updateSortIcons();
        loadUsers();
    });

    $('#btnBlock').on('click', () => bulkAction('block'));
    $('#btnUnblock').on('click', () => bulkAction('unblock'));
    $('#btnDelete').on('click', () => bulkAction('delete'));

    $('#logoutBtn').on('click', function () {
        clearError();
        $.ajax({
            url: '/api/account/logout',
            type: 'POST',
            success: () => window.location.href = '/Account/Login',
            error: () => window.location.href = '/Account/Login'
        });
    });

    function loadUsers() {
        clearError();
        $('#loadingState').removeClass('d-none');
        $('#emptyState').addClass('d-none');
        $('#usersBody').empty();

        $.ajax({
            url: '/api/admin/users',
            type: 'GET',
            data: {
                page: state.page,
                pageSize: state.pageSize,
                orderBy: state.orderBy,
                search: state.search || null,
                isAsc: state.isAsc
            },
            success: function (res) {
                const items = res.users || [];
                const total = res.totalRecords ?? 0;
                const totalPages = res.totalPages ?? 1;

                renderRows(items);
                renderPagination(total, totalPages);
                updateActionButtons();
            },
            error: function (xhr) {
                let message = 'Failed to load users. Please try again later.';
                if (xhr.responseJSON && xhr.responseJSON.error) {
                    message = xhr.responseJSON.error;
                }

                showError(message);
                $('#usersBody').html(
                    `<tr><td colspan="5" class="text-center text-danger py-4">Failed to load users</td></tr>`
                );
            },
            complete: function () {
                $('#loadingState').addClass('d-none');
            }
        });
    }

    function renderRows(items) {
        const $body = $('#usersBody');
        $body.empty();
        state.selected.clear();
        $('#selectAll').prop('checked', false);

        if (!items.length) {
            $('#emptyState').removeClass('d-none');
            return;
        }

        items.forEach(user => {
            const email = user.email || '';
            const fullName = [user.name, user.surname].filter(Boolean).join(' ') || email;
            const lastSeen = user.lastLoginTime;

            const rawStatus = user?.status !== undefined && user?.status !== null 
                ? user.status.toString().toLowerCase() 
                : '0';

            const statusMap = {
                '0': 'Active',
                'active': 'Active',
                '1': 'Blocked',
                'blocked': 'Blocked',
                '2': 'Unverified',
                'unverified': 'Unverified'
            };

            const statusText = statusMap[rawStatus] || 'Active';
            const statusLower = statusText.toLowerCase();

            const statusClass = {
                active: 'status-active',
                blocked: 'status-blocked',
                unverified: 'status-unverified'
            }[statusLower] || 'status-active';

            const row = `
                <tr data-email="${escapeHtml(email)}">
                    <td>
                        <input type="checkbox" class="form-check-input row-check" value="${escapeHtml(email)}" />
                    </td>
                    <td>
                        <div class="user-name">${escapeHtml(fullName)}</div>
                    </td>
                    <td>${escapeHtml(email)}</td>
                    <td>
                        <span class="status-badge ${statusClass}">${escapeHtml(statusText)}</span>
                    </td>
                    <td>
                        <span class="last-seen-text" title="${formatDateTime(lastSeen)}">
                            ${formatLastSeen(lastSeen)}
                        </span>
                    </td>
                </tr>
            `;
            $body.append(row);
        });

        $body.find('.row-check').on('change', function () {
            const email = this.value;
            if (this.checked) {
                state.selected.add(email);
                $(this).closest('tr').addClass('selected');
            } else {
                state.selected.delete(email);
                $(this).closest('tr').removeClass('selected');
            }
            updateActionButtons();

            const total = $body.find('.row-check').length;
            const checked = $body.find('.row-check:checked').length;
            $('#selectAll').prop('checked', total > 0 && checked === total);
        });
    }

    function renderPagination(total, totalPages) {
        const $pag = $('#pagination');
        $pag.empty();

        $('#pageInfo').text(
            total === 0
                ? 'No users'
                : `Showing ${(state.page - 1) * state.pageSize + 1}–${Math.min(state.page * state.pageSize, total)} of ${total}`
        );

        if (totalPages <= 1) return;

        const addPage = (p, label, disabled = false, active = false) => {
            $pag.append(`
                <li class="page-item ${disabled ? 'disabled' : ''} ${active ? 'active' : ''}">
                    <a class="page-link" href="#" data-page="${p}">${label}</a>
                </li>
            `);
        };

        addPage(state.page - 1, '‹', state.page === 1);

        for (let i = 1; i <= totalPages; i++) {
            if (i === 1 || i === totalPages || Math.abs(i - state.page) <= 1) {
                addPage(i, i, false, i === state.page);
            } else if (Math.abs(i - state.page) === 2) {
                $pag.append(`<li class="page-item disabled"><span class="page-link">…</span></li>`);
            }
        }

        addPage(state.page + 1, '›', state.page === totalPages);

        $pag.find('a[data-page]').on('click', function (e) {
            e.preventDefault();
            const p = parseInt($(this).data('page'), 10);
            if (p >= 1 && p <= totalPages && p !== state.page) {
                state.page = p;
                loadUsers();
            }
        });
    }

    function updateActionButtons() {
        const hasSelection = state.selected.size > 0;
        $('#btnBlock, #btnUnblock, #btnDelete').prop('disabled', !hasSelection);
    }

    function updateSortIcons() {
        $('.sortable').removeClass('asc desc');
        $(`.sortable[data-order="${state.orderBy}"]`)
            .addClass(state.isAsc ? 'asc' : 'desc');
    }

    async function bulkAction(action) {
        clearError();
        const emails = Array.from(state.selected);
        if (!emails.length) return;

        $('#btnBlock, #btnUnblock, #btnDelete').prop('disabled', true);

        const requests = emails.map(email => {
            let url, type;
            if (action === 'block') {
                url = `/api/admin/users/${encodeURIComponent(email)}/block`;
                type = 'POST';
            } else if (action === 'unblock') {
                url = `/api/admin/users/${encodeURIComponent(email)}/unblock`;
                type = 'POST';
            } else {
                url = `/api/admin/users/${encodeURIComponent(email)}`;
                type = 'DELETE';
            }
            return $.ajax({ url, type });
        });

        try {
            await Promise.all(requests);
        } catch (xhr) {
            let message = 'Some operations failed. Please try again.';
            if (xhr.responseJSON && xhr.responseJSON.error) {
                message = xhr.responseJSON.error;
            }
            showError(message);
        }

        loadUsers();
    }

    function showError(text) {
        const $status = $('#statusMessage');
        if ($status.length) {
            $status.html(`
                <div class="alert alert-danger mb-3 alert-dismissible fade show" role="alert">
                    <h5 class="alert-heading mb-2">Error</h5>
                    <p class="mb-0">${escapeHtml(text)}</p>
                    <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                </div>
            `);
        }
    }

    function clearError() {
        const $status = $('#statusMessage');
        if ($status.length) {
            $status.empty();
        }
    }

    function formatLastSeen(value) {
        if (!value) return '—';
        const date = new Date(value);
        if (isNaN(date.getTime())) return '—';

        const diffMs = Date.now() - date.getTime();
        const diffMin = Math.floor(diffMs / 60000);
        const diffH = Math.floor(diffMin / 60);
        const diffD = Math.floor(diffH / 24);

        if (diffMin < 1) return 'just now';
        if (diffMin < 60) return `${diffMin} minute${diffMin === 1 ? '' : 's'} ago`;
        if (diffH < 24) return `${diffH} hour${diffH === 1 ? '' : 's'} ago`;
        if (diffD < 30) return `${diffD} day${diffD === 1 ? '' : 's'} ago`;

        return date.toLocaleDateString();
    }

    function formatDateTime(value) {
        if (!value) return '';
        const date = new Date(value);
        if (isNaN(date.getTime())) return '';
        return date.toLocaleString();
    }

    function escapeHtml(str) {
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }

    updateSortIcons();
});