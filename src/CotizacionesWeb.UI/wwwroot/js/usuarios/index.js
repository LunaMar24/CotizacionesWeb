// ============================================
// USUARIOS INDEX - JavaScript
// ============================================

$(document).ready(function() {
    
    // ============================================
    // FILTROS DE BUSQUEDA
    // ============================================
    
    function filterTable() {
        const nameFilter = $('#filterName').val().toLowerCase();
        const emailFilter = $('#filterEmail').val().toLowerCase();
        const statusFilter = $('#filterStatus').val();
        
        $('#usuariosTable tbody tr').each(function() {
            const row = $(this);
            const nombre = row.attr('data-nombre') || '';
            const email = row.attr('data-email') || '';
            const activo = row.attr('data-activo') || '';
            
            const matchName = nombre.includes(nameFilter);
            const matchEmail = email.includes(emailFilter);
            const matchStatus = statusFilter === '' || activo === statusFilter;
            
            if (matchName && matchEmail && matchStatus) {
                row.show();
            } else {
                row.hide();
            }
        });
    }
    
    $('#filterName, #filterEmail, #filterStatus').on('keyup change', filterTable);
    
    // ============================================
    // VER ROLES
    // ============================================
    
    $('.btn-view-roles').on('click', function() {
        const usuarioId = $(this).data('usuario-id');
        const usuarioNombre = $(this).data('usuario-nombre');
        
        $('#rolesViewContent').html('<div class="text-center"><div class="spinner-border text-primary" role="status"></div></div>');
        $('#rolesViewModal').modal('show');
        
        $.get('/Usuarios/GetRoles/' + usuarioId, function(data) {
            const roles = $(data).find('.list-group-item');
            let rolesHtml = '<div class="list-group">';
            
            roles.each(function() {
                const rolNombre = $(this).find('strong').text();
                const rolDesc = $(this).find('small').text();
                rolesHtml += `
                    <div class="list-group-item">
                        <i class="fas fa-check-circle text-success mr-2"></i>
                        <strong>${rolNombre}</strong>
                        ${rolDesc ? '<br><small class="text-muted">' + rolDesc + '</small>' : ''}
                    </div>
                `;
            });
            
            rolesHtml += '</div>';
            $('#rolesViewContent').html(rolesHtml);
        }).fail(function() {
            $('#rolesViewContent').html('<div class="alert alert-danger">Error al cargar los roles.</div>');
        });
    });
    
    // ============================================
    // GESTIONAR ROLES
    // ============================================
    
    $('.btn-manage-roles').on('click', function() {
        const usuarioId = $(this).data('usuario-id');
        
        $('#rolesManageContent').html('<div class="text-center p-5"><div class="spinner-border text-primary" role="status"></div></div>');
        $('#rolesManageModal').modal('show');
        
        $.get('/Usuarios/GetRoles/' + usuarioId, function(data) {
            $('#rolesManageContent').html(data);
        }).fail(function() {
            $('#rolesManageContent').html('<div class="alert alert-danger m-3">Error al cargar los roles.</div>');
        });
    });
    
    $(document).on('submit', '#updateRolesForm', function(e) {
        e.preventDefault();
        
        const form = $(this);
        const formData = form.serialize();
        
        $.post('/Usuarios/UpdateRoles', formData, function(response) {
            if (response.success) {
                $('#rolesManageModal').modal('hide');
                showNotification('success', response.message);
                setTimeout(() => location.reload(), 1500);
            } else {
                showNotification('error', response.message);
            }
        }).fail(function() {
            showNotification('error', 'Error al actualizar los roles.');
        });
    });
    
    // ============================================
    // RESETEAR CONTRASEÑA
    // ============================================
    
    $('.btn-reset-password').on('click', function() {
        const usuarioId = $(this).data('usuario-id');
        const usuarioNombre = $(this).data('usuario-nombre');
        
        $('#resetPasswordUsuarioId').val(usuarioId);
        $('#resetPasswordUsuarioNombre').text(usuarioNombre);
        $('#newPassword').val('');
        $('#confirmNewPassword').val('');
        hideModalAlert('resetPasswordAlert');
        $('#resetPasswordModal').modal('show');
    });
    
    $('#resetPasswordForm').on('submit', function(e) {
        e.preventDefault();
        
        const newPassword = $('#newPassword').val();
        const confirmPassword = $('#confirmNewPassword').val();
        const usuarioId = $('#resetPasswordUsuarioId').val();
        
        if (newPassword !== confirmPassword) {
            showModalAlert('resetPasswordAlert', 'Las contraseñas no coinciden.');
            return;
        }

        if (newPassword.length < 6) {
            showModalAlert('resetPasswordAlert', 'La contraseña debe tener al menos 6 caracteres.');
            return;
        }
        
        const token = $('input[name="__RequestVerificationToken"]').val();
        
        $.post('/Usuarios/ResetPassword', {
            id: usuarioId,
            newPassword: newPassword,
            __RequestVerificationToken: token
        }, function(response) {
            if (response.success) {
                $('#resetPasswordModal').modal('hide');
                showNotification('success', response.message);
            } else {
                showModalAlert('resetPasswordAlert', response.message);
            }
        }).fail(function() {
            showModalAlert('resetPasswordAlert', 'Error al resetear la contraseña.');
        });
    });
    
    // ============================================
    // ELIMINAR USUARIO (CON MODAL DE CONFIRMACION)
    // ============================================
    
    $('.btn-delete-usuario').on('click', function() {
        const usuarioId = $(this).data('usuario-id');
        const usuarioNombre = $(this).data('usuario-nombre');
        
        $('#deleteUsuarioId').val(usuarioId);
        $('#deleteUsuarioNombre').text(usuarioNombre);
        hideModalAlert('deleteUsuarioAlert');
        $('#deleteConfirmModal').modal('show');
    });
    
    $('#confirmDeleteBtn').on('click', function() {
        const usuarioId = $('#deleteUsuarioId').val();
        const token = $('input[name="__RequestVerificationToken"]').val();
        
        $(this).prop('disabled', true);
        
        $.post('/Usuarios/Delete', {
            id: usuarioId,
            __RequestVerificationToken: token
        }, function(response) {
            if (response.success) {
                $('#deleteConfirmModal').modal('hide');
                showNotification('success', response.message);
                setTimeout(() => location.reload(), 1500);
            } else {
                showModalAlert('deleteUsuarioAlert', response.message);
                $('#confirmDeleteBtn').prop('disabled', false);
            }
        }).fail(function() {
            showModalAlert('deleteUsuarioAlert', 'Error al eliminar el usuario.');
            $('#confirmDeleteBtn').prop('disabled', false);
        });
    });
    
});